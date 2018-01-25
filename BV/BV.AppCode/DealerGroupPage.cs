using System;
using System.IO;
using System.Net.Mail;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Web.UI;
using System.Xml;
using System.Xml.Resolvers;
using VB.DomainModel.Oltp;

namespace BV.AppCode
{

    public class DealerGroupPage : Page
    {
        //
        // Support Methods
        //

        private MailMessage _message;

        private static readonly string Token = SoftwareSystemComponentStateFacade.DealerGroupComponentToken;


        protected SoftwareSystemComponentState SoftwareSystemComponentState
        {
            get
            {
                return (SoftwareSystemComponentState) Context.Items[SoftwareSystemComponentStateFacade.HttpContextKey];
            }
            set { Context.Items[SoftwareSystemComponentStateFacade.HttpContextKey] = value; }
        }



        //
        // Page Initialize
        //

        protected void Page_Init(object sender, EventArgs e)
        {
            if (SoftwareSystemComponentState == null)
            {
                SoftwareSystemComponentState state = SoftwareSystemComponentStateFacade.FindOrCreate(
                    User.Identity.Name, Token);
                AccessController.Instance().VerifyPermissions(state, Token);
                SoftwareSystemComponentState = state;
            }
        }

        protected void EmailClient_Send(object sender, EmailEventArgs e)
        {
            Context.Items["EmailRequest"] = "true";
            Context.Items["EmailAttachmentName"] = e.AttachmentName + "." + e.AttachmentType.ToString().ToLower();

            _message = new MailMessage {ReplyTo = e.ReplyTo};
            _message.From = new MailAddress("no-reply@VB.biz");
            foreach (MailAddress to in e.To)
                _message.To.Add(to);
            _message.Subject = e.Subject;
            _message.IsBodyHtml = true;
            _message.Body = e.Body;
        }

        protected override void Render(HtmlTextWriter writer)
        {
            StringBuilder sbOut = new StringBuilder();
            StringWriter swOut = new StringWriter(sbOut);
            HtmlTextWriter htwOut = new HtmlTextWriter(swOut);
            base.Render(htwOut);

            string originalHTML = sbOut.ToString();

            if ("true".Equals(Context.Items["EmailRequest"]))
            {
                XmlDocument document = new XmlDocument();

                string xhtmlStr = sbOut.ToString();

                XmlPreloadedResolver resolver = new XmlPreloadedResolver();
                resolver.Add(resolver.ResolveUri(null, "Static/xhtml-trans10.dtd"), xhtmlStr);
                XmlReaderSettings readerSettings = new XmlReaderSettings();
                readerSettings.XmlResolver = resolver;
                readerSettings.DtdProcessing = DtdProcessing.Parse;

                StringReader sr = new StringReader(xhtmlStr);
                XmlReader reader = XmlReader.Create(sr, readerSettings);

                document.Load(reader);
                XmlElement root = document.DocumentElement;
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(document.NameTable);
                nsmgr.AddNamespace("h", document.DocumentElement.NamespaceURI);

                RewriteImageURLs(root.SelectNodes("//h:img", nsmgr));
                RewriteCSSPaths(root.SelectNodes("//h:link", nsmgr));
                TransformNodes(document, root.SelectNodes("//h:a", nsmgr), "span");
                DeleteNodes(root.SelectNodes("//h:div[@id='inventory-graph']", nsmgr));
                DeleteNodes(root.SelectNodes("//h:div[@id='chart']", nsmgr));
                DeleteNodes(root.SelectNodes("//h:script", nsmgr));
                DeleteNodes(root.SelectNodes("//h:input[@type='hidden']", nsmgr));
                // this bit of xpath gets all elements that have onclick attributes
                RemoveOnClickEvents(root.SelectNodes("self::node()/descendant-or-self::node()[@onclick]", nsmgr));


                string reportHTML =
                    "<!DOCTYPE html PUBLIC \" -//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\n";
                reportHTML += document.DocumentElement.OuterXml;
                _message.Body += "<br/>" + reportHTML;

                if (Context.Items["EmailAttachmentName"].ToString().EndsWith("pdf"))
                {
                    PdfService service = new PdfService();
                    byte[] pdf = service.GeneratePdf(reportHTML);

                    _message.Attachments.Add(new Attachment(new MemoryStream(pdf),
                        Context.Items["EmailAttachmentName"].ToString()));
                }
                else if (Context.Items["EmailAttachmentName"].ToString().EndsWith("html"))
                {
                    ASCIIEncoding encoder = new ASCIIEncoding();
                    _message.Attachments.Add(new Attachment(new MemoryStream(encoder.GetBytes(reportHTML)),
                        Context.Items["EmailAttachmentName"].ToString()));
                }

                SmtpClient client = new SmtpClient();
                client.Send(_message);
            }

            writer.Write(originalHTML);
        }

        private void RewriteImageURLs(XmlNodeList nodes)
        {
            string filePath;
            foreach (XmlNode node in nodes)
            {
                filePath = node.Attributes["src"].Value;
                if (filePath.IndexOf("VB.biz") == -1 && filePath.IndexOf("localhost") == -1)
                {
                    node.Attributes["src"].Value = GetBasePath() + ResolveUrl(filePath);
                }
            }
        }

        private void RewriteCSSPaths(XmlNodeList nodes)
        {
            foreach (XmlNode node in nodes)
            {
                if (node.Attributes["type"] != null && node.Attributes["type"].Value.Contains("css"))
                {
                    node.Attributes["href"].Value = GetBasePath() + ResolveUrl(node.Attributes["href"].Value);
                }
            }
        }

        // transform nodes into a newNodeType while keeping their contents
        private static void TransformNodes(XmlDocument document, XmlNodeList nodes, string newNodeType)
        {
            foreach (XmlNode node in nodes)
            {
                XmlNode newNode = document.CreateNode("element", newNodeType, "http://www.w3.org/1999/xhtml");
                foreach (XmlAttribute oldAttribute in node.Attributes)
                {
                    // do not copy anchor-specific attributes
                    if (node.Name.Equals("a") && !newNodeType.Equals("a"))
                    {
                        if (oldAttribute.Name.Contains("href") || oldAttribute.Name.Contains("shape") ||
                            oldAttribute.Name.Contains("xmlns"))
                        {
                            continue;
                        }
                    }
                    XmlAttribute newAttribute = document.CreateAttribute(oldAttribute.Name);
                    newAttribute.Value = oldAttribute.Value;
                    newNode.Attributes.Append(newAttribute);
                }

                foreach (XmlNode childNode in node.ChildNodes)
                {
                    newNode.AppendChild(childNode);
                }
                node.ParentNode.InsertBefore(newNode, node);
                node.ParentNode.RemoveChild(node);

            }
        }

        // remove nodes including their contents
        private static void DeleteNodes(XmlNodeList nodes)
        {
            foreach (XmlNode node in nodes)
            {
                node.ParentNode.RemoveChild(node);
            }
        }

        private static void RemoveOnClickEvents(XmlNodeList nodes)
        {
            foreach (XmlNode node in nodes)
            {
                node.Attributes.RemoveNamedItem("onclick");
            }
        }

        private string GetBasePath()
        {
            string protocol;

            string port = Request.ServerVariables["SERVER_PORT"];

            if (port == null || port == "80" || port == "443")
                port = "";
            else
                port = ":" + port;

            if (Request.IsSecureConnection || "https".Equals(Request.Headers["X_FORWARDED_PROTO"]))
            {
                protocol = "https://";
            }
            else
            {
                protocol = "http://";
            }

            return protocol + Request.ServerVariables["SERVER_NAME"] + port;
        }

        // helper method for front-end

        protected bool IsNull(object item)
        {
            return item == null || item == DBNull.Value;
        }

        protected double ToDouble(object item)
        {
            return (IsNull(item) || double.IsNaN(Convert.ToDouble(item))) ? 0 : Convert.ToDouble(item);
        }



        #region IGAnalytics Members

        public string PageTitle { get; set; }

        public bool IsAnalyicsEnabled { get; set; }

        #endregion
    }

}