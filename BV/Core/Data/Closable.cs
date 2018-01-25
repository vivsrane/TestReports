using System;

namespace VB.Common.Core.Data
{
    public abstract class Closable : IClosable
    {
        public event EventHandler<EventArgs> Closed;

        protected void OnClosed(EventArgs e)
        {
            EventHandler<EventArgs> handler = Closed;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        private bool _closed;

        public void Close()
        {
            try
            {
                Dispose(true);
            }
            finally
            {
                _closed = true;

                OnClosed(EventArgs.Empty);
            }
        }

        public bool IsClosed
        {
            get { return _closed; }
        }

        protected void EnsureOpen()
        {
            if (_closed)
            {
                throw new InvalidOperationException("Closed");
            }
        }

        ~Closable()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Close();

            GC.SuppressFinalize(this);
        }

        protected abstract void Dispose(bool disposing);
    }
}
