using System;
using System.Collections.Generic;
using System.Net.Mail;

namespace BV.AppCode
{
    public class EmailEventArgs : EventArgs
    {
        public enum EmailAttachmentType
        {
            HTML,
            PDF
        };

        private MailAddress replyTo;

        private readonly List<MailAddress> to;
        private string subject;
        private string body;
        private EmailAttachmentType attachmentType;
        private string attachmentName;

        public EmailEventArgs()
        {
            to = new List<MailAddress>();
        }

        public MailAddress ReplyTo
        {
            get { return replyTo; }
            set { replyTo = value; }
        }

        public IEnumerable<MailAddress> To
        {
            get { return to; }
        }

        public string Subject
        {
            get { return subject; }
            set { subject = value; }
        }

        public string AttachmentName
        {
            get { return attachmentName; }
            set { attachmentName = value; }
        }

        public string Body
        {
            get { return body; }
            set { body = value; }
        }

        public EmailAttachmentType AttachmentType
        {
            get { return attachmentType; }
            set { attachmentType = value; }
        }

        public void AddRecipient(MailAddress recipient)
        {
            to.Add(recipient);
        }
    }
}