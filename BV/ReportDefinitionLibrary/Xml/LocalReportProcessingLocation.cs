namespace VB.Reports.App.ReportDefinitionLibrary.Xml
{
    internal class LocalReportProcessingLocation : ILocalReportProcessingLocation
    {
        #region ILocalReportProcessingLocation Members

        private string dataSourceName;
        private string file;

        public string DataSourceName
        {
            get { return dataSourceName; }
            set { dataSourceName = value; }
        }

        public string File
        {
            get { return file; }
            set { file = value; }
        }

        #endregion
    }
}
