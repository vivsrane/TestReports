<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="BV.Controls.Reports_NewReportControl" Codebehind="NewReportControl.ascx.cs" %>
<%@ Register TagPrefix="VB" TagName="Email" Src="~/Controls/Common/NewEmail.ascx" %>
<%@ Register TagPrefix="VB" TagName="ReportNavigation" Src="~/Controls/ReportNavigation.ascx" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<style type="text/css">
    .hideJS
    {
        display: none;
    }
    #controls
    {
        clear: both;
        float: left;
        margin-top: 10px;
    }
    .afterlink
    {
        margin-right: 10px;
        margin-left: 5px;
    }
    .beforelink
    {
        margin-left: 20px;
    }
    .seletMargin
    {
        margin-left: 10px;
        margin-right: 10px;
    }
    .a_font
    {
        font-size: 1.1em;
        font-family: Verdana;
    }
    .a_color
    {
        color: rgb(76,129,169);
        text-decoration: none;
        border-bottom: 1px solid rgb(76,129,169);
    }
    .newRow
    {
        clear: both;
        float: left;
        margin-left: 20px;
    }
    .linkbutton
    {
        background-image: -webkit-linear-gradient(top, #FFFFFF, #E8E8E8, #FFFFFF);
        background-image: -moz-linear-gradient(top, #FFFFFF, #E8E8E8, #FFFFFF);
        background-image: -ms-linear-gradient(top,#FFFFFF, #E8E8E8, #FFFFFF);
        background-image: -ms-linear-gradient(top,#FFFFFF, #E8E8E8, #FFFFFF);
        filter: progid:DXImageTransform.Microsoft.Gradient(startColorStr='#FFFFFF',endColorStr='#E8E8E8',GradientType=0);
        border: 1px solid #ACBECF;
        cursor: pointer;
        width: 50px;
        height: 19px;
    }
    .emailbutton
    {
        height: 25px;
        width: 65px;
        float: right;
        margin-right: 7px;
        font-weight: bold;
        font-family: Verdana, Helvetica, sans-serif;
    }
    .NumberOfDays
    {
        margin-left: 12px;
    }
    .desc_ItalicFont
    {
        font-size: 1em;
        font-style: italic;
        font-family: Verdana, Helvetica, sans-serif;
    }
</style>
<div id='loader'>
    <asp:Image ID="Image1" runat="server" SkinID="ProgressWheel" />
    Loading&hellip;</div>
<div id="controls">
    <fieldset>
        <div style="clear: both; float: left;">
            <asp:PlaceHolder ID="ReportHeaderNavigation" runat="server" OnInit="ReportHeaderNavigation_Init" />
            <br />
            <br />
        </div>
        <div style="clear: both; float: left; margin-left: 10px">
            <asp:PlaceHolder ID="ReportParameterOptions" runat="server" OnInit="ReportParameterOptions_Init" />
            <asp:Label ID="ReportParameterSubmit" runat="server" CssClass="hide_nonJS dyna_link">
                <asp:Button CssClass="linkbutton" runat="server" ID="LinkButton1" Width="50px" Text="Go"
                    OnClientClick="$('loader').show();" OnClick="ReportParameterSubmit_Click" />
                <%--<asp:LinkButton ID="LinkButton1" CssClass="" runat="server" Text="Go" OnClientClick="$('loader').show();"  OnClick="ReportParameterSubmit_Click"/>--%>
            </asp:Label>
        </div>
        <asp:Label CssClass="buttonwrapper hide_nonJS dyna_link" runat="server" ID="ReportParameterBack"
            Visible="false">
            <asp:LinkButton ID="ReportParameterBackText" CssClass="button" runat="server" OnLoad="ReportParameterBackText_Load"
                Text="Back" />
        </asp:Label>
    </fieldset>
    <asp:Panel CssClass="tools" ID="ReportExportPanel" runat="server">
        <%--   <span class="buttonwrapper hide_nonJS right dyna_link" runat="server" ID="spanExport">
            <asp:LinkButton ID="ReportExportPDF" runat="server" CssClass="button" Text="Create PDF" OnClick="ReportExportPDF_Click" />
        </span>--%>
    </asp:Panel>
</div>
<input type="hidden" id="IsPmrReport" runat="server" viewstatemode="Enabled" value="false" />
<div style="width: 200px; float: right; padding-top: 12px;">
    <%-- <span class="buttonwrapper hide_nonJS right dyna_link">--%>
    <%--<asp:LinkButton ID="ReportExportExcel" runat="server" CssClass="button" Text="Export" OnClick="ReportExportExcel_Click" />--%>
    <asp:Button runat="server" ID="ReportExportExcel" CssClass="linkbutton emailbutton"
        Text="Export" OnClick="ReportExportExcel_Click" />
    <%-- </span>--%>
    <VB:Email ID="EmailClient" OnSend="ReportExportEmail_Click" OnLoad="ReportExportEmail_Load"
        runat="server" />
</div>
<div id="paging_top" class="paging hideJS">
    <VB:ReportNavigation ID="ReportNavigationTop" DirtyPageID="DirtyPage" CurrentPageControlID="CurrentPage"
        ReportViewerID="ReportViewer" AfterControlID="ReportNavigationBottom" runat="server">
    </VB:ReportNavigation>
</div>
<div id="report" style="display: none;">
    <rsweb:ReportViewer ID="ReportViewer" BorderStyle="none" BackColor="White" runat="server"
        InternalBorderStyle="None" SizeToReportContent="true" ToolBarItemBorderStyle="None"
        ShowExportControls="False" ShowFindControls="False" ExportContentDisposition="AlwaysAttachment"
        ShowDocumentMapButton="false" ShowPageNavigationControls="false" ShowPrintButton="false"
        ShowPromptAreaButton="false" ShowRefreshButton="false" ShowZoomControl="false"
        ShowBackButton="False" Height="100%" Width="100%" OnInit="ReportViewer_Init"
        OnDrillthrough="ReportViewer_Drillthrough" OnBookmarkNavigation="ReportViewer_BookmarkNavigation">
    </rsweb:ReportViewer>
    <asp:HiddenField ID="CurrentPage" runat="server" />
    <asp:HiddenField ID="DirtyPage" runat="server" />
    <div id="paging_bottom" class="paging">
        <VB:ReportNavigation ID="ReportNavigationBottom" DirtyPageID="DirtyPage" CurrentPageControlID="CurrentPage"
            ReportViewerID="ReportViewer" runat="server"></VB:ReportNavigation>
    </div>
</div>
