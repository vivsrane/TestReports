using System;
using System.Runtime.Serialization;

namespace VB.Common.ActiveRecord
{
    [Serializable]
    public class MissingTableAnnotationException : ActiveRecordException
    {
        public MissingTableAnnotationException() { }

        public MissingTableAnnotationException(string message) : base(message) { }

        public MissingTableAnnotationException(string message, Exception cause) : base(message, cause) { }

        protected MissingTableAnnotationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
