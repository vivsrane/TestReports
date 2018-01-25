using System.Collections.Generic;

namespace VB.Reports.App.ReportDefinitionLibrary
{
    internal abstract class AbstractReportParameterValues
    {
        protected IList<IReportParameterValue> reportParameterValues = new List<IReportParameterValue>();

        public abstract IList<IReportParameterValue> GetReportParameterValues(ResolveParameterValue resolver);

        internal void AddReportParameterValue(IReportParameterValue value)
        {
            reportParameterValues.Add(value);
        }

        internal void AddReportParameterValues(IList<IReportParameterValue> values)
        {
            foreach (IReportParameterValue value in values)
            {
                reportParameterValues.Add(value);
            }
        }
    }
}
