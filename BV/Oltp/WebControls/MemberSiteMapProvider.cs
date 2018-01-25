using System.Runtime.CompilerServices;
using System.Web;

namespace VB.DomainModel.Oltp.WebControls
{
    /// <summary>
    /// Summary description for ApplicationSiteMapProvider.
    /// </summary>
    /// <remarks>
    /// When the provider is configured in the web configuration file it must be
    /// marked as "security trimming enabled".
    /// </remarks>
    public class MemberSiteMapProvider : StaticSiteMapProvider
    {
        private readonly string[] AllRoles = new string[] { "*" };

        private SiteMapNode rootNode;

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override SiteMapNode BuildSiteMap()
        {
            lock (this)
            {
                if (rootNode != null)
                    return rootNode;

                SiteMapNode tmpRootNode = new SiteMapNode(this,
                                                          "RootNode",
                                                          null,
                                                          "Root Node",
                                                          "The Root Node of the SiteMap but is not part of the navigation");
                tmpRootNode.Roles = AllRoles;

                SiteMapNode memberProfile =
                    new SiteMapNode(this, "Member Profile", "/IMT/EditMemberProfileAction.go", "Member Profile");
                memberProfile.Roles = AllRoles;
                memberProfile["target"] = "memberProfile";
                AddNode(memberProfile, tmpRootNode);

                SiteMapNode aboutFirtLook = new SiteMapNode(this, "About First Look", "/support/Marketing_Pages/AboutUs.aspx", "About First Look");
                aboutFirtLook.Roles = AllRoles;
                aboutFirtLook["target"] = "about";
                AddNode(aboutFirtLook, tmpRootNode);

                SiteMapNode contactVB =
                    new SiteMapNode(this, "Contact First Look", "/support/Marketing_Pages/ContactUs.aspx", "Contact First Look");
                contactVB.Roles = AllRoles;
                contactVB["target"] = "contact";

                AddNode(contactVB, tmpRootNode);
                SiteMapNode logOut = null;
                if (HttpContext.Current.Request.Url.AbsolutePath.ToLower().Contains("/support/"))
                {
                    logOut = new SiteMapNode(this, "Log Out", "/support/LogOff.aspx", "Log Out");
                }
                else
                {
                    logOut = new SiteMapNode(this, "Log Out", "/merchandising/LogOff.aspx", "Log Out");
                }
                // TODO: Needs to be a global url or configurable by solution
                logOut.Roles = AllRoles;
                AddNode(logOut, tmpRootNode);

                rootNode = tmpRootNode;

                return rootNode;
            }
        }

        protected override SiteMapNode GetRootNodeCore()
        {
            lock (this)
            {
                if (rootNode == null)
                {
                    BuildSiteMap();
                }
                return rootNode;
            }
        }
    }
}