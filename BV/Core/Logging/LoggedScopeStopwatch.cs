using System;
using System.Threading;

namespace VB.Common.Core.Logging
{
    public class LoggedScopeStopwatch : IDisposable
    {
        private readonly ILog _log;
        private readonly LogLevel _level;
        private readonly Action<ILog> _logStartFunc;
        private readonly Action<ILog, TimeSpan> _logStopFunc;
        private readonly DateTimeOffset _startTime;
        private int _disposeOnce;

        public LoggedScopeStopwatch(ILog log, LogLevel level, Action<ILog> logStartFunc, Action<ILog, TimeSpan> logStopFunc)
        {
            _log = log;
            _level = level;
            _logStartFunc = logStartFunc;
            _logStopFunc = logStopFunc;
            _startTime = DateTimeOffset.UtcNow;

            if (_level.IsEnabledFor(_log))
                _logStartFunc(_log);
        }

        public void Dispose()
        {
            if (0 != Interlocked.CompareExchange(ref _disposeOnce, 1, 0)) return;

            if (_level.IsEnabledFor(_log))
                _logStopFunc(_log, DateTimeOffset.UtcNow - _startTime);
        }
    }
}