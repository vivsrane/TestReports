using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using iTextSharp;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace FirstLook.Common.Core.Utilities
{
    public class PdfDocumentWrapper
    {

        private PdfDocument _pdf { get; set; }
        public bool IsLoaded { get { return _pdf != null; } }
        public int PageCount { get { return _pdf == null ? 0 : _pdf.PageCount; } }

        public void Load( byte[] bytes )
        {
            using( MemoryStream stream = new MemoryStream( bytes ) )
            {
                _pdf = PdfReader.Open( stream, PdfDocumentOpenMode.Modify );
            }
        }

        public void Append( byte[] input )
        {
            if( !IsLoaded ) { return; } //TODO: PdfDocumentWrapper.Append(): consider throwing an exception on null.
            PdfDocument tmpPdf;
            using( MemoryStream stream = new MemoryStream( input ) )
            {
                tmpPdf = PdfReader.Open( stream, PdfDocumentOpenMode.Import );
            }
            for( int i = 0; i < tmpPdf.PageCount; ++i )
            {
                _pdf.AddPage( tmpPdf.Pages[i] );
            }
        }

        public byte[] ToByteArray()
        {
            if( !IsLoaded ) { return new byte[] { }; } //TODO: PdfDocumentWrapper.ToByteArray(): consider throwing an exception on null.
            using( MemoryStream stream = new MemoryStream() )
            {
                _pdf.Save( stream );
                return stream.ToArray();
            }
        }

    }
}
