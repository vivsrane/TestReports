using System;
using System.Collections.Generic;
using System.Data;

namespace VB.Common.Data
{
    internal class IdentityDataMap : IDataMap
    {
        private readonly Dictionary<string, IDataMapEntry> entryMap;

        public IdentityDataMap(DataTable schemaTable)
        {
            entryMap = new Dictionary<string, IDataMapEntry>();

            for (int i = 0; i < schemaTable.Rows.Count; i++)
            {
                string name = schemaTable.Rows[i]["ColumnName"].ToString();
                string type = ((Type)schemaTable.Rows[i]["DataType"]).FullName;

                DataMapEntry entry = new DataMapEntry(name, name, type);
                entryMap.Add(name, entry);
            }
        }

        #region IDataMap Members

        public ICollection<IDataMapEntry> Entries
        {
            get { return entryMap.Values; }
        }

        public IDataMapEntry FindField(string fieldName)
        {
            return entryMap[fieldName];
        }

        public IDataMapEntry FindColumn(string columnName)
        {
            return entryMap[columnName];
        }
        public IDataMapEntry FindOlapColumn(string columnName)
        {
            return entryMap[columnName];
        }
        #endregion
    }
}
