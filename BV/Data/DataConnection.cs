using System.Data;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AnalysisServices.AdomdClient;

namespace VB.Common.Data
{
    [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "Parent implementation sufficient")]
    public class DataConnection : DbConnectionWrapper, IDataConnection
    {
        public DataConnection(IDbConnection connection) : base(connection)
        {
        }

        public IDataCommand CreateCommand(IDataCommandTemplate commandTemplate, DataParameterValue parameterValue)
        {
            IDbCommand command = Connection().CreateCommand();
            command.Connection = UnderlyingConnection();
            command.CommandType = commandTemplate.CommandType;
            command.CommandText = commandTemplate.CommandText;

            foreach (IDataParameterTemplate parameterTemplate in commandTemplate.Parameters)
            {
                IDbDataParameter parameter = command.CreateParameter();
                parameter.ParameterName = parameterTemplate.Name;
                
                // AdomdParamater has not implemented the property DbType - awesome
                if ( !parameter.GetType().Equals(typeof(AdomdParameter)))
                {
                    parameter.DbType = parameterTemplate.DbType;
                }

                parameter.Value = parameterValue(parameterTemplate);

                command.Parameters.Add(parameter);
            }

            return new DataCommand(command);
        }
    }
}
