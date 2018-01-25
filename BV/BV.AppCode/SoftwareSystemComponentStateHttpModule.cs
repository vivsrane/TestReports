using System;
using System.Web;
using System.Web.Configuration;
using BV.DomainModel.SoftwareSystem;
using VB.DomainModel.Oltp;

namespace BV.AppCode
{
    public class SoftwareSystemComponentStateHttpModule : IHttpModule
    {
        public static readonly string Key = "BV.WebResourceRegistry";

        public void Dispose()
        {
            // no state to destroy
        }

        public void Init(HttpApplication application)
        {
            application.PostAcquireRequestState += OnPostAcquireRequestState;
        }

        public void OnPostAcquireRequestState(object sender, EventArgs args)
        {
            HttpRequest request = HttpContext.Current.Request;

            string configuration = WebConfigurationManager.AppSettings[Key];

            int i = request.Path.IndexOf('/', 1) + 1;

            string path = request.Path.Substring(i, request.Path.Length - i);

            if (!string.IsNullOrEmpty(path))
            {
                if (path.StartsWith("ErrorPage.aspx"))
                    return;
            }
            else
            {
                path = "Default.aspx";
            }

            try
            {
                string token = WebResourceRegistryCache.CacheCopy(configuration).SoftwareSystemComponent(path);

                SoftwareSystemComponentState state =
                    SoftwareSystemComponentStateFacade.FindOrCreate(HttpContext.Current.User.Identity.Name, token);

                AccessController.Instance().VerifyPermissions(state, token);

                HttpContext.Current.Items.Add(SoftwareSystemComponentStateFacade.HttpContextKey, state);
            }
            catch (IndexOutOfRangeException)
            {
                // ignore the code
            }
        }
    }
}
