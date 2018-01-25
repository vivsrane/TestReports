using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace VB.Common.Pool.Spi
{
    public abstract class AbstractKeyedPool<TItem,TKey> : IKeyedPool<TItem,TKey>, IDisposable where TItem : class
    {
        private readonly ReaderWriterLock keyLock = new ReaderWriterLock();

        private readonly Dictionary<TKey, Threading.IQueue<TimeRecord<TItem>>> keys = new Dictionary<TKey, Threading.IQueue<TimeRecord<TItem>>>();

        private readonly List<KeyedReceipt<TItem, TKey>> borrowed = new List<KeyedReceipt<TItem, TKey>>();

        private readonly EvictionCriteria evictionCriteria;

        private readonly Timer evictionTimer;

        private readonly Timer leakTimer;

        private volatile bool closed = false;

        protected AbstractKeyedPool(string instanceName, IKeyedPooledItemFactory<TItem, TKey> factory, EvictionCriteria evictionCriteria)
        {
            this.instanceName = instanceName;
            this.factory = factory;
            this.evictionCriteria = evictionCriteria;
            // setup eviction timer
            evictionTimer = new Timer();
            evictionTimer.Interval = evictionCriteria.Interval.TotalMilliseconds;
            evictionTimer.Elapsed += EvictIdleItems;
            evictionTimer.Enabled = true;
            // item leak timer
            leakTimer = new Timer();
            leakTimer.Interval = 1000;
            leakTimer.Elapsed += ProcessLeakedItems;
            leakTimer.Enabled = true;
        }

        ~AbstractKeyedPool()
        {
            Dispose(false);
        }

        protected void AssertOpen()
        {
            if (closed)
            {
                throw new PoolClosedException("Pool not open");
            }
        }

        #region KeyedPool<TItem,TKey> Members

        private readonly string instanceName;

        private readonly IKeyedPooledItemFactory<TItem, TKey> factory;

        protected event EventHandler<DeadItemEventArgs> DeadItem;

        public string InstanceName
        {
            get { return instanceName; }
        }

        public IKeyedPooledItemFactory<TItem,TKey> Factory
        {
            get { return factory; }
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Writing a pool class is complex and authors will not be put off by nested generics")]
        protected Dictionary<TKey, Threading.IQueue<TimeRecord<TItem>>> Keys
        {
            get { return keys; }
        }

        protected ReaderWriterLock KeyLock
        {
            get { return keyLock; }
        }

        public virtual TItem BorrowItem(TKey key)
        {
            return BorrowItem(key, new TimeSpan(Timeout.Infinite));
        }

        public virtual TItem BorrowItem(TKey key, TimeSpan timeout)
        {
            if (timeout.Ticks < Timeout.Infinite)
            {
                throw new ArgumentException("Timeout must be greater than or equal to Timeout.Infinite", "timeout");
            }

            AssertOpen();

            DateTime startTime = DateTime.Now;

            TItem item;

            KeyLock.AcquireReaderLock(timeout);
            try
            {
                TimeSpan waitTime = NewWaitTime(timeout, startTime);

                if (!Keys.ContainsKey(key))
                {
                    LockCookie cookie = KeyLock.UpgradeToWriterLock(waitTime);
                    try
                    {
                        if (!Keys.ContainsKey(key))
                            Keys.Add(key, NewPool(key));

                        waitTime = NewWaitTime(timeout, startTime);
                    }
                    finally
                    {
                        KeyLock.DowngradeFromWriterLock(ref cookie);
                    }
                }

                TimeRecord<TItem> record = Keys[key].Poll(new TimeSpan(0));

                if (record == null)
                {
                    item = GrowPool(key);

                    if (item == null)
                    {
                        item = Keys[key].Poll(waitTime).Item;
                    }

                    if (item == null)
                    {
                        return null;
                    }
                }
                else
                {
                    item = record.Item;
                }
            }
            finally
            {
                KeyLock.ReleaseReaderLock();
            }

            return Borrow(key, item);
        }

        private TItem Borrow(TKey key, TItem item)
        {
            try
            {
                item = Validate(key, Activate(key, item, true), true);

                lock (borrowed)
                {
                    borrowed.Add(new KeyedReceipt<TItem,TKey>(key, item));
                }

                return item;
            }
            catch (PoolLifeCycleException e)
            {
                if (DeadItem != null)
                    DeadItem(this, new DeadItemEventArgs(key));

                throw new PoolImplementationException("Failed to borrow item", e);
            }
        }

        private static TimeSpan NewWaitTime(TimeSpan timeout, DateTime startTime)
        {
            // if they want to wait forever so let them
            if (timeout.Ticks == Timeout.Infinite)
                return timeout;

            // for people on a deadline figure out how much longer they have
            TimeSpan waitTime = timeout.Subtract(DateTime.Now.Subtract(startTime));

            // if their deadline is up do not let them wait any longer
            if (waitTime.Ticks <= 0)
                return new TimeSpan(0);
            
            // otherwise let them how much longer they have left
            return waitTime;
        }

        public virtual void ReturnItem(TItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            AssertOpen();

            Predicate<KeyedReceipt<TItem,TKey>> itemFilter = delegate(KeyedReceipt<TItem,TKey> r) { return r.Match(item); };

            KeyLock.AcquireReaderLock(new TimeSpan(Timeout.Infinite));
            try
            {
                lock (borrowed)
                {
                    KeyedReceipt<TItem, TKey> receipt = borrowed.Find(itemFilter);

                    if (receipt != null)
                    {
                        // FIXME: Count the borrowed time

                        borrowed.Remove(receipt);

                        Passivate(receipt.Key, item);
                    }
                    else
                    {
                        throw new ArgumentException(string.Format(CultureInfo.InstalledUICulture, "Item not from pool '{0}'", InstanceName), "item");
                    }
                }
            }
            finally
            {
                KeyLock.ReleaseReaderLock();
            }
        }

        public virtual void Clear()
        {
            Clear(true);
        }

        protected void Clear(bool assertOpen)
        {
            if (assertOpen)
                AssertOpen();

            KeyLock.AcquireReaderLock(new TimeSpan(-1));
            try
            {
                foreach (KeyValuePair<TKey,Threading.IQueue<TimeRecord<TItem>>> entry in Keys)
                {
                    Threading.IQueue<TimeRecord<TItem>> pool = entry.Value;

                    while (!pool.IsEmpty())
                    {
                        TimeRecord<TItem> record = pool.Poll();

                        if (record != null)
                        {
                            Destroy(entry.Key, record.Item, false);
                        }
                    }
                }
            }
            finally
            {
                KeyLock.ReleaseReaderLock();
            }
        }

        public virtual void Close()
        {
            if (!closed)
            {
                closed = true;

                Clear(false);
            }
        }

        #endregion

        #region Life-Cycle Methods

        protected abstract TItem GrowPool(TKey key);

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Writing a pool class is complex and authors will not be put off by nested generics")]
        protected abstract Threading.IQueue<TimeRecord<TItem>> NewPool(TKey key);

        protected abstract void ShrinkPool(TKey key, TItem item);

        protected TItem Activate(TKey key, TItem item, bool replaceItem)
        {
            try
            {
                factory.ActivateItem(key, item);
            }
            catch (PoolLifeCycleException)
            {
                if (replaceItem)
                {
                    try
                    {
                        item = Activate(key, Replace(key, item), false);
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

        protected TItem Validate(TKey key, TItem item, bool replaceItem)
        {
            try
            {
                if (!factory.ValidateItem(key, item))
                {
                    throw new PoolLifeCycleException("Failed to validate item");
                }
            }
            catch (PoolLifeCycleException)
            {
                if (replaceItem)
                {
                    item = Validate(key, Activate(key, Replace(key, item), false), false);
                }
                else
                {
                    throw;
                }
            }

            return item;
        }

        protected abstract void Passivate(TKey key, TItem item);

        protected void Passivate(TKey key, TItem item, bool replaceItem)
        {
            try
            {
                factory.PassivateItem(key, item);
            }
            catch (PoolLifeCycleException e)
            {
                Logger.Info(e.Message);

                item = Destroy(key, item, replaceItem);
            }
            finally
            {
                if (item != null)
                    Keys[key].Offer(new TimeRecord<TItem>(item));
            }
        }

        protected TItem Destroy(TKey key, TItem item, bool replaceItem)
        {
            try
            {
                factory.DestroyItem(key, item);
            }
            catch (PoolLifeCycleException e)
            {
                Logger.Info(e.Message);
            }

            return replaceItem ? factory.MakeItem(key) : null;
        }

        protected TItem Replace(TKey key, TItem item)
        {
            return Destroy(key, item, true);
        }

        #endregion

        #region Timer Event Handlers

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Timer thread errors are put in the event viewer")]
        private void EvictIdleItems(object source, ElapsedEventArgs e)
        {
            AssertOpen();

            TimeSpan noWait = new TimeSpan(0);

            evictionTimer.Stop();

            KeyLock.AcquireReaderLock(new TimeSpan(-1));
            try
            {
                try
                {
                    foreach (KeyValuePair<TKey, Threading.IQueue<TimeRecord<TItem>>> entry in Keys)
                    {
                        Threading.IQueue<TimeRecord<TItem>> pool = entry.Value;

                        TimeRecord<TItem> record;

                        while ((record = pool.Peek()) != null)
                        {
                            if (evictionCriteria.Evict(record, pool.Size()))
                            {
                                record = pool.Poll(noWait);

                                if (record != null)
                                {
                                    ShrinkPool(entry.Key, record.Item);
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
                catch (Exception x)
                {
                    Logger.Error(x.Message);
                }
            }
            finally
            {
                KeyLock.ReleaseReaderLock();

                evictionTimer.Start();
            }
        }

        private void ProcessLeakedItems(object source, ElapsedEventArgs e)
        {
            AssertOpen();

            Predicate<KeyedReceipt<TItem, TKey>> deadFilter = delegate(KeyedReceipt<TItem, TKey> r) { return r.Dead(); };

            leakTimer.Stop();
            try
            {
                lock (borrowed)
                {
                    KeyedReceipt<TItem,TKey> receipt;

                    while ((receipt = borrowed.Find(deadFilter)) != null)
                    {
                        borrowed.Remove(receipt);

                        receipt.Dispose();

                        HandleItemLeak(receipt.Key);
                    }
                }
            }
            finally
            {
                leakTimer.Start();
            }
        }

        protected virtual void HandleItemLeak(TKey key)
        {
            Logger.Error(string.Format(CultureInfo.InstalledUICulture, "Item leaked from BoundedPool '{0}' key '{1}'", instanceName, key));
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
