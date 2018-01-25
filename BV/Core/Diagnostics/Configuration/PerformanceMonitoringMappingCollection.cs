using System;
using System.Configuration;

namespace VB.Common.Core.Diagnostics.Configuration
{
    public class PerformanceMonitoringMappingCollection : ConfigurationElementCollection
    {
        public PerformanceMonitoringMappingCollection() { }

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new PerformanceMonitoringMappingElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            var me = (PerformanceMonitoringMappingElement)element;
            return string.Format("{0}_{1}", me.Process, me.Type);
        }

        public void Add(PerformanceMonitoringMappingElement data)
        {
            BaseAdd(data);
        }

        public void Remove(Type T)
        {
            BaseRemove(T);
        }

        public PerformanceMonitoringMappingElement this[Type T]
        {
            get { return (PerformanceMonitoringMappingElement)BaseGet(T); }
        }

        public PerformanceMonitoringMappingElement this[int index]
        {
            get { return (PerformanceMonitoringMappingElement)BaseGet(index); }
        }
    }
}
