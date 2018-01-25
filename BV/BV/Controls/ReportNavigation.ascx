<%@ Control Language="C#" AutoEventWireup="true" Inherits="BV.Controls.ReportNavigation" Codebehind="ReportNavigation.ascx.cs" %>

<asp:Panel CssClass="tools" ID="NavigationPanel" runat="server">
    <asp:ImageButton ID="FirstPage" SkinID="FirstPageIcon" AlternateText="First Page" Enabled="false" OnClick="FirstPage_Click" runat="server" />
    <asp:ImageButton ID="FirstPagePassive" SkinID="FirstPagePassiveIcon" AlternateText="First Page" Enabled="false" OnClick="FirstPage_Click" runat="server" />
    <asp:ImageButton ID="PreviousPage" SkinID="PreviousPageIcon" AlternateText="Previous Page" Enabled="false" OnClick="PreviousPage_Click" runat="server" />
    <asp:ImageButton ID="PreviousPagePassive" SkinID="PreviousPagePassiveIcon" AlternateText="Previous Page" Enabled="false" OnClick="PreviousPage_Click" runat="server" />
    &nbsp;<asp:TextBox MaxLength="8" size="3" ID="PageNumberTextBox" runat="server" OnTextChanged="PageTextChanged"/>&nbsp;of&nbsp;<asp:Label ID="TotalPages" runat="server" Text="Label"></asp:Label>&nbsp;
    <asp:ImageButton ID="NextPage" SkinID="NextPageIcon" AlternateText="Next Page" Enabled="false" OnClick="NextPage_Click" runat="server" />
    <asp:ImageButton ID="NextPagePassive" SkinID="NextPagePassiveIcon" AlternateText="Next Page" Enabled="false" OnClick="NextPage_Click" runat="server" />
    <asp:ImageButton ID="LastPage" SkinID="LastPageIcon" AlternateText="Last Page" Enabled="false" OnClick="LastPage_Click" runat="server" />
    <asp:ImageButton ID="LastPagePassive" SkinID="LastPagePassiveIcon" AlternateText="Last Page" Enabled="false" OnClick="LastPage_Click" runat="server" />
    <div class="error none">Invalid Page Number</div>
    <asp:HiddenField ID="PageCountHidden" runat="server"/>
</asp:Panel>
