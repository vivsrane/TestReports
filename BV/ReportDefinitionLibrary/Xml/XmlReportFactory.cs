using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;

namespace VB.Reports.App.ReportDefinitionLibrary.Xml
{
    internal class XmlReportFactory : ReportFactory
    {
        private readonly string configurationFilename;

        protected internal XmlReportFactory(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path", "Report configurationFilename cannot be null or empty");

            configurationFilename = path;
        }

        public override IReport FindReport(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException("id", "Report ID cannot be null or empty");

            Serialization.ReportMetadata reportMetadata = (Serialization.ReportMetadata)Serialization.ReportDefinitionLibraryMetadataCache.GetReportDefinitionLibraryMetadata(configurationFilename, typeof(Serialization.ReportMetadata));

            IReport report = null;

            foreach (Serialization.Report xmlReport in reportMetadata.Report)
            {
                if (xmlReport.Id.Equals(id))
                {
                    report = LoadReport(xmlReport);
                    break;
                }
            }

            if (report == null)
                throw new ReportNotFoundException("No such report: " + id);

            return report;
        }

        public override IReport FindReportByServerPath(string serverPath)
        {
            Serialization.ReportMetadata metadata = (Serialization.ReportMetadata)Serialization.ReportDefinitionLibraryMetadataCache.GetReportDefinitionLibraryMetadata(configurationFilename, typeof(Serialization.ReportMetadata));

            IReport report = null;

            foreach (Serialization.Report xmlReport in metadata.Report)
            {
                if (xmlReport.ReportProcessingLocation.ServerReport != null)
                {
                    if (xmlReport.ReportProcessingLocation.ServerReport.Path.Equals(serverPath))
                    {
                        report = LoadReport(xmlReport);
                        break;
                    }
                }
            }

            if (report == null)
                throw new ReportNotFoundException("No such report: " + serverPath);

            return report;
        }


        private IReport LoadReport(Serialization.Report xmlReport)
        {
            Report report = new Report(this);

            report.Id = xmlReport.Id;
            report.Name = xmlReport.Name;
            report.Description = xmlReport.Description;

            if (xmlReport.NewUntilSpecified)
            {
                report.IsNew = xmlReport.NewUntil > DateTime.Now;
            }
            else
            {
                report.IsNew = false;
            }

            report.ReportType = (ReportType)Enum.Parse(typeof(ReportType), xmlReport.ReportType.ToString());

            return report;
        }

        internal override void CompleteReport(Report report)
        {
            Serialization.ReportMetadata reportMetadata = (Serialization.ReportMetadata)Serialization.ReportDefinitionLibraryMetadataCache.GetReportDefinitionLibraryMetadata(configurationFilename, typeof(Serialization.ReportMetadata));

            foreach (Serialization.Report xmlReport in reportMetadata.Report)
            {
                if (xmlReport.Id.Equals(report.Id))
                {
                    report.ReportParameters = GetReportParameters(xmlReport); ;
                    report.ReportProcessingLocation = GetReportProcessingLocation(xmlReport);
                    report.Completed();
                    VerifyParameters(report);
                    break;
                }
            }
            return;
        }

        private static void VerifyParameters(Report report)
        {
            // Work against the RDL to extract data sources and verify parameters
            if (report.ReportProcessingLocation is LocalReportProcessingLocation)
            {
                string fileName = ((LocalReportProcessingLocation)report.ReportProcessingLocation).File;

                ReportDefinitionLanguage.Report rdl = ReportDefinitionLanguageFacade.LoadReport(fileName);

                ReportDefinitionLanguageFacade.PopulateReportDataSources(report, rdl);

                ReportDefinitionLanguageFacade.VerifyReportParameters(report, rdl);
            }
        }

        private static IReportProcessingLocation GetReportProcessingLocation(Serialization.Report xmlReport)
        {
            IReportProcessingLocation location;
            if (xmlReport.ReportProcessingLocation.Use.Equals(Serialization.ActiveReportProcessingLocation.LocalReport))
            {
                string directory = ConfigurationManager.AppSettings["VB.Reports.App.ReportDefinitionLibrary.Xml.XmlReportDefinitionFactory.Report.Directory"];

                Serialization.LocalReport localReport = xmlReport.ReportProcessingLocation.LocalReport;

                LocalReportProcessingLocation local = new LocalReportProcessingLocation();
                local.File = directory + Path.DirectorySeparatorChar + localReport.File;
                local.DataSourceName = localReport.DataSourceName;

                location = local;
            }
            else
            {
                string serverUrl = ConfigurationManager.AppSettings["VB.Reports.App.ReportDefinitionLibrary.Xml.XmlReportDefinitionFactory.Report.ServerUrl"];

                Serialization.ServerReport serverReport = xmlReport.ReportProcessingLocation.ServerReport;

                RemoteReportProcessingLocation remote = new RemoteReportProcessingLocation();
                remote.Path = serverReport.Path;
                remote.ServerUrl = new Uri(serverUrl);

                location = remote;
            }
            return location;
        }

        [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Int32.ToString", Justification = "Enum.ToString(IFormatProvider) is obsolete")]
        private static IList<IReportParameter> GetReportParameters(Serialization.Report xmlReport)
        {
            IList<IReportParameter> reportParameters = new List<IReportParameter>();

            if (xmlReport.Parameters != null && xmlReport.Parameters.Length > 0)
            {
                foreach (Serialization.Parameter xmlParameter in xmlReport.Parameters)
                {
                    ReportParameter reportParameter = new ReportParameter();
                    reportParameter.AllowBlank = xmlParameter.AllowBlank;
                    reportParameter.IsNullable = xmlParameter.AllowNull;
                    reportParameter.Label = xmlParameter.Label;
                    reportParameter.Name = xmlParameter.Name;
                    reportParameter.DbType = ToDbType((ReportParameterDataType)Enum.Parse(typeof(ReportParameterDataType), xmlParameter.ParameterDataType.ToString()));
                    reportParameter.ReportParameterInputType = (ReportParameterInputType)Enum.Parse(typeof(ReportParameterInputType), xmlParameter.ParameterInputType.ToString());

                    AbstractReportParameterValues reportParameterValues = new ReportParameterValuesConstant();

                    if (xmlParameter.ValidValues != null)
                    {
                        if (xmlParameter.ValidValues.Item is Serialization.ParameterRange)
                        {
                            Serialization.ParameterRange xmlParameterRange = (Serialization.ParameterRange)xmlParameter.ValidValues.Item;

                            if (xmlParameterRange.Item is Serialization.DateRange)
                            {
                                Serialization.DateRange xmlDateRange = xmlParameterRange.Item;

                                DateRange dateRange = new DateRange();
                                dateRange.InitialUnit = (TimeUnit)Enum.Parse(typeof(TimeUnit), xmlDateRange.InitialUnit.ToString());
                                dateRange.InitialDistance = Int32.Parse(xmlDateRange.InitialDistance, CultureInfo.InvariantCulture);
                                dateRange.DistanceUnit = (TimeUnit)Enum.Parse(typeof(TimeUnit), xmlDateRange.DistanceUnit.ToString());
                                dateRange.Distance = Int32.Parse(xmlDateRange.Distance, CultureInfo.InvariantCulture);
                                dateRange.NumberOfItems = Int32.Parse(xmlDateRange.NumberOfItems, CultureInfo.InvariantCulture);
                                dateRange.Direction = (TimeDirection)Enum.Parse(typeof(TimeDirection), xmlDateRange.Direction.ToString());

                                Serialization.DateTimeFormatInfo xmlDtfi = xmlDateRange.DateTimeFormatInfo;

                                DateTimeFormatInfo dtfi = new DateTimeFormatInfo();

                                if (!string.IsNullOrEmpty(xmlDtfi.FullDateTimePattern))
                                    dtfi.FullDateTimePattern = xmlDtfi.FullDateTimePattern;

                                if (!string.IsNullOrEmpty(xmlDtfi.LongDatePattern))
                                    dtfi.LongDatePattern = xmlDtfi.LongDatePattern;

                                if (!string.IsNullOrEmpty(xmlDtfi.LongTimePattern))
                                    dtfi.LongTimePattern = xmlDtfi.LongTimePattern;

                                if (!string.IsNullOrEmpty(xmlDtfi.MonthDayPattern))
                                    dtfi.MonthDayPattern = xmlDtfi.MonthDayPattern;

                                if (!string.IsNullOrEmpty(xmlDtfi.ShortDatePattern))
                                    dtfi.ShortDatePattern = xmlDtfi.ShortDatePattern;

                                if (!string.IsNullOrEmpty(xmlDtfi.ShortTimePattern))
                                    dtfi.ShortTimePattern = xmlDtfi.ShortTimePattern;

                                if (!string.IsNullOrEmpty(xmlDtfi.YearMonthPattern))
                                    dtfi.YearMonthPattern = xmlDtfi.YearMonthPattern;

                                int index = 0, selectedIndex = Int32.Parse(xmlDateRange.SelectedIndex, CultureInfo.InvariantCulture);

                                foreach (DateTime value in dateRange.Items())
                                {
                                    ReportParameterValue reportParameterValue = new ReportParameterValue();
                                    reportParameterValue.Label = value.ToString(xmlDateRange.LabelDateTimeFormatSpecifier.ToString(), dtfi);
                                    reportParameterValue.Value = value.ToString(xmlDateRange.ValueDateTimeFormatSpecifier.ToString(), dtfi);
                                    reportParameterValue.Selected = (index++ == selectedIndex);
                                    reportParameterValues.AddReportParameterValue(reportParameterValue);
                                }
                            }
                            else
                            {
                                // ADD NEW RANGE ELEMENT CODE HERE
                            }
                        }
                        else if (xmlParameter.ValidValues.Item is Serialization.DomainModel)
                        {


                            Serialization.DomainModel xmlDomainModel = (Serialization.DomainModel)xmlParameter.ValidValues.Item;

                            IDictionary<string, string> parameterLabelValuePair = new Dictionary<string, string>();
                            parameterLabelValuePair.Add("Label", xmlDomainModel.Label.Name);
                            parameterLabelValuePair.Add("Value", xmlDomainModel.Value.Name);

                            reportParameterValues = new ReportParameterValuesDynamic(Type.GetType(xmlDomainModel.DomainModelName), xmlDomainModel.InvokeMethod.MethodName,
                                                                                     parameterLabelValuePair);

                            foreach (Serialization.MethodArgument argument in xmlDomainModel.InvokeMethod.Arguments)
                            {
                                ((ReportParameterValuesDynamic)reportParameterValues).AddMethodArgumentEntry(argument.Name, Type.GetType(argument.Type), argument.Value, argument.Source.ToString());
                            }

                            IList<IReportParameterValue> staticParameterValues = new List<IReportParameterValue>();

                            if (xmlDomainModel.CustomValues != null)
                            {
                                foreach (Serialization.ParameterValue parameterValue in xmlDomainModel.CustomValues.Insert.ParameterValue)
                                {
                                    ReportParameterValue reportParameterValue = new ReportParameterValue();
                                    reportParameterValue.Label = parameterValue.Label;
                                    reportParameterValue.Value = parameterValue.Value;
                                    reportParameterValue.Selected = parameterValue.Selected;

                                    staticParameterValues.Add(reportParameterValue);
                                }
                            }

                            bool itemSelected = false;

                            // this logic is build on the assumption you can only preselect custom fields - is this a safe assumption?
                            if (xmlDomainModel.Selected != null)
                            {
                                if (xmlDomainModel.Selected.Index != null)
                                {
                                    ((ReportParameterValue)staticParameterValues[Int32.Parse(xmlDomainModel.Selected.Index, CultureInfo.InvariantCulture)]).Selected = true;
                                    itemSelected = true;
                                }
                                if (xmlDomainModel.Selected.Value != null)
                                {
                                    foreach (ReportParameterValue x in staticParameterValues)
                                    {
                                        if (x.Value.Equals(xmlDomainModel.Selected.Value))
                                        {
                                            x.Selected = true;
                                            itemSelected = true;
                                            break;
                                        }
                                    }
                                }
                            }

                            if (!itemSelected)
                            {
                                if (staticParameterValues.Count > 0)
                                {
                                    ((ReportParameterValue)staticParameterValues[0]).Selected = true;
                                }
                            }

                            reportParameterValues.AddReportParameterValues(staticParameterValues);
                        }
                        else // ReportDefinitionLibrary.Xml.ParameterValues
                        {
                            Serialization.ParameterValue[] xmlParameterValues = (Serialization.ParameterValue[])xmlParameter.ValidValues.Item;

                            foreach (Serialization.ParameterValue parameterValue in xmlParameterValues)
                            {
                                ReportParameterValue reportParameterValue = new ReportParameterValue();
                                reportParameterValue.Label = parameterValue.Label;
                                reportParameterValue.Value = parameterValue.Value;
                                reportParameterValue.Selected = parameterValue.Selected;

                                reportParameterValues.AddReportParameterValue(reportParameterValue);
                            }
                        }
                    }

                    reportParameter.ReportParameterValues = reportParameterValues;

                    reportParameters.Add(reportParameter);
                }
            }
            return reportParameters;
        }

        public override bool IsAuthorized(string id, int businessUnitId)
        {
            Serialization.ReportMetadata metadata = (Serialization.ReportMetadata)Serialization.ReportDefinitionLibraryMetadataCache.GetReportDefinitionLibraryMetadata(configurationFilename, typeof(Serialization.ReportMetadata));

            bool foundReport = false;

            bool authorized = false;

            foreach (Serialization.Report xmlReport in metadata.Report)
            {
                if (!xmlReport.Id.Equals(id))
                    continue;

                foundReport = true;

                if (xmlReport.Authorization == null)
                {
                    authorized = true;
                }
                else
                {
                    Acl<string> acl = new Acl<string>();

                    int i = 0, j = xmlReport.Authorization.Items.Length;

                    for (; i < j; i++)
                    {
                        string value = xmlReport.Authorization.Items[i];

                        switch (xmlReport.Authorization.ItemsElementName[i])
                        {
                            case Serialization.ItemsChoiceType.Allow:
                                acl.Install(AceType.Allow, new StringAce(value));
                                break;
                            case Serialization.ItemsChoiceType.Deny:
                                acl.Install(AceType.Deny, new StringAce(value));
                                break;
                        }
                    }

                    authorized = acl.Allow(businessUnitId.ToString(CultureInfo.InvariantCulture));
                }
            }

            if (!foundReport)
                throw new ReportNotFoundException(id);

            return authorized;
        }


        internal static DbType ToDbType(ReportParameterDataType type)
        {
            DbType result = DbType.Int32;

            switch (type)
            {
                case ReportParameterDataType.Boolean:
                    result = DbType.Boolean;
                    break;
                case ReportParameterDataType.DateTime:
                    result = DbType.DateTime;
                    break;
                case ReportParameterDataType.Float:
                    result = DbType.Decimal;
                    break;
                case ReportParameterDataType.Integer:
                    result = DbType.Int32;
                    break;
                case ReportParameterDataType.String:
                    result = DbType.String;
                    break;
            }

            return result;
        }

    }
}
