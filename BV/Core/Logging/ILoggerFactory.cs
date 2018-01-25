using System;

namespace VB.Common.Core.Logging
{
    public interface ILoggerFactory
    {
        ILog GetLogger<T>();
        ILog GetLogger(Type type);
        ILog GetLogger(string name);
    }
}