using System;
using System.Data;
using System.Data.Common;
using System.Globalization;
using Microsoft.AnalysisServices.AdomdClient;
using VB.Common.Impersonation;
using VB.Common.Pool;
using VB.Common.Pool.Spi;

namespace VB.Common.Data
{
    public abstract class DataProviderFactory
    {
        private const string ADOMD_PROVIDER_NAME = "Microsoft.AnalysisServices.AdomdClient.XmlaClientProvider";

        private static readonly DataProviderFactory adomdProviderfactory = new AdomdDataProviderFactory();

        public static DataProviderFactory GetFactory(string providerName)
        {
            if (string.IsNullOrEmpty(providerName))
            {
                throw new ArgumentNullException("providerName");
            }

            DataProviderFactory factory;

            if (providerName.Equals(ADOMD_PROVIDER_NAME))
            {
                factory = adomdProviderfactory;
            }
            else
            {
                factory = new DbDataProviderFactory(DbProviderFactories.GetFactory(providerName));
            }

            return factory;
        }

        public abstract IDataConnection CreateConnection();

        public IDataConnection CreateConnection(string connectionString)
        {
            IDataConnection connection = CreateConnection();
            connection.ConnectionString = connectionString;
            return connection;
        }

        class DbDataProviderFactory : DataProviderFactory
        {
            private readonly DbProviderFactory factory;

            public DbDataProviderFactory(DbProviderFactory factory)
            {
                this.factory = factory;
            }

            public override IDataConnection CreateConnection()
            {
                return new DataConnection(factory.CreateConnection());
            }
        }

        class AdomdDataProviderFactory : DataProviderFactory, IDisposable
        {
            private readonly BoundedKeyedPool<IDbConnection, string> pool;

            public AdomdDataProviderFactory()
            {
                // parameters

                TimeSpan evictionInterval = new TimeSpan(TimeSpan.TicksPerSecond);

                TimeSpan idleInterval = new TimeSpan(TimeSpan.TicksPerMinute*5);

                int idleCount = 10;

                TimeSpan idleCounterInterval = new TimeSpan(TimeSpan.TicksPerMinute * 2);

                IKeyedPooledItemFactory<IDbConnection, string> factory = new ConnectionFactory();

                Bounds bounds = new Bounds(0, 100);

                EvictionCriteria criteria = new EvictionCriteria(evictionInterval, idleInterval, idleCount, idleCounterInterval);

                const string name = "BoundedKeyedPool<IDataConnection,string>";

                // create the pool

                pool = new BoundedKeyedPool<IDbConnection, string>(name, factory, bounds, criteria);
            }

            ~AdomdDataProviderFactory()
            {
                Dispose(false);
            }

            public override IDataConnection CreateConnection()
            {
                return new DataConnection(new ConnectionProxy(pool));
            }

            class ConnectionProxy : IDbConnection, IDbConnectionProxy, IDisposableHandle
            {
                private readonly DbConnectionStringBuilder connectionStringBuilder = new DbConnectionStringBuilder();

                private readonly IKeyedPool<IDbConnection, string> connectionFactory;

                private IDbConnection connection;

                private string database = String.Empty;

                private ConnectionState connectionState = ConnectionState.Closed;

                public ConnectionProxy(IKeyedPool<IDbConnection, string> connectionFactory)
                {
                    this.connectionFactory = connectionFactory;
                }

                #region Helper Methods

                protected bool IsOpen()
                {
                    return (connectionState.Equals(ConnectionState.Open));
                }

                protected void AssertOpen()
                {
                    if (!connectionState.Equals(ConnectionState.Open))
                    {
                        throw new InvalidOperationException("The connection is not open");
                    }
                }

                #endregion

                #region IDbConnection Members

                public IDbTransaction BeginTransaction(IsolationLevel il)
                {
                    AssertOpen();

                    return connection.BeginTransaction(il);
                }

                public IDbTransaction BeginTransaction()
                {
                    AssertOpen();

                    return connection.BeginTransaction();
                }

                public void ChangeDatabase(string databaseName)
                {
                    AssertOpen();

                    database = databaseName;

                    connection.ChangeDatabase(databaseName);
                }

                public void Close()
                {
                    if (IsOpen())
                    {
                        connectionState = ConnectionState.Closed;

                        connectionFactory.ReturnItem(connection);

                        connection = null;
                    }
                }

                public string ConnectionString
                {
                    get { return connectionStringBuilder.ConnectionString; }
                    set { connectionStringBuilder.ConnectionString = value; }
                }

                public int ConnectionTimeout
                {
                    get
                    {
                        const string fmt = "The connection timeout '{0}' is less than zero";

                        int timeout = 15;

                        string timeoutText = (string)connectionStringBuilder["Connection Timeout"];

                        if (!string.IsNullOrEmpty(timeoutText))
                        {
                            if (!Int32.TryParse(timeoutText, out timeout))
                            {
                                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, fmt, timeoutText));
                            }
                        }

                        if (timeout < 0)
                        {
                            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, fmt, timeout));
                        }

                        return timeout;
                    }
                }

                public IDbCommand CreateCommand()
                {
                    return connection.CreateCommand();
                }

                public string Database
                {
                    get { return database; }
                }

                public void Open()
                {
                    if (!IsOpen())
                    {
                        connection = connectionFactory.BorrowItem(ConnectionString, new TimeSpan(0,0,30));

                        connectionState = ConnectionState.Open;
                    }
                }

                public ConnectionState State
                {
                    get
                    {
                        return IsOpen() ? connection.State : connectionState;
                    }
                }

                #endregion

                #region IDisposable Members

                public void Dispose()
                {
                    Dispose(true);
                    GC.SuppressFinalize(this);
                }

                protected virtual void Dispose(bool disposing)
                {
                    if (disposing)
                    {
                        Close();
                    }
                }

                #endregion

                #region IDisposableHandle Members

                public IDisposable Disposable
                {
                    get { return connection; }
                }

                #endregion

                #region IDbConnectionProxy Members

                public IDbConnection Connection()
                {
                    return connection;
                }

                #endregion
            }

            class ConnectionFactory : IKeyedPooledItemFactory<IDbConnection, string>
            {
                public IDbConnection MakeItem(string key)
                {
                    return new AdomdConnection();
                }

                public void DestroyItem(string key, IDbConnection item)
                {
                    try
                    {
                        item.Close();
                    }
                    catch (Exception e)
                    {
                        throw new PoolLifeCycleException("Failed to close the connection", e);
                    }
                }

                public bool ValidateItem(string key, IDbConnection item)
                {
                    DbConnectionStringBuilder x = new DbConnectionStringBuilder();

                    x.ConnectionString = key;

                    object catalog;

                    string query = "WITH MEMBER [One] AS \"1\" SELECT { [ONE] } ON COLUMNS FROM [Inventory]";
                    
                    if (x.TryGetValue("Initial Catalog", out catalog))
                    {
                        if (string.Equals("JDPower", catalog))
                        {
                            query = query.Replace("[Inventory]", "[JDPower]");
                        }
                    }

                    try
                    {
                        using (IDbCommand command = item.CreateCommand())
                        {
                            command.CommandText = query;

                            using (IDataReader reader = command.ExecuteReader())
                            {
                                return reader.Read();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        throw new PoolLifeCycleException("Error testing connection", e);
                    }
                }

                public void ActivateItem(string key, IDbConnection item)
                {
                    if (item.State.Equals(ConnectionState.Open))
                    {
                        return;
                    }

                    try
                    {
                        using (ImpersonationCodeSection section = new ImpersonationCredentials(key).LogOn())
                        {
                            item.ConnectionString = key;

                            item.Open();
                        }
                    }
                    catch (Exception e)
                    {
                        throw new PoolLifeCycleException("Failed to activate item", e);
                    }
                }

                public void PassivateItem(string key, IDbConnection item)
                {
                    // do nothing
                }
            }

            #region IDisposable Members

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (disposing)
                    if (pool != null)
                        pool.Dispose();
            }

            #endregion
        }
    }
}
