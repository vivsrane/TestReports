using System;
using System.Data;

namespace VB.Common.Data
{
    public class DbTransaction : ITransaction
    {
        private readonly string _databaseName;
        private IDbConnection _connection;
        private IDbTransaction _transaction;

        public DbTransaction(string databaseName)
        {
            _databaseName = databaseName;
        }

        ~DbTransaction()
        {
            Dispose(false);
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
                    _connection = SimpleQuery.ConfigurationManagerConnection(DatabaseName);

                    if (_connection.State != ConnectionState.Open)
                    {
                        _connection.Open();
                    }
                }

                return _connection;
            }
        }

        protected IDbTransaction Transaction
        {
            get
            {
                if (_transaction == null)
                {
                    _transaction = Connection.BeginTransaction();
                }

                return _transaction;
            }
        }

        public IDbCommand CreateCommand()
        {
            IDbCommand command = Connection.CreateCommand();

            command.Transaction = Transaction;

            return command;
        }

        public void Commit()
        {
            Transaction.Commit();

            _transaction = null;
        }

        public void Rollback()
        {
            Transaction.Rollback();

            _transaction = null;
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_transaction != null)
                {
                    _transaction.Rollback();

                    _transaction = null;
                }

                if (_connection != null)
                {
                    _connection.Dispose();

                    _connection = null;
                }
            }


        }
    }
}