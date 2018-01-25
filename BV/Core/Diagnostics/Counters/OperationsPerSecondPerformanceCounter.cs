namespace VB.Common.Core.Diagnostics.Counters
{
    class OperationsPerSecondPerformanceCounter : BasePerformanceCounter
    {
        public OperationsPerSecondPerformanceCounter(string name, PerformanceCounterType type, PerformanceCounterCategory category) : base(name, type, category)
        {

        }

        public new PerformanceCounterType CounterType
        {
            get { return PerformanceCounterType.OperationsPerSecond; }
        }

#region " interface implementation "

        override public long Increment()
        {
            Counter.Increment();
            return Counter.RawValue;
        }

        override public long IncrementBy(long value)
        {
            Counter.IncrementBy(value);
            return Counter.RawValue;
        }

#endregion


    }
}
