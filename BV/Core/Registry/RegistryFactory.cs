using System;
using System.Collections.Generic;
using System.Reflection;

namespace VB.Common.Core.Registry
{
    public static class RegistryFactory
    {
        private static readonly IRegistry RegistryImpl;

        private static readonly IResolver ResolverImpl;

        static RegistryFactory()
        {
            RegistryImpl = new Registry();

            ResolverImpl = RegistryImpl.CreateScope();
        }
        
        public static IRegistry GetRegistry()
        {
            return RegistryImpl;
        }

        public static IResolver GetResolver()
        {
            return ResolverImpl;
        }

        private class Implementation
        {
            private readonly ImplementationScope _implementationScope;
            private readonly Type _type;

            public Implementation(ImplementationScope implementationScope, Type type)
            {
                _implementationScope = implementationScope;
                _type = type;
            }

            public ImplementationScope ImplementationScope
            {
                get { return _implementationScope; }
            }

            public Type Type
            {
                get { return _type; }
            }
        }

        private class Registry : IRegistry
        {
            public event EventHandler<EventArgs> Disposed;

            private void OnDisposed(EventArgs e)
            {
                EventHandler<EventArgs> handler = Disposed;

                if (handler != null)
                {
                    handler(this, e);
                }
            }

            private static readonly Type[] NoArgsType = new Type[0];

            private static readonly object[] NoArgsVals = new object[0];

            private readonly IDictionary<Type, Implementation> _registry = new Dictionary<Type, Implementation>();

            public IRegistry Register<TContract, TImplementation>(ImplementationScope implementationScope)
                where TImplementation : TContract, new()
            {
                _registry[typeof (TContract)] = new Implementation(implementationScope, typeof (TImplementation));

                return this;
            }

            public TContract Resolve<TContract>()
            {
                Type type = typeof (TContract);

                if (!_registry.ContainsKey(type))
                {
                    throw new ApplicationException("The type " + type + " does not exist in the registry.");
                }

                Implementation implementation = _registry[type];

                ConstructorInfo constructor = implementation.Type.GetConstructor(NoArgsType);

                return (TContract) constructor.Invoke(NoArgsVals);
            }

            public IResolver CreateScope()
            {
                return new Resolver(this);
            }

            public IRegistry Register<TModule>() where TModule : class, IModule, new()
            {
                new TModule().Configure(this);

                return this;
            }

            public IRegistry Clone()
            {
                Registry registry = new Registry();

                foreach (KeyValuePair<Type, Implementation> pair in _registry)
                {
                    registry._registry.Add(pair.Key, pair.Value);
                }

                return registry;
            }

            internal ImplementationScope TypeOf(Type type)
            {
                if (!_registry.ContainsKey(type))
                {
                    throw new ApplicationException("The type " + type + " does not exist in the registry.");
                }

                return _registry[type].ImplementationScope;
            }

            #region IDisposable Members

            ~Registry()
            {
                Dispose(false);
            }

            public void Dispose()
            {
                Dispose(true);

                GC.SuppressFinalize(this);
            }

            private void Dispose(bool disposing)
            {
                if (disposing)
                {
                    OnDisposed(EventArgs.Empty);

                    _registry.Clear();
                }
            }

            #endregion
        }

        private class Resolver : IResolver
        {
            private static readonly object LockObject = new object();

            private readonly Dictionary<Type, object> _cache = new Dictionary<Type, object>();

            private readonly Registry _registry;

            public Resolver(Registry registry)
            {
                _registry = registry;

                _registry.Disposed += Registry_Disposed;
            }

            void Registry_Disposed(object sender, EventArgs e)
            {
                _cache.Clear();
            }

            public TContract Resolve<TContract>()
            {
                Type type = typeof (TContract);

                if (_registry.TypeOf(type) == ImplementationScope.Isolated)
                {
                    return _registry.Resolve<TContract>();
                }

                lock (LockObject)
                {
                    if (_cache.ContainsKey(type))
                    {
                        return (TContract) _cache[type];
                    }

                    TContract implementation = _registry.Resolve<TContract>();

                    _cache[type] = implementation;

                    return implementation;
                }
            }

            #region IDisposable Members

            ~Resolver()
            {
                Dispose(true);
            }

            public void Dispose()
            {
                Dispose(false);

                GC.SuppressFinalize(this);
            }

            private void Dispose(bool disposing)
            {
                if (disposing)
                {
                    _cache.Clear();
                }
            }

            #endregion
        }
    }
}