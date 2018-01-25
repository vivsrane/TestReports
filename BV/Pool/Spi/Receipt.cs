using System;
using System.Diagnostics.CodeAnalysis;

namespace VB.Common.Pool.Spi
{
    public class Receipt<TItem> : IDisposable
    {
        private readonly WeakReference reference;

        private readonly DateTime created = DateTime.Now;

        private IDisposable disposable;

        public Receipt(TItem item)
        {
            reference = new WeakReference(item, false);

            IDisposableHandle handle = item as IDisposableHandle;

            if (handle != null)
            {
                disposable = handle.Disposable;
            }
        }

        ~Receipt()
        {
            Dispose(false);
        }

        public WeakReference Reference
        {
            get { return reference; }
        }

        public DateTime Created
        {
            get { return created; }
        }

        public bool Match(TItem item)
        {
            return (reference.IsAlive && reference.Target != null && ReferenceEquals(reference.Target, item));
        }

        public bool Dead()
        {
            return !reference.IsAlive;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Disposa errors are written to the event log")]
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                IDisposable handle = disposable; // take a thread local copy

                disposable = null; // stop double disposal

                if (handle != null)
                {
                    try
                    {
                        handle.Dispose();
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e.Message); // log any errors in the disposal
                    }
                }
            }
        }
    }
}
