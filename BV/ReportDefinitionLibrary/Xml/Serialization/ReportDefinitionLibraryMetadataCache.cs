using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace VB.Reports.App.ReportDefinitionLibrary.Xml.Serialization
{
    internal class ReportDefinitionLibraryMetadataCache
    {
        private static readonly IDictionary<string, ReportDefinitionLibraryMetadataEntry> cache = new Dictionary<string, ReportDefinitionLibraryMetadataEntry>();

        public static object GetReportDefinitionLibraryMetadata(string path, Type xmlSerializedType)
        {
            object metadata;

            lock (cache)
            {
                if (cache.Keys.Contains(path) && !IsReportStale(path, cache[path].LastWriteTime))
                {
                    metadata = cache[path].Metadata;
                }
                else
                {
                    if (cache.Keys.Contains(path))
                    {
                        cache.Remove(path);
                    }

                    ReportDefinitionLibraryMetadataEntry newEntry = LoadMetadata(path, xmlSerializedType);
                    cache.Add(path, newEntry);
                    metadata = newEntry.Metadata;
                }
            }

            return metadata;
        }

        private static Boolean IsReportStale(string path, DateTime lastWriteTime)
        {
            return File.GetLastWriteTime(path).CompareTo(lastWriteTime) > 0;
        }

        private static ReportDefinitionLibraryMetadataEntry LoadMetadata(string path, Type xmlSerializedType)
        {
            ReportDefinitionLibraryMetadataEntry newEntry = new ReportDefinitionLibraryMetadataEntry();

            XmlSerializer serializer = new XmlSerializer(xmlSerializedType);

            using (Stream s = File.OpenRead(path))
            {
                newEntry.Metadata = serializer.Deserialize(s);
                newEntry.LastWriteTime = File.GetLastWriteTime(path);
            }

            return newEntry;
        }

        private class ReportDefinitionLibraryMetadataEntry
        {
            private object metadata;
            private DateTime lastWriteTime;

            public object Metadata
            {
                set { metadata = value; }
                get { return metadata; }
            }

            public DateTime LastWriteTime
            {
                set { lastWriteTime = value; }
                get { return lastWriteTime; }
            }
        }
    }
}