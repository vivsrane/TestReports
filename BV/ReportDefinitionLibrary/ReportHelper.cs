using System.Collections.Generic;
using System.Configuration;
using System.Data;
using VB.Common.Data;

namespace VB.Reports.App.ReportDefinitionLibrary
{
    public static class ReportHelper
    {
        public static DataTable LoadReportDataTable(string reportId, string dataSetName, IDictionary<string, object> parameterValues, DataTableCallback callback)
        {
            string reportPath = ConfigurationManager.AppSettings["VB.Reports.App.ReportDefinitionLibrary.Xml.Serialization.ReportMetadata.Path"];
            IReport report = ReportFactory.NewReportFactory(reportPath).FindReport(reportId);
            ILocalReportProcessingLocation location = (ILocalReportProcessingLocation)report.ReportProcessingLocation;

            DataTable table;
            using (IDataConnection connection = SimpleQuery.ConfigurationManagerConnection(location.DataSourceName))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                IReportDataCommandTemplate reportDataCommandTemplate = report.DataSource(dataSetName);

                using (IDataCommand command = connection.CreateCommand(reportDataCommandTemplate, new DictionaryDataParameterValue(parameterValues).DataParameterValue))
                {
                    command.CommandTimeout = 90;

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        table = DataTableMapper.ToDataTable(reader, reportDataCommandTemplate.DataMap, callback);
                    }
                }
            }
            return table;
        }
    }
}