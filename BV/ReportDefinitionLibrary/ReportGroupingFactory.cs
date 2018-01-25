
namespace VB.Reports.App.ReportDefinitionLibrary
{
    /// <remarks>Abstract factory pattern with static factory method</remarks>
    public abstract class ReportGroupingFactory
    {
        public abstract IReportTreeNode BuildReportTree(ReportType reportType, int businessUnitID);

        public abstract IReportTreeNode BuildReportTree(string id, int businessUnitID);

        public static ReportGroupingFactory NewReportGroupingFactory(string path, ReportFactory reportFactory)
        {
            return new Xml.XmlReportGroupingFactory(path, reportFactory);
        }
    }
}
