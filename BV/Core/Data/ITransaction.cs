using System;

namespace VB.Common.Core.Data
{
    public interface ITransaction : IDisposable
    {
        void Commit();

        void Rollback();
    }
}