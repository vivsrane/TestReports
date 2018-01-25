using System.Data;

namespace VB.Common.Core.Data
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        public IDbConnection GetConnection(string connectionName)
        {
            return Database.Connection(connectionName);
        }
    }
}