using System;

namespace VB.Common.Core.Diagnostics
{
    public interface IPerformanceCounter : IDisposable 
    {
        long Increment();

        long IncrementBy(long value);

        PerformanceCounterType CounterType { get; }
    }
}


