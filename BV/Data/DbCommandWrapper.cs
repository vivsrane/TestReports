using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace VB.Common.Data
{
    [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Db", Justification = "Consistency with System.Data namespace")]
    public class DbCommandWrapper : IDbCommand
    {
        private readonly IDbCommand command;

        public DbCommandWrapper(IDbCommand command)
        {
            this.command = command;
        }

        public virtual IDbCommand Command()
        {
            return command;
        }

        #region IDbCommand Members

        public void Cancel()
        {
            Command().Cancel();
        }

        public string CommandText
        {
            get
            {
                return Command().CommandText;
            }
            set
            {
                Command().CommandText = value;
            }
        }

        public int CommandTimeout
        {
            get
            {
                return Command().CommandTimeout;
            }
            set
            {
                Command().CommandTimeout = value;
            }
        }

        public CommandType CommandType
        {
            get
            {
                return Command().CommandType;
            }
            set
            {
                Command().CommandType = value;
            }
        }

        public IDbConnection Connection
        {
            get
            {
                return Command().Connection;
            }
            set
            {
                Command().Connection = value;
            }
        }

        public IDbDataParameter CreateParameter()
        {
            return Command().CreateParameter();
        }

        public int ExecuteNonQuery()
        {
            return Command().ExecuteNonQuery();
        }

        public IDataReader ExecuteReader(CommandBehavior behavior)
        {
            return Command().ExecuteReader(behavior);
        }

        public IDataReader ExecuteReader()
        {
            return Command().ExecuteReader();
        }

        public object ExecuteScalar()
        {
            return Command().ExecuteScalar();
        }

        public IDataParameterCollection Parameters
        {
            get { return Command().Parameters; }
        }

        public void Prepare()
        {
            Command().Prepare();
        }

        public IDbTransaction Transaction
        {
            get
            {
                return Command().Transaction;
            }
            set
            {
                Command().Transaction = value;
            }
        }

        public UpdateRowSource UpdatedRowSource
        {
            get
            {
                return Command().UpdatedRowSource;
            }
            set
            {
                Command().UpdatedRowSource = value;
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            System.GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                IDbCommand value = Command();

                if (value != null)
                {
                    value.Dispose();
                }
            }
        }

        #endregion
    }
}
