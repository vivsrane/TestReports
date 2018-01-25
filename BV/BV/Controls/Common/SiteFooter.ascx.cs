using System;

public partial class SiteFooter : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CopyrightYear.Text = DateTime.Now.Year.ToString();
    }
}
