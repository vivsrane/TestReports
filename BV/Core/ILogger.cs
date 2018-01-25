using System;

namespace VB.Common.Core
{
    public interface ILogger
    {
        void Log(Exception e);
        void Log(string message, object source);
    }
}