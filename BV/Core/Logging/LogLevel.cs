using System;

namespace VB.Common.Core.Logging
{
    public class LogLevel
    {
        private readonly Func<ILog, bool> _isEnabledFunc;

        private LogLevel(Func<ILog, bool> isEnabledFunc)
        {
            _isEnabledFunc = isEnabledFunc;
        }

        public bool IsEnabledFor(ILog log)
        {
            return _isEnabledFunc(log);
        }

        public static readonly LogLevel Debug = new LogLevel(log => log.IsDebugEnabled);
        public static readonly LogLevel Info = new LogLevel(log => log.IsInfoEnabled);
        public static readonly LogLevel Warn = new LogLevel(log => log.IsWarnEnabled);
        public static readonly LogLevel Error = new LogLevel(log => log.IsErrorEnabled);
        public static readonly LogLevel Fatal = new LogLevel(log => log.IsFatalEnabled);
    }
}