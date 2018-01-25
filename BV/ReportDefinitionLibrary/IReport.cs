using System.Collections.Generic;

namespace VB.Reports.App.ReportDefinitionLibrary
{
    public interface IReport
    {
        /// <summary>
        /// Unique identifier of the report
        /// </summary>
        string Id
        {
            get;
        }



        /// <summary>
        /// Name of the report
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// Description of the report
        /// </summary>
        string Description
        {
            get;
        }

        ReportType ReportType
        {
            get;
        }

        IReportProcessingLocation ReportProcessingLocation
        {
            get;
        }

        /// <summary>
        /// List of parameters needed to process the report
        /// </summary>
        IList<IReportParameter> ReportParameters
        {
            get;
        }

        IList<IReportDataCommandTemplate> ReportDataCommandTemplates
        {
            get;
        }

        /// <summary>
        /// Indicates whether the report is "new"
        /// </summary>
        bool IsNew
        {
            get;
        }

        IReportParameter ReportParameter(string reportParameterName);

        IReportDataCommandTemplate DataSource(string reportDataSourceName);
    }
}
