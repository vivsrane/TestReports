using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace VB.Reports.App.ReportDefinitionLibrary
{
    public class ReportDefinitionCache
    {
        [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible", Justification = "All accesses to this field are made under locks")]
        protected static IDictionary<string, ReportDefinitionCache> cache = new Dictionary<string, ReportDefinitionCache>();

        public static ReportDefinitionCache GetReportDefinition(string fileName)
        {
            ValidateFileName(fileName);

            ReportDefinitionCache reportDefinitionCache;

            lock (cache)
            {
                if (cache.Keys.Contains(fileName))
                {
                    reportDefinitionCache = cache[fileName];
                }
                else
                {
                    reportDefinitionCache = new ReportDefinitionCache(fileName);

                    cache.Add(fileName, reportDefinitionCache);
                }
            }

            return reportDefinitionCache;
        }

        protected static void ValidateFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException("fileName", "File name cannot be null or empty.");
            }

            FileInfo info = new FileInfo(fileName);

            if (!info.Exists)
            {
                throw new FileNotFoundException("File '" + fileName + "' does not exist.");
            }
        }

        private readonly string fileName;
        private DateTime lastWriteTime;
        private DateTime lastCheckTime;
        private byte[] buffer;

        public ReportDefinitionCache(string fileName)
        {
            this.fileName = fileName;
        }

        protected ReportDefinitionCache(ReportDefinitionCache reportDefinitionCache)
        {
            fileName = reportDefinitionCache.fileName;
            lastWriteTime = reportDefinitionCache.lastWriteTime;
            lastCheckTime = reportDefinitionCache.lastCheckTime;
            buffer = reportDefinitionCache.buffer;
        }

        public string FileName
        {
            get { return fileName; }
        }

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Operation is expensive")]
        public Stream GetReportStream()
        {
            MemoryStream stream;

            lock (this)
            {
                if (IsNew() || IsUpdated())
                {
                    Load();
                }

                stream = new MemoryStream(buffer, false);
            }

            return stream;
        }

        private void Load()
        {
            XmlDocument document = new XmlDocument();

            document.Load(fileName);

            XmlElement root = document.DocumentElement;

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(document.NameTable);

            nsmgr.AddNamespace("r", document.DocumentElement.NamespaceURI);

            nsmgr.AddNamespace("rd", document.DocumentElement.Attributes["xmlns:rd"].Value);

            XmlNode nodeMdx = root.SelectSingleNode(@"/r:Report/r:DataSets/r:DataSet/r:Fields/r:Field/r:DataField[contains(text(), 'xsi:type=""Level""')]", nsmgr);

            if(nodeMdx == null) nodeMdx = root.SelectSingleNode(@"/r:Report/r:DataSets/r:DataSet/r:Fields/r:Field/r:DataField[contains(text(), 'xsi:type=""Measure""')]", nsmgr); 
            // Make changes only if the rdl file contains mdx data

            if (nodeMdx != null)
            {
                XmlNodeList nodes = root.SelectNodes(@"/r:Report/r:DataSets/r:DataSet/r:Fields/r:Field/r:DataField", nsmgr);

                foreach (XmlNode node in nodes)
                {
                    XmlDocument fieldDoc = new XmlDocument();

                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (StreamWriter writer = new StreamWriter(ms))
                        {
                            try
                            {
                                writer.Write(node.FirstChild.Value);
                                writer.Flush();
                                ms.Seek(0, SeekOrigin.Begin);
                                fieldDoc.Load(ms);
                            }
                            catch 
                            {
                                    continue;
                            }
                           

                            XmlAttribute uniqueNameAttribute = fieldDoc.DocumentElement.Attributes["UniqueName"];


                            if (fieldDoc.DocumentElement.Attributes["xsi:type"].Value.Equals("Level"))
                            {
                                //node.InnerXml = getOlapColumnName(uniqueNameAttribute.Value);
                               node.InnerXml= uniqueNameAttribute.Value + ".[MEMBER_CAPTION]";
                            }
                            else
                            {
                                node.InnerXml = uniqueNameAttribute.Value;
                            }
                        }
                    }
                }
            }

            StringWriter stringWriter = new StringWriter(new StringBuilder(), CultureInfo.InvariantCulture);

            document.Save(stringWriter);

            buffer = Encoding.Unicode.GetBytes(stringWriter.ToString());

            lastWriteTime = File.GetLastWriteTime(fileName);
            lastCheckTime = lastWriteTime;
        }

        protected virtual bool IsNew()
        {
            return buffer == null;
        }

        protected virtual bool IsUpdated()
        {
            if (DateTime.Now.Subtract(lastCheckTime).TotalMinutes > 5)
            {
                lastCheckTime = DateTime.Now;

                return File.GetLastWriteTime(fileName).CompareTo(lastWriteTime) > 0;
            }
            else
            {
                return false;
            }
        }
    }
}