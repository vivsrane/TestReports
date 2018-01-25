using System;
using System.Runtime.Serialization;

namespace VB.Common.ActiveRecord
{
    [Serializable]
    public class NoPrimaryKeyAnnotationException : ActiveRecordException
    {
        public NoPrimaryKeyAnnotationException() { }

        public NoPrimaryKeyAnnotationException(string message) : base(message) { }

        public NoPrimaryKeyAnnotationException(string message, Exception cause) : base(message, cause) { }

        protected NoPrimaryKeyAnnotationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
