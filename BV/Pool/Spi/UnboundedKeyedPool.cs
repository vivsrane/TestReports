using VB.Common.Threading;

namespace VB.Common.Pool.Spi
{
    public class UnboundedKeyedPool<TItem,TKey> : AbstractKeyedPool<TItem,TKey> where TItem : class
    {
        public UnboundedKeyedPool(string instanceName, IKeyedPooledItemFactory<TItem, TKey> factory, EvictionCriteria evictionCriteria)
            : base(instanceName, factory, evictionCriteria)
        {
        }

        protected override TItem GrowPool(TKey key)
        {
            return Factory.MakeItem(key);
        }

        protected override IQueue<TimeRecord<TItem>> NewPool(TKey key)
        {
            return new LinkedQueue<TimeRecord<TItem>>();
        }

        protected override void ShrinkPool(TKey key, TItem item)
        {
            Destroy(key, item, false);
        }

        protected override void Passivate(TKey key, TItem item)
        {
            Passivate(key, item, false);
        }
    }
}
