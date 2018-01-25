using System;
using System.Runtime.Serialization;

namespace VB.Common.Pool
{
    /// <summary>
    /// Indicates that a method has been called on a closed pool.
    /// </summary>
    [Serializable]
    public class PoolClosedException : PoolException
    {
        protected PoolClosedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public PoolClosedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public PoolClosedException(string message)
            : base(message)
        {
        }

        public PoolClosedException()
        {
        }
    }
}
