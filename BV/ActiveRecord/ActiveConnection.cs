using System.Configuration;
using System.Data.Common;

namespace VB.Common.ActiveRecord
{
    public class ActiveConnection
    {
        private readonly string connectionString;
        private readonly string providerName;

        public ActiveConnection(string database)
        {
            this.connectionString = ConfigurationManager.ConnectionStrings[database].ConnectionString;
            this.providerName = ConfigurationManager.ConnectionStrings[database].ProviderName;
        }

        public DbConnection CreateConnection()
        {
            DbConnection connection = DbProviderFactories.GetFactory(providerName).CreateConnection();
            connection.ConnectionString = connectionString;
            return connection;
        }
    }
}
