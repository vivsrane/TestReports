using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;

namespace VB.Common.Data
{

    /// <summary>
    /// This class is a call back that allows the user of the data Table mapper to add custom columns to a dataTable.
    /// This class is meant to be extended.  This is a dummy implmentation that does nothing.
    /// 
    /// There are 2 methods on this class.  A New Columns property that returns an array of DataColumns that will be added to the dataTable.
    /// And a RegisterRow method that takes in a row, populates all the columns added by the NewColumns Property.
    /// 
    /// </summary>
    public class DataTableCallback
    {
        private readonly List<DataColumn> _newColumns = new List<DataColumn>();

        public virtual ReadOnlyCollection<DataColumn> NewColumns
        {
            get { return new ReadOnlyCollection<DataColumn>(_newColumns); }
        }

        public virtual bool RegisterRow(DataRow row)
        {
            return true;
        }
    }
}
