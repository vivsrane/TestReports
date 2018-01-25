using System;
using BV.AppCode;
using VB.DomainModel.Oltp;
using VB.Reports.App.ReportDefinitionLibrary;

namespace BV.Controls
{
    public partial class Controls_Reports_DealershipReports : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SoftwareSystemComponentState state = (SoftwareSystemComponentState)Context.Items[SoftwareSystemComponentStateFacade.HttpContextKey];
            
            ReportGroupingFactory reportGroupingFactory = WebReportDefinitionFactory.NewReportGroupingFactory();

            //IReportListHelper.PopulateReportList(InventoryOverview, reportGroupingFactory.BuildReportTree("AC3E9462-354C-425c-B667-DF1509FB206D", dealerGroupId));
            //IReportListHelper.PopulateReportList(RepricingReports, reportGroupingFactory.BuildReportTree("F5B3F2BB-9D1D-4bf6-8BDB-D11793CD31A9", dealerGroupId));
            //IReportListHelper.PopulateReportList(SalesReports, reportGroupingFactory.BuildReportTree("C73658DC-40E1-41d3-8D91-1841C44CE5C0", dealerGroupId));
            //IReportListHelper.PopulateReportList(RedistributionReports, reportGroupingFactory.BuildReportTree("B763E0AC-9FBF-4211-9118-D190A672E473", dealerGroupId));
            ////    IReportListHelper.PopulateReportList(SourcingAndSelling, reportGroupingFactory.BuildReportTree("EAFF746F-00C6-4fd2-B9DC-B419F232B343", dealerGroupId));
            //IReportListHelper.PopulateReportList(AppraisalsOverview, reportGroupingFactory.BuildReportTree("D4877301-62E6-455f-B319-F17936582FDF", dealerGroupId));
        }
    }
}
