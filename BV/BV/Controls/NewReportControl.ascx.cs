using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using VB.Common.Data;
using VB.Common.Impersonation;
using VB.DomainModel.Oltp;
using VB.Reports.App.ReportDefinitionLibrary;
using VB.Reports.App.ReportDefinitionLibrary.Xml;
using CheckBox = System.Web.UI.WebControls.CheckBox;
using Control = System.Web.UI.Control;
using Label = System.Web.UI.WebControls.Label;
using MsBookmarkNavigationEventArgs = Microsoft.Reporting.WebForms.BookmarkNavigationEventArgs;
using MsDrillthroughEventArgs = Microsoft.Reporting.WebForms.DrillthroughEventArgs;
using MsLocalReport = Microsoft.Reporting.WebForms.LocalReport;
using MsReportDataSource = Microsoft.Reporting.WebForms.ReportDataSource;
using MsReportParameter = Microsoft.Reporting.WebForms.ReportParameter;
using MsReportViewer = Microsoft.Reporting.WebForms.ReportViewer;
using MsServerReport = Microsoft.Reporting.WebForms.ServerReport;
using MsWarning = Microsoft.Reporting.WebForms.Warning;
using TextBox = System.Web.UI.WebControls.TextBox;
using UserControl = System.Web.UI.UserControl;
using BV.AppCode;
using ContentDisposition = Microsoft.Reporting.WebForms.ContentDisposition;
using DrillthroughEventHandler = Microsoft.Reporting.WebForms.DrillthroughEventHandler;

namespace BV.Controls
{
    // ReSharper disable once InconsistentNaming
    public partial class Reports_NewReportControl : UserControl
    {

        #region Report Cache Method

        private static readonly string ReportKey = "VB.Reports.App.ReportDefinitionLibrary.Report";

        protected IReport TheReport()
        {
            IReport theReport = (IReport)Context.Items[ReportKey];

            if (theReport == null)
            {
                string reportPath = ConfigurationManager.AppSettings["VB.Reports.App.ReportDefinitionLibrary.Xml.Serialization.ReportMetadata.Path"];
                ReportFactory factory = ReportFactory.NewReportFactory(reportPath);
                theReport = factory.FindReport(Request.Params["Id"]);
                Context.Items[ReportKey] = theReport;
            }

            return theReport;
        }

        public string ReportName
        {
            get
            {
                IReport report = TheReport();
                return report != null ? report.Name : null;
            }
        }
        public string ReportId
        {
            get
            {
                IReport report = TheReport();
                return report != null ? report.Id : null;
            }
        }

        // ReSharper disable once InconsistentNaming
        protected string _DrillThroughId;
        public string DrillThroughId
        {
            get
            {
                if (_DrillThroughId != null)
                {
                    return _DrillThroughId;
                }

                if (_DrillThroughId == null && Request.QueryString.HasKeys())
                {
                    if (Request.QueryString.GetValues("drillthrough") != null)
                    {
                        var strings = Request.QueryString.GetValues("drillthrough");
                        if (strings != null)
                            _DrillThroughId = strings[0];
                    }

                    return _DrillThroughId;
                }
                
                return null;

            }
        }
        
        //Event to track the drill through in parent
        public event DrillthroughEventHandler DrillThrough;
        public SoftwareSystemComponentState SoftwareSystemComponentState
        {
            get { return (SoftwareSystemComponentState)Context.Items[SoftwareSystemComponentStateFacade.HttpContextKey]; }
            set { Context.Items[SoftwareSystemComponentStateFacade.HttpContextKey] = value; }
        }
        
        #endregion

        /// <summary>
        /// Property stored in the HttpContext that says whether the InitComplete state was been executed.
        /// </summary>
        protected bool IsInitComplete
        {
            get { return (Context.Items.Contains("InitComplete")) ? (bool)Context.Items["InitComplete"] : false; }
            set { Context.Items["InitComplete"] = value; }
        }

        /// <summary>
        /// Property stored in the HttpContext that says whether we are to [re]bind the report datasources.
        /// </summary>
        protected bool ExecuteBindDataSource
        {
            get { return (Context.Items.Contains("BindDataSource")) ? (bool)Context.Items["BindDataSource"] : false; }
            set { Context.Items["BindDataSource"] = value; }
        }

        #region Init Page State
        /// <summary>
        /// Put the report paramter controls on the page.
        /// </summary>
        protected void ReportParameterOptions_Init(object sender, EventArgs e)
        {

            ReportParameterOptions.Controls.Add(ReportParametersControl(TheReport()));
        }

        protected void ReportHeaderNavigation_Init(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// Initialize the report viewer control.
        /// </summary>
        protected void ReportViewer_Init(object sender, EventArgs e)
        {
            ReportViewer.AsyncRendering = false;
            ReportViewer.ExportContentDisposition = ContentDisposition.AlwaysAttachment;
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            IsInitComplete = true;

            CreateCookieCrum(TheReport().Id);

            ExecuteBindDataSource = !Page.IsPostBack;

        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (ExecuteBindDataSource)
            {
                BindDataSource(ReportViewer, TheReport());
            }
        }

        // change report parameter

        protected void ReportParameterSubmit_Click(object sender, EventArgs e)
        {
            ExecuteBindDataSource = true;
        }

        protected void DropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Session[((DropDownList) sender).ID] = ((DropDownList) sender).SelectedValue;
            ExecuteBindDataSource = true;
        }

        #region Export Report
        /// <summary>
        /// Event handler for when the PDF export button is clicked. The formats supported in LocalProcessingMode
        /// are: Excel, IMAGE and PDF.
        /// </summary>
        /// 
        protected void ReportExportEmail_Load(object sender, EventArgs e)
        {
            EmailClient.Subject = TheReport().Name;
        }

        protected void ReportExportEmail_Click(object sender, EmailEventArgs e)
        {
            string mimeType, encoding, fileNameExtension; string[] streams; MsWarning[] warnings;

            //Asked by jen to attach file as Pdf formate
            //byte[] content = ReportViewer.LocalReport.Render(
            //    "PDF", "<DeviceInfo></DeviceInfo>",
            //    out mimeType, out encoding, out fileNameExtension, out streams, out warnings);
            byte[] content = ReportViewer.LocalReport.Render("Excel", "<DeviceInfo><SimplePageHeaders>False</SimplePageHeaders></DeviceInfo>",
                out mimeType, out encoding, out fileNameExtension, out streams, out warnings);
            string attachmentName = Regex.Replace(TheReport().Name, "\\W|[0-9]", "") + "." + fileNameExtension;

#pragma warning disable 618
            MailMessage message = new MailMessage {ReplyTo = e.ReplyTo};
#pragma warning restore 618
            message.From = new MailAddress("no-reply@VB.biz");
            foreach (MailAddress to in e.To)
                message.To.Add(to);
            message.Subject = e.Subject;
            message.IsBodyHtml = true;
            message.Body = e.Body;
            message.Attachments.Add(new Attachment(new MemoryStream(content), attachmentName));

            new SmtpClient().Send(message);
        }

        /// <summary>
        /// Event handler for when the PDF export button is clicked.
        /// </summary>
        protected void ReportExportPDF_Click(object sender, EventArgs e)
        {
            ExportReport("PDF", "<DeviceInfo></DeviceInfo>");
        }

        /// <summary>
        /// Event handler for when the Excel export button is clicked.
        /// </summary>
        protected void ReportExportExcel_Click(object sender, EventArgs e)
        {
            ExportReport("Excel", "<DeviceInfo><SimplePageHeaders>False</SimplePageHeaders></DeviceInfo>");
        }

        /// <summary>
        /// Utility method to export the current report in a given format.
        /// </summary>
        /// <param name="format">the export format</param>
        /// <param name="deviceInfo">parameters for the formatter</param>
        protected void ExportReport(string format, string deviceInfo)
        {
            // out parameters
            string mimeType, encoding, fileNameExtension; string[] streams; MsWarning[] warnings;
            // render to byte array
            byte[] content = ReportViewer.LocalReport.Render(
                format, deviceInfo,
                out mimeType, out encoding, out fileNameExtension, out streams, out warnings);
            // write response
            Response.Clear();
            Response.AddHeader("Content-Disposition", "attachment; filename=" + Regex.Replace(TheReport().Name, "\\W|[0-9]", "") + "." + fileNameExtension);
            Response.AddHeader("Content-Length", content.Length.ToString());
            Response.ContentType = mimeType;
            Response.BinaryWrite(content);
            Response.End();
        }
        #endregion

        // Bookmark Navigation
        protected void ReportViewer_BookmarkNavigation(object sender, MsBookmarkNavigationEventArgs e)
        {
            ReportViewer.JumpToBookmark(e.BookmarkId);
        }

        // drill through
        protected void ReportViewer_Drillthrough(object sender, MsDrillthroughEventArgs e)
        {
            if (DrillThrough != null) DrillThrough(this, e);
            // find the drill-through report from the library
            string configurationFilename = ConfigurationManager.AppSettings["VB.Reports.App.ReportDefinitionLibrary.Xml.Serialization.ReportMetadata.Path"];
            ReportFactory factory = ReportFactory.NewReportFactory(configurationFilename);

            IReport report = factory.FindReportByServerPath(e.ReportPath);

            // bind the data source

            if (e.Report is MsLocalReport)
            {
                BindLocalReport((MsLocalReport)e.Report, report);
            }
            else
            {
                BindServerReport((MsServerReport)e.Report, report);
            }

            // update the report parameter section to be hidden from view

            ReportParameterOptions.Visible = false;
            ReportParameterSubmit.Visible = false;

            // show the back to parent report button

            ReportParameterBack.Visible = true;
            ReportParameterBackText.Text = @"Back to " + TheReport().Name;
        }

        #region Bind Data Source
        protected void BindDataSource(MsReportViewer reportViewer, IReport report)
        {
            if (report.ReportProcessingLocation is ILocalReportProcessingLocation)
            {
                reportViewer.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
                BindLocalReport(reportViewer.LocalReport, report);
            }
            else
            {
                reportViewer.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Remote;

                BindServerReport(reportViewer.ServerReport, report);
            }
        }

        protected void BindServerReport(MsServerReport serverReport, IReport report)
        {
            IRemoteReportProcessingLocation location = (IRemoteReportProcessingLocation)report.ReportProcessingLocation;

            try
            {
                // ReSharper disable once UnusedVariable
                using (ImpersonationCodeSection i = ImpersonationCredentials.FromConnectionString("AnalysisServices").LogOn())
                {
                    serverReport.ReportPath = location.Path;

                    serverReport.ReportServerUrl = location.ServerUrl;

                    if (serverReport.IsDrillthroughReport)
                    {
                        // do not need to handle the callback
                    }
                    else
                    {
                        IList<MsReportParameter> parameters = new List<MsReportParameter>();

                        foreach (IReportParameter parameter in report.ReportParameters)
                        {
                            parameters.Add(new MsReportParameter(parameter.Name, GetParameterValue(parameter)));
                        }

                        serverReport.SetParameters(parameters);
                    }

                    serverReport.Refresh();
                }
            }
            catch
            {
                // can do nothing ?
            }
        }

        protected void BindLocalReport(MsLocalReport localReport, IReport report)
        {
            ILocalReportProcessingLocation location = (ILocalReportProcessingLocation)report.ReportProcessingLocation;

            localReport.LoadReportDefinition(ReportDefinitionCache.GetReportDefinition(location.File).GetReportStream());

            localReport.EnableHyperlinks = true;

            localReport.EnableExternalImages = true;

            localReport.SetBasePermissionsForSandboxAppDomain(new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted));

            if (localReport.IsDrillthroughReport)
            {
                localReport.SetParameters(localReport.OriginalParametersToDrillthrough);
            }
            else
            {
                IList<MsReportParameter> parameters = new List<MsReportParameter>();

                foreach (IReportParameter parameter in report.ReportParameters)
                {
                    parameters.Add(new MsReportParameter(parameter.Name, GetParameterValue(parameter)));
                }

                localReport.SetParameters(parameters);
            }

            localReport.DataSources.Clear();


            foreach (IReportDataCommandTemplate reportDataCommandTemplate in report.ReportDataCommandTemplates)
            {
                DictionaryDataParameterValue parameterValues = new DictionaryDataParameterValue();

                foreach (var dataParameterTemplate in reportDataCommandTemplate.Parameters)
                {
                    var parameter = (IReportParameter) dataParameterTemplate;
                    if (localReport.IsDrillthroughReport)
                    {
                        parameterValues[parameter.Name] =
                            GetParameterValue(parameter, localReport.OriginalParametersToDrillthrough);
                    }
                    else
                    {
                        parameterValues[parameter.Name] = GetParameterValue(parameter);
                    }
                }

                var dataCommandTemplate = reportDataCommandTemplate as ReportDataCommandTemplate;
                if (dataCommandTemplate != null)
                    using (IDataConnection connection = SimpleQuery.ConfigurationManagerConnection(dataCommandTemplate.DataSourceName))
                    {
                        if (connection.State == ConnectionState.Closed)
                            connection.Open();
                        DataTable table = null;
                        Boolean isOlap = ChangeOlapParameter(dataCommandTemplate.DataSourceName,
                            parameterValues);

                        if (reportDataCommandTemplate.Name.Equals("DealerGroupID")) continue;

                        if (isOlap)
                        {
                            table = ReportAnalyticsClient.GetReportData<DataTable>(GetParameterList(parameterValues));
                        }
                        if (table == null)
                        {
                            using (IDataCommand command = connection.CreateCommand(reportDataCommandTemplate, parameterValues.DataParameterValue))
                            {

                                command.CommandTimeout = 200;
                                using (IDataReader reader = command.ExecuteReader())
                                {
                                    table = (isOlap.Equals(true) ? DataTableMapper.ToOlapDataTable(reader, reportDataCommandTemplate.DataMap, new DataTableCallback()) :
                                        DataTableMapper.ToDataTable(reader, reportDataCommandTemplate.DataMap, new DataTableCallback()));
                                }
                            }

                        }
                    
                        localReport.DataSources.Add(new MsReportDataSource(reportDataCommandTemplate.Name, table));

                    }
            }

            localReport.Refresh();
        }


        private List<string> GetParameterList(DictionaryDataParameterValue dicData)
        {
            List<string> lstObj = new List<string>();
            if (dicData != null)
            {
                // ReSharper disable once GenericEnumeratorNotDisposed
                IEnumerator<KeyValuePair<string, object>> edata = dicData.GetValues().GetEnumerator();

                while (edata.MoveNext())
                {
                    lstObj.Add(Convert.ToString(edata.Current.Value));
                }
            }
            return lstObj;
        }
        #endregion
    
        #region Report Parameter Control Builder Methods
        protected Control ReportParametersControl(IReport report)
        {
            PlaceHolder panel = new PlaceHolder();

            bool autoPostBack = IsAutoPostBack(report.ReportParameters);

            foreach (IReportParameter reportParameter in report.ReportParameters)
            {
                panel.Controls.Add(BuildPanel(reportParameter, autoPostBack));
            }

            ReportParameterSubmit.Visible = !autoPostBack;

            return panel;
        }

        protected static bool IsAutoPostBack(IList<IReportParameter> parameters)
        {
            int numberOfNonHiddenParameters = 0;

            foreach (IReportParameter parameter in parameters)
            {
                if (!parameter.ReportParameterInputType.Equals(ReportParameterInputType.Hidden))
                {
                    numberOfNonHiddenParameters += 1;
                }
            }

            return (numberOfNonHiddenParameters <= 1);
        }


        protected string GetParameterValue(IReportParameter parameter)
        {
            if (parameter.Name.Equals("ReportTitle"))
            {
                return TheReport().Name;
            }

            string value = GetParameterValue(parameter.Name);

            if (value == null)
            {
                IReportParameterValue parameterValue = parameter.DefaultValue(GetParameterValue);
                value = parameterValue?.Value;
            }

            return value;
        }

        private static string GetParameterValue(IDataParameterTemplate parameter, IEnumerable<MsReportParameter> reportParameters)
        {
            foreach (MsReportParameter reportParameter in reportParameters)
            {
                if (reportParameter.Name.Equals(parameter.Name))
                {
                    foreach (string value in reportParameter.Values)
                    {
                        return value;
                    }
                }
            }

            return null;
        }

        protected Control BuildPanel(IReportParameter parameter, bool autoPostBack)
        {
            PlaceHolder panel = new PlaceHolder();

            Control theInput = BuildInput(parameter, autoPostBack);

            if (!parameter.ReportParameterInputType.Equals(ReportParameterInputType.Hidden) && parameter.Label != null)
            {
                panel.Controls.Add(BuildLabel(parameter, theInput.ID));
            }

            panel.Controls.Add(theInput);

            return panel;
        }

        protected static Control BuildLabel(IReportParameter parameter, string id)
        {
            if (parameter.ReportParameterInputType == ReportParameterInputType.Hidden)
            {
                throw new ReportParameterException("Hidden report parameters do not have labels");
            }

            Label label = new Label();
            label.CssClass = parameter.Name;
            label.Text = string.IsNullOrEmpty(parameter.Label) ? "" : parameter.Label + ": ";
            label.AssociatedControlID = id;

            return label;
        }

        protected Control BuildInput(IReportParameter parameter, bool autoPostBack)
        {
            Control control;

            switch (parameter.ReportParameterInputType)
            {
                case ReportParameterInputType.Hidden:
                    HiddenField hiddenField = new HiddenField();
                    hiddenField.ID = parameter.Name;
                    hiddenField.Value = GetParameterValue(parameter);
                    control = hiddenField;
                    break;
                case ReportParameterInputType.Select:
                    DropDownList dropDownList = new DropDownList();
                    dropDownList.ID = parameter.Name;
                    dropDownList.CssClass = parameter.Name + " seletMargin";

                    IList<IReportParameterValue> validValues = parameter.ValidValues(GetParameterValue);

                    var listItems = new ListItem[validValues.Count];
                    int i = 0;
                    foreach (IReportParameterValue value in validValues)
                    {
                        ListItem listItem = new ListItem(value.Label, value.Value);
                        listItem.Selected = value.Selected;
                        listItems[i++] = listItem;
                    }



                    dropDownList.Items.AddRange(listItems);
                    autoPostBack |= String.Equals(parameter.Name, "varToggleRow");
                    dropDownList.AutoPostBack = autoPostBack;
                    if (autoPostBack)
                    {
                        dropDownList.SelectedIndexChanged += DropDown_SelectedIndexChanged;
                    }
                    control = dropDownList;
                    break;
                case ReportParameterInputType.Text:
                    TextBox textBox = new TextBox();
                    textBox.ID = parameter.Name;
                    textBox.CssClass = parameter.Name;
                    textBox.Text = GetParameterValue(parameter);
                    textBox.AutoPostBack = autoPostBack;
                    control = textBox;
                    break;
                case ReportParameterInputType.Checkbox:
                    CheckBox checkBox = new CheckBox();
                    checkBox.ID = parameter.Name;
                    checkBox.CssClass = parameter.Name;
                    checkBox.Checked = ("True".Equals(GetParameterValue(parameter)) || "1".Equals(GetParameterValue(parameter)));
                    checkBox.AutoPostBack = autoPostBack;
                    control = checkBox;
                    break;
                default:
                    throw new ArgumentException("Method not implemented for input type");
            }

            return control;
        }

        protected Boolean ChangeOlapParameter(string connectionName, DictionaryDataParameterValue paramValue)
        {
            bool isOlap = connectionName.Contains("Analysis");
            return isOlap;
        }


        protected void CreateCookieCrum(string reportId)
        {

            switch (reportId)
            {
                default:
                    HyperLink hlGroup = new HyperLink();
                    hlGroup.ID = "id";
                    hlGroup.Text = @"Text";
                    hlGroup.CssClass = "a_font a_color beforelink";
                    hlGroup.NavigateUrl = "";
                    ReportHeaderNavigation.Controls.Add(hlGroup);

                    Label lbSeparater = new Label();
                    lbSeparater.Text = @" > ";
                    ReportHeaderNavigation.Controls.Add(lbSeparater);

                    HyperLink hlFirstChild = new HyperLink();
                    hlFirstChild.ID = "FirstChild";

                    hlFirstChild.NavigateUrl = "";
                    hlFirstChild.CssClass = "afterlink a_font";
                    ReportHeaderNavigation.Controls.Add(hlFirstChild);
                    break;

            }
        }
        #endregion

        protected string GetCurrentPageName()
        {
            string sPath = Request.Url.AbsolutePath;
            var oInfo = new FileInfo(sPath);
            string sRet = oInfo.Name;
            return sRet;
        }
        protected string GetParameterValue(string parameter)
        {
            string value = null;

            // try the web-controls first

            if (IsInitComplete)
            {

                Control c = ControllerUtils.FindControlRecursive(Page, parameter);

                if (c != null)
                {
                    if (c is TextBox)
                    {
                        value = ((TextBox)c).Text;
                    }
                    else if (c is DropDownList)
                    {
                        value = ((DropDownList)c).SelectedValue;
                    }
                    else if (c is CheckBox)
                    {
                        // concert bool check box to 0 or 1 for sql
                        value = ("True".Equals(((CheckBox)c).Checked.ToString())) ? "1" : "0";
                    }
                    else if (c is HiddenField)
                    {
                        value = ((HiddenField)(c)).Value;
                    }
                }
            }

            // then the request parameters
            if (value == null)
            {
                foreach (string key in Request.QueryString.AllKeys)
                {
                    if (key.Equals(parameter))
                    {
                        value = Request.QueryString[key];
                        break;
                    }
                }
            }

            //To check for the pingUrl 
            if (parameter.ToLower().Equals("pingurlprefix"))
            {
                Uri uri = new Uri(Request.Url.ToString());
                string url = string.Format("{0}://{1}", uri.Scheme, uri.Authority);
                url = url + "/IMT/EStock.go?isPopup=true&";
                return url;
            }

            // special check for encoded dealerId
            bool isDealerIdParam = (parameter.Equals("DealerIDs") || parameter.Equals("DealershipID") ||
                                    parameter.Equals("DealerID") || parameter.Equals("UserSelectedDealerID"));
            if (isDealerIdParam && value == null)
            {
                value = Request.QueryString["ContextCode"];
            }

            // ReSharper disable once UnusedVariable
            SoftwareSystemComponentState state = (SoftwareSystemComponentState)Context.Items[SoftwareSystemComponentStateFacade.HttpContextKey];

            // then the report center session
            if (value == null)
            {
                if (isDealerIdParam)
                {
                    //if (state.Dealer.GetValue() != null)
                    //{
                    //    value = state.Dealer.GetValue().Id.ToString();
                    //}
                }
                else if (parameter.Equals("DealerGroupID"))
                {
                    //if (state.DealerGroup.GetValue() != null)
                    //{
                    //    value = state.DealerGroup.GetValue().Id.ToString();
                    //}
                }
                else if (parameter.Equals("MemberId") || parameter.Equals("MemberID"))
                {
                    //value = state.DealerGroupMember().Id.ToString();
                }

                else if (parameter.Equals("AccessibleDealerId") || parameter.Equals("AccessibleDealerID"))
                {
                    value = GetAccessibleDealersId();
                }
            }

            return value;
        }

        //private IEnumerable<BusinessUnit> GetSelectedDealers()
        //{
        //    return (IEnumerable<BusinessUnit>)BusinessUnitSet;
        //}

        private string GetAccessibleDealersId()
        {
            string value = string.Empty;

            //ICollection<BusinessUnit> accessibleDealers = SoftwareSystemComponentState.DealerGroupMember().AccessibleDealers(SoftwareSystemComponentState.DealerGroup.GetValue());
            //if (accessibleDealers != null) value = string.Join(",", accessibleDealers.Select(x => x.Id));
            return value;
        }
        //private ICollection<BusinessUnit> GetAccessibleDealers()
        //{
        //    ICollection<BusinessUnit> accessibleDealers = SoftwareSystemComponentState.DealerGroupMember().AccessibleDealers(SoftwareSystemComponentState.DealerGroup.GetValue());

        //    return accessibleDealers;
        //}
        protected static ReportDataCommandType GetDataSourceType(string name)
        {
            return ("REPORTHOST".Equals(name)) ? ReportDataCommandType.Oltp : ReportDataCommandType.Olap;
        }

        protected void ReportParameterBackText_Load(object sender, EventArgs e)
        {
            Regex regEx = new Regex(@"[^0-9]"); // Everything that is not a number
            if (ReportParameterBackText.Visible && regEx.IsMatch(ReportParameterBackText.OnClientClick))
            {
                int drilldowndepth = int.Parse(regEx.Replace(ReportParameterBackText.OnClientClick, string.Empty));
                ReportParameterBackText.OnClientClick = string.Format("window.history.go(-{0}); return false;", ++drilldowndepth);
            }
            else
            {
                ReportParameterBackText.OnClientClick = "window.history.go(-1); return false;";
            }
        }
    }
}
