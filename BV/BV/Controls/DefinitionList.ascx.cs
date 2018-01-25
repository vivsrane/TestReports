using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI.WebControls;
using VB.DomainModel.Oltp;
using VB.Reports.App.ReportDefinitionLibrary;
using BV;
using BV.AppCode;

namespace BV.Controls
{
    public partial class Controls_Reports_DefinitionList : System.Web.UI.UserControl, IReportList
    {
        private string _title = "";

        private readonly IList<IReportHandle> _reports = new List<IReportHandle>();

        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        public IList<IReportHandle> Reports
        {
            get { return _reports; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            TitleControl.Text = HttpUtility.HtmlEncode(_title);
            DefinitionListControl.DataSource = _reports;
            DefinitionListControl.DataBind();
        }

        protected void DefinitionListControl_ItemDataBound(object source, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                HyperLink link = (HyperLink)e.Item.FindControl("TitleInLink");
                Label text = (Label)e.Item.FindControl("TitleNoLink");
                Label desc = (Label)e.Item.FindControl("Description");
                Image soon = (Image)e.Item.FindControl("ComingSoon");
                Image newReport = (Image)e.Item.FindControl("NewReport");
                if (link != null && text != null && soon != null)
                {
                    IReportHandle handle = (IReportHandle)e.Item.DataItem;
                    desc.Text = HttpUtility.HtmlEncode(handle.Description);
                    if (handle.ComingSoon)
                    {
                        text.Text = HttpUtility.HtmlEncode(handle.Title);
                        link.Visible = false;
                        newReport.Visible = false;
                    }
                    else
                    {
                        SoftwareSystemComponentState state = (SoftwareSystemComponentState) Context.Items[SoftwareSystemComponentStateFacade.HttpContextKey];

                        string token = state.SoftwareSystemComponent.GetValue().Token;

                        string path;

                        if (token.Equals(SoftwareSystemComponentStateFacade.DealerGroupComponentToken))
                        {
                            if (handle.Report != null && handle.Report.Id.Equals("E8E56305-3117-48CF-AB00-CD117F15AE9E"))
                            {
                                path = "~/DealerGroupNewReport.aspx";
                            }
                            else if (handle.Report != null && handle.Report.Name.Contains("Water"))
                            {
                                path = "~/DealerWaterGroupReport.aspx";
                            }
                            else
                            {
                                path = "~/DealerGroupReport.aspx";    
                            }
                        
                        }
                        else
                        {
                            path = "~/DealerReport.aspx";
                        }

                        link.Text = HttpUtility.HtmlEncode(handle.Title);
                        link.NavigateUrl = path + "?Id=" + handle.Report.Id;
                        text.Visible = false;
                        soon.Visible = false;
                        newReport.Visible = handle.Report.IsNew;
                    }
                }
            }
        }
    }
}
