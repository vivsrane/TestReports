using System.Data;

namespace VB.Common.Data
{
    public interface IDataConnection : IDbConnection
    {
        IDataCommand CreateCommand(IDataCommandTemplate commandTemplate, DataParameterValue parameterValue);
    }
}
