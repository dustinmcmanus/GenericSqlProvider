using System;

namespace GenericSqlProvider.Configuration
{
    public class ConnectionStringBuilder
    {
        private ConnectionConfiguration database;

        public ConnectionStringBuilder(ConnectionConfiguration dbInfo)
        {

            if (string.IsNullOrWhiteSpace(dbInfo.HostName))
            {
                throw new ArgumentNullException("databaseHostName");
            }
            if (string.IsNullOrWhiteSpace(dbInfo.Port))
            {
                throw new ArgumentNullException("databasePort");
            }
            if (string.IsNullOrWhiteSpace(dbInfo.Name))
            {
                throw new ArgumentNullException("databaseName");
            }
            if (string.IsNullOrWhiteSpace(dbInfo.UserName))
            {
                throw new ArgumentNullException("databaseUserName");
            }
            if (string.IsNullOrWhiteSpace(dbInfo.UserPassword))
            {
                throw new ArgumentNullException("databaseUserPassword");
            }

            // change to commented lines if not using C# 7 out parameter discard (with Visual Studio 2019 or later):
            //int dbPort = 0;
            //if (!int.TryParse(dbInfo.Port, out dbPort))
            if (!int.TryParse(dbInfo.Port, out _)) 
            {
                throw new InvalidCastException("Invalid port number");
            }

            database = dbInfo;
        }

        public string GetConnectionString(DatabaseProviderInfo provider)
        {
            switch (provider.IntegerValue)
            {
                case (int)DatabaseProviderType.Oracle:
                    return GetOracleConnectionString();
                case (int)DatabaseProviderType.SqlServer:
                    return GetSqlServerConnectionString();
                default:
                    throw new NotImplementedException($"Connection strings for {provider.InvariantName} are not supported yet");
            }
        }

        public string GetConnectionString(string providerInvariantName)
        {
            switch (providerInvariantName)
            {
                case "Oracle.ManagedDataAccess.Client":
                    return GetOracleConnectionString();
                case "System.Data.SqlClient":
                    return GetSqlServerConnectionString();
                default:
                    throw new NotImplementedException($"Connection strings for {providerInvariantName} are not supported yet");
            }
        }

        public string GetOracleConnectionString()
        {
            string dataSource = string.Format("{0}:{1}/{2}", database.HostName, database.Port, database.Name);
            string oraConString = string.Format("Password={0};User ID={1};Data Source={2};", database.UserPassword, database.UserName, dataSource);
            return oraConString;
        }

        public string GetSqlServerConnectionString()
        {
            return string.Format("user id={0};password={1};server={2},{4};Trusted_Connection=no;database={3};",
                                                database.UserName, database.UserPassword, database.HostName, database.Name, database.Port);
        }
    }
}
