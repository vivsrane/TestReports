using VB.Common.Threading;

namespace VB.Common.Pool.Spi
{
    public class UnboundedPool<TItem> : AbstractPool<TItem> where TItem : class
    {
        private readonly Bounds bounds;

        public UnboundedPool(string instanceName, IPooledItemFactory<TItem> factory, Bounds bounds, EvictionCriteria criteria)
            : base(instanceName, factory, new LinkedQueue<TimeRecord<TItem>>(), criteria)
        {
            this.bounds = bounds;

            PreFill();
        }

        protected void PreFill()
        {
            for (int i = 0; i < bounds.MinCount; i++)
            {
                Pool.Offer(new TimeRecord<TItem>(Factory.MakeItem()));
            }
        }

        protected override TItem GrowPool()
        {
            return Factory.MakeItem();
        }

        protected override void Passivate(TItem item)
        {
            Passivate(item, false);
        }
    }
}
