using System;
using System.Runtime.Serialization;

namespace VB.Reports.App.ReportDefinitionLibrary
{
    [Serializable]
    public class ReportDataSourceException : ReportException
    {
        public ReportDataSourceException() { }

        public ReportDataSourceException(string message) : base(message) { }

        public ReportDataSourceException(string message, Exception cause) : base(message, cause) { }

        protected ReportDataSourceException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
