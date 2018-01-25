namespace VB.Common.Core
{
    public class DataServiceArguments
    {
        private int maximumRows;
        private bool retrieveTotalRowCount;
        private string sortExpression;
        private int startRowIndex;
        private int totalRowCount;

        public DataServiceArguments(int maximumRows, bool retrieveTotalRowCount, string sortExpression, int startRowIndex, int totalRowCount)
        {
            this.maximumRows = maximumRows;
            this.retrieveTotalRowCount = retrieveTotalRowCount;
            this.sortExpression = sortExpression;
            this.startRowIndex = startRowIndex;
            this.totalRowCount = totalRowCount;
        }

        /// <summary>
        /// Gets or sets a value that represents the maximum number of data rows
        /// that a data source control returns for a data retrieval operation.
        /// </summary>
        public int MaximumRows
        {
            get { return maximumRows; }
            set { maximumRows = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a data source control should
        /// retrieve a count of all the data rows during a data retrieval operation.
        /// </summary>
        public bool RetrieveTotalRowCount
        {
            get { return retrieveTotalRowCount; }
            set { retrieveTotalRowCount = value; }
        }

        /// <summary>
        /// Gets or sets an expression that the data source view uses to sort the
        /// data retrieved.
        /// </summary>
        public string SortExpression
        {
            get { return sortExpression; }
            set { sortExpression = value; }
        }

        /// <summary>
        /// Gets or sets a value that represents the starting position the data source
        /// control should use when retrieving data rows during a data retrieval operation.
        /// </summary>
        public int StartRowIndex
        {
            get { return startRowIndex; }
            set { startRowIndex = value; }
        }

        /// <summary>
        /// Gets or sets the number of rows retrieved during a data retrieval operation.
        /// </summary>
        public int TotalRowCount
        {
            get { return totalRowCount; }
            set { totalRowCount = value; }
        }
    }
}
