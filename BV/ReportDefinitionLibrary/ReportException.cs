using System;
using System.Runtime.Serialization;

namespace VB.Reports.App.ReportDefinitionLibrary
{
    [Serializable]
    public class ReportException : Exception
    {
        public ReportException() { }

        public ReportException(string message) : base(message) { }

        public ReportException(string message, Exception cause) : base(message, cause) { }

        protected ReportException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
