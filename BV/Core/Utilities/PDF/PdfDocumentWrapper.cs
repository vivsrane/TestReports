using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace VB.Common.Core.Utilities.PDF
{
    public class PdfDocumentWrapper
    {
        private static BaseColor FooterColor;

        static PdfDocumentWrapper()
        {
            //#5b5f62
            int red = int.Parse( "5b", NumberStyles.HexNumber );
            int green = int.Parse( "5f", NumberStyles.HexNumber );
            int blue = int.Parse( "62", NumberStyles.HexNumber );

            FooterColor = new BaseColor( red, green, blue );
        }

        public static byte[] Merge( Collection<byte[]> pdfCollection )
        {
            return Merge( pdfCollection, null );
        }

        public static byte[] Merge( Collection<byte[]> pdfCollection, string script )
        {
            Document document = new Document(PageSize.LETTER);
            return Merge(pdfCollection, document, script);
        }

        public static byte[] Merge (Collection<byte[]> pdfCollection, float topMargin, float leftMargin, float rightMargin, float bottomMargin, string script )
        {
            Document document = new Document(PageSize.LETTER, leftMargin, rightMargin, topMargin, bottomMargin);
            return Merge(pdfCollection, document, script);
        }

        private static byte[] Merge(Collection<byte[]> pdfCollection, Document document, string script)
        {
            if(pdfCollection == null)
                throw new ArgumentNullException("pdfCollection");

            // get a list of readers for the collection of bytes passed in
            var readers = pdfCollection.Where(b => b != null).Select(b => new PdfReader(b)).ToList();
            var totalPages = readers.Sum(r => r.NumberOfPages);

            byte[] mergedPdf;
            using (var stream = new MemoryStream())
            {
                var writer = new PdfSmartCopy(document, stream);
                document.Open();

                var currentPage = 1;
                readers.ForEach(r => { currentPage = Append(writer, r, document, currentPage, totalPages); });

                if (!string.IsNullOrEmpty(script))
                {
                    var action = PdfAction.JavaScript(script, writer);
                    writer.AddJavaScript(action);
                }

                document.Close();
                stream.Flush();
                mergedPdf = stream.ToArray();
            }

            return mergedPdf;
        }

        private static int Append(PdfCopy writer, PdfReader reader, Document document, int currentPage, int totalPages)
        {
            var origPageSize = document.PageSize;

            for (int i = 0; i < reader.NumberOfPages; i++)
            {
                // read the page size and rotation from the original pdf
                var pageRect = reader.GetPageSizeWithRotation(i + 1);

                // set the page size of the read in page (and if there is an orientation change)
                // this has to happen *before* adding a "new" page
                document.SetPageSize(pageRect);
                document.NewPage();

                // get the imported page from the reader
                var importedPage = writer.GetImportedPage(reader, i + 1);

                // create a page stamp so we can add page numbers
                var stamp = writer.CreatePageStamp(importedPage);

                // get the content to write the page numbers to
                var content = stamp.GetUnderContent();

                // add the text into the page stamp content
                var pageText = String.Format("Page {0} of {1}", currentPage, totalPages);
                content.BeginText();
                content.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED), 8);
                content.SetColorFill(FooterColor);
                content.ShowTextAligned(PdfContentByte.ALIGN_CENTER, pageText, pageRect.Width - 92, 5, 0);
                content.EndText();

                // commit the changes made to the page stamp content
                stamp.AlterContents();

                // append the altered page to the copy
                writer.AddPage(importedPage);

                document.SetPageSize(origPageSize);
                currentPage++;
            }

            return currentPage;
        }

        public void SignDocument()
        {
            //if( !IsLoaded ) { return; } //TODO: PdfDocumentWrapper.SignDocument(): consider throwing an exception on null.

            PdfReader reader = new PdfReader( new byte[0] );
            Document document = new Document( reader.GetPageSizeWithRotation( 1 ) );
            using( MemoryStream stream = new MemoryStream() )
            {
                PdfStamper stp = PdfStamper.CreateSignature( reader, stream, PdfWriter.VERSION_1_7 );
                PdfSignatureAppearance sap = stp.SignatureAppearance;
                sap.SetVisibleSignature( "blah" );
                sap.SignDate = DateTime.Now;
                sap.SetCrypto( null, null, null, null );
                sap.Reason = "";
                sap.Location = "";
                sap.Acro6Layers = true;
                sap.Render = PdfSignatureAppearance.SignatureRender.NameAndDescription;

                PdfSignature sig = new PdfSignature( PdfName.ADOBE_PPKLITE, PdfName.ADBE_PKCS7_DETACHED );
                sig.Date = new PdfDate( sap.SignDate );
                //sig.Name = PdfPKCS7.GetSubjectFields(  
                sig.Reason = sap.Reason;
                sig.Location = sap.Location;

                sap.CryptoDictionary = sig;

                Dictionary<PdfName, int> dic = new Dictionary<PdfName, int>();
                dic[PdfName.CONTENTS] = 4000*2 + 2;
                sap.PreClose( dic );

                //PdfCopy writer = new PdfCopy( document, stream );

                //document.Open();
                //for( int i = 0; i < reader.NumberOfPages; )
                //{
                //    writer.AddPage( writer.GetImportedPage( reader, ++i ) );
                //}

                //PRAcroForm form = reader.AcroForm;
                //if( form != null ) { writer.CopyAcroForm( reader ); }

                stp.Close();
                document.Close();
                reader.Close();

                //_pdf = stream.ToArray();
            }

        }
    }
}
