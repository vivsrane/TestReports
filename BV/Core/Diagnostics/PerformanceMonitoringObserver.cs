using VB.Common.Core.Command;
using VB.Common.Core.Diagnostics.Counters;

namespace VB.Common.Core.Diagnostics
{
    public class PerformanceMonitoringObserver : ICommandObserver
    {
        
        public PerformanceMonitoringObserver()
        {
            // empty constructor
        }

        public void OnInvoke(CommandEventArgs e)
        {
            foreach (IPerformanceCounter c in PerformanceCounterRegistry.Fetch(e.CommandType))
            {
                var counter = (IPerformanceCounter)c;
                switch (counter.CounterType)
                {
                    case PerformanceCounterType.AverageOperationTime:
                        ((AverageOperationTimePerformanceCounter)counter).Stopwatch.Start();
                        break;
                }
            }
        }

        public void OnInvokeComplete(CommandEventArgs e)
        {
            foreach (IPerformanceCounter c in PerformanceCounterRegistry.Fetch(e.CommandType))
            {
                var counter = (IPerformanceCounter)c;
                switch (counter.CounterType)
                {
                    case PerformanceCounterType.AverageOperationTime:
                    case PerformanceCounterType.OperationsPerSecond:
                        counter.Increment();
                        break;
                    case PerformanceCounterType.OperationSuccessRatio:
                        ((OperationSuccessRatioPerformanceCounter)counter).OperationSuccessful = true;
                        counter.Increment();
                        break;
                }
            }

            
        }

        public void OnInvokeException(CommandExceptionEventArgs e)
        {
            foreach (IPerformanceCounter c in PerformanceCounterRegistry.Fetch(e.CommandType))
            {
                var counter = (IPerformanceCounter)c;
                switch (counter.CounterType)
                {
                    case PerformanceCounterType.AverageOperationTime: 
                    case PerformanceCounterType.OperationSuccessRatio:
                    case PerformanceCounterType.OperationsPerSecond:
                        counter.Increment();
                        break;
                }
            }
        }

    }
}
