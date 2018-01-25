using System.Configuration;

namespace VB.Common.Core.Diagnostics.Configuration
{
    public class PerformanceMonitoringMappingElement : ConfigurationElement
    {
        private const string ProcessType = "processType";
        private const string CounterCategory = "counterCategory";
        private const string CounterType = "counterType";

        [ConfigurationProperty(ProcessType, IsRequired = true)]
        public string Process
        {
            get
            {
                return (string)this[ProcessType];
            }
            set
            {
                this[ProcessType] = value;
            }
        }

        [ConfigurationProperty(CounterCategory, IsRequired = true)]
        public PerformanceCounterCategory Category
        {
            get
            {
                return (PerformanceCounterCategory)this[CounterCategory];
            }
            set
            {
                this[CounterCategory] = value;
            }
        }

        [ConfigurationProperty(CounterType, IsRequired = true)]
        public PerformanceCounterType Type
        {
            get
            {
                return (PerformanceCounterType)this[CounterType];
            }
            set
            {
                this[CounterType] = value;
            }
        }
    }
}
