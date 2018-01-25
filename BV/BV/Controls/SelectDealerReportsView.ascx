<%@ Control Language="C#" AutoEventWireup="true" Inherits="BV.Controls.Controls_Reports_SelectDealerReportsView" Codebehind="SelectDealerReportsView.ascx.cs" %>

<%@ Register Assembly="VB.DomainModel.Oltp" Namespace="VB.DomainModel.Oltp.WebControls" TagPrefix="owc" %>

<asp:ObjectDataSource
    id="BusinessUnitDataSource"
    TypeName="VB.DomainModel.Oltp.BusinessUnitFinder"
    SelectMethod="FindAllDealershipsByDealerGroupAndMember"
    runat="server">
    <SelectParameters>
        <owc:DealerGroupParameter Name="dealerGroup" Type="Object" />
        <owc:MemberParameter Name="member" Type="Object" />
    </SelectParameters>
</asp:ObjectDataSource>
<asp:Label id="DealershipReportSelectLabel" runat="server" AssociatedControlid="DealershipReportSelect" Text="Dealer Reports for:"></asp:Label>
<asp:DropDownList
    id="DealershipReportSelect"
    runat="server"
    DataSourceid="BusinessUnitDataSource"
    DataTextField="Name"
    DataValueField="Id"
    OnDataBound="DealershipReportSelect_DataBound"
    OnSelectedIndexChanged="DealershipReportSelect_SelectedIndexChanged"
    AutoPostBack="true"
    AppendDataBoundItems="true">
</asp:DropDownList>
<input type="hidden" id="DealershipReportSelectID" value="<%= DealershipReportSelect.ClientID %>" />
<asp:Button ID="SubmitButton" runat="server" Text="Submit" CssClass="hide_inline"/>