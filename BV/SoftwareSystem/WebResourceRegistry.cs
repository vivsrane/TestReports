using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Xml.Serialization;
using BV.DomainModel.SoftwareSystem.Xml;

namespace BV.DomainModel.SoftwareSystem
{
    public class WebResourceRegistry
    {
        private readonly string path;

        private Xml.WebResourceRegistry registry;

        public WebResourceRegistry(String path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            if (!new FileInfo(path).Exists)
                throw new FileNotFoundException(path);

            this.path = path;
        }

        public string SoftwareSystemComponent(string page)
        {
            AssertAndInitialize(page);

            return FindComponent(page).Name;
        }
        
        public ErrorAction OnError(string page)
        {
            AssertAndInitialize(page);

            try
            {
                return (ErrorAction) Enum.Parse(typeof (ErrorAction), FindPage(page).OnError.ToString());
            }
            catch (IndexOutOfRangeException)
            {
                return ErrorAction.Close;
            }
        }

        public string OnErrorTarget(string page)
        {
            AssertAndInitialize(page);

            Page p = FindPage(page);

            if (p.OnError.Equals(Xml.OnError.Home))
            {
                return FindComponent(page).Home;
            }
            else
            {
                return p.Target;
            }
        }

        private void AssertAndInitialize(string page)
        {
            if (string.IsNullOrEmpty(page))
                throw new ArgumentNullException("page");

            if (registry == null)
                Load();
        }

        [SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes")]
        private Page FindPage(string page)
        {
            foreach (SoftwareSystemComponent c in GetSoftwareSystemComponent())
                foreach (Page p in c.Page)
                    if (string.Equals(p.Path, page, StringComparison.CurrentCultureIgnoreCase))
                        return p;

            throw new IndexOutOfRangeException(string.Format(CultureInfo.CurrentUICulture, "No such page: {0}", page));
        }

        [SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes")]
        private SoftwareSystemComponent FindComponent(string page)
        {
            foreach (SoftwareSystemComponent c in GetSoftwareSystemComponent())
                foreach (Page p in c.Page)
                    if (string.Equals(p.Path, page, StringComparison.CurrentCultureIgnoreCase))
                        return c;

            throw new IndexOutOfRangeException(string.Format(CultureInfo.CurrentUICulture, "No such page: {0}", page));
        }

        private IEnumerable<SoftwareSystemComponent> GetSoftwareSystemComponent()
        {
            if (registry.SoftwareSystemComponent == null)
                return new List<SoftwareSystemComponent>();
            return registry.SoftwareSystemComponent;

        }

        private void Load()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Xml.WebResourceRegistry));

            using (Stream s = File.OpenRead(path))
            {
                registry = (Xml.WebResourceRegistry) serializer.Deserialize(s);
            }
        }
    }
}
