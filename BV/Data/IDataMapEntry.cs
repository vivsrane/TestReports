namespace VB.Common.Data
{
    public interface IDataMapEntry
    {
        string ColumnName
        {
            get;
        }

        string FieldName
        {
            get;
        }

        string TypeName
        {
            get;
        }
    }
}
