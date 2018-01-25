using System;
using System.Runtime.Serialization;

namespace VB.Reports.App.ReportDefinitionLibrary
{
    [Serializable]
    public class ReportParameterException : ReportException
    {
        public ReportParameterException() { }

        public ReportParameterException(string message) : base(message) { }

        public ReportParameterException(string message, Exception cause) : base(message, cause) { }

        protected ReportParameterException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
