using System.Data;

namespace GenericSqlProvider
{
    public interface IDbProviderFactory
    {
        IDbConnection CreateConnection();
        DatabaseProviderType GetDatabaseProvider();
    }
}
