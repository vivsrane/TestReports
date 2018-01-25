using System;

namespace VB.Common.Core
{
    /// <summary>
    /// Class that contains summary information on a sample data-set.
    /// </summary>
    /// <typeparam name="T">Type of the data-item</typeparam>
    [Serializable]
    public class SampleSummary<T> where T : struct
    {
        private readonly T? minimum;
        private readonly T? average;
        private readonly T? maximum;
        private readonly int sampleSize;

        public SampleSummary()
        {
        }

        public SampleSummary(T? minimum, T? average, T? maximum, int sampleSize)
        {
            this.minimum = minimum;
            this.average = average;
            this.maximum = maximum;
            this.sampleSize = sampleSize;
        }

        public T? Minimum
        {
            get { return minimum; }
        }

        public T? Average
        {
            get { return average; }
        }

        public T? Maximum
        {
            get { return maximum; }
        }

        public int SampleSize
        {
            get { return sampleSize; }
        }
    }
}