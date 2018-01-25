
namespace FirstLook.Reports.App.ReportDefinitionLibrary
{
    /// <remarks>Abstract factory pattern with static factory method</remarks>
    public abstract class ReportDefinitionFactory
    {
        public delegate object ResolveParameterValue(string parameterName);

        public delegate ReportDataSourceType ResolveDataSourceType(string dataSourceName);

        public abstract IReport FindReport(string id);

        public abstract IReport FindReportByServerPath(string path);

        public abstract IReportTreeNode BuildReportTree(ReportType reportType, int businessUnitID);

        public abstract IReportTreeNode BuildReportTree(string id, int businessUnitID);

        public static ReportDefinitionFactory NewReportDefinitionFactory()
        {
            return new Xml.XmlReportDefinitionFactory();
        }
    }
}
