using System.Data;

namespace GenericSqlProvider.SqlServer
{
    // CreateConnection is similar to System.Data.Common.DbProviderFactory usage
    public class GenericSqlServerProviderFactory : IDbProviderFactory
    {
        public string connectionString;

        public GenericSqlServerProviderFactory(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public IDbConnection CreateConnection()
        {
            return new GenericSqlServerConnection(connectionString);
        }

        public DatabaseProviderType GetDatabaseProvider()
        {
            return DatabaseProviderType.SqlServer;
        }
    }
}
