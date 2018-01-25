using System;

namespace VB.Common.Pool.Spi
{
    public class EvictionCriteria
    {
        private readonly TimeSpan interval;
        private readonly TimeSpan idleTime;
        private readonly int idleCount;
        private readonly TimeSpan idleCountTime;

        public EvictionCriteria(TimeSpan interval, TimeSpan idleTime, int idleCount, TimeSpan idleCountTime)
        {
            this.interval = interval;
            this.idleTime = idleTime;
            this.idleCount = idleCount;
            this.idleCountTime = idleCountTime;
        }

        public TimeSpan IdleTime
        {
            get { return idleTime; }
        }

        public int IdleCount
        {
            get { return idleCount; }
        }

        public TimeSpan IdleCountTime
        {
            get { return idleCountTime; }
        }

        public TimeSpan Interval
        {
            get { return interval; }
        }

        public bool Evict<TItem>(TimeRecord<TItem> record, int numberIdle)
        {
            bool evict = false;

            if (idleTime.Ticks > 0 )
            {
                if (Ticks(idleTime, record.Created) < 0)
                {
                    evict = true;
                }
            }

            if (idleCountTime.Ticks > 0)
            {
                if (numberIdle > idleCount && Ticks(idleCountTime, record.Created) < 0)
                {
                    evict = true;
                }
            }

            return evict;
        }

        private static long Ticks(TimeSpan span, DateTime time)
        {
            return span.Subtract(DateTime.Now.Subtract(time)).Ticks;
        }
    }
}
