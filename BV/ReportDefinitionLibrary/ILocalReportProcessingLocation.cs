namespace VB.Reports.App.ReportDefinitionLibrary
{
    public interface ILocalReportProcessingLocation : IReportProcessingLocation
    {
        string DataSourceName
        {
            get;
        }

        string File
        {
            get;
        }
    }
}
