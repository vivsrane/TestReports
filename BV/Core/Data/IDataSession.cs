using System;
using System.Collections;

namespace VB.Common.Core.Data
{
    public interface IDataSession : IDisposable
    {
        IDataConnection OpenConnection();

        IDataConnection Connection { get; }

        void Close();

        IDictionary Items { get; }
    }
}