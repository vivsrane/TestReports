<%@ Control Language="C#" AutoEventWireup="true" Inherits="BV.Controls.Controls_Reports_DefinitionList" Codebehind="DefinitionList.ascx.cs" %>
<h3><asp:Literal ID="TitleControl" runat="server"></asp:Literal></h3>
<asp:Repeater ID="DefinitionListControl" runat="server" OnItemDataBound="DefinitionListControl_ItemDataBound">
    <HeaderTemplate>
        <dl class="group">
    </HeaderTemplate>
    <ItemTemplate>
        <dt>
            <asp:HyperLink ID="TitleInLink" CssClass="reportlink" runat="server"></asp:HyperLink>
            <asp:Label ID="TitleNoLink" runat="server"></asp:Label>
        </dt>
        <dd><asp:Label ID="Description" runat="server"></asp:Label> <asp:Image ID="ComingSoon" SkinID="ComingSoonIcon" ImageAlign="AbsBottom" runat="server" /><asp:Image ID="NewReport" SkinID="NewIcon" ImageAlign="AbsBottom" runat="server" /></dd>
    </ItemTemplate>
    <FooterTemplate>
        </dl>
    </FooterTemplate>
</asp:Repeater>
