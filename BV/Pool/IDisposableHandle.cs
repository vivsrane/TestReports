using System;

namespace VB.Common.Pool
{
    /// <summary>
    /// When a borrowed pooled item implements this interface and the user does not return it
    /// on garbage collection detection the <code>Disposable</code> is disposed.  This way we do
    /// not leak resources.
    /// </summary>
    public interface IDisposableHandle
    {
        IDisposable Disposable
        {
            get;
        }
    }
}
