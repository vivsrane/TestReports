using System;

namespace VB.Common.Data
{
    public interface ITransaction : IDisposable
    {
        void Commit();

        void Rollback();

    }
}