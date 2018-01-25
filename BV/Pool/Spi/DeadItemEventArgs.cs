using System;

namespace VB.Common.Pool.Spi
{
    public class DeadItemEventArgs : EventArgs
    {
        private readonly object key;

        public DeadItemEventArgs(object key)
        {
            this.key = key;
        }

        public object Key
        {
            get { return key; }
        }
    }
}
