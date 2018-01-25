using System.Diagnostics;

namespace VB.Common.Core.Diagnostics.Counters
{

    public class AverageOperationTimePerformanceCounter : BasePerformanceCounter 
    {
        private readonly Stopwatch _stopwatch;

        public Stopwatch Stopwatch
        {
            get { return _stopwatch; }
        }

        public new PerformanceCounterType CounterType
        {
            get { return PerformanceCounterType.AverageOperationTime; }
        }

#region " constructors "

        public AverageOperationTimePerformanceCounter(string name, PerformanceCounterType type, PerformanceCounterCategory category) : base(name, type, category)
        {
            _stopwatch = new Stopwatch();
        }

#endregion


#region " interface implementation "

		override public long Increment()
		{
            _stopwatch.Stop();
            Counter.IncrementBy(_stopwatch.ElapsedTicks);
            BaseCounter.Increment();
            _stopwatch.Reset();
            return Counter.RawValue;
		}

        override public long IncrementBy(long value)
        {
            Counter.IncrementBy(value);
            BaseCounter.Increment();
            return Counter.RawValue;
        }

#endregion


	}
}
