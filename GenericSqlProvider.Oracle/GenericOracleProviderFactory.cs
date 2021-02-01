using System.Data;

namespace GenericSqlProvider.Oracle
{
    // CreateConnection is similar to System.Data.Common.DbProviderFactory usage
    public class GenericOracleProviderFactory : IDbProviderFactory
    {
        public string connectionString;

        public GenericOracleProviderFactory(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public IDbConnection CreateConnection()
        {
            return new GenericOracleConnection(connectionString);
        }

        public DatabaseProviderType GetDatabaseProvider()
        {
            return DatabaseProviderType.Oracle;
        }
    }
}
