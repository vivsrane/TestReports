using System;
using System.Runtime.Serialization;

namespace VB.Common.ActiveRecord
{
    [Serializable]
    public class RecordNotFoundException : ActiveRecordException
    {
        public RecordNotFoundException() { }

        public RecordNotFoundException(string message) : base(message) { }

        public RecordNotFoundException(string message, Exception cause) : base(message, cause) { }

        protected RecordNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
