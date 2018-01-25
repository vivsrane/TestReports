using System;

namespace VB.Common.Core
{
    public sealed class SynchronousAsyncResult<TResult> : SynchronousAsyncResultNoResult
    {
        private readonly TResult result = default(TResult);

        public SynchronousAsyncResult(Exception exception, AsyncCallback asyncCallback, Object asyncState)
            : base(exception, asyncCallback, asyncState)
        {
        }

        public SynchronousAsyncResult(TResult result, AsyncCallback asyncCallback, Object asyncState)
            : base(asyncCallback, asyncState)
        {
            this.result = result;
        }

        public new TResult EndInvoke()
        {
            base.EndInvoke(); // Throw exception if present
            return result;    // Return the result (if above didn't throw)
        }
    }
}