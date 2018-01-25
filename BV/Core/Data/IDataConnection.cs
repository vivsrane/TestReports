using System.Data;

namespace VB.Common.Core.Data
{
    public interface IDataConnection : ITransactionFactory
    {
        IDbCommand CreateCommand();

        void Close();
    }
}
