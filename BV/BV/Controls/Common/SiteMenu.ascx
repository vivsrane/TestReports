<%@ Control Language="C#" AutoEventWireup="true" Inherits="Controls_Common_SiteMenu" Codebehind="SiteMenu.ascx.cs" %>

<div id="topnavmenubar">
<div runat="server" id="NonPerformance">
<a id="HomeLink" runat="server">Home</a> | 
<a runat="server" id="MemeberProfile" href="~/EnterDealership.aspx?application=profile" target="_blank">Member Profile</a> | 
<a href="/support/Marketing_Pages/AboutUs.aspx" runat="server" id="AboutFL" class="dyna_link">About First Look</a> | 
</div>
<span runat="server" id="backLink" Visible="False"><asp:HyperLink ID="back" runat="server">Back</asp:HyperLink>  | </span>
<a href="/support/Marketing_Pages/ContactUs.aspx" id="ContactFL" class="dyna_link">Contact First Look</a>  |
<asp:HyperLink ID="LogOffLink" NavigateUrl="~/LogOff.aspx" runat="server">Log Out</asp:HyperLink>
</div>