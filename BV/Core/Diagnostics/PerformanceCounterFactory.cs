using System.Linq;
using VB.Common.Core.Diagnostics.Counters;

namespace VB.Common.Core.Diagnostics
{
    class PerformanceCounterFactory
    {
        public static IPerformanceCounter Create(Configuration.PerformanceMonitoringMappingElement mapping)
        {
            IPerformanceCounter counter = null;

            string name = string.Format("{0}{1}", mapping.Process.Split('.').Last(), mapping.Type);

            switch (mapping.Type) {

                case PerformanceCounterType.AverageOperationTime:
                    counter = new AverageOperationTimePerformanceCounter(name, mapping.Type, mapping.Category);
                    break;
                case PerformanceCounterType.OperationsPerSecond:
                    counter = new OperationsPerSecondPerformanceCounter(name, mapping.Type, mapping.Category);
                    break;
                case PerformanceCounterType.OperationSuccessRatio:
                    counter = new OperationSuccessRatioPerformanceCounter(name, mapping.Type, mapping.Category);
                    break;
            }

            return counter;
        }

    }
    
    public enum PerformanceCounterCategory
    {
        VBCommon = 0     //,
        //VBOther = 1
    }

    public enum PerformanceCounterType
    {
        OperationsPerSecond = 0,
        AverageOperationTime = 1,
        OperationSuccessRatio = 2  

    }
}
