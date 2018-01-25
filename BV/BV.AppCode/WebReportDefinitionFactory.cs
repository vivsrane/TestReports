using System.Configuration;
using VB.Reports.App.ReportDefinitionLibrary;

namespace BV.AppCode
{
    public static class WebReportDefinitionFactory
    {
        public static string ReportKey =
            "VB.Reports.App.ReportDefinitionLibrary.Xml.Serialization.ReportMetadata.Path";

        public static string ReportGroupingKey =
            "VB.Reports.App.ReportDefinitionLibrary.Xml.Serialization.ReportGroupingMetadata.Path";

        public static ReportFactory NewReportFactory()
        {
            return ReportFactory.NewReportFactory(ConfigurationManager.AppSettings[ReportKey]);
        }

        public static ReportGroupingFactory NewReportGroupingFactory()
        {
            return ReportGroupingFactory.NewReportGroupingFactory(
                ConfigurationManager.AppSettings[ReportGroupingKey],
                NewReportFactory());
        }
    }
}
