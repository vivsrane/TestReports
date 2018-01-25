using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading;
using System.Timers;
using Timer=System.Timers.Timer;

namespace VB.Common.Pool.Spi
{
    public abstract class AbstractPool<TItem> : IPool<TItem>, IDisposable where TItem : class
    {
        private readonly Threading.IQueue<TimeRecord<TItem>> pool;

        private readonly List<Receipt<TItem>> borrowed = new List<Receipt<TItem>>();

        private readonly EvictionCriteria evictionCriteria;

        private readonly Timer evictionTimer;

        private readonly Timer leakTimer;

        private volatile bool closed = false;

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Writing a pool class is complex and authors will not be put off by nested generics")]
        protected AbstractPool(string instanceName, IPooledItemFactory<TItem> factory, Threading.IQueue<TimeRecord<TItem>> pool, EvictionCriteria evictionCriteria)
        {
            this.instanceName = instanceName;
            this.factory = factory;
            this.pool = pool;
            this.evictionCriteria = evictionCriteria;
            // setup eviction timer
            evictionTimer = new Timer();
            evictionTimer.Interval = evictionCriteria.Interval.TotalMilliseconds;
            evictionTimer.Elapsed += EvictIdleItems;
            evictionTimer.Start();
            // item leak timer
            leakTimer = new Timer();
            leakTimer.Interval = 1000;
            leakTimer.Elapsed += ProcessLeakedItems;
            leakTimer.Start();
        }

        ~AbstractPool()
        {
            Dispose(false);
        }

        protected void AssertOpen()
        {
            if (closed)
            {
                throw new PoolClosedException();
            }
        }

        #region Pool<E> Members

        private readonly string instanceName;

        private readonly IPooledItemFactory<TItem> factory;

        protected event EventHandler<DeadItemEventArgs> DeadItem;

        public string InstanceName
        {
            get { return instanceName; }
        }

        public IPooledItemFactory<TItem> Factory
        {
            get { return factory; }
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Writing a pool class is complex and authors will not be put off by nested generics")]
        protected Threading.IQueue<TimeRecord<TItem>> Pool
        {
            get { return pool; }
        }

        public virtual TItem BorrowItem()
        {
            return BorrowItem(new TimeSpan(Timeout.Infinite));
        }

        public virtual TItem BorrowItem(TimeSpan timeout)
        {
            if (timeout.Ticks < Timeout.Infinite)
            {
                throw new ArgumentException("Timeout must be greater then or equal to Timeout.Infinite", "timeout");
            }

            AssertOpen();

            TimeRecord<TItem> record = (timeout.Ticks == Timeout.Infinite)
                ? Pool.Poll(new TimeSpan(0,0,0,0,100))
                : Pool.Poll(timeout);

            TItem item;

            if (record == null)
            {
                item = GrowPool(); // there are no idle entries so we must grow

                if (item == null)
                {
                    if (timeout.Ticks == Timeout.Infinite)
                    {
                        item = Pool.Poll().Item;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            else
            {
                item = record.Item;
            }
            
            return Borrow(item);
        }

        private TItem Borrow(TItem item)
        {
            try
            {
                item = Validate(Activate(item, true), true);

                lock (borrowed)
                {
                    borrowed.Add(new Receipt<TItem>(item));
                }

                return item;
            }
            catch (PoolLifeCycleException e)
            {
                if (DeadItem != null)
                    DeadItem(this, new DeadItemEventArgs(null));

                throw new PoolImplementationException("Failed to borrow item", e);
            }
        }

        public virtual void ReturnItem(TItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            AssertOpen();

            Predicate<Receipt<TItem>> itemFilter = delegate(Receipt<TItem> r) { return r.Match(item); };

            lock (borrowed)
            {
                Receipt<TItem> receipt = borrowed.Find(itemFilter);

                if (receipt != null)
                {
                    // FIXME: Count the borrowed time

                    borrowed.Remove(receipt);
                }
                else
                {
                    throw new ArgumentException("Item not from the pool", "item");
                }
            }

            Passivate(item);
        }

        public virtual void Clear()
        {
            Clear(true);
        }

        protected void Clear(bool assertOpen)
        {
            if (assertOpen)
                AssertOpen();

            while (!Pool.IsEmpty())
            {
                TimeRecord<TItem> record = Pool.Poll();

                if (record != null)
                {
                    Destroy(record.Item, false);
                }
            }
        }

        public virtual void Close()
        {
            if (!closed)
            {
                closed = true;

                Clear(false);

                evictionTimer.Dispose();
            }
        }

        #endregion

        #region Life-Cycle Methods

        protected abstract TItem GrowPool();

        protected virtual void ShrinkPool(TItem item)
        {
            Destroy(item, false);
        }

        protected TItem Activate(TItem item, bool replaceItem)
        {
            try
            {
                factory.ActivateItem(item);
            }
            catch (PoolLifeCycleException)
            {
                if (replaceItem)
                {
                    try
                    {
                        item = Activate(Replace(item), false);
                    }
                    catch (PoolLifeCycleException e)
                    {
                        throw new PoolLifeCycleException("Failed to activate item", e);
                    }
                }
                else
                {
                    throw;
                }
            }

            return item;
        }

        protected TItem Validate(TItem item, bool replaceItem)
        {
            try
            {
                if (!factory.ValidateItem(item))
                {
                    throw new PoolLifeCycleException("Failed to validate item");
                }
            }
            catch
            {
                if (replaceItem)
                {
                    item = Validate(Activate(Replace(item), false), false);
                }
                else
                {
                    throw;
                }
            }

            return item;
        }

        protected abstract void Passivate(TItem item);

        protected void Passivate(TItem item, bool replaceItem)
        {
            try
            {
                factory.PassivateItem(item);
            }
            catch (PoolLifeCycleException e)
            {
                Logger.Info(e.Message);

                item = Destroy(item, replaceItem);
            }
            finally
            {
                if (item != null) Pool.Offer(new TimeRecord<TItem>(item));
            }
        }

        protected TItem Destroy(TItem item, bool replaceItem)
        {
            try
            {
                factory.DestroyItem(item);
            }
            catch (PoolLifeCycleException e)
            {
                Logger.Info(e.Message);
            }

            return replaceItem ? factory.MakeItem() : null;
        }

        protected TItem Replace(TItem item)
        {
            return Destroy(item, true);
        }

        #endregion

        #region Timer Events

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exceptions in the background timer thread are written to the event log")]
        private void EvictIdleItems(object source, ElapsedEventArgs e)
        {
            AssertOpen();

            TimeSpan noWait = new TimeSpan(0);

            evictionTimer.Stop();
            try
            {
                TimeRecord<TItem> record;

                while ((record = pool.Peek()) != null)
                {
                    if (evictionCriteria.Evict(record, pool.Size()))
                    {
                        record = pool.Poll(noWait);

                        if (record != null)
                        {
                            ShrinkPool(record.Item);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch (Exception x)
            {
                Logger.Error(x.Message);
            }
            finally
            {
                evictionTimer.Start();
            }
        }

        private void ProcessLeakedItems(object source, ElapsedEventArgs e)
        {
            AssertOpen();

            Predicate<Receipt<TItem>> deadFilter = delegate(Receipt<TItem> r) { return r.Dead(); };

            leakTimer.Stop();
            try
            {
                lock (borrowed)
                {
                    Receipt<TItem> receipt;

                    int deadCount = 0;

                    while ((receipt = borrowed.Find(deadFilter)) != null)
                    {
                        borrowed.Remove(receipt);

                        receipt.Dispose();

                        ++deadCount;
                    }

                    if (deadCount > 0)
                    {
                        HandleItemLeak(deadCount);
                    }
                }
            }
            finally
            {
                leakTimer.Start();
            }
        }

        protected virtual void HandleItemLeak(int count)
        {
            Logger.Error(string.Format(CultureInfo.InstalledUICulture, "{0} items leaked from BoundedPool '{1}'", count, instanceName));
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (evictionTimer != null)
                {
                    evictionTimer.Dispose();
                }

                if (leakTimer != null)
                {
                    leakTimer.Dispose();
                }

                Close();
            }
        }

        #endregion
    }
}
