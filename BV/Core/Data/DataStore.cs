using System;
using System.Data;
using System.Reflection;
using VB.Common.Core.Registry;

namespace VB.Common.Core.Data
{
    public abstract class DataStore
    {
        protected internal class Parameter
        {
            private readonly string _name;
            private readonly object _value;
            private readonly DbType _dataType;

            public Parameter(string name, object value, DbType dataType)
            {
                _name = name;
                _value = value;
                _dataType = dataType;
            }

            public string Name
            {
                get { return _name; }
            }

            public object Value
            {
                get { return _value; }
            }

            public DbType DataType
            {
                get { return _dataType; }
            }
        }

        protected class Copy
        {
            private readonly DataSet _set = new DataSet();

            private readonly string[] _names;

            public Copy(params string[] names)
            {
                _names = names;
            }

            public DataSet Set
            {
                get { return _set; }
            }

            public void Read(IDataReader reader)
            {
                foreach (string name in _names)
                {
                    DataTable table = new DataTable(name);

                    table.Load(reader);

                    _set.Tables.Add(table);
                }
            }
        }

        protected abstract Assembly Assembly { get; }

        protected abstract IDbCommand CreateCommand();

        protected IDataReader Query(
            string[] tableNames,
            string queryName,
            params Parameter[] parameters)
        {
            Copy c = new Copy(tableNames);

            Query(c.Read, queryName, parameters);

            return c.Set.CreateDataReader();
        }

        protected void Query(
            Action<IDataReader> f,
            string queryName,
            params Parameter[] parameters)
        {
            using (IDbCommand command = CreateCommand())
            {
                command.CommandText = Database.GetCommandText(Assembly, queryName);

                foreach (Parameter parameter in parameters)
                {
                    Database.AddWithValue(command, parameter.Name, parameter.Value, parameter.DataType);
                }

                using (IDataReader reader = command.ExecuteReader())
                {
                    f(reader);
                }
            }
        }

        protected int NonQuery(
            string queryName,
            params Parameter[] parameters)
        {
            using (IDbCommand command = CreateCommand())
            {
                command.CommandText = Database.GetCommandText(Assembly, queryName);

                foreach (Parameter parameter in parameters)
                {
                    Database.AddWithValue(command, parameter.Name, parameter.Value, parameter.DataType);
                }

                return command.ExecuteNonQuery();
            }
        }
    }

    public abstract class SessionDataStore : DataStore
    {
        private static IDataSessionManager DataSessionManager
        {
            get { return RegistryFactory.GetResolver().Resolve<IDataSessionManager>(); }
        }

        protected IDataSession DataSession
        {
            get { return DataSessionManager.Session; }
        }

        protected override IDbCommand CreateCommand()
        {
            return DataSession.Connection.CreateCommand();
        }
    }

    public abstract class SimpleDataStore : DataStore
    {
        protected abstract string DatabaseName { get; }

        protected override IDbCommand CreateCommand()
        {
            IDbConnection connection = null;

            IDbCommand command = null;

            try
            {
                connection = Database.Connection(DatabaseName);

                command = connection.CreateCommand();

                return new DbCommandWrapper(connection, command);
            }
            catch (Exception)
            {
                if (command != null)
                {
                    command.Dispose();
                }

                if (connection != null)
                {
                    connection.Dispose();
                }

                throw;
            }
        }

        private class DbCommandWrapper : IDbCommand
        {
            private readonly IDbConnection _connection;
            private readonly IDbCommand _command;

            public DbCommandWrapper(IDbConnection connection, IDbCommand command)
            {
                _connection = connection;
                _command = command;
            }

            #region IDbCommand Members

            public void Cancel()
            {
                _command.Cancel();
            }

            public string CommandText
            {
                get { return _command.CommandText; }
                set { _command.CommandText = value; }
            }

            public int CommandTimeout
            {
                get { return _command.CommandTimeout; }
                set { _command.CommandTimeout = value; }
            }

            public CommandType CommandType
            {
                get { return _command.CommandType; }
                set { _command.CommandType = value; }
            }

            public IDbConnection Connection
            {
                get { return _command.Connection; }
                set { _command.Connection = value; }
            }

            public IDbDataParameter CreateParameter()
            {
                return _command.CreateParameter();
            }

            public int ExecuteNonQuery()
            {
                return _command.ExecuteNonQuery();
            }

            public IDataReader ExecuteReader(CommandBehavior behavior)
            {
                return _command.ExecuteReader(behavior);
            }

            public IDataReader ExecuteReader()
            {
                return _command.ExecuteReader();
            }

            public object ExecuteScalar()
            {
                return _command.ExecuteScalar();
            }

            public IDataParameterCollection Parameters
            {
                get { return _command.Parameters; }
            }

            public void Prepare()
            {
                _command.Prepare();
            }

            public IDbTransaction Transaction
            {
                get { return _command.Transaction; }
                set { _command.Transaction = value; }
            }

            public UpdateRowSource UpdatedRowSource
            {
                get { return _command.UpdatedRowSource; }
                set { _command.UpdatedRowSource = value; }
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
                _command.Dispose();

                _connection.Dispose();
            }

            #endregion
        }
    }
}