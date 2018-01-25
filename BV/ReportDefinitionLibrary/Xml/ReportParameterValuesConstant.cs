using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace VB.Reports.App.ReportDefinitionLibrary.Xml
{
    internal class ReportParameterValuesConstant : AbstractReportParameterValues
    {
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public ReportParameterValuesConstant()
        {
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public ReportParameterValuesConstant(IList<IReportParameterValue> reportParameterValues)
        {
            this.reportParameterValues = reportParameterValues;
        }

        public override IList<IReportParameterValue> GetReportParameterValues(ResolveParameterValue resolver)
        {
            return reportParameterValues;
        }
    }
}
