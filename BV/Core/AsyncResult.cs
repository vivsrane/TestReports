using System;

namespace VB.Common.Core
{
    /// <summary>
    /// Implementation of IAsyncResult with return value by Jeffrey Richter.
    /// </summary>
    /// <typeparam name="TResult">Asynchronous operation result type</typeparam>
    /// <see cref="http://msdn.microsoft.com/msdnmag/issues/07/03/ConcurrentAffairs/" />
    public class AsyncResult<TResult> : AsyncResultNoResult
    {
        // Field set when operation completes
        private TResult m_result = default(TResult);

        public AsyncResult(AsyncCallback asyncCallback, Object state) : base(asyncCallback, state)
        {
        }

        public void SetAsCompleted(TResult result, Boolean completedSynchronously)
        {
            // Save the asynchronous operation's result
            m_result = result;

            // Tell the base class that the operation completed sucessfully (no exception)
            base.SetAsCompleted(null, completedSynchronously);
        }

        public new TResult EndInvoke()
        {
            base.EndInvoke(); // Wait until operation has completed 
            return m_result; // Return the result (if above didn't throw)
        }
    }
}