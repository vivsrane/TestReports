using System;

namespace VB.Common.Core.Registry
{
    public interface IResolver : IDisposable
    {
        TContract Resolve<TContract>();
    }
}