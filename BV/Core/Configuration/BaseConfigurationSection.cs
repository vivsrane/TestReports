using System;
using System.Configuration;
using System.Xml;

namespace VB.Common.Core.Configuration
{
    public abstract class BaseConfigurationSection : ConfigurationSection
    {
        public void Deserialize(XmlReader reader)
        {
            if (reader == null) throw new ArgumentNullException("XmlReader reader");

            this.DeserializeSection(reader);
        }
    }
}
