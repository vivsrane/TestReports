using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Xml.Serialization;
using VB.Common.Data;
using Rdl = VB.Reports.App.ReportDefinitionLibrary.ReportDefinitionLanguage.Report;
using RdlChildElement = VB.Reports.App.ReportDefinitionLibrary.ReportDefinitionLanguage.ItemsChoiceType37;
using RdlReportParameters = VB.Reports.App.ReportDefinitionLibrary.ReportDefinitionLanguage.ReportParametersType;
using RdlReportParameter = VB.Reports.App.ReportDefinitionLibrary.ReportDefinitionLanguage.ReportParameterType;
using RdlReportParameterChildElement = VB.Reports.App.ReportDefinitionLibrary.ReportDefinitionLanguage.ItemsChoiceType33;
using DataSets = VB.Reports.App.ReportDefinitionLibrary.ReportDefinitionLanguage.DataSetsType;
using DataSet = VB.Reports.App.ReportDefinitionLibrary.ReportDefinitionLanguage.DataSetType;
using DataSetQuery = VB.Reports.App.ReportDefinitionLibrary.ReportDefinitionLanguage.QueryType;
using DataSetQueryParameters = VB.Reports.App.ReportDefinitionLibrary.ReportDefinitionLanguage.QueryParametersType;
using DataSetQueryParameter = VB.Reports.App.ReportDefinitionLibrary.ReportDefinitionLanguage.QueryParameterType;
using DataSetQueryChildElement = VB.Reports.App.ReportDefinitionLibrary.ReportDefinitionLanguage.ItemsChoiceType2;
using RdlFields = VB.Reports.App.ReportDefinitionLibrary.ReportDefinitionLanguage.FieldsType;
using RdlField = VB.Reports.App.ReportDefinitionLibrary.ReportDefinitionLanguage.FieldType;
using RdlFieldType = VB.Reports.App.ReportDefinitionLibrary.ReportDefinitionLanguage.ItemsChoiceType1;

namespace VB.Reports.App.ReportDefinitionLibrary.Xml
{
    /// <summary>
    /// Caches the de-serialized report definition so we only process the underlying XML
    /// when it changes.
    /// </summary>
    internal class SerializedReportDefinition : ReportDefinitionCache
    {
        public static SerializedReportDefinition GetSerializedReportDefinition(string fileName)
        {
            ValidateFileName(fileName);

            SerializedReportDefinition serializedReportDefinition;

            lock (cache)
            {
                if (cache.Keys.Contains(fileName))
                {
                    ReportDefinitionCache reportDefinition = cache[fileName];

                    serializedReportDefinition = reportDefinition as SerializedReportDefinition;

                    if (serializedReportDefinition == null)
                    {
                        serializedReportDefinition = new SerializedReportDefinition(reportDefinition);

                        cache.Remove(fileName);

                        cache.Add(fileName, serializedReportDefinition);
                    }
                }
                else
                {
                    serializedReportDefinition = new SerializedReportDefinition(fileName);

                    cache.Add(fileName, serializedReportDefinition);
                }
            }

            return serializedReportDefinition;
        }

        private Rdl report;

        public SerializedReportDefinition(string fileName) : base(fileName)
        {
        }

        protected SerializedReportDefinition(ReportDefinitionCache reportDefinition) : base(reportDefinition)
        {
        }

        public Rdl GetReport()
        {
            lock (this)
            {
                if (IsNew() || IsUpdated())
                {
                    Load();
                }
            }
            return report;
        }

        private void Load()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Rdl));

            using (Stream s = GetReportStream())
            {
                report = (Rdl) serializer.Deserialize(s);
            }
        }

        protected override bool IsNew()
        {
            return (base.IsNew() || report == null);
        }
    }

    internal static class ReportDefinitionLanguageFacade
    {
        public static Rdl LoadReport(string fileName)
        {
            return SerializedReportDefinition.GetSerializedReportDefinition(fileName).GetReport();
        }

        public static void PopulateReportDataSources(Report report, Rdl rdl)
        {
            IList<IReportDataCommandTemplate> datasources = new List<IReportDataCommandTemplate>();

            for (int i = 0; i < rdl.Items.Length; i++)
            {
                if (rdl.ItemsElementName[i].Equals(RdlChildElement.DataSets))
                {
                    DataSets dataSets = (DataSets) rdl.Items[i];

                    foreach (DataSet dataSet in dataSets.DataSet)
                    {
                        ReportDataCommandTemplate reportDataCommandTemplate = new ReportDataCommandTemplate(dataSet.Name);
                        ReportDataMap reportDataMap = new ReportDataMap();
                        foreach (object dataSetItem in dataSet.Items)
                        {
                            DataSetQuery queryType = dataSetItem as DataSetQuery;
                            RdlFields fieldCollection = dataSetItem as RdlFields;

                            if (queryType != null)
                            {
                                for (int j = 0; j < queryType.Items.Length; j++)
                                {
                                    switch (queryType.ItemsElementName[j])
                                    {
                                        case DataSetQueryChildElement.CommandType:
                                            reportDataCommandTemplate.CommandType = (CommandType) Enum.Parse(typeof (CommandType), queryType.Items[j].ToString());
                                            break;
                                        case DataSetQueryChildElement.CommandText:
                                            reportDataCommandTemplate.CommandText = queryType.Items[j].ToString();
                                            break;
                                        case DataSetQueryChildElement.QueryParameters:
                                            DataSetQueryParameters queryParameters = (DataSetQueryParameters) queryType.Items[j];
                                            foreach (DataSetQueryParameter queryParameter in queryParameters.QueryParameter)
                                            {
                                                string xname = queryParameter.Name;
                                                if (xname.StartsWith("@", StringComparison.OrdinalIgnoreCase))
                                                    xname = queryParameter.Name.Substring(1);
                                                IDataParameterTemplate parameter = report.ReportParameter(xname);
                                                if (parameter == null)
                                                    throw new ReportParameterException(
                                                        "Report does not define parameter: " + xname);
                                                reportDataCommandTemplate.Parameters.Add(parameter);
                                            }
                                            break;
                                        case DataSetQueryChildElement.DataSourceName:
                                            reportDataCommandTemplate.DataSourceName = queryType.Items[j].ToString();
                                            break;
                                    }
                                }
                            }
                            else if (fieldCollection != null)
                            {
                                RdlField[] fields = fieldCollection.Field;
                                for (int c = 0; c < fields.Length; c++)
                                {
                                    DataMapEntry newEntry = new DataMapEntry();
                                    newEntry.FieldName = fields[c].Name;
                                    for (int d = 0; d < fields[c].ItemsElementName.Length; d++)
                                    {
                                        switch (fields[c].ItemsElementName[d])
                                        {
                                            case RdlFieldType.Item:
                                                if (fields[c].Items[d] is System.Xml.XmlElement)
                                                {
                                                    newEntry.TypeName = ((System.Xml.XmlElement) fields[c].Items[d]).InnerText;
                                                }
                                                break;
                                            case RdlFieldType.DataField:
                                                newEntry.ColumnName = fields[c].Items[d].ToString();
                                                break;
                                        }
                                    }
                                    reportDataMap.AddDataMapEntry(newEntry);
                                }
                            }
                        }
                        reportDataCommandTemplate.DataMap = reportDataMap;
                        datasources.Add(reportDataCommandTemplate);
                    }
                }
            }

            report.ReportDataCommandTemplates = datasources;
        }

        public static void VerifyReportParameters(Report report, Rdl rdl)
        {
            for (int i = 0; i < rdl.Items.Length; i++)
            {
                if (rdl.ItemsElementName[i].Equals(RdlChildElement.ReportParameters))
                {
                    RdlReportParameters reportParameters = (RdlReportParameters) rdl.Items[i];

                    foreach (RdlReportParameter reportParameter in reportParameters.ReportParameter)
                    {
                        IReportParameter target = report.ReportParameter(reportParameter.Name);

                        if (target == null)
                            throw new ReportParameterException("XML metadata is missing parameter: " + reportParameter.Name);

                        for (int j = 0; j < reportParameter.Items.Length; j++)
                        {
                            switch (reportParameter.ItemsElementName[j])
                            {
                                case RdlReportParameterChildElement.DataType:
                                    ReportParameterDataType rpdt = (ReportParameterDataType)Enum.Parse(typeof(ReportParameterDataType), reportParameter.Items[j].ToString());
                                    DbType dbType = XmlReportFactory.ToDbType(rpdt);
                                    if (!target.DbType.Equals(dbType))
                                        throw new ReportParameterException("XML metadata mismatch (DataType)! Parameter: " + target.Name);
                                    break;
                            }
                        }
                    }
                }
            }
        }
    }
}
