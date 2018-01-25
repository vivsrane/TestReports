using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace VB.Common.Pool
{
    /// <summary>
    /// The error thrown by pool item factory classes when a life cycle method fails for a given object.
    /// The pool classes are expected to catch and recover from it and its subclasses.
    /// </summary>
    [Serializable]
    [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "LifeCycle")]
    public class PoolLifeCycleException : PoolException
    {
        protected PoolLifeCycleException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public PoolLifeCycleException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public PoolLifeCycleException(string message) : base(message)
        {
        }

        public PoolLifeCycleException()
        {
        }
    }
}
