using System.Threading;
using VB.Common.Threading;

namespace VB.Common.Pool.Spi
{
    public class BoundedPool<TItem> : AbstractPool<TItem> where TItem : class
    {
        private readonly Bounds bounds;

        private int allocated = 0;

        public BoundedPool(string instanceName, IPooledItemFactory<TItem> factory, Bounds bounds, EvictionCriteria criteria)
            : base(instanceName, factory, new BoundedLinkedQueue<TimeRecord<TItem>>(bounds.MaxCount), criteria)
        {
            this.bounds = bounds;

            PreFill();

            DeadItem += OnDeadItem;
        }

        protected void PreFill()
        {
            for (int i = 0; i < bounds.MinCount; i++)
            {
                Pool.Offer(new TimeRecord<TItem>(Factory.MakeItem()));
            }

            allocated = bounds.MinCount;
        }

        protected override TItem GrowPool()
        {
            TItem item = null;

            lock (this)
            {
                int slotsFree = bounds.MaxCount - allocated;

                if (slotsFree > 0)
                {
                    Interlocked.Increment(ref allocated);

                    item = Factory.MakeItem();
                }
            }

            return item;
        }

        protected override void ShrinkPool(TItem item)
        {
            lock (this)
            {
                Interlocked.Decrement(ref allocated);
            }

            base.ShrinkPool(item);
        }

        protected override void Passivate(TItem item)
        {
            Passivate(item, true);
        }

        protected override void HandleItemLeak(int count)
        {
            base.HandleItemLeak(count);

            for (int i = 0; i < count; i++)
            {
                Pool.Offer(new TimeRecord<TItem>(Factory.MakeItem()));
            }
        }

        private void OnDeadItem(object sender, DeadItemEventArgs e)
        {
            Pool.Offer(new TimeRecord<TItem>(Factory.MakeItem()));
        }
    }
}
