using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

namespace VB.Common.Data
{
    public static class SimpleQuery
    {
        public static IDataConnection ConfigurationManagerConnection(string database)
        {
            string connectionString = ConfigurationManager.ConnectionStrings[database].ConnectionString;
            string providerName = ConfigurationManager.ConnectionStrings[database].ProviderName;
            return DataProviderFactory.GetFactory(providerName).CreateConnection(connectionString);
        }

        public static void AddWithValue(IDbCommand command, string parameterName, object parameterValue, DbType dbType)
        {
            IDbDataParameter parameter = command.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.DbType = dbType;
            parameter.Value = parameterValue;
            command.Parameters.Add(parameter);
        }

        public static void AddNullValue(IDbCommand command, string parameterName, DbType dbType)
        {
            IDbDataParameter parameter = command.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.DbType = dbType;
            parameter.Value = DBNull.Value;

            DbParameter p = parameter as DbParameter;
            if (p != null) p.IsNullable = true;

            command.Parameters.Add(parameter);
        }

        public static IDataParameter AddOutParameter(IDbCommand command, string name, bool nullable, DbType type)
        {
            IDataParameter parameter = command.CreateParameter();

            parameter.DbType = type;

            DbParameter db = parameter as DbParameter;

            if (db != null)
            {
                db.IsNullable = nullable;
            }

            parameter.ParameterName = name;

            parameter.Direction = ParameterDirection.Output;

            command.Parameters.Add(parameter);

            return parameter;
        }

        public static void AddArrayParameter(IDbCommand command, string name, DbType type, bool nullable, Array value)
        {
            IDbDataParameter parameter = command.CreateParameter();
            parameter.DbType = type;
            parameter.ParameterName = name;
            parameter.Size = value.Length;
            parameter.Value = value;

            DbParameter db = parameter as DbParameter;

            if (db != null)
            {
                db.IsNullable = nullable;
            }

            command.Parameters.Add(parameter);
        }

        public static IDataParameter AddArrayOutParameter(IDbCommand command, string name, DbType type, bool nullable, int size)
        {
            IDbDataParameter parameter = command.CreateParameter();
            parameter.DbType = type;
            parameter.Direction = ParameterDirection.Output;
            parameter.ParameterName = name;
            parameter.Size = size;

            DbParameter db = parameter as DbParameter;

            if (db != null)
            {
                db.IsNullable = nullable;
            }

            command.Parameters.Add(parameter);

            return parameter;
        }

        [SuppressMessage("Microsoft.Design", "CA1007:UseGenericsWhereAppropriate", Justification = "None: End client should know its type")]
        public static NonQueryResult ExecuteNonQuery(string database, CommandType commandType, string commandText, IList<IDataParameterTemplate> parameters, IDictionary<string,object> parameterValues, bool collectPrimaryKey)
        {
            using (IDataConnection connection = ConfigurationManagerConnection(database))
            {
                return ExecuteNonQuery(
                    connection,
                    new DataCommandTemplate(commandType, commandText, parameters),
                    new DictionaryDataParameterValue(parameterValues).DataParameterValue,
                    collectPrimaryKey);
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1007:UseGenericsWhereAppropriate", Justification = "None: End client should know its type")]
        public static NonQueryResult ExecuteNonQuery(IDataConnection connection, IDataCommandTemplate commandTemplate, DataParameterValue parameterValue, bool collectPrimaryKey)
        {
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            using (IDataCommand command = connection.CreateCommand(commandTemplate, parameterValue))
            {
                using (IDbTransaction transaction = connection.BeginTransaction())
                {
                    command.Transaction = transaction;

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected != 1)
                    {
                        throw new DataException("Number of rows affected = " + rowsAffected + " expected 1");
                    }

                    object primaryKey;

                    if (collectPrimaryKey)
                    {
                        DataCommandTemplate identity = new DataCommandTemplate(CommandType.Text, "SELECT @@IDENTITY Id");

                        using (IDbCommand primaryKeyCommand = connection.CreateCommand(identity, parameterValue))
                        {
                            primaryKeyCommand.Transaction = transaction;

                            primaryKey = primaryKeyCommand.ExecuteScalar();
                        }
                    }
                    else
                    {
                        primaryKey = null;
                    }

                    transaction.Commit();

                    return new NonQueryResult(primaryKey, rowsAffected);
                }
            }
        }
    }
}
