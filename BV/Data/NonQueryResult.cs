namespace VB.Common.Data
{
    public sealed class NonQueryResult
    {
        private readonly object primaryKey;
        private readonly int rowsAffected;

        public NonQueryResult(object primaryKey, int rowsAffected)
        {
            this.primaryKey = primaryKey;
            this.rowsAffected = rowsAffected;
        }

        public object PrimaryKey
        {
            get { return primaryKey; }
        }

        public int RowsAffected
        {
            get { return rowsAffected; }
        }
    }
}
