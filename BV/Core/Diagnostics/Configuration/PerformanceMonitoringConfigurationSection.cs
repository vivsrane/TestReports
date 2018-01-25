using System.Configuration;
using VB.Common.Core.Configuration;

namespace VB.Common.Core.Diagnostics.Configuration
{
    public class PerformanceMonitoringConfigurationSection : BaseConfigurationSection
    {
        public static readonly string SectionName = "performanceMonitoring";
        public static readonly PerformanceMonitoringConfigurationSection Current = (PerformanceMonitoringConfigurationSection)ConfigurationManager.GetSection(PerformanceMonitoringConfigurationSection.SectionName);

        private const string MappingCollectionProperty = "mapping";
        private const string EnabledProperty = "enabled";

        [ConfigurationProperty(MappingCollectionProperty)]
        [ConfigurationCollection(typeof(PerformanceMonitoringMappingElement))]
        public PerformanceMonitoringMappingCollection Mappings
        {
            get
            {
                return (PerformanceMonitoringMappingCollection)base[MappingCollectionProperty];
            }
        }

        [ConfigurationProperty(EnabledProperty)]
        public bool Enabled
        {
            get
            {
                return (bool)this[EnabledProperty];
            }
            set
            {
                this[EnabledProperty] = value;
            }
        }

    }
}
