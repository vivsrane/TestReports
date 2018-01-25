using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using VB.DomainModel.Oltp;
using VB.Reports.App.ReportDefinitionLibrary;
using MsBookmarkNavigationEventArgs = Microsoft.Reporting.WebForms.BookmarkNavigationEventArgs;
using MsDrillthroughEventArgs = Microsoft.Reporting.WebForms.DrillthroughEventArgs;
using MsLocalReport = Microsoft.Reporting.WebForms.LocalReport;
using MsReportDataSource = Microsoft.Reporting.WebForms.ReportDataSource;
using MsReportParameter = Microsoft.Reporting.WebForms.ReportParameter;
using MsReportViewer = Microsoft.Reporting.WebForms.ReportViewer;
using MsServerReport = Microsoft.Reporting.WebForms.ServerReport;
using MsWarning = Microsoft.Reporting.WebForms.Warning;

namespace BV.Controls
{
    public partial class Reports_ReportControl : UserControl
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
                return report!=null? report.Name:null;
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
        //Event to track the drill through in parent
        public event DrillthroughEventHandler DrillThrough;
        protected SoftwareSystemComponentState SoftwareSystemComponentState
        {
            get { return (SoftwareSystemComponentState)Context.Items[SoftwareSystemComponentStateFacade.HttpContextKey]; }
            set { Context.Items[SoftwareSystemComponentStateFacade.HttpContextKey] = value; }
        }
        //protected ICollection<BusinessUnit> BusinessUnitSet
        //{
        //    get { return (ICollection<BusinessUnit>)Context.Items[MemberBusinessUnitSetFacade.HttpContextKey]; }
        //    set { Context.Items[MemberBusinessUnitSetFacade.HttpContextKey] = value; }
        //}
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

        /// <summary>
        /// Initialize the report viewer control.
        /// </summary>
        protected void ReportViewer_Init(object sender, EventArgs e)
        {
            ReportViewer.AsyncRendering = false;
            ReportViewer.ExportContentDisposition = MsContentDisposition.AlwaysAttachment;
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (ScriptManager.GetCurrent(Page) == null)
            {
                throw new Exception("No Script Manager. Please add script manager on page.");
            }
            IsInitComplete = true;

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

            byte[] content;
            if (TheReport().Name.ToLower().Contains("water"))
            {
                content = ReportViewer.LocalReport.Render("Excel", "<DeviceInfo><SimplePageHeaders>False</SimplePageHeaders></DeviceInfo>",
                    out mimeType, out encoding, out fileNameExtension, out streams, out warnings);

            }
            else
            {
                content = ReportViewer.LocalReport.Render(
                    "PDF", "<DeviceInfo></DeviceInfo>",
                    out mimeType, out encoding, out fileNameExtension, out streams, out warnings);    
            
            }


            string attachmentName = Regex.Replace(TheReport().Name, "\\W|[0-9]", "") + "." + fileNameExtension;

            MailMessage message = new MailMessage();
            message.From = new MailAddress("no-reply@VB.biz");
            message.ReplyTo = e.ReplyTo;
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
            Response.AddHeader("Content-Disposition", "attachment; filename=" + TheReport().Name + "." + fileNameExtension);
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
            if (ScriptManager.GetCurrent(this.Page) != null && ScriptManager.GetCurrent(this.Page).EnableHistory)
            {
                ScriptManager.GetCurrent(this.Page).AddHistoryPoint("reportname", e.Report.DisplayName);
            }
        
            if (DrillThrough != null)
            {
                DrillThrough(this, e);
            }
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
            ReportParameterBackText.Text = "Back to " + TheReport().Name;

        
        }

        #region Bind Data Source
        protected void BindDataSource(MsReportViewer reportViewer, IReport report)
        {
            if (report.ReportProcessingLocation is ILocalReportProcessingLocation)
            {
                reportViewer.ProcessingMode = MsProcessingMode.Local;

                BindLocalReport(reportViewer.LocalReport, report);
            }
            else
            {
                reportViewer.ProcessingMode = MsProcessingMode.Remote;

                BindServerReport(reportViewer.ServerReport, report);
            }
        }

        protected void BindServerReport(MsServerReport serverReport, IReport report)
        {
            IRemoteReportProcessingLocation location = (IRemoteReportProcessingLocation)report.ReportProcessingLocation;

            try
            {
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
            //localReport.ExecuteReportInCurrentAppDomain(AppDomain.CurrentDomain.Evidence);
            //localReport.AddFullTrustModuleInSandboxAppDomain(new System.Security.Policy.StrongName(new System.Security.Permissions.StrongNamePublicKeyBlob(Nullable),"VB.Reports.App.ReportStyleLibrary.StyleSheet","1.0.0"));
            //localReport.AddTrustedCodeModuleInCurrentAppDomain(System.Reflection.Assembly.GetAssembly(typeof(VB.Reports.App.ReportStyleLibrary.StyleSheet)).ToString());

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

                foreach (IReportParameter parameter in reportDataCommandTemplate.Parameters)
                {
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

                using (IDataConnection connection = SimpleQuery.ConfigurationManagerConnection(location.DataSourceName))
                {
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    using (IDataCommand command = connection.CreateCommand(reportDataCommandTemplate, parameterValues.DataParameterValue))
                    {
                        command.CommandTimeout = 200;
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            DataTable table = DataTableMapper.ToDataTable(reader);
                            localReport.DataSources.Add(new MsReportDataSource(reportDataCommandTemplate.Name, table));
                        }
                    }
                }
            }

            localReport.Refresh();
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
                value = (parameterValue == null ? null : parameterValue.Value);
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
            label.Text = parameter.Label + ": ";
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
                    dropDownList.CssClass = parameter.Name;
                    IList<IReportParameterValue> validValues = parameter.ValidValues(this.GetParameterValue);
                    ListItem[] listItems;
                    Boolean hide = false;
                    // this report need to show the Group name as well in the drop down
                    if (TheReport().Id == "E6A6F567-2E14-433A-ABCA-0E97DA8A1595")
                    {
                        listItems = new ListItem[validValues.Count + 1];
                        string DrillThroughBussinessId = string.Empty;
                        if (Request.QueryString.HasKeys() && Request.QueryString.GetValues("drillthrough") != null)
                        {
                            DrillThroughBussinessId = Request.QueryString.GetValues("drillthrough")[0].ToString();
                        }
                        else
                        {
                            if (SoftwareSystemComponentState.Dealer != null) DrillThroughBussinessId = SoftwareSystemComponentState.Dealer.GetValue().Id.ToString();
                        }
                        //check if page request is made from the PerformanceManagementPage
                        string parentCallingPage = Convert.ToString(System.Web.HttpContext.Current.Request.ServerVariables["HTTP_REFERER"] as object)
                            .ToUpper();
                        string pmrType = string.Empty;
                        if (Request.QueryString.HasKeys() && Request.QueryString.GetValues("type") != null)
                        {
                            pmrType = Request.QueryString.GetValues("type")[0];
                        }
                        if (parentCallingPage.Contains("PERFORMANCE") || parentCallingPage.Contains("DEALERREPORT") || pmrType == "pmr")
                        {
                            hide = true;
                        }
                        ListItem firstListItem = new ListItem(SoftwareSystemComponentState.DealerGroup.GetValue().Name, GetSelectedDealersId());
                        firstListItem.Selected = false;
                        listItems[0] = firstListItem;
                        int i = 1;
                        foreach (IReportParameterValue value in validValues)
                        {
                            ListItem listItem = new ListItem(value.Label, value.Value);
                            listItem.Selected = DrillThroughBussinessId == value.Value ? true : value.Selected;
                            listItems[i++] = listItem;
                        }
                   
                    }
                    else
                    {
                        listItems = new ListItem[validValues.Count];
                        int i = 0;
                        foreach (IReportParameterValue value in validValues)
                        {
                            ListItem listItem = new ListItem(value.Label, value.Value);
                            listItem.Selected = value.Selected;
                            listItems[i++] = listItem;
                        }
                    }

                    //Trade analyzer and Avg Imm. wholesale dealer report does not require Delaer list to be displayed.
                    if ((TheReport().Id.ToLower() == "c625a438-4429-4a8c-a3a2-366805acddf9" || TheReport().Id.ToLower() == "59c0e4ef-95bb-4031-8b61-28020d9e91f9") && Request.QueryString.HasKeys() && Request.QueryString.GetValues("type") != null && parameter.Name.Contains("SelectedDealerId"))
                    {
                        string pmrType = Request.QueryString.GetValues("type")[0];
                        if (pmrType == "pmr")
                        {
                            hide = true;
                        }
                    }
                    dropDownList.Items.AddRange(listItems);
                    autoPostBack |= String.Equals(parameter.Name, "varToggleRow");
                    dropDownList.AutoPostBack = autoPostBack;
                    if (autoPostBack)
                    {
                        dropDownList.SelectedIndexChanged += DropDown_SelectedIndexChanged;
                    }
                    control = dropDownList;
                    if (hide) control.Visible = false;
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
                    checkBox.Checked = ( "True".Equals(GetParameterValue(parameter)) || "1".Equals(GetParameterValue(parameter)) );
                    checkBox.AutoPostBack = autoPostBack;
                    control = checkBox;
                    break;
                default:
                    throw new ArgumentException("Method not implemented for input type");
            }

            return control;
        }
        #endregion

        //
        //
        //

        protected string GetParameterValue(string parameter)
        {
            string value = null;

            // try the web-controls first

            if (IsInitComplete)
            {

                Control c= ControllerUtils.FindControlRecursive(Page, parameter);    
           
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
                        value = ("True".Equals( ((CheckBox)c).Checked.ToString() ) ) ? "1" : "0";
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

            // special check for encoded dealerId
            bool isDealerIdParam = (parameter.Equals("DealerIDs") || parameter.Equals("DealershipID") ||
                                    parameter.Equals("DealerID") || parameter.Equals("UserSelectedDealerID"));
            if (isDealerIdParam && value == null)
            {
                value = Request.QueryString["ContextCode"];
            }

            SoftwareSystemComponentState state = (SoftwareSystemComponentState) Context.Items[SoftwareSystemComponentStateFacade.HttpContextKey];

            // then the report center session
            if (value == null)
            {
                if (isDealerIdParam)
                {
                    if (state.Dealer.GetValue() != null)
                    {
                        value = state.Dealer.GetValue().Id.ToString();
                    }
                }
                else if (parameter.Equals("DealerGroupID"))
                {
                    if (state.DealerGroup.GetValue() != null)
                    {
                        value = state.DealerGroup.GetValue().Id.ToString();
                    }
                }
                else if (parameter.Equals("MemberId") || parameter.Equals("MemberID"))
                {
                    value = state.DealerGroupMember().Id.ToString();
                }
                else if (parameter.Equals("SelectedDealerId") || parameter.Equals("SelectedDealerID"))
                {
                    value = GetSelectedDealersId();
                }
                else if (parameter.Equals("AccessibleDealerId") || parameter.Equals("AccessibleDealerID"))
                {
                    value = GetAccessibleDealersId();
                }
            }

            return value;
        }
        private string GetSelectedDealersId()
        {
            string value = string.Empty;
            //int memberId = SoftwareSystemComponentState.DealerGroupMember().Id;
        
            //IList<BusinessUnit> businessunitList = BusinessUnitFinder.Instance().FindAllDealershipsByDealerGroupAndMember(SoftwareSystemComponentState.DealerGroup.GetValue().Id, memberId);
            //if (BusinessUnitSet != null && BusinessUnitSet.Count>0) value = string.Join(",", BusinessUnitSet.Select(x => x.Id));
            return value;
        }

        private string GetAccessibleDealersId()
        {
            string value = string.Empty;
        
            ICollection<BusinessUnit> accessibleDealers = SoftwareSystemComponentState.DealerGroupMember().AccessibleDealers(SoftwareSystemComponentState.DealerGroup.GetValue());
            if (accessibleDealers != null) value = string.Join(",", accessibleDealers.Select(x => x.Id));
            return value;
        }
        protected static ReportDataCommandType GetDataSourceType(string name)
        {
            return ("REPORTHOST".Equals(name)) ? ReportDataCommandType.Oltp : ReportDataCommandType.Olap;
        }

        protected void ReportParameterBackText_Load(object sender, EventArgs e)
        {
            //Regex regEx = new Regex(@"[^0-9]"); // Everything that is not a number
            //if (ReportParameterBackText.Visible && regEx.IsMatch(ReportParameterBackText.OnClientClick))
            //{
            //    int drilldowndepth = int.Parse(regEx.Replace(ReportParameterBackText.OnClientClick, string.Empty));
            //    ReportParameterBackText.OnClientClick = string.Format("window.history.go(-{0});", ++drilldowndepth);
            //}
            //else
            //{
            //    ReportParameterBackText.OnClientClick = "window.history.go(-1);";
            //}
            ReportParameterBackText.OnClientClick = "window.history.go(-1);window.location.reload();";
        }
    

    }
}
