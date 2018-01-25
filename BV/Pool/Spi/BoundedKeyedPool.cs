using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using VB.Common.Threading;

namespace VB.Common.Pool.Spi
{
    public class BoundedKeyedPool<TItem,TKey> : AbstractKeyedPool<TItem,TKey> where TItem : class
    {
        private readonly Bounds bounds;

        private readonly Dictionary<TKey, int> keyAllocation = new Dictionary<TKey, int>();

        private int totalAllocation = 0;

        public BoundedKeyedPool(string instanceName, IKeyedPooledItemFactory<TItem, TKey> factory, Bounds bounds, EvictionCriteria evictionCriteria)
            : base(instanceName, factory, evictionCriteria)
        {
            this.bounds = bounds;

            DeadItem += OnDeadItem;
        }

        protected override TItem GrowPool(TKey key)
        {
            TItem item = null;

            lock (this)
            {
                int slotsFree = bounds.MaxCount - totalAllocation;

                if (slotsFree == 0)
                {
                    // try and evict a single item from the most heavily populated key

                    List<KeyValuePair<TKey, int>> ordering = new List<KeyValuePair<TKey, int>>();

                    Dictionary<TKey, int>.Enumerator en = keyAllocation.GetEnumerator();

                    while (en.MoveNext()) ordering.Add(en.Current);

                    ordering.Sort(delegate(KeyValuePair<TKey, int> k1, KeyValuePair<TKey, int> k2) { return k1.Value.CompareTo(k2.Value); });

                    for (int i = ordering.Count-1; i >= 0; i--)
                    {
                        TimeRecord<TItem> record = Keys[ordering[i].Key].Poll(new TimeSpan(0));

                        if (record != null)
                        {
                            ShrinkPool(ordering[i].Key, record.Item);
                            ++slotsFree;
                            break;
                        }
                    }
                }

                if (slotsFree > 0)
                {
                    Interlocked.Increment(ref totalAllocation);

                    if (keyAllocation.ContainsKey(key))
                        keyAllocation[key] = keyAllocation[key] + 1;
                    else
                        keyAllocation.Add(key, 1);

                    item = Factory.MakeItem(key);
                }
            }

            return item;
        }

        protected override Threading.IQueue<TimeRecord<TItem>> NewPool(TKey key)
        {
            return new LinkedQueue<TimeRecord<TItem>>();
        }

        protected override void Passivate(TKey key, TItem item)
        {
            Passivate(key, item, true);
        }

        protected override void ShrinkPool(TKey key, TItem item)
        {
            lock (this)
            {
                Interlocked.Decrement(ref totalAllocation);

                keyAllocation[key] = keyAllocation[key] - 1;
            }

            Destroy(key, item, false);
        }

        protected override void HandleItemLeak(TKey key)
        {
            base.HandleItemLeak(key);

            OfferToPool(key);
        }

        private void OnDeadItem(object sender, DeadItemEventArgs e)
        {
            if (e.Key is TKey)
            {
                OfferToPool((TKey) e.Key);
            }
            else
            {
                Logger.Error(string.Format(CultureInfo.InstalledUICulture, "Key type mismatch {0}. Expected {1}", e.Key.GetType(), typeof(TKey)));
            }
        }

        private void OfferToPool(TKey key)
        {
            KeyLock.AcquireReaderLock(new TimeSpan(-1));
            try
            {
                Keys[key].Offer(new TimeRecord<TItem>(Factory.MakeItem(key)));
            }
            finally
            {
                KeyLock.ReleaseReaderLock();
            }
        }
    }
}
