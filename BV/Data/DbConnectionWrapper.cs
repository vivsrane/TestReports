using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace VB.Common.Data
{
    [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Db", Justification = "Consistency with System.Data namespace")]
    public class DbConnectionWrapper : IDbConnection, IDbConnectionProxy
    {
        private readonly IDbConnection connection;

        public DbConnectionWrapper(IDbConnection connection)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }

            this.connection = connection;
        }

        public IDbConnection Connection()
        {
            return connection;
        }

        public IDbConnection UnderlyingConnection()
        {
            IDbConnection dbConnection = connection;

            while (dbConnection is IDbConnectionProxy)
            {
                dbConnection = ((IDbConnectionProxy)dbConnection).Connection();
            }

            return dbConnection;
        }

        #region IDbConnection Members

        public virtual IDbTransaction BeginTransaction(IsolationLevel il)
        {
            return Connection().BeginTransaction(il);
        }

        public virtual IDbTransaction BeginTransaction()
        {
            return Connection().BeginTransaction();
        }

        public virtual void ChangeDatabase(string databaseName)
        {
            Connection().ChangeDatabase(databaseName);
        }

        public virtual void Close()
        {
            Connection().Close();
        }

        public virtual string ConnectionString
        {
            get
            {
                return Connection().ConnectionString;
            }
            set
            {
                Connection().ConnectionString = value;
            }
        }

        public virtual int ConnectionTimeout
        {
            get { return Connection().ConnectionTimeout; }
        }

        public virtual IDbCommand CreateCommand()
        {
            return Connection().CreateCommand();
        }

        public virtual string Database
        {
            get { return Connection().Database; }
        }

        public virtual void Open()
        {
            Connection().Open();
        }

        public virtual ConnectionState State
        {
            get { return Connection().State; }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Connection().Dispose();
            }
        }

        #endregion
    }
}
