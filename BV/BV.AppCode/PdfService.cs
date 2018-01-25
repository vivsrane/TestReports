using System.Web.Configuration;
using VB.Common.Core.Utilities.PDF;

namespace BV.AppCode
{
    public class PdfService
    {
        public byte[] GeneratePdf(string pagesHtml)
        {
            PrinceXmlWebServiceProxy service = new PrinceXmlWebServiceProxy(WebConfigurationManager.AppSettings["prince_xml_webservice_url"]);
            
            Input input = new Input();

            input.Content = pagesHtml;
            input.ContentType = ContentType.Html;
            OutputType outputType = new OutputType();
            outputType.OutputTypeCode = OutputTypeCode.ByteStream;
            outputType.Item = new SharedFileSystem();
            Output output;
            service.generate(input, outputType, out output);
            return (byte[])output.Item;
        }
    }
}
