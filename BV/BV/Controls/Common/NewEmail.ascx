<%@ Control Language="C#" AutoEventWireup="true" Inherits="Controls_Common_NewEmail" Codebehind="NewEmail.ascx.cs" %>

<asp:UpdatePanel runat="server" ID="EmailUpdatePanel">
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="EmailHeaderPanel" />
        <asp:AsyncPostBackTrigger ControlID="EmailButtonOpen" />
        <asp:PostBackTrigger ControlID="EmailButtonSend" />
    </Triggers>
    
    <ContentTemplate>
    <%--<span class="buttonwrapper right hide_nonJS dyna_link">
        <asp:LinkButton ID="EmailButtonOpen" runat="server" Text="Email" CssClass="button" OnClick="EmailButtonOpen_Click" />
    </span>--%>
    <asp:Button runat="server" ID="EmailButtonOpen" Text="Email" CssClass="linkbutton emailbutton" OnClick="EmailButtonOpen_Click"></asp:Button>
    <asp:Panel ID="EmailPanel" runat="server" CssClass="email inlinepopup" Visible="false">
        <asp:LinkButton ID="EmailHeaderPanel" runat="server" OnClientClick="Element.extend(this).up().hide(); return false;" OnClick="EmailButtonCancel_Click" CssClass="closebox right">
            <asp:Literal ID="EmailButtonClose" runat="server">Cancel</asp:Literal>
        </asp:LinkButton>
        <h3>Email</h3>
        <asp:ValidationSummary ID="EmailValidationSummary" EnableClientScript="false" ValidationGroup="EmailValidationGroup" runat="server" DisplayMode="List" CssClass="error" />
        <ul>
            <li class="toSection">
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" Display="Dynamic" EnableClientScript="true" ControlToValidate="ToField" Text="***" SetFocusOnError="true" ErrorMessage="'To' email address needs to be filled in." ValidationGroup="EmailValidationGroup"></asp:RequiredFieldValidator>
                <asp:CustomValidator ID="CustomValidator1" runat="server" Display="Dynamic" EnableClientScript="false" Text="***" ValidateEmptyText="false" ControlToValidate="ToField" ErrorMessage="You have entered an invalid email address." OnServerValidate="CustomValidation_EmailList" ValidationGroup="EmailValidationGroup"></asp:CustomValidator>
                <asp:Label ID="ToFieldLabel" AssociatedControlID="ToField" runat="server">To <small>(required)</small>:</asp:Label>
                <asp:TextBox ID="ToField" CssClass="wide" runat="server" TextMode="MultiLine" Rows="2" Columns="35" AccessKey="1"></asp:TextBox>
                <small>Enter multiple email addresses seperated by a comma.</small>
            </li>
            <li class="replySection">
                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" Display="Dynamic" EnableClientScript="false" ControlToValidate="ReplyField" Text="***" SetFocusOnError="true" ErrorMessage="'Reply To' email address needs to be filled in." ValidationGroup="EmailValidationGroup"></asp:RequiredFieldValidator>
                <asp:CustomValidator ID="CustomValidator2" runat="server" Display="Dynamic" EnableClientScript="false" Text="***" ValidateEmptyText="false" ControlToValidate="ReplyField" ErrorMessage="'Reply To' doesn't contain a valid email address." OnServerValidate="CustomValidation_Email" ValidationGroup="EmailValidationGroup"></asp:CustomValidator>
                <asp:Label ID="ReplyFieldLabel" AssociatedControlID="ReplyField" runat="server">Reply To <small>(required)</small>:</asp:Label>
                <asp:TextBox ID="ReplyField" CssClass="wide" runat="server" AccessKey="2"></asp:TextBox>
            </li>
            <li class="subjectSection">
                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" Display="Dynamic" EnableClientScript="false" ControlToValidate="SubjectField" Text="***" SetFocusOnError="true" ErrorMessage="'Subject' needs to be filled in." ValidationGroup="EmailValidationGroup"></asp:RequiredFieldValidator>
                <asp:Label ID="SubjectFieldLabel" AssociatedControlID="SubjectField" runat="server">Subject <small>(required)</small>:</asp:Label>
                <asp:TextBox ID="SubjectField" CssClass="wide" runat="server" TextMode="MultiLine" AccessKey="3" Rows="2" Columns="35"></asp:TextBox>
            </li>
            <li class="bodySection">
                <asp:Label ID="BodyFieldLabel" AssociatedControlID="BodyField" runat="server">Body:</asp:Label>
                <asp:TextBox ID="BodyField" CssClass="wide" runat="server" TextMode="MultiLine" Rows="5" Columns="35" AccessKey="4"></asp:TextBox>
                <small>Enter any note you wish to include along with the report.</small>
            </li>
        </ul>
               <%-- <p> BUZID:12217
                <asp:Label ID="AttachmentTypeLabel" runat="server" >
                    Send as an HTML or a PDF attachment?
                </asp:Label>
                </p>
                <asp:RadioButton ID="HTMLAttachment" GroupName="AttachmentType" Text=" HTML" Checked="true" runat="server" />
                <asp:RadioButton ID="PDFAttachment" GroupName="AttachmentType" Text=" PDF" Checked="false" runat="server" />--%>
                <span class="buttonwrapper right hide_nonJS dyna_link">
                    <asp:LinkButton ID="EmailButtonSend" runat="server" Text="Send Email" CssClass="button" OnClick="EmailButtonSend_Click" ValidationGroup="EmailValidationGroup" />
                </span>

    </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>

