namespace VB.Common.Data
{
    public class DataMapEntry : IDataMapEntry
    {
        private string columnName;
        private string fieldName;
        private string typeName;

        public DataMapEntry()
        {
        }

        public DataMapEntry(string columnName, string fieldName, string typeName)
        {
            this.columnName = columnName;
            this.fieldName = fieldName;
            this.typeName = typeName;
        }

        #region IDataMapEntry Members

        public string ColumnName
        {
            get{ return columnName;}
            set{ columnName = value;}
        }

        public string FieldName
        {
            get{return fieldName;}
            set { fieldName = value; }
        }

        public string TypeName
        {
            get{return typeName;}
            set { typeName = value; }
        }

        #endregion
    }
}
