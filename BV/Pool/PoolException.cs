using System;
using System.Runtime.Serialization;

namespace VB.Common.Pool
{
    /// <summary>
    /// Indicates conditions that a reasonable application might want to catch.
    /// </summary>
    [Serializable]
    public class PoolException : Exception
    {
        protected PoolException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public PoolException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public PoolException(string message) : base(message)
        {
        }

        public PoolException()
        {
        }
    }
}
