using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Web;

// ReSharper disable All

namespace VB.DomainModel.Oltp.WebControls
{
	/// <summary>
	/// Summary description for ApplicationSiteMapProvider.
	/// </summary>
	/// <remarks>
	/// When the provider is configured in the web configuration file it must be
	/// marked as "security trimming enabled".
	/// </remarks>
	public class ApplicationSiteMapProvider : StaticSiteMapProvider
	{
		private readonly string[] AllRoles = new string[] { "*" };

		SiteMapNode rootNode;

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

				SiteMapNode home = new SiteMapNode(this, "Home", "/IMT/DealerHomeDisplayAction.go", "Home", "Home Page");
				home.Roles = AllRoles;
				AddNode(home, tmpRootNode);

				#region Tools Menu

				SiteMapNode tools = new SiteMapNode(this, "Tools", null, "Tools", "Tools Menu");
				tools.Roles = AllRoles;
				AddNode(tools, tmpRootNode);

				AddNodes(new CustomSiteMapNode[]
				         	{
				         		new DealerUpgradeNode(this,"Custom Inventory Analysis", "/IMT/CIASummaryDisplayAction.go", true, 3, false),
				         		new DealerUpgradeNode(this,"First Look Search Engine", "/NextGen/SearchHomePage.go", true, 3, false),		         
                            	new DealerUpgradeNode(this,"Flash Locator", "/NextGen/FlashLocateSummary.go", true, null, false),
				         		new DealerUpgradeNode(this,"Forecaster", "/IMT/DealerOldHomeDisplayAction.go?forecast=1", true, 3, false),
				         		new DealerUpgradeNode(this,"Inventory Management Plan", "/NextGen/InventoryPlan.go", true, 2, false),
				         		new DealerUpgradeNode(this,"Loan Value - Book Value Calculator", "/NextGen/EquityAnalyzer.go", true, 11, false),
                                new DealerUpgradeNode(this,"Make A Deal","/appraisal_review",true,21,false), 
				         		new DealerUpgradeNode(this,"Management Summary", "/IMT/ucbp/TileManagementCenter.go", true, 4, false),
                                new DealerUpgradeNode(this,"Market Velocity Stocking Guide", "/pricing/?token=DEALER_SYSTEM_COMPONENT&pageName=Pages/Market/", true, 22, false),
				         		new DealerUpgradeNode(this,"Performance Analyzer", "/IMT/PerformanceAnalyzerDisplayAction.go", true, 3, false),                        
				         		new DealerUpgradeNode(this,"Trade Analyzer", "/home?ta=true", false, 1, false),
				         		new DealerUpgradeNode(this,"Trade Manager", "/IMT/TradeManagerDisplayAction.go?pageName=bullpen", false,1, false),
				         		new DealerUpgradeNode(this,"Vehicle Analyzer", "/IMT/VehicleAnalyzerDisplayAction.go", false, 3, false),
                                new DealerUpgradeNode(this,"Appraisal Manager","/IMT/AppraisalManagerAction.go",true,null,false), 
                                new DealerPreferenceNode(this,"Create New Inventory", "/support/?pageName=Pages/Inventory/", true, "InTransitInventory", false)
				         	}, tools);

				#endregion

				#region Reports Menu

                SiteMapNode reports = new DealerUpgradeNode(this, "Reports", null, "Reports", "Reports Menu", true, null, false);
				reports.Roles = AllRoles;
				AddNode(reports, tmpRootNode);

				AddNodes(new CustomSiteMapNode[]
				         	{
				         		new DealerUpgradeNode(this,"Action Plans", "/NextGen/ActionPlansReports.go", true, 2, false),
				         		new DealerUpgradeNode(this,"Deal Log", "/IMT/DealLogDisplayAction.go", true, 2, false),
				         		new DealerUpgradeNode(this,"Fastest Sellers", "/IMT/FullReportDisplayAction.go?weeks=26&ReportType=FASTESTSELLER", true, 3, false),
				         		new DealerUpgradeNode(this,"Inventory Manager", "/IMT/DashboardDisplayAction.go?module=U", true, 3, false),
				         		new DealerUpgradeNode(this,"Inventory Overview", "/NextGen/InventoryOverview.go", true, 2, false),
				         		new DealerUpgradeNode(this,"Most Profitable Vehicles", "/IMT/FullReportDisplayAction.go?weeks=26&ReportType=MOSTPROFITABLE", true, 3, false),
				         		new DealerUpgradeNode(this,"Performance Management Reports", "/IMT/ReportCenterRedirectionAction.go", true, 7, false),
				         		new DealerUpgradeNode(this,"Pricing List", "/IMT/PricingListDisplayAction.go", true, 2, false),
				         		new DealerUpgradeNode(this,"Stocking Reports", "/NextGen/OptimalInventoryStockingReports.go", true, 3, false),
				         		new DealerUpgradeNode(this,"Top Sellers", "/IMT/FullReportDisplayAction.go?weeks=26&ReportType=TOPSELLER", true, 3, false),
				         		new DealerUpgradeNode(this,"Total Inventory Report", "/IMT/TotalInventoryReportDisplayAction.go", true, 2, false),
				         		new DealerUpgradeNode(this,"Trades & Purchases Reports", "/IMT/DealerTradesDisplayAction.go", true, 2, false),
				         	}, reports);

				#endregion

				#region Redistribution Menu

                SiteMapNode redistribution = new DealerUpgradeNode(this, "Redistribution", "/IMT/RedistributionHomeDisplayAction.go", "Redistribution", "Redistribution Menu", true, null, false);
				redistribution.Roles = AllRoles;
				AddNode(redistribution, tmpRootNode);

				AddNodes(new CustomSiteMapNode[]
				         	{
				         		new DealerUpgradeNode(this,"Aged Inventory Exchange", "/IMT/InventoryExchangeDisplayAction.go", true, 4, false),
				         		new DealerUpgradeNode(this,"Showroom", "/IMT/ShowroomDisplayAction.go", true, 4, false)
				         	}, redistribution);

				#endregion

				SiteMapNode print = new SiteMapNode(this, "Print", "#Print", "Print", "Print Page");
				print.Roles = AllRoles;
				print["target"] = "Print";
				AddNode(print, tmpRootNode);

                SiteMapNode exit = new DealerUpgradeNode(this, "Exit", "/IMT/ExitStoreAction.go", "Exit Store", "Exit Store", false, null, true);
				exit.Roles = AllRoles;
				AddNode(exit, tmpRootNode);

				rootNode = tmpRootNode;

				return rootNode;
			}
		}

		private void AddNodes(IEnumerable<CustomSiteMapNode> nodes, SiteMapNode parent)
		{
			foreach (CustomSiteMapNode node in nodes)
			{
				node.Roles = AllRoles;
				AddNode(node, parent);
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

        /// <summary>
        /// Custom site map node. Use derived types DealerUpgradeNode or DealerPreferenceNode.
        /// </summary>
        abstract class CustomSiteMapNode : SiteMapNode
        {
            protected readonly bool needsNormalUser;

            protected readonly bool needsStores;

            protected CustomSiteMapNode(SiteMapProvider provider, string name, string url, bool needsNormalUser, bool needsStores)
                : this(provider, name, url, name, name, needsNormalUser, needsStores)
            {
            }

            protected CustomSiteMapNode(SiteMapProvider provider, string key, string url, string title, string description, bool needsNormalUser, bool needsStores)
                : base(provider, key, url, title, description)
            {
                this.needsNormalUser = needsNormalUser;
                this.needsStores = needsStores;
            }
        }

        /// <summary>
        /// Sitemap node whose accessibility is based on a dealer's upgrades.
        /// </summary>
        class DealerUpgradeNode : CustomSiteMapNode
        {
            private readonly int? needsUpgrade;

            public DealerUpgradeNode(SiteMapProvider provider, string name, string url, bool needsNormalUser, int? needsUpgrade, bool needsStores)
				: base(provider, name, url, name, name, needsNormalUser, needsStores)
			{
                this.needsUpgrade = needsUpgrade;
			}

            public DealerUpgradeNode(SiteMapProvider provider, string key, string url, string title, string description, bool needsNormalUser, int? needsUpgrade, bool needsStores)
				: base(provider, key, url, title, description, needsNormalUser, needsStores)
			{				
				this.needsUpgrade = needsUpgrade;				
			}

            public override bool IsAccessibleToUser(HttpContext context)
            {
                bool accessible = base.IsAccessibleToUser(context);

                if (accessible)
                {
                    SoftwareSystemComponentState state = (SoftwareSystemComponentState)context.Items[SoftwareSystemComponentStateFacade.HttpContextKey];
                    //Member member;
                    //BusinessUnit dealer;

                    //if (state != null)
                    //{
                    //    dealer = state.Dealer.GetValue();
                    //    member = state.DealerGroupMember();
                    //}
                    //else
                    //{
                    //    dealer = (BusinessUnit)context.Items[BusinessUnit.HttpContextKey];
                    //    member = (Member)context.Items[MemberFacade.HttpContextKey];
                    //}


                    //if (needsStores)
                    //{
                    //    dealer = (BusinessUnit)context.Items[BusinessUnit.HttpContextKey];
                    //    IPrincipal user = context.User;
                    //    accessible &= (
                    //                    user.IsInRole("Administrator") ||
                    //                    user.IsInRole("AccountRepresentative") ||
                    //                    (member != null && member.AccessibleDealers(dealer.DealerGroup()).Count > 1)
                    //                );
                    //}

                    //if (needsNormalUser)
                    //{
                    //    accessible &= (member != null && !member.HasRestrictedAccess());
                    //}

                    //if (needsUpgrade.HasValue)
                    //{
                    //    accessible &= (dealer != null && dealer.HasDealerUpgrade((Upgrade)needsUpgrade.Value));
                    //}
                }

                return accessible;
            }
        }

        /// <summary>
        /// Sitemap node whose accessibility is based on a dealer's preferences.
        /// </summary>
        class DealerPreferenceNode : CustomSiteMapNode
        {
            private readonly string needsPreference;

            public DealerPreferenceNode(SiteMapProvider provider, string name, string url, bool needsNormalUser, string needsPreference, bool needsStores)
                : base(provider, name, url, name, name, needsNormalUser, needsStores)
            {
                this.needsPreference = needsPreference;
            }

            public DealerPreferenceNode(SiteMapProvider provider, string key, string url, string title, string description, bool needsNormalUser, string needsPreference, bool needsStores)
                : base(provider, key, url, title, description, needsNormalUser, needsStores)
            {
                this.needsPreference = needsPreference;
            }

            public override bool IsAccessibleToUser(HttpContext context)
            {
                bool accessible = base.IsAccessibleToUser(context);

                if (accessible)
                {
                    //SoftwareSystemComponentState state = (SoftwareSystemComponentState)context.Items[SoftwareSystemComponentStateFacade.HttpContextKey];
                    //Member member;
                    //BusinessUnit dealer;

                    //if (state != null)
                    //{
                    //    dealer = state.Dealer.GetValue();
                    //    member = state.DealerGroupMember();
                    //}
                    //else
                    //{
                    //    dealer = (BusinessUnit)context.Items[BusinessUnit.HttpContextKey];
                    //    member = (Member)context.Items[MemberFacade.HttpContextKey];
                    //}

                    //if (needsStores)
                    //{
                    //    IPrincipal user = context.User;
                    //    accessible &= (
                    //                    user.IsInRole("Administator") ||
                    //                    user.IsInRole("AccountRepresentative") ||
                    //                    (member != null && member.Dealers.Count > 1)
                    //                );
                    //}

                    //if (needsNormalUser)
                    //{
                    //    accessible &= (member != null && !member.HasRestrictedAccess());
                    //}

                    //if (!string.IsNullOrEmpty(needsPreference) && needsPreference == "InTransitInventory")
                    //{
                    //    accessible &= (dealer != null && dealer.DealerPreference.ShowInTransitInventoryForm);                        
                    //}
                }

                return accessible;
            }
        }		
	}
}