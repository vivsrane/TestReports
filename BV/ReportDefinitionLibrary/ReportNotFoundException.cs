using System;
using System.Runtime.Serialization;

namespace VB.Reports.App.ReportDefinitionLibrary
{
    [Serializable]
    public class ReportNotFoundException : ReportException
    {
        public ReportNotFoundException() { }

        public ReportNotFoundException(string message) : base(message) { }

        public ReportNotFoundException(string message, Exception cause) : base(message, cause) { }

        protected ReportNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
