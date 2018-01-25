using System;
using System.Data;

namespace VB.Common.Data
{
    public static class CommonMethods
    {
        public static void AddWithValue(IDbCommand command, string parameterName, object parameterValue, DbType dbType)
        {
            AddWithValue(command, parameterName, parameterValue, dbType, ParameterDirection.Input, false);
        }

        public static void AddWithValue(IDbCommand command, string parameterName, object parameterValue, DbType dbType, bool handleNullValues)
        {
            AddWithValue(command, parameterName, parameterValue, dbType, ParameterDirection.Input, handleNullValues);
        }

        public static void AddWithValue(IDbCommand command, string parameterName, object parameterValue, DbType dbType, ParameterDirection direction, bool handleNullValues)
        {
            IDbDataParameter parameter = command.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.DbType = dbType;
            if (handleNullValues && parameterValue == null)
                parameter.Value = DBNull.Value;
            else
                parameter.Value = parameterValue;
            parameter.Direction = direction;
            command.Parameters.Add(parameter);
        }

    }
}
