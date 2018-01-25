using System;
using System.Threading;

namespace VB.Common.Core
{
    public class SynchronousAsyncResultNoResult : IAsyncResult
    {
        private readonly Object asyncState;
        private readonly Exception exception;

        public SynchronousAsyncResultNoResult(AsyncCallback asyncCallback, Object asyncState) : this(null, asyncCallback, asyncState)
        {
        }

        public SynchronousAsyncResultNoResult(Exception exception, AsyncCallback asyncCallback, Object asyncState)
        {
            this.asyncState = asyncState;
            this.exception = exception;

            if (asyncCallback != null)
                asyncCallback(this);
        }

        public void EndInvoke()
        {
            if (exception != null) throw exception;
        }

        #region IAsyncResult Members

        protected virtual object AsyncState
        {
            get { return asyncState; }
        }

        object IAsyncResult.AsyncState
        {
            get { return AsyncState; }
        }

        protected virtual WaitHandle AsyncWaitHandle
        {
            get { return null; }
        }

        WaitHandle IAsyncResult.AsyncWaitHandle
        {
            get { return AsyncWaitHandle; }
        }

        protected virtual bool CompletedSynchronously
        {
            get { return true; }
        }

        bool IAsyncResult.CompletedSynchronously
        {
            get { return CompletedSynchronously; }
        }

        protected virtual bool IsCompleted
        {
            get { return true; }
        }

        bool IAsyncResult.IsCompleted
        {
            get { return IsCompleted; }
        }

        #endregion
    }
}