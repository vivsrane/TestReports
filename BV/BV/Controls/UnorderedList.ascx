<%@ Control Language="C#" AutoEventWireup="true" Inherits="BV.Controls.Controls_Reports_UnorderedList" Codebehind="UnorderedList.ascx.cs" %>

    <h4 class="toggle">
        <asp:Image SkinID="ToggleArrowOpen" runat="server" CssClass="invisible_nonJS" />
        <asp:Literal ID="TitleControl" runat="server"></asp:Literal>
    </h4>
    <asp:Repeater ID="UnorderedListControl" runat="server" OnItemDataBound="DefinitionListControl_ItemDataBound">
        <HeaderTemplate>
            <ul>
        </HeaderTemplate>
        <ItemTemplate>
            <li>
                <asp:HyperLink ID="TitleInLink" CssClass="reportlink" runat="server"></asp:HyperLink>
                <asp:Label ID="TitleNoLink" runat="server"></asp:Label>
                <asp:Image ID="NewReport" SkinID="NewIcon" ImageAlign="AbsBottom" runat="server" />
                <asp:Image ID="ComingSoon" SkinID="ComingSoonIcon" ImageAlign="AbsBottom" runat="server" />
            </li>
        </ItemTemplate>
        <FooterTemplate>
            </ul>
        </FooterTemplate>
    </asp:Repeater>

