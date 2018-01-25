<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="BV.Controls.Reports_ReportControl" Codebehind="ReportControl.ascx.cs" %>
<%@ Register TagPrefix="VB" TagName="Email" Src="~/Controls/Common/Email.ascx" %>
<%@ Register TagPrefix="VB" TagName="ReportNavigation" Src="~/Controls/Reports/ReportNavigation.ascx" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<div id='loader'>
    <asp:Image ID="Image1" runat="server" SkinID="ProgressWheel" />
    Loading&hellip;</div>
<div id="controls">
    <asp:UpdatePanel runat="server" UpdateMode="Always" ID="updatePanel1">
        <ContentTemplate>
            <fieldset>
                <asp:PlaceHolder ID="ReportParameterOptions" runat="server" OnInit="ReportParameterOptions_Init" />
                <asp:Label ID="ReportParameterSubmit" runat="server" CssClass="buttonwrapper hide_nonJS dyna_link">
            <asp:LinkButton CssClass="button" runat="server" Text="Go" OnClientClick="$('loader').show();"  OnClick="ReportParameterSubmit_Click"/>
                </asp:Label>
                <asp:Label CssClass="buttonwrapper hide_nonJS dyna_link" runat="server" ID="ReportParameterBack"
                    Visible="false">
                    <asp:LinkButton ID="ReportParameterBackText" CssClass="button" runat="server" OnLoad="ReportParameterBackText_Load"
                        Text="Back"/>
                </asp:Label>
            </fieldset>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:Panel CssClass="tools" ID="ReportExportPanel" runat="server">
        <span class="buttonwrapper hide_nonJS right dyna_link" runat="server">
            <asp:LinkButton ID="ReportExportPDF" runat="server" CssClass="button" Text="Create PDF"
                OnClick="ReportExportPDF_Click" />
        </span><span class="buttonwrapper hide_nonJS right dyna_link">
            <asp:LinkButton ID="ReportExportExcel" runat="server" CssClass="button" Text="Save As Excel"
                OnClick="ReportExportExcel_Click" />
        </span>
        <VB:Email ID="EmailClient" OnSend="ReportExportEmail_Click" OnLoad="ReportExportEmail_Load"
            runat="server" />
    </asp:Panel>
</div>
<asp:UpdatePanel runat="server" UpdateMode="Always" ID="updatePanelTopPagination">
    <ContentTemplate>
        <div id="paging_top" class="paging">
            <VB:ReportNavigation ID="ReportNavigationTop" DirtyPageID="DirtyPage" CurrentPageControlID="CurrentPage"
                ReportViewerID="ReportViewer" AfterControlID="ReportNavigationBottom" runat="server">
            </VB:ReportNavigation>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>
<div id="report" style="display: none;">
    <rsweb:ReportViewer ID="ReportViewer" BorderStyle="none" BackColor="White" runat="server"
        ShowBackButton="false" InternalBorderStyle="None" SizeToReportContent="true"
        ToolBarItemBorderStyle="None" ShowExportControls="False" ShowFindControls="False"
        ExportContentDisposition="AlwaysAttachment" ShowDocumentMapButton="false" ShowPageNavigationControls="false"
        ShowPrintButton="false" ShowPromptAreaButton="false" ShowRefreshButton="false"
        ShowZoomControl="false" Height="100%" Width="100%" OnInit="ReportViewer_Init"
        OnDrillthrough="ReportViewer_Drillthrough" OnBookmarkNavigation="ReportViewer_BookmarkNavigation">
    </rsweb:ReportViewer>
    <asp:HiddenField ID="CurrentPage" runat="server" />
    <asp:HiddenField ID="DirtyPage" runat="server" />
    <asp:UpdatePanel runat="server" UpdateMode="Always" ID="updatePanelBottomPagination">
        <ContentTemplate>
            <div id="paging_bottom" class="paging">
                <VB:ReportNavigation ID="ReportNavigationBottom" DirtyPageID="DirtyPage" CurrentPageControlID="CurrentPage"
                    ReportViewerID="ReportViewer" runat="server"></VB:ReportNavigation>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>
