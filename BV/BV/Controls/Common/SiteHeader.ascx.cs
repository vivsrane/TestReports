using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;


public partial class SiteHeader : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //DealerStoreNickname.Text = "Windy City Automotive Group";
        
        //DealerGroup.Text = "Windy City Automotive Group";
        //ReportCenterSession reportCenterSession = (ReportCenterSession)Session[SessionStateHttpModule.REPORT_CENTER_SESSION_KEY];

        //if (PageHeader.Controls.Contains(DealerStoreNickname))
        //{
        //    if (reportCenterSession.IsDealerGroup())
        //    {
        //        DealerStoreNickname.Text = reportCenterSession.DealerGroup().Name;
        //    }
        //    else
        //    {
        //        DealerStoreNickname.Text = reportCenterSession.Dealership().Name;
        //    }
        //}
    }
}
