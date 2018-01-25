using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BV.AppCode;
using VB.DomainModel.Oltp;
using VB.Reports.App.ReportDefinitionLibrary;

namespace BV.Controls
{
    public partial class Controls_Reports_UnorderedList : System.Web.UI.UserControl, IReportList
    {
        private string title = "";

        private readonly IList<IReportHandle> reports = new List<IReportHandle>();

        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        public IList<IReportHandle> Reports
        {
            get { return reports; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            TitleControl.Text = HttpUtility.HtmlEncode(title);
            UnorderedListControl.DataSource = reports;
            UnorderedListControl.DataBind();
        }

        protected void DefinitionListControl_ItemDataBound(object source, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                HyperLink link = (HyperLink)e.Item.FindControl("TitleInLink");
                Label text = (Label)e.Item.FindControl("TitleNoLink");
                Image soon = (Image)e.Item.FindControl("ComingSoon");
                Image newReport = (Image)e.Item.FindControl("NewReport");
                if (link != null && text != null && soon != null)
                {
                    IReportHandle handle = (IReportHandle)e.Item.DataItem;
                    if (handle.ComingSoon)
                    {
                        text.Text = HttpUtility.HtmlEncode(handle.Title);
                        link.Visible = false;
                        newReport.Visible = false;
                    }
                    else
                    {
                        SoftwareSystemComponentState state = (SoftwareSystemComponentState)Context.Items[SoftwareSystemComponentStateFacade.HttpContextKey];

                        string token = state.SoftwareSystemComponent.GetValue().Token;

                        string path;

                        if (token.Equals(SoftwareSystemComponentStateFacade.DealerGroupComponentToken))
                        {
                            if (handle.Title.ToUpper().Contains("WATER"))
                            {
                                path = "~/DealerWaterGroupReport.aspx?drillthrough=" + GetDealerId(state, "DealershipReportSelect") + "&type=pmr&cp=cc";
                            }
                            else if (handle.Report.Id.Equals("C4DE8552-68E4-451E-9F22-F6CEE9359D89") || handle.Report.Id.Equals("B3D7BCDC-8BC9-4929-BFC5-0F6C0B1AD97E") )
                            {
                                path = "~/DealerGroupNewReport.aspx?drillthrough=" + GetDealerId(state, "DealershipReportSelect") + "&type=pmr";
                            }
                            else if (handle.Report.Id.Equals("E44A8300-742C-4CB9-A8CB-3A87CAAA1DF6") || handle.Report.Id.Equals("D57CF3D5-A075-4032-938C-8A6A2AA5B32B"))
                            {
                                string qTradeOrPurchase = "&TradeOrPurchase=";
                                qTradeOrPurchase += handle.Report.Id.Equals("E44A8300-742C-4CB9-A8CB-3A87CAAA1DF6")
                                    ? "1"
                                    : "2";
                                path = "~/DealerGroupNewReport.aspx?drillthrough=" + GetDealerId(state, "DealershipReportSelect") + "&type=pmr" + qTradeOrPurchase;
                            }
                            else if (handle.Report.Id.Equals("C82458C3-A914-4F38-8741-8AB2CCF7D5AD"))//New RetailInventorySalesAnalysis
                            {
                                string tradeOrPurchase = "&TradeOrPurchase=";
                                tradeOrPurchase += "A";
                                path = "~/DealerGroupNewReport.aspx?drillthrough=" + GetDealerId(state, "DealershipReportSelect") + "&type=pmr&cp=cc" + tradeOrPurchase;
                            }
                            else if (handle.Report.Id.Equals("F6B512EC-D492-47D3-BB7F-5A437D531816") || handle.Report.Id.Equals("BE4CD8E3-37C4-4E06-876E-0E6F9746D7A5") || handle.Report.Id.Equals("A4CBC38A-81C0-47D0-BC48-15A7D6F2B701"))
                            {
                                path = "~/DealerGroupNewReport.aspx?SelectedDealerId=" + GetDealerId(state, "DealershipReportSelect") + "&type=pmr";
                            }
                            else
                            {
                                path = "~/DealerGroupReport.aspx";    
                            }
                        
                        }
                        else
                    
                        {
                            if (handle.Report.Id.Equals("C4DE8552-68E4-451E-9F22-F6CEE9359D89") || handle.Report.Id.Equals("B3D7BCDC-8BC9-4929-BFC5-0F6C0B1AD97E") )
                            {
                                path = "~/DealerLevelGroupPage.aspx?drillthrough=" + GetDealerId(state, "DealershipReportSelect") + "&type=pmr";
                            }
                            else if (handle.Report.Id.Equals("F6B512EC-D492-47D3-BB7F-5A437D531816") || handle.Report.Id.Equals("BE4CD8E3-37C4-4E06-876E-0E6F9746D7A5") || handle.Report.Id.Equals("A4CBC38A-81C0-47D0-BC48-15A7D6F2B701"))
                            {
                                path = "~/DealerLevelGroupPage.aspx?SelectedDealerId=" + GetDealerId(state, "DealershipReportSelect") + "&type=pmr";
                            }
                            else if (handle.Report.Id.Equals("E44A8300-742C-4CB9-A8CB-3A87CAAA1DF6") || handle.Report.Id.Equals("D57CF3D5-A075-4032-938C-8A6A2AA5B32B"))
                            {
                                string qTradeOrPurchase = "&TradeOrPurchase=";
                                qTradeOrPurchase += handle.Report.Id.Equals("E44A8300-742C-4CB9-A8CB-3A87CAAA1DF6")
                                    ? "1"
                                    : "2";
                                path = "~/DealerLevelGroupPage.aspx?drillthrough=" + GetDealerId(state, "DealershipReportSelect") + "&type=pmr" + qTradeOrPurchase;
                            }
                            else if (handle.Report.Id.Equals("C82458C3-A914-4F38-8741-8AB2CCF7D5AD"))//New RetailInventorySalesAnalysis
                            {
                                string tradeOrPurchase = "&TradeOrPurchase=";
                                tradeOrPurchase += "A";
                                path = "~/DealerLevelGroupPage.aspx?drillthrough=" + GetDealerId(state, "DealershipReportSelect") + "&type=pmr&cp=dr" + tradeOrPurchase;
                            }
                            else
                            {
                                path = handle.Title.ToUpper().Contains("WATER") ? "~/DealerWaterGroupReport.aspx?drillthrough=" + GetDealerId(state, "DealershipReportSelect") + "&type=pmr&cp=dr" : "~/DealerReport.aspx";
                            }
                        }

                        link.Text = HttpUtility.HtmlEncode(handle.Title);
                        link.NavigateUrl = path + (path.Contains("?") ? "&Id=" : "?Id=") + handle.Report.Id;
                        text.Visible = false;
                        soon.Visible = false;
                        newReport.Visible = handle.Report.IsNew;
                    }
                }

            }
        }

        protected string GetDealerId(SoftwareSystemComponentState state,string param="")
        {
            string value = String.Empty;
            Control c = ControllerUtils.FindControlRecursive(Page, param);
            if (c == null)
            {
                //value =  Convert.ToString(state.Dealer.GetValue().Id);
            }
            else
            {
                if (Convert.ToString((c as DropDownList).SelectedValue as object) != "")
                {
                    value = (c as DropDownList).SelectedValue;
                }
                else
                {
                    //value = Convert.ToString(state.Dealer.GetValue().Id);
                }
            
            }
            return value;
        }
    }
}
