using VB.Common.Data;

namespace VB.Reports.App.ReportDefinitionLibrary
{
    public interface IReportDataCommandTemplate : IDataCommandTemplate
    {
        string Name
        {
            get;
        }

        ReportDataCommandType ReportDataSourceType(ResolveDataSourceType dsResolver);
    }
}
