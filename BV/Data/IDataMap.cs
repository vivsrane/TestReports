using System.Collections.Generic;

namespace VB.Common.Data
{
    public interface IDataMap
    {
        ICollection<IDataMapEntry> Entries
        {
            get;
        }

        IDataMapEntry FindField(string fieldName);

        IDataMapEntry FindColumn(string columnName);
        IDataMapEntry FindOlapColumn(string columnName);
    }
}
