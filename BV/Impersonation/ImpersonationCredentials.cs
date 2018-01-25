using System;
using System.Configuration;
using System.Data.Common;
using System.Globalization;

namespace VB.Common.Impersonation
{
    public class ImpersonationCredentials
    {
        public static ImpersonationCredentials FromConnectionString(string connectionStringName)
        {
            ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings[connectionStringName];

            if (connectionStringSettings != null)
            {
                string connectionString = connectionStringSettings.ConnectionString;

                if (!string.IsNullOrEmpty(connectionString))
                {
                    return new ImpersonationCredentials(connectionString);
                }
            }

            throw new InvalidOperationException(string.Format(CultureInfo.CurrentUICulture, "No such connection string '{0}'", connectionStringName));
        }

        private readonly string _connectionString;
        private readonly string _user;
        private readonly string _domain;
        private readonly string _password;

        public ImpersonationCredentials(string connectionString)
        {
            _connectionString = connectionString;

            DbConnectionStringBuilder builder = new DbConnectionStringBuilder();

            builder.ConnectionString = connectionString;

            _user = (string) builder["User ID"];

            _password = (string) builder["Password"];

            int seperatorIndex = _user.IndexOf('\\');

            if (seperatorIndex == -1)
            {
                _domain = string.Empty;
            }
            else
            {
                _domain = _user.Substring(0, seperatorIndex);
                _user = _user.Substring(seperatorIndex + 1);
            }
        }

        public string ConnectionString
        {
            get { return _connectionString; }
        }

        public string User
        {
            get { return _user; }
        }

        public string Domain
        {
            get { return _domain; }
        }

        public string Password
        {
            get { return _password; }
        }

        public ImpersonationCodeSection LogOn()
        {
            return new ImpersonationCodeSection(User, Domain, Password);
        }
    }
}
