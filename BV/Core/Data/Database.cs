using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Reflection;
using System.Text;

namespace VB.Common.Core.Data
{
    /// <summary>
    /// Database connection helpers.
    /// </summary>
    public static class Database
    {
        /// <summary>
        /// Create a connection for the database with the given name. Pulls all relevant settings on the database from
        /// the configuration manager. Does not ensure the connection is open.
        /// </summary>
        /// <param name="database">Database to connect to.</param>
        /// <returns>Database connection.</returns>
        public static IDbConnection ConfigurationManagerConnection(string database)
        {
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[database];
            if (settings == null)
            {
                throw new ArgumentException("No such database", "database");
            }

            DbProviderFactory factory = DbProviderFactories.GetFactory(settings.ProviderName);

            IDbConnection connection = factory.CreateConnection();
            if (connection == null)
            {
                throw new ApplicationException("Could not create database connection");
            }

            connection.ConnectionString = settings.ConnectionString;
            return connection;
        }

        /// <summary>
        /// Get a connection for the database with the given name, and ensure the connection is open.
        /// </summary>
        /// <param name="name">Database to connect to.</param>
        /// <returns>Database connection.</returns>
        public static IDbConnection Connection(string name)
        {
            IDbConnection connection = ConfigurationManagerConnection(name);

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            return connection;
        }

        /// <summary>
        /// Add the a parameter to the given database command.
        /// </summary>
        /// <param name="command">Database command to add parameter to.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="parameterValue">Parameter value.</param>
        /// <param name="dbType">Parameter data type.</param>
        public static void AddWithValue(IDbCommand command, string parameterName, object parameterValue, DbType dbType)
        {
            IDbDataParameter parameter = command.CreateParameter();

            parameter.ParameterName = parameterName;
            parameter.DbType        = dbType;
            parameter.Value         = parameterValue ?? DBNull.Value;
            
            command.Parameters.Add(parameter);
        }

        /// <summary>
        /// Get command text from a resource in the given assembly.
        /// </summary>
        /// <param name="a">Assembly.</param>
        /// <param name="resourceName">Resource name.</param>
        /// <returns>Command text string.</returns>
        private static string GetCommandTextFromAssembly(Assembly a, string resourceName)
        {            
            StringBuilder sb = new StringBuilder();

            using (StringWriter sw = new StringWriter(sb))
            {
                Stream stream = a.GetManifestResourceStream(resourceName);

                if (stream != null)
                {
                    using (StreamReader sr = new StreamReader(stream))
                    {
                        string line;

                        while ((line = sr.ReadLine()) != null)
                        {
                            sw.WriteLine(line);
                        }
                    }
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Cache of command text.
        /// </summary>
        private static readonly Dictionary<string, string> Scripts = new Dictionary<string, string>();

        /// <summary>
        /// Get command text from a resource in the given assembly, or from the scripts cache if its been previously
        /// requested. 
        /// </summary>
        /// <param name="a">Assembly.</param>
        /// <param name="resourceName">Resource name.</param>
        /// <returns>Command text string.</returns>
        public static string GetCommandText(Assembly a, string resourceName)
        {
            if (Scripts.ContainsKey(resourceName))
            {
                return Scripts[resourceName];
            }

            string script = GetCommandTextFromAssembly(a, resourceName);

            Scripts[resourceName] = script;

            return script;
        }
    }
}