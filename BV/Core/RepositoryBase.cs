using System;
using VB.Common.Core.Data;
using VB.Common.Core.Registry;

namespace VB.Common.Core
{
    public abstract class RepositoryBase
    {
        public ITransactionFactory TransactionFactory
        {
            get { return new TransactionFactoryImpl(this); }
        }

        protected T Resolve<T>()
        {
            return RegistryFactory.GetRegistry().Resolve<T>();
        }

        protected IDataSession CreateSession()
        {
            return CreateSession(DatabaseName);
        }

        protected IDataSession CreateSession(string databaseName)
        {
            IDataSessionManager manager = Resolve<IDataSessionManager>();

            if (manager == null)
            {
                throw new InvalidOperationException("DataSessionManager not registered");
            }

            return manager.CreateSession(databaseName);
        }

        protected delegate T Block<T>();

        protected delegate void Begin(IDataSession session);

        protected T DoInSession<T>(Block<T> block)
        {
            IDataSessionManager sessionManager = Resolve<IDataSessionManager>();

            using (IDataSession session = sessionManager.CreateSession(DatabaseName))
            {
                IDataConnection connection = null;

                try
                {
                    connection = session.OpenConnection();

                    return block();
                }
                finally
                {
                    if (connection != null)
                    {
                        connection.Dispose();
                    }
                }
            }
        }

        protected T DoInTransaction<T>(Begin begin, Block<T> block)
        {
            IDataSessionManager sessionManager = Resolve<IDataSessionManager>();

            using (IDataSession session = sessionManager.CreateSession(DatabaseName))
            {
                using (IDataConnection connection = session.OpenConnection())
                {
                    using (ITransaction transaction = connection.BeginTransaction())
                    {
                        begin(session);

                        T value = block();

                        transaction.Commit();

                        return value;
                    }
                }
            }
        }

        protected T DoInTransaction<T>(Block<T> block)
        {
            IDataSessionManager sessionManager = Resolve<IDataSessionManager>();

            using (IDataSession session = sessionManager.CreateSession(DatabaseName))
            {
                using (IDataConnection connection = session.OpenConnection())
                {
                    using (ITransaction transaction = connection.BeginTransaction())
                    {
                        OnBeginTransaction(session);

                        T value = block();

                        transaction.Commit();

                        return value;
                    }
                }
            }
        }

        protected abstract string DatabaseName { get; }

        protected abstract void OnBeginTransaction(IDataSession session);

        protected class TransactionFactoryImpl : Closable, ITransactionFactory
        {
            private RepositoryBase _repository;

            private IDataSession _session;

            public TransactionFactoryImpl(RepositoryBase repository)
            {
                _repository = repository;
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    try
                    {
                        if (_session != null)
                        {
                            _session.Close();
                        }
                    }
                    finally
                    {
                        _session = null;

                        _repository = null;
                    }
                }
            }

            public ITransaction BeginTransaction()
            {
                EnsureOpen();

                if (_session == null)
                {
                    _session = _repository.CreateSession();

                    IDataConnection connection = _session.OpenConnection();

                    ITransaction transaction = connection.BeginTransaction();

                    _repository.OnBeginTransaction(_session);

                    return transaction;
                }
                
                return new NullTransaction();
            }

            class NullTransaction : ITransaction
            {
                public void Dispose()
                {
                    
                }

                public void Commit()
                {
                    
                }

                public void Rollback()
                {
                    
                }
            }
        }
    }
}