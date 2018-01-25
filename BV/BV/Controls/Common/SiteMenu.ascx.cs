using System;
using System.Web.UI;
using VB.DomainModel.Oltp;

public partial class Controls_Common_SiteMenu : UserControl
{

    public string backButtonPostBackUrl
    {
        get { return back.NavigateUrl; }
        set { back.NavigateUrl = value; }
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        if (Page != null && (Convert.ToString(Page.Title).Contains("Performance") || Convert.ToString(Page.Title).Contains("Water Report") || Page.ToString().ToUpper().Contains("DEALERGROUPNEWREPORT") || Page.ToString().ToUpper().Contains("DEALERLEVELGROUPPAGE")))
        {
            //HomeLink.Visible = false;
            //MemeberProfile.Visible = false;
            //AboutFL.Visible = false;
            NonPerformance.Visible = false;
            return;
        }
        HomeLink.HRef = ResolveUrl("~/Default.aspx?token="+ SoftwareSystemComponentStateFacade.DealerGroupComponentToken);
    }
}
