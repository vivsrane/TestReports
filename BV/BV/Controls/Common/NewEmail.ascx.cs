using System;
using System.Net.Mail;
using System.Web.UI.WebControls;
using BV.AppCode;
using VB.DomainModel.Oltp;

public partial class Controls_Common_NewEmail : System.Web.UI.UserControl
{
    public delegate void EmptyEmailEventHandler(object sender, EventArgs e);

    public delegate void SendEmailEventHandler(object sender, EmailEventArgs e);

    public event EmptyEmailEventHandler Open;

    public event EmptyEmailEventHandler Cancel;

    public event SendEmailEventHandler Send;

    private string subject;
    
    public string Subject
    {
        get { return subject; }
        set { subject = value;}
    }

    protected void EmailButtonOpen_Click(object sender, EventArgs e)
    {
        ClearForm();
        EmailPanel.Visible = true;
        //ReplyField.Text = ((SoftwareSystemComponentState)Context.Items[SoftwareSystemComponentStateFacade.HttpContextKey]).
        //                        AuthorizedMember.GetValue().EmailAddress;
        SubjectField.Text = subject + " - " + DateTime.Now.ToShortDateString();
        if (Open != null)
            Open(this, EventArgs.Empty);
    }

    protected void EmailButtonCancel_Click(object sender, EventArgs e)
    {
        EmailPanel.Visible = false;
        ClearForm();
        if (Cancel != null)
            Cancel(this, EventArgs.Empty);
    }

    protected void EmailButtonSend_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            // build the args
            EmailEventArgs args = new EmailEventArgs();
            args.ReplyTo = new MailAddress(ReplyField.Text);
            foreach (string to in ToField.Text.Split(new char[] {',', ';'}))
            {
                if (!"".Equals(to.Trim()))
                {
                    args.AddRecipient(new MailAddress(to.Trim()));
                }
            }
            args.Subject = SubjectField.Text;
            args.Body = BodyField.Text.Replace(" ", "&nbsp;").Replace(Environment.NewLine, "<br/>");
            //****BUZID:12217
            //args.AttachmentType = (HTMLAttachment.Checked)
            //                          ? EmailEventArgs.EmailAttachmentType.HTML
            //                          : EmailEventArgs.EmailAttachmentType.PDF;
            args.AttachmentType = EmailEventArgs.EmailAttachmentType.PDF;
            //****
            args.AttachmentName = Subject + " - " +
                                  ((SoftwareSystemComponentState)
                                   Context.Items[SoftwareSystemComponentStateFacade.HttpContextKey]).Id + " - " + DateTime.Now.ToString("M-d-yyyy");

            // clear the form
            ClearForm();
            // hide the form
            EmailPanel.Visible = false;
            // broadcast the event
            if (Send != null)
                Send(this, args);
        }
    }

    protected void ClearForm()
    {
        ReplyField.Text = "";
        ToField.Text = "";
        SubjectField.Text = "";
        BodyField.Text = "";
        //BUZID:12217
        //HTMLAttachment.Checked = true;
        //PDFAttachment.Checked = false;
    }

    protected void CustomValidation_Email(object source, ServerValidateEventArgs args)
    {
        args.IsValid = true;
        try
        {
            new MailAddress(args.Value.Trim());
        } catch( FormatException)
        {
            args.IsValid = false;
        }
    }

    protected void CustomValidation_EmailList(object source, ServerValidateEventArgs args)
    {
        args.IsValid = true;
        try
        {
            foreach (string to in ToField.Text.Split(new char[] {',', ';'}))
            {
                if (!"".Equals(to.Trim()))
                {
                    new MailAddress(to.Trim());
                }
            }
        }catch(FormatException)
        {
            args.IsValid = false;
        }
    }
}
