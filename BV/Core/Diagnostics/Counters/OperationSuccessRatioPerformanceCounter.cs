using System;
using System.ComponentModel;

namespace VB.Common.Core.Diagnostics.Counters
{
    class OperationSuccessRatioPerformanceCounter : BasePerformanceCounter
    {
        [DefaultValue(false)]
        public bool OperationSuccessful { get; set; }

        public OperationSuccessRatioPerformanceCounter(string name, PerformanceCounterType type, PerformanceCounterCategory category) : base(name, type, category)
        {

        }

        public new PerformanceCounterType CounterType
        {
            get { return PerformanceCounterType.OperationSuccessRatio; }
        }

        #region " interface implementation "

        override public long Increment()
        {
            if (OperationSuccessful) { Counter.Increment(); }
            BaseCounter.Increment();

            return Counter.RawValue;
        }

        override public long IncrementBy(long value)
        {
            throw new NotImplementedException();
        }


#endregion
    }
}
