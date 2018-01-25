using System;
using System.Data;

namespace VB.Common.Core.Data
{
    public class DataConnection : Closable, IDataConnection
    {
        private readonly string _databaseName;
        private IDbConnection _connection;
        private TransactionManager _manager;

        public DataConnection(string databaseName)
        {
            _databaseName = databaseName;
        }

        public string DatabaseName
        {
            get { return _databaseName; }
        }

        protected IDbConnection Connection
        {
            get
            {
                if (_connection == null)
                {
                    _connection = Database.ConfigurationManagerConnection(DatabaseName);

                    if (_connection.State != ConnectionState.Open)
                    {
                        _connection.Open();
                    }
                }

                return _connection;
            }
        }

        public ITransaction BeginTransaction()
        {
            if (_manager == null)
            {
                _manager = new TransactionManager(Connection.BeginTransaction());

                _manager.Closed += Manager_Closed;
            }

            return _manager;
        }

        private void Manager_Closed(object sender, EventArgs e)
        {
            _manager.Closed -= Manager_Closed;

            _manager = null;
        }

        public IDbCommand CreateCommand()
        {
            IDbCommand command = Connection.CreateCommand();

            if (_manager != null)
            {
                command.Transaction = _manager.Transaction;
            }
            
            return command;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    if (_manager != null)
                    {
                        _manager.Rollback();

                        _manager = null;
                    }

                    if (_connection != null)
                    {
                        _connection.Close();

                        _connection = null;
                    }
                }
                finally
                {
                    OnClosed(EventArgs.Empty);
                }
            }
        }

        class TransactionManager : Closable, ITransaction
        {
            private IDbTransaction _transaction;

            public TransactionManager(IDbTransaction transaction)
            {
                _transaction = transaction;
            }

            public IDbTransaction Transaction
            {
                get { return _transaction; }
            }

            public void Commit()
            {
                if (_transaction != null)
                {
                    _transaction.Commit();

                    _transaction = null;
                }
            }

            public void Rollback()
            {
                if (_transaction != null)
                {
                    _transaction.Rollback();

                    _transaction = null;
                }
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    Rollback();
                }
            }
        }
    }
}
