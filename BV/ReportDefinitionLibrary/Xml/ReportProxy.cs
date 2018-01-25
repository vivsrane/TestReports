using System;
using System.Collections.Generic;
using System.Text;

using FirstLook.Reports.App.ReportDefinitionLibrary;

namespace FirstLook.Reports.App.ReportDefinitionLibrary.Xml
{
    internal class ReportProxy : AbstractReport, IReport
    {
        IReport report;

        public ReportProxy() : base()
        {
        }

        IList<IReportDataSource> IReport.DataSources
        {
            get { ResolveProxyHandle(); return report.DataSources; }
        }

        public override IList<IReportDataSource> DataSources
        {
            get { ResolveProxyHandle(); return report.DataSources; }
            set { throw new NotSupportedException("Cannot set DataSources on a Proxy"); }
        }

        public override IReportProcessingLocation ReportProcessingLocation
        {
            get { ResolveProxyHandle(); return report.ReportProcessingLocation; }
            set { throw new NotSupportedException("Cannot set ReportProcessingLocation on a Proxy"); }
        }

        public override IList<IReportParameter> ReportParameters
        {
            get { ResolveProxyHandle(); return report.ReportParameters; }
            set { throw new NotSupportedException("Cannot set ReportParameters on a Proxy"); }
        }

        private void ResolveProxyHandle()
        {
            if (report == null )
            {
                report = ReportFactory.NewReportFactory().FindReport(Id);
            }
        }
    }
}
