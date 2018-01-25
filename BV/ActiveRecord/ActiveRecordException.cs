using System;
using System.Runtime.Serialization;

namespace VB.Common.ActiveRecord
{
    [Serializable]
    public class ActiveRecordException: Exception
    {
        public ActiveRecordException() { }

        public ActiveRecordException(string message) : base(message) { }

        public ActiveRecordException(string message, Exception cause) : base(message, cause) { }

        protected ActiveRecordException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
