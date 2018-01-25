using System;

namespace VB.Common.Pool.Spi
{
    public class Bounds
    {
        private readonly int minCount;
        private readonly int maxCount;

        public Bounds(int minCount, int maxCount)
        {
            if (minCount < 0)
                throw new ArgumentException("Must be greater or equal to zero", "minCount");

            if (maxCount <= 0 || maxCount < minCount)
                throw new ArgumentException("Must be greater than zero -and- greater or equal to minCount", "maxCount");

            this.minCount = minCount;
            this.maxCount = maxCount;
        }

        public int MinCount
        {
            get { return minCount; }
        }

        public int MaxCount
        {
            get { return maxCount; }
        }
    }
}
