using System;
using System.Collections.Generic;

namespace VB.Reports.App.ReportDefinitionLibrary.Xml
{
    internal class Report : IReport
    {
        #region IReport Members

        private string id;
        private string name;
        private string description;
        private ReportType reportType;
        private IList<IReportDataCommandTemplate> reportDataCommandTemplate;
        private IReportProcessingLocation reportProcessingLocation;
        private IList<IReportParameter> reportParameters;
        private bool isNew;
        private bool isComplete;
        private readonly ReportFactory reportFactory;

        public Report(ReportFactory reportFactory)
        {
            reportDataCommandTemplate = new List<IReportDataCommandTemplate>();
            reportParameters = new List<IReportParameter>();
            this.reportFactory = reportFactory;
        }

        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public ReportType ReportType
        {
            get { return reportType; }
            set { reportType = value; }
        }

        public bool IsNew
        {
            get { return isNew; }
            set { isNew = value; }
        }

        public IList<IReportDataCommandTemplate> ReportDataCommandTemplates
        {
            get { CompleteReport(); return reportDataCommandTemplate; }
            set { reportDataCommandTemplate = value; }
        }

        public IReportProcessingLocation ReportProcessingLocation
        {
            get { CompleteReport(); return reportProcessingLocation; }
            set { reportProcessingLocation = value; }
        }


        public IList<IReportParameter> ReportParameters
        {
            get { CompleteReport(); return reportParameters; }
            set { reportParameters = value; }
        }

        public IReportDataCommandTemplate DataSource(string dataSourceName)
        {
            if (ReportDataCommandTemplates == null)
                throw new ArgumentNullException("dataSourceName", "ReportDataCommandTemplates must be set before calling ReportDataCommandTemplate(string)");

            foreach (IReportDataCommandTemplate dataSource in ReportDataCommandTemplates)
                if (dataSource.Name.Equals(dataSourceName))
                    return dataSource;

            return null;
        }

        public IReportParameter ReportParameter(string reportParameterName)
        {
            if (ReportParameters == null)
                throw new ArgumentNullException("reportParameterName", "ReportParameters must be set before calling ReportParameter(string)");

            foreach (IReportParameter reportParameter in ReportParameters)
                if (reportParameter.Name.Equals(reportParameterName))
                    return reportParameter;

            return null;
        }

        private void CompleteReport()
        {
            if (!isComplete)
            {
                reportFactory.CompleteReport(this);
            }
        }

        internal void Completed()
        {
            isComplete = true;
        }
        #endregion
    }
}
