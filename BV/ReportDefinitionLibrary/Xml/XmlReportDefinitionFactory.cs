using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

using FirstLook.Reports.App.ReportDefinitionLibrary;

using Serialization = FirstLook.Reports.App.ReportDefinitionLibrary.Xml.Serialization;

namespace FirstLook.Reports.App.ReportDefinitionLibrary.Xml
{
    internal class XmlReportDefinitionFactory : ReportDefinitionFactory
    {
        public override IReport FindReport(string id)
        {
            return FindReport(id, false);
        }

        private IReport FindReport(string id, bool proxy)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException("Report ID cannot be null or empty");

            Serialization.ReportCenterMetadata metadata = Serialization.Metadata.Instance.Cache;

            IReport report = null;

            foreach (Serialization.Report xmlReport in metadata.Reports)
            {
                if (xmlReport.Id.Equals(id))
                {
                    report = LoadReport(xmlReport, proxy);
                    break;
                }
            }

            if (report == null)
                throw new ReportNotFoundException("No such report: " + id);
            
            return report;
        }

        public override IReport FindReportByServerPath(string path)
        {
            return FindReportByServerPath(path, false);
        }

        private IReport FindReportByServerPath(string path, bool proxy)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("Report path cannot be null or empty");

            Serialization.ReportCenterMetadata metadata = Serialization.Metadata.Instance.Cache;

            IReport report = null;

            foreach (Serialization.Report xmlReport in metadata.Reports)
            {
                if (xmlReport.ReportProcessingLocation.ServerReport != null)
                {
                    if (xmlReport.ReportProcessingLocation.ServerReport.Path.Equals(path))
                    {
                        report = LoadReport(xmlReport, proxy);
                        break;
                    }
                }
            }

            if (report == null)
                throw new ReportNotFoundException("No such report: " + path);

            return report;
        }

        private IReport LoadReport(Serialization.Report xmlReport, bool proxy)
        {
            AbstractReport report = null;

            if (proxy)
            {
                report = new ReportProxy();
            }
            else
            {
                report = new Report();
            }

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

            if (xmlReport.ReportProcessingLocation.Use.Equals(Serialization.ActiveReportProcessingLocation.LocalReport))
            {
                string directory = ConfigurationManager.AppSettings["FirstLook.Reports.App.ReportDefinitionLibrary.Xml.XmlReportDefinitionFactory.Report.Directory"];

                Serialization.LocalReport localReport = xmlReport.ReportProcessingLocation.LocalReport;

                LocalReportProcessingLocation location = new LocalReportProcessingLocation();
                location.File = directory + Path.DirectorySeparatorChar + localReport.File;
                location.DataSourceName = localReport.DataSourceName;

                report.ReportProcessingLocation = location;
            }
            else
            {
                string serverUrl = ConfigurationManager.AppSettings["FirstLook.Reports.App.ReportDefinitionLibrary.Xml.XmlReportDefinitionFactory.Report.ServerUrl"];

                Serialization.ServerReport serverReport = xmlReport.ReportProcessingLocation.ServerReport;

                RemoteReportProcessingLocation location = new RemoteReportProcessingLocation();
                location.Path = serverReport.Path;
                location.ServerUrl = new Uri(serverUrl);

                report.ReportProcessingLocation = location;
            }

            IList<IReportParameter> reportParameters = new List<IReportParameter>();

            if (xmlReport.Parameters != null && xmlReport.Parameters.Length > 0)
            {
                foreach (Serialization.Parameter xmlParameter in xmlReport.Parameters)
                {
                    ReportParameter reportParameter = new ReportParameter();
                    reportParameter.AllowBlank = xmlParameter.AllowBlank;
                    reportParameter.AllowNull = xmlParameter.AllowNull;
                    reportParameter.Label = xmlParameter.Label;
                    reportParameter.Name = xmlParameter.Name;
                    reportParameter.ReportParameterDataType = (ReportParameterDataType)Enum.Parse(typeof(ReportParameterDataType), xmlParameter.ParameterDataType.ToString());
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
                                dateRange.InitialDistance = Int32.Parse(xmlDateRange.InitialDistance);
                                dateRange.DistanceUnit = (TimeUnit)Enum.Parse(typeof(TimeUnit), xmlDateRange.DistanceUnit.ToString()); ;
                                dateRange.Distance = Int32.Parse(xmlDateRange.Distance);
                                dateRange.NumberOfItems = Int32.Parse(xmlDateRange.NumberOfItems);
                                dateRange.Direction = (TimeDirection)Enum.Parse(typeof(TimeDirection), xmlDateRange.Direction.ToString());

                                Serialization.DateTimeFormatInfo xmlDtfi = xmlDateRange.DateTimeFormatInfo;

                                System.Globalization.DateTimeFormatInfo dtfi = new System.Globalization.DateTimeFormatInfo();

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

                                int index = 0, selectedIndex = Int32.Parse(xmlDateRange.SelectedIndex);

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

                            reportParameterValues = new ReportParameterValuesDynamic( Type.GetType(xmlDomainModel.DomainModelName), xmlDomainModel.InvokeMethod.MethodName, 
                                                        parameterLabelValuePair);

                            foreach (Serialization.MethodArgument argument in xmlDomainModel.InvokeMethod.Arguments)
                            {
                                ((ReportParameterValuesDynamic)reportParameterValues).AddMethodArgumentEntry(argument.Name, Type.GetType(argument.Type), argument.Value, argument.Source.ToString());
                            }

                            IList<IReportParameterValue> staticParameterValues = new List<IReportParameterValue>();

                            if (xmlDomainModel.CustomValues != null)
                            {
                                int index = System.Int32.Parse(xmlDomainModel.CustomValues.Insert.Index);

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
                                    ((ReportParameterValue)staticParameterValues[System.Int32.Parse(xmlDomainModel.Selected.Index)]).Selected = true;
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

            report.ReportParameters = reportParameters;

            if (!proxy)
            {
                // Work against the RDL to extract data sources and verify parameters

                IList<IReportDataSource> dataSources = new List<IReportDataSource>();

                if (report.ReportProcessingLocation is LocalReportProcessingLocation)
                {
                    string filename = ((LocalReportProcessingLocation)report.ReportProcessingLocation).File;

                    FirstLook.Reports.App.ReportDefinitionLibrary.ReportDefinitionLanguage.Report rdl = ReportDefinitionLanguageFacade.LoadReport(filename);

                    ReportDefinitionLanguageFacade.PopulateReportDataSources((Report)report, rdl);

                    ReportDefinitionLanguageFacade.VerifyReportParameters((Report)report, rdl);
                }
            }

            return report;
        }

        public override IReportTreeNode BuildReportTree(ReportType reportType, int businessUnitId)
        {
            Serialization.ReportCenterMetadata metadata = Serialization.Metadata.Instance.Cache;

            Serialization.PanelType panelType;

            if (reportType == ReportType.Dealer)
            {
                panelType = Serialization.PanelType.Dealer;
            }
            else
            {
                panelType = Serialization.PanelType.DealerGroup;
            }

            IReportTreeNode root = null;

            foreach (Serialization.Panel panel in metadata.Panels)
            {
                if (!panel.PanelType.Equals(panelType))
                    continue;

                root = Copy(panel.Node, businessUnitId);

                break;
            }

            return root;
        }

        public override IReportTreeNode BuildReportTree(string id, int businessUnitId)
        {
            Serialization.ReportCenterMetadata metadata = Serialization.Metadata.Instance.Cache;

            IReportTreeNode root = null;

            foreach (Serialization.Panel panel in metadata.Panels)
            {
                Serialization.Node node = FindNode(panel.Node, id);

                if (node != null)
                {
                    root = Copy(node, businessUnitId);
                    break;
                }
            }

            return root;
        }

        private Serialization.Node FindNode(Serialization.Node node, string id)
        {
            if (node.Id.Equals(id))
            {
                return node;
            }
            else if (node.NodeType.Equals(Serialization.NodeType.ReportHandle))
            {
                return null;
            }
            else
            {
                foreach (Serialization.Node child in node.Content.Items)
                {
                    Serialization.Node result = FindNode(child, id);

                    if (result != null)
                    {
                        return result;
                    }
                }

                return null;
            }
        }

        private ReportTreeNode Copy(Serialization.Node node, int businessUnitId)
        {
            if (node.NodeType.Equals(Serialization.NodeType.ReportGroup))
            {
                return CopyGroup(node, businessUnitId);
            }
            else
            {
                return CopyReport(node, businessUnitId);
            }
        }

        private ReportTreeNode CopyGroup(Serialization.Node src, int businessUnitId)
        {
            ReportGroup dst = new ReportGroup();
            
            CopyFields(dst, src);

            foreach (Serialization.Node node in src.Content.Items)
            {
                ReportTreeNode child = Copy(node, businessUnitId);

                if (child != null)
                {
                    dst.Children.Add(child);
                }
            }

            return dst;
        }

        private ReportTreeNode CopyReport(Serialization.Node src, int businessUnitId)
        {
            ReportTreeNode dst = null;

            if (IsAuthorized(src, businessUnitId))
            {
                ReportHandle handle = new ReportHandle();

                CopyFields(handle, src);

                Serialization.ReportReference reference = (Serialization.ReportReference) src.Content.Items[0];

                if (reference.ComingSoon)
                {
                    handle.ComingSoon = true;
                }
                else
                {
                    handle.Report = FindReport(reference.Value, true);
                }

                dst = handle;
            }

            return dst;
        }

        private void CopyFields(ReportTreeNode dst, Serialization.Node src)
        {
            dst.Title = src.Title;
            dst.Description = src.Description;
            dst.NodeType = (ReportTreeNodeType) Enum.Parse(typeof(ReportTreeNodeType), src.NodeType.ToString());
        }

        private bool IsAuthorized(Serialization.Node node, int businessUnitId)
        {
            return IsAuthorized(((Serialization.ReportReference) node.Content.Items[0]).Value, businessUnitId);
        }

        private bool IsAuthorized(string idref, int businessUnitId)
        {
            Serialization.ReportCenterMetadata metadata = Serialization.Metadata.Instance.Cache;

            bool foundReport = false;

            bool authorized = false;

            foreach (Serialization.Report xmlReport in metadata.Reports)
            {
                if (!xmlReport.Id.Equals(idref))
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

                    authorized = acl.Allow(businessUnitId.ToString());
                }
            }

            if (!foundReport)
                throw new ReportNotFoundException(idref);

            return authorized;
        }

    }
}
