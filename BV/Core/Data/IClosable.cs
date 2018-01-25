using System;

namespace VB.Common.Core.Data
{
    public interface IClosable : IDisposable
    {
        event EventHandler<EventArgs> Closed;

        void Close();

        bool IsClosed { get; }
    }
}