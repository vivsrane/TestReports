using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

namespace VB.Common.Data
{
    [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "Parent implementation sufficient")]
    public class DataCommand : DbCommandWrapper, IDataCommand
    {
        public DataCommand(IDbCommand command) : base(command)
        {
        }

        public void AddParameterWithValue(string name, DbType type, bool nullable, object value)
        {
            IDataParameter parameter = CreateParameter(new DataParameterTemplate(name, type, nullable));
            if (nullable && value == null)
                parameter.Value = DBNull.Value;
            else
                parameter.Value = value;
            Command().Parameters.Add(parameter);
        }

        public IDataParameter AddOutParameter(string name, DbType type)
        {
            IDataParameter parameter = CreateParameter(new DataParameterTemplate(name, type, true));
            parameter.Direction = ParameterDirection.Output;
            Command().Parameters.Add(parameter);
            return parameter;
        }

        public IDataParameter CreateParameter(IDataParameterTemplate parameterTemplate)
        {
            IDataParameter parameter = Command().CreateParameter();
            
            parameter.DbType = parameterTemplate.DbType;

            DbParameter db = parameter as DbParameter;

            if (db != null)
            {
                db.IsNullable = parameterTemplate.IsNullable;
            }
            
            parameter.ParameterName = parameterTemplate.Name;

            return parameter;
        }
    }
}
