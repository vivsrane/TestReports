using System.Data;

namespace VB.Common.Core.Data
{
    public interface IDbConnectionFactory
    {
        IDbConnection GetConnection(string connectionName);
    }
}