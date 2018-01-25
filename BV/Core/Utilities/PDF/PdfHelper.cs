using System;
using System.Web.Configuration;

namespace VB.Common.Core.Utilities.PDF
{
    public static class PdfHelper
    {
        public static byte[] GeneratePdf(string htmlPage)
        {
            return GeneratePdf(htmlPage, null, null);
        }

        public static byte[] GeneratePdf(string htmlPage, string princeXmlUrl)
        {
            return GeneratePdf(htmlPage, null, princeXmlUrl);
        }

        public static byte[] GeneratePdf(string htmlPage, string[] cssAbsolutePath)
        {
            return GeneratePdf(htmlPage, cssAbsolutePath, null);
        }

        public static byte[] GeneratePdf(string htmlPage, string[] cssAbsolutePath, string princeXmlUrl)
        {
            
            PrinceXmlWebServiceProxy service = new PrinceXmlWebServiceProxy();

            if (!String.IsNullOrEmpty(princeXmlUrl))
                service.Url = princeXmlUrl;
            else
                service.Url = WebConfigurationManager.AppSettings["prince_xml_webservice_url"];            
            
            Input input = new Input();
            input.Content = htmlPage;
            input.ContentType = ContentType.Html;
            if (cssAbsolutePath != null)
            {
                input.StyleSheetPaths = cssAbsolutePath;
            }
            
            OutputType outputType = new OutputType();
            outputType.OutputTypeCode = OutputTypeCode.ByteStream;
            outputType.Item = new SharedFileSystem();
            
            Output output;

            Status status = service.generate(input, outputType, out output);

            if (status.Code == StatusCode.Success)
            {
                return (byte[])output.Item;
            }
            
            throw new Exception(status.Message);
        }

    }    
}
