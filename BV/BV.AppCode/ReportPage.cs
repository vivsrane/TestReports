using System;
using System.Web.UI;
using VB.DomainModel.Oltp;

namespace BV.AppCode
{
    public abstract class ReportPage : Page
    {
        protected abstract string Token();

        protected virtual void Page_PreInit(object sender, EventArgs e)
        {
            bool haveReportId = false;

            foreach (string key in Request.Params.Keys)
            {
                if ("Id".Equals(key))
                {
                    haveReportId = true;
                }
            }

            if (haveReportId)
            {
                SoftwareSystemComponentState state =
                    SoftwareSystemComponentStateFacade.FindOrCreate(Context.User.Identity.Name, Token());

                AccessController.Instance().VerifyPermissions(state, Token());

                Context.Items[SoftwareSystemComponentStateFacade.HttpContextKey] = state;
            }
            else
            {
                Response.Redirect("~/Default.aspx", true);
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            //Add Google Analytics code if IsAnalyicsEnabled is true.
            if (this.IsAnalyicsEnabled)
            {
               
            }
        }


        #region IGAnalytics Members

        public string PageTitle { get; set; }

        public bool IsAnalyicsEnabled { get; set; }


        #endregion
    }
}