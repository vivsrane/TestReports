using System;

namespace VB.Common.Core.Data
{
    public interface ITransactionFactory : IDisposable
    {
        ITransaction BeginTransaction();
    }
}