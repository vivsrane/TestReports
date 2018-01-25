using System;
using System.ComponentModel;
using System.Diagnostics;

namespace VB.Common.Core.Diagnostics
{
    /// <code>installutil "C:\full\path\to\VB.Common.Core.dll"</code>.
    /// <see cref="http://msdn2.microsoft.com/en-us/library/50614e95(VS.80).aspx"/>
    [RunInstaller(true)]
    public partial class PerformanceCounterInstaller : System.Configuration.Install.Installer
    {
        public PerformanceCounterInstaller()
        {
            InstallCounters();
        }

        private static void InstallCounters()
        {
            foreach (var ctgr in Enum.GetValues(typeof(PerformanceCounterCategory)))
            {
                var category = ctgr;

                if (System.Diagnostics.PerformanceCounterCategory.Exists(category.ToString())) System.Diagnostics.PerformanceCounterCategory.Delete(category.ToString());

                var ccdc = new CounterCreationDataCollection();

                foreach (PerformanceCounterType cntr in Enum.GetValues(typeof(PerformanceCounterType)))
                {
                    var counterType = cntr;
                    switch (counterType)
                    {
                        case PerformanceCounterType.AverageOperationTime:
                            var avgOpsTime = new CounterCreationData(counterType.ToString(), "Ticks per operation", System.Diagnostics.PerformanceCounterType.AverageTimer32);
                            var avgOpsTimeBase = new CounterCreationData(string.Format("{0}Base", counterType), "Number of operations", System.Diagnostics.PerformanceCounterType.AverageBase);

                            ccdc.Add(avgOpsTime);
                            ccdc.Add(avgOpsTimeBase);

                            break;
                        case PerformanceCounterType.OperationsPerSecond:
                            var operationsPerSecond = new CounterCreationData(counterType.ToString(), "Operations per second", System.Diagnostics.PerformanceCounterType.RateOfCountsPerSecond32);
                            ccdc.Add(operationsPerSecond);

                            break;
                        case PerformanceCounterType.OperationSuccessRatio:
                            var avgSuccessRatio = new CounterCreationData(counterType.ToString(), "Count of successful operations", System.Diagnostics.PerformanceCounterType.AverageCount64);
                            var avgSuccessRatioBase = new CounterCreationData(string.Format("{0}Base", counterType), "Count of all operations", System.Diagnostics.PerformanceCounterType.AverageBase);

                            ccdc.Add(avgSuccessRatio);
                            ccdc.Add(avgSuccessRatioBase);
                            break;
                    }
                }

                // Create the category
                System.Diagnostics.PerformanceCounterCategory.Create(category.ToString(), "VB custom counters", PerformanceCounterCategoryType.MultiInstance, ccdc);
                System.Diagnostics.PerformanceCounter.CloseSharedResources(); // need this call after the creation of category to prevent cashing issues
                
            }
        }
    }
}
