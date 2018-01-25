using System;

namespace BV.Controls
{
    public partial class Controls_Reports_DatePicker : System.Web.UI.UserControl
    {
        public System.Web.UI.WebControls.TextBox To
        {
            get { return to; }
            set { to = value; }
        }
        public System.Web.UI.WebControls.TextBox From
        {
            get { return from; }
            set { from = value; }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
            {
                From.Text = Request[from.UniqueID];
                To.Text = Request[to.UniqueID];
            }
            else
            {
                From.Text = string.IsNullOrEmpty(From.Text) ?  DateTime.Now.Date.AddDays(-30).ToString(System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern) : From.Text;
                To.Text =string.IsNullOrEmpty(To.Text) ? DateTime.Now.Date.ToString(System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern) : To.Text;
            }
        }
    }
}