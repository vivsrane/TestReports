using System;
using System.Runtime.Serialization;

namespace VB.Common.ActiveRecord
{
    [Serializable]
    public class NoColumnAttributeException : ActiveRecordException
    {
        public NoColumnAttributeException() { }

        public NoColumnAttributeException(string message) : base(message) { }

        public NoColumnAttributeException(string message, Exception cause) : base(message, cause) { }

        protected NoColumnAttributeException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
