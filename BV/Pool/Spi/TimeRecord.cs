using System;

namespace VB.Common.Pool.Spi
{
    public class TimeRecord<TItem>
    {
        private readonly TItem item;

        private readonly DateTime created = DateTime.Now;

        public TimeRecord(TItem item)
        {
            this.item = item;
        }

        public TItem Item
        {
            get { return item; }
        }

        public DateTime Created
        {
            get { return created; }
        }
    }
}
