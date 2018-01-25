using System.Data;

namespace VB.Common.Core.Data
{
    /// <summary>
    /// Extension methods for DbCommand.
    /// </summary>
    public static class DbCommand
    {
        /// <summary>
        /// Add a paramter to the database command.
        /// </summary>
        /// <param name="command">Database command.</param>
        /// <param name="name">Name of the parameter.</param>
        /// <param name="type">Type of the parameter.</param>
        /// <param name="value">Value of the parameter.</param>
        public static void AddParameter(this IDbCommand command, string name, DbType type, object value)
        {
            IDbDataParameter parameter = command.CreateParameter();
            parameter.DbType = type;
            parameter.ParameterName = name;
            parameter.Value = value;
            command.Parameters.Add(parameter);
        }

        /// <summary>
        /// Add an array parameter to the database command.
        /// </summary>
        /// <param name="command">Database command.</param>
        /// <param name="name">Name of the array.</param>
        /// <param name="type">Type of the array.</param>
        /// <param name="value">Values of the array.</param>
        /// <returns>Database parameter.</returns>
        public static IDbDataParameter AddArrayParameter(this IDbCommand command, string name, DbType type, byte[] value)
        {
            IDbDataParameter parameter = command.CreateParameter();
            parameter.DbType = type;
            parameter.Direction = ParameterDirection.Output;
            parameter.ParameterName = name;
            parameter.Size = value.Length;
            parameter.Value = value;
            command.Parameters.Add(parameter);
            return parameter;
        }

        /// <summary>
        /// Add an output parameter to the database command.
        /// </summary>
        /// <param name="command">Database command.</param>
        /// <param name="name">Name of the parameter.</param>
        /// <param name="type">Type of the parameter.</param>
        /// <returns>Database parameter.</returns>
        public static IDbDataParameter OutParameter(this IDbCommand command, string name, DbType type)
        {
            IDbDataParameter parameter = command.CreateParameter();
            parameter.DbType = type;
            parameter.Direction = ParameterDirection.Output;
            parameter.ParameterName = name;
            command.Parameters.Add(parameter);
            return parameter;
        }

        /// <summary>
        /// Add an output array parameter to the database command.
        /// </summary>
        /// <param name="command">Database command.</param>
        /// <param name="name">Name of the array.</param>
        /// <param name="type">Type of the array.</param>
        /// <param name="length">Length of the array.</param>
        /// <returns>Database parameter.</returns>
        public static IDbDataParameter OutArrayParameter(this IDbCommand command, string name, DbType type, int length)
        {
            IDbDataParameter parameter = command.CreateParameter();
            parameter.DbType = type;
            parameter.Direction = ParameterDirection.Output;
            parameter.ParameterName = name;
            parameter.Size = length;
            command.Parameters.Add(parameter);
            return parameter;
        }
    }
}
