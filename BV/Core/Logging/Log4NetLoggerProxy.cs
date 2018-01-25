using System;

namespace VB.Common.Core.Logging
{
    internal class Log4NetLoggerProxy : ILog
    {
        private readonly log4net.ILog inner;

        internal Log4NetLoggerProxy(log4net.ILog inner)
        {
            if(inner == null)
                throw new ArgumentNullException("inner");
            this.inner = inner;
        }

        public bool IsDebugEnabled
        {
            get { return inner.IsDebugEnabled; }
        }

        public bool IsInfoEnabled
        {
            get { return inner.IsInfoEnabled; }
        }

        public bool IsWarnEnabled
        {
            get { return inner.IsWarnEnabled; }
        }

        public bool IsErrorEnabled
        {
            get { return inner.IsErrorEnabled; }
        }

        public bool IsFatalEnabled
        {
            get { return inner.IsFatalEnabled; }
        }

        public void Debug(object message)
        {
            inner.Debug(message);
        }

        public void Debug(object message, Exception exception)
        {
            inner.Debug(message, exception);
        }

        public void DebugFormat(string format, params object[] args)
        {
            inner.DebugFormat(format, args);
        }

        public void DebugFormat(string format, object arg0)
        {
            inner.DebugFormat(format, arg0);
        }

        public void DebugFormat(string format, object arg0, object arg1)
        {
            inner.DebugFormat(format, arg0, arg1);
        }

        public void DebugFormat(string format, object arg0, object arg1, object arg2)
        {
            inner.DebugFormat(format, arg0, arg1, arg2);
        }

        public void DebugFormat(IFormatProvider provider, string format, params object[] args)
        {
            inner.DebugFormat(provider, format, args);
        }

        public void Info(object message)
        {
            inner.Info(message);
        }

        public void Info(object message, Exception exception)
        {
            inner.Info(message, exception);
        }

        public void InfoFormat(string format, params object[] args)
        {
            inner.InfoFormat(format, args);
        }

        public void InfoFormat(string format, object arg0)
        {
            inner.InfoFormat(format, arg0);
        }

        public void InfoFormat(string format, object arg0, object arg1)
        {
            inner.InfoFormat(format, arg0, arg1);
        }

        public void InfoFormat(string format, object arg0, object arg1, object arg2)
        {
            inner.InfoFormat(format, arg0, arg1, arg2);
        }

        public void InfoFormat(IFormatProvider provider, string format, params object[] args)
        {
            inner.InfoFormat(provider, format, args);
        }

        public void Warn(object message)
        {
            inner.Warn(message);
        }

        public void Warn(object message, Exception exception)
        {
            inner.Warn(message, exception);
        }

        public void WarnFormat(string format, params object[] args)
        {
            inner.WarnFormat(format, args);
        }

        public void WarnFormat(string format, object arg0)
        {
            inner.WarnFormat(format, arg0);
        }

        public void WarnFormat(string format, object arg0, object arg1)
        {
            inner.WarnFormat(format, arg0, arg1);
        }

        public void WarnFormat(string format, object arg0, object arg1, object arg2)
        {
            inner.WarnFormat(format, arg0, arg1, arg2);
        }

        public void WarnFormat(IFormatProvider provider, string format, params object[] args)
        {
            inner.WarnFormat(provider, format, args);
        }

        public void Error(object message)
        {
            inner.Error(message);
        }

        public void Error(object message, Exception exception)
        {
            inner.Error(message, exception);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            inner.ErrorFormat(format, args);
        }

        public void ErrorFormat(string format, object arg0)
        {
            inner.ErrorFormat(format, arg0);
        }

        public void ErrorFormat(string format, object arg0, object arg1)
        {
            inner.ErrorFormat(format, arg0, arg1);
        }

        public void ErrorFormat(string format, object arg0, object arg1, object arg2)
        {
            inner.ErrorFormat(format, arg0, arg1, arg2);
        }

        public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
        {
            inner.ErrorFormat(provider, format, args);
        }

        public void Fatal(object message)
        {
            inner.Fatal(message);
        }

        public void Fatal(object message, Exception exception)
        {
            inner.Fatal(message, exception);
        }

        public void FatalFormat(string format, params object[] args)
        {
            inner.FatalFormat(format, args);
        }

        public void FatalFormat(string format, object arg0)
        {
            inner.FatalFormat(format, arg0);
        }

        public void FatalFormat(string format, object arg0, object arg1)
        {
            inner.FatalFormat(format, arg0, arg1);
        }

        public void FatalFormat(string format, object arg0, object arg1, object arg2)
        {
            inner.FatalFormat(format, arg0, arg1, arg2);
        }

        public void FatalFormat(IFormatProvider provider, string format, params object[] args)
        {
            inner.FatalFormat(provider, format, args);
        }
    }
}