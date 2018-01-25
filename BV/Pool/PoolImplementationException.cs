using System;
using System.Runtime.Serialization;

namespace VB.Common.Pool
{
    /// <summary>
    /// Indicates that the pool has encountered a fatal error. This indicates either an
    /// internal error or that the pool factory breaks the API contract (i.e. it cannot
    /// create a new, activated and valid item.
    /// </summary>
    [Serializable]
    public class PoolImplementationException : Exception
    {
        protected PoolImplementationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public PoolImplementationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public PoolImplementationException(string message)
            : base(message)
        {
        }

        public PoolImplementationException()
        {
        }
    }
}
