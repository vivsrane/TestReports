using System;
using System.Diagnostics;
using Autofac;

namespace VB.Common.Core.IOC
{
    /// <summary>
    /// Registry pattern.
    /// </summary>
    public static class Registry
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(Registry));

        private static IContainer _container;

        public static IContainer Container
        {
            get { return _container; }
            set { _container = value; }
        }

        public static void RegisterContainer(IContainer container)
        {
            _container = container;
        }
        
        /// <summary>
        /// Return something which implements the type.
        /// </summary>
        /// <typeparam name="TContract"></typeparam>
        /// <returns></returns>
        public static TContract Resolve<TContract>()
        {
            return _container.Resolve<TContract>();
        }

        /// <summary>
        /// Return something which implements the type.
        /// </summary>
        /// <typeparam name="TContract"></typeparam>
        /// <param name="namedParameter">Name/Value pair for passed parameters.</param>
        /// <returns></returns>
        public static TContract Resolve<TContract>(NamedParameter namedParameter)
        {
            return _container.Resolve<TContract>(namedParameter);
        }

        public static bool IsRegistered<T>()
        {
            return IsRegistered(typeof (T));
        }

        public static bool IsRegistered(Type t)
        {
            return _container != null
                   && _container.IsRegistered(t);
        }

        /// <summary>
        /// Setter injection - set any null properties that we know how to resolve.
        /// </summary>
        /// <param name="obj"></param>
        public static void BuildUp(object obj)
        {
            if (_container == null)
            {
                const string ERROR = "The Registry has no IContainer and cannot BuildUp() objects without one.";
                Trace.WriteLine(ERROR);
                Log.Error(ERROR);
                return;
            }
            
            _container.InjectUnsetProperties(obj);
        }

        /// <summary>
        /// Reset the registry - removing all registrations.
        /// </summary>
        public static void Reset()
        {
            _container = null;
        }

    }
}