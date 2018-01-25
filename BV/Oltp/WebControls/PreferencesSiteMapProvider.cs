using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Web;
using Microsoft.AnalysisServices.AdomdClient;

namespace VB.DomainModel.Oltp.WebControls
{
    class PreferencesSiteMapProvider : StaticSiteMapProvider
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

				SiteMapNode targets =
                    new SiteMapNode(this, "Targets", "/preferences/targets/appraisal_closing_rate", "Targets");
				targets.Roles = AllRoles;
				AddNode(targets, tmpRootNode);


			    SiteMapNode makeADeal =
			        new HasUpgradeSiteMapNode(this, "Make a Deal", "/preferences/make_a_deal/preferences", "Make a Deal",
			                                  string.Empty, true, 21, false);
                makeADeal.Roles = AllRoles;
                AddNode(makeADeal, tmpRootNode);

                // The #DefaultPage is needed to prevent duplicate URL's in the SiteMapNode, PM
			    SiteMapNode internetAdvertisingAccelerator =
                    new OwnerHandleSiteMapNode(this, "Internet Advertising Accelerator", "/pricing/Pages/Preferences/",
                                    "Internet Advertising Accelerator", true, 19, false);
			    internetAdvertisingAccelerator.Roles = AllRoles;
                AddNode(internetAdvertisingAccelerator, tmpRootNode);

			    AddNodes(new SiteMapNode[]
			                 {
                                 new OwnerHandleSiteMapNode(this, "",
			                                     "",
			                                     "", true, 19, false)
			                 },     internetAdvertisingAccelerator);
			
                
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

        private void AddNodes(IEnumerable<SiteMapNode> nodes, SiteMapNode parent)
        {
            foreach (SiteMapNode node in nodes)
            {
                node.Roles = AllRoles;
                AddNode(node, parent);
            }
        }

        class OwnerHandleSiteMapNode : SiteMapNode
        {
            private readonly bool needsNormalUser;
            private readonly int? needsUpgrade;
            private readonly bool needsStores;
            private string ownerHandle;

            public OwnerHandleSiteMapNode(SiteMapProvider provider, string key, string url, string title, bool needsNormalUser, int? needsUpgrade, bool needsStores)
                : base(provider, key, url, title)
            {
                this.needsNormalUser = needsNormalUser;
                this.needsUpgrade = needsUpgrade;
                this.needsStores = needsStores;
            }

            public override bool IsAccessibleToUser(HttpContext context)
            {
                bool isAccessible =  base.IsAccessibleToUser(context);

                if (isAccessible)
                {
                    ownerHandle = context.Request.Params["oh"];

                    if (string.IsNullOrEmpty(ownerHandle))
                    {
                        isAccessible = false;
                    }
                }

                if (isAccessible)
                {
                    SoftwareSystemComponentState state = (SoftwareSystemComponentState)context.Items[SoftwareSystemComponentStateFacade.HttpContextKey];
                   
                }

                return isAccessible;
            }

            public override string Url
            {
                get
                {
                    string returnValue = base.Url;
                    if (!string.IsNullOrEmpty(ownerHandle))
                    {
                        string parameterSeperator = returnValue.Contains("?") ? "&" : "?";

                        if (returnValue.Contains("#"))
                        {
                            int indexOfPound = base.Url.IndexOf("#", StringComparison.OrdinalIgnoreCase);
                            returnValue = returnValue.Substring(0, indexOfPound) + parameterSeperator + "oh=" +
                                          ownerHandle +
                                          returnValue.Substring(indexOfPound, returnValue.Length - indexOfPound);
                        }
                        else
                        {
                            returnValue += parameterSeperator + "oh=" + ownerHandle;
                        }    
                    }

                    return returnValue;
                }
                set
                {
                    base.Url = value;
                }
            }
        }

        class HasUpgradeSiteMapNode : SiteMapNode
		{
			private readonly bool needsNormalUser;
			private readonly int? needsUpgrade;
			private readonly bool needsStores;

			public HasUpgradeSiteMapNode(SiteMapProvider provider, string name, string url, bool needsNormalUser, int? needsUpgrade, bool needsStores)
				: this(provider, name, url, name, name, needsNormalUser, needsUpgrade, needsStores)
			{
			}

			public HasUpgradeSiteMapNode(SiteMapProvider provider, string key, string url, string title, string description, bool needsNormalUser, int? needsUpgrade, bool needsStores)
				: base(provider, key, url, title, description)
			{
				this.needsNormalUser = needsNormalUser;
				this.needsUpgrade = needsUpgrade;
				this.needsStores = needsStores;
			}

			public override bool IsAccessibleToUser(HttpContext context)
			{
				bool accessible = base.IsAccessibleToUser(context);

				if (accessible)
				{
					SoftwareSystemComponentState state = (SoftwareSystemComponentState)context.Items[SoftwareSystemComponentStateFacade.HttpContextKey];
					
				}

				return accessible;
			}
		}        
    }
}
