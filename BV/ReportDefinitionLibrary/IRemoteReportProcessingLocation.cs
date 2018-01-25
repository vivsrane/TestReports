using System;

namespace VB.Reports.App.ReportDefinitionLibrary
{
    public interface IRemoteReportProcessingLocation : IReportProcessingLocation
    {
        string Path
        {
            get;
        }

        Uri ServerUrl
        {
            get;
        }
    }
}
