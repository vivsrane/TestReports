using System;

namespace VB.Common.Core.Logging
{
    public class LoggerFactory : ILoggerFactory
    {
        public static ILog GetLogger<T>()
        {
            return IOC.Registry.IsRegistered<ILoggerFactory>()
                       ? IOC.Registry.Resolve<ILoggerFactory>().GetLogger<T>()
                       : GetLoggerImpl(typeof(T));
        }

        public static ILog GetLogger(Type t)
        {
            return IOC.Registry.IsRegistered<ILoggerFactory>()
                       ? IOC.Registry.Resolve<ILoggerFactory>().GetLogger(t)
                       : GetLoggerImpl(t);
        }

        public static ILog GetLogger(string name)
        {
            return IOC.Registry.IsRegistered<ILoggerFactory>()
                       ? IOC.Registry.Resolve<ILoggerFactory>().GetLogger(name)
                       : GetLoggerImpl(name);
        }

        internal LoggerFactory()
        {
        }

        ILog ILoggerFactory.GetLogger(Type type)
        {
            return GetLoggerImpl(type);
        }

        ILog ILoggerFactory.GetLogger<T>()
        {
            return GetLoggerImpl(typeof (T));
        }

        ILog ILoggerFactory.GetLogger(string name)
        {
            return GetLoggerImpl(name);
        }

        private static ILog GetLoggerImpl(string name)
        {
            var log = log4net.LogManager.GetLogger(name);
            return new Log4NetLoggerProxy(log);
        }

        private static ILog GetLoggerImpl(Type type)
        {
            var log = log4net.LogManager.GetLogger(type);
            return new Log4NetLoggerProxy(log);
        }
    }
}