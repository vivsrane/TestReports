using System;
using System.Runtime.CompilerServices;
using System.Web;

namespace VB.DomainModel.Oltp.WebControls
{
    public class FooterSiteMapProvider : StaticSiteMapProvider
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

                SiteMapNode preferences = new SiteMapNode(this, "Preferences", "/preferences/make_a_deal/preferences", "Preferences");
                preferences.Roles = AllRoles;
                preferences["target"] = "preferences";
                AddNode(preferences, tmpRootNode);

                SiteMapNode performanceManagementReports = new SiteMapNode(this, "Performance Management Reports", "/IMT/ReportCenterRedirectionAction.go", "Performance Management Reports");
                performanceManagementReports.Roles = AllRoles;
                performanceManagementReports["target"] = "performanceManagementReports";
                AddNode(performanceManagementReports, tmpRootNode);

                SiteMapNode loanValueAndBookValueCalculator = new SiteMapNode(this, "Loan Value - Book Value Calculator", "/NextGen/EquityAnalyzer.go", "Loan Value - Book Value Calculator");
                loanValueAndBookValueCalculator.Roles = AllRoles;
                loanValueAndBookValueCalculator["target"] = "loanValueAndBookValueCalculator";
                AddNode(loanValueAndBookValueCalculator, tmpRootNode);

                //var dealer = (BusinessUnit)HttpContext.Current.Items[BusinessUnit.HttpContextKey];
                SiteMapNode waterReport = new SiteMapNode(this, "Water Report", String.Format("/command_center/DealerWaterGroupReport.aspx?drillthrough={0}&type=pmr&cp=dr&Id=E6A6F567-2E14-433A-ABCA-0E97DA8A1595",0),"Water Report");
                waterReport.Roles = AllRoles;
                waterReport["target"] = "waterReport";
                AddNode(waterReport, tmpRootNode);

                SiteMapNode performanceAnalyzer = new SiteMapNode(this, "Performance Analyzer", "/IMT/PerformanceAnalyzerDisplayAction.go", "Performance Analyzer");
                performanceAnalyzer.Roles = AllRoles;
                performanceAnalyzer["target"] = "performanceAnalyzer";
                AddNode(performanceAnalyzer, tmpRootNode);

                SiteMapNode mgmtSummary = new SiteMapNode(this, "Mgmt. Summary", "/IMT/ucbp/TileManagementCenter.go", "Mgmt. Summary");
                mgmtSummary.Roles = AllRoles;
                mgmtSummary["target"] = "mgmtSummary";
                AddNode(mgmtSummary, tmpRootNode);

                SiteMapNode inventoryOverview = new SiteMapNode(this, "Inventory Overview", "/NextGen/InventoryOverview.go", "Inventory Overview");
                inventoryOverview.Roles = AllRoles;
                inventoryOverview["target"] = "inventoryOverview";
                AddNode(inventoryOverview, tmpRootNode);

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
