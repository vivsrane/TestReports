using System.Collections.Generic;
using System.Linq;
using VB.Common.Data;

namespace VB.Reports.App.ReportDefinitionLibrary
{
    class ReportDataMap : IDataMap
    {
        private IDictionary<string, IDataMapEntry> entries;

        public ReportDataMap()
        {
            entries = new Dictionary<string, IDataMapEntry>();
        }

        public ICollection<IDataMapEntry> Entries
        {
            get { return entries.Values; }
        }

        public IDataMapEntry FindField(string fieldName)
        {
            return entries[fieldName];
        }

        public IDataMapEntry FindColumn(string columnName)
        {
            foreach (DataMapEntry entry in entries.Values)
            {
                if (entry.ColumnName.Equals(columnName))
                {
                    return entry;
                }
            }
            throw new ReportException("Column: " + columnName + " does not exist in report.");
        }

        public IDataMapEntry FindOlapColumn(string columnName)
        {
            string dataColumnName = getOlapColumnName(columnName);
            if (!string.IsNullOrEmpty(dataColumnName)) dataColumnName = dataColumnName.Replace(" ", "_");

            foreach (DataMapEntry entry in entries.Values)
            {
                if (entry.ColumnName.Equals(dataColumnName))
                {
                    return entry;
                }
            }
            throw new ReportException("Column: " + columnName + " does not exist in report.");
        }

        private string getOlapColumnName(string columnName)
        {
            columnName = columnName.Replace("[", "").Replace("]", "");
           
            string[] objSplit = columnName.Split(".".ToCharArray());
            int lst =  objSplit.Select((s, i) => new {i, s})
                    .Where(t => t.s == "MEMBER_CAPTION")
                    .Select(t => t.i)
                    .ToList().FirstOrDefault();
                if (lst > 0)
                {
                    return objSplit[lst - 1];
                }
            return objSplit[objSplit.Length - 1];
        }
        public void AddDataMapEntry(IDataMapEntry entry)
        {
            if (entries.Keys.Contains(entry.FieldName))
            {
                throw new ReportException("Field name: " + entry.FieldName + " already exists in ReportDatamap.");
            }
            entries.Add(entry.FieldName, entry);
        }
    }
}
