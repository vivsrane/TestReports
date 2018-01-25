using System;

namespace VB.Common.Core.Logging
{
    public static class Extensions
    {
         public static IDisposable StartLoggedScopeStopwatch(this ILog log, LogLevel level, Action<ILog> logStartFunc, Action<ILog, TimeSpan> logStopFunc)
         {
             return new LoggedScopeStopwatch(log, level, logStartFunc, logStopFunc);
         }
    }
}