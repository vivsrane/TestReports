<%@ Control Language="C#" AutoEventWireup="true" Inherits="BV.Controls.Controls_Reports_DealershipReports" Codebehind="DealershipReports.ascx.cs" %>
<%@ Register TagPrefix="reports" TagName="UnorderedList" Src="~/Controls/Reports/UnorderedList.ascx" %>

<div class="x_ dealer_reports">
    <h3>Inventory Management</h3>
    <div class="group">
        <reports:UnorderedList id="InventoryOverview" runat="server"></reports:UnorderedList>
    </div>
    <div class="group">
        <reports:UnorderedList id="RepricingReports" runat="server"></reports:UnorderedList>
    </div>
    <div class="group">
        <reports:UnorderedList id="SalesReports" runat="server"></reports:UnorderedList>
    </div>
</div>
<div class="y_ dealer_reports">
    <h3>Wholesale Buying &amp; Selling</h3>
    <div class="group">
        <reports:UnorderedList id="RedistributionReports" runat="server"></reports:UnorderedList>
    </div>
<%--    <div class="group">
        <reports:UnorderedList id="SourcingAndSelling" runat="server"></reports:UnorderedList>
    </div>--%>
</div>
<div class="z_ dealer_reports">
    <h3>Appraisals</h3>
    <div class="group">
        <reports:UnorderedList id="AppraisalsOverview" runat="server"></reports:UnorderedList>
    </div>
</div>
