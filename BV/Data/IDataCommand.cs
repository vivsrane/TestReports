using System.Data;

namespace VB.Common.Data
{
    public interface IDataCommand : IDbCommand
    {
        void AddParameterWithValue(string name, DbType type, bool nullable, object value);

        IDataParameter AddOutParameter(string name, DbType type);

        IDataParameter CreateParameter(IDataParameterTemplate parameterTemplate);
    }
}
