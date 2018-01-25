using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using VB.Common.Core.Diagnostics.Configuration;

namespace VB.Common.Core.Diagnostics
{
    public class PerformanceCounterRegistry
    {
        private static Dictionary<string, List<IPerformanceCounter>> _counterRegistry = new Dictionary<string, List<IPerformanceCounter>>();
        private static bool _enabled = ((Configuration.PerformanceMonitoringConfigurationSection)ConfigurationManager.GetSection(PerformanceMonitoringConfigurationSection.SectionName)).Enabled;
        
        public static List<IPerformanceCounter> Fetch(Type T)
        {
            var counters = new List<IPerformanceCounter>();
            if (!_enabled) return counters;

            if (!string.IsNullOrEmpty(T.FullName)) {

                if (!_counterRegistry.ContainsKey(T.FullName)) {
                    _counterRegistry.Add(T.FullName, GetCounters(T.FullName));
                }

                counters.AddRange(_counterRegistry[T.FullName]);
            }

            return counters;   
        }

        private static List<IPerformanceCounter> GetCounters(string processName)
        {
            var counters = new List<IPerformanceCounter>();
            var settings = (Configuration.PerformanceMonitoringConfigurationSection)ConfigurationManager.GetSection(PerformanceMonitoringConfigurationSection.SectionName);

            (from Configuration.PerformanceMonitoringMappingElement me in settings.Mappings
                    where me.Process == processName
                    select me).ToList().ForEach(i => counters.Add(PerformanceCounterFactory.Create(i)));

            return counters;
        }
    }
}
