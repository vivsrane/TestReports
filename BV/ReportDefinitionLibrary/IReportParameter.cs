using System.Collections.Generic;
using VB.Common.Data;

namespace VB.Reports.App.ReportDefinitionLibrary
{
    public interface IReportParameter : IDataParameterTemplate
    {
        /// <summary>
        /// Human readable description of the parameter
        /// </summary>
        string Label
        {
            get;
        }

        /// <summary>
        /// Whether the empty string is a valid parameter value
        /// </summary>
        bool AllowBlank
        {
            get;
        }

        ReportParameterInputType ReportParameterInputType
        {
            get;
        }
        
        IList<IReportParameterValue> ValidValues(ResolveParameterValue resolver);

        /// <summary>
        /// The default value of the report parameter
        /// </summary>
        IReportParameterValue DefaultValue(ResolveParameterValue resolver);
    }
}
