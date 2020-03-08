using GenericSqlProvider;
using GenericSqlProvider;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericSqlServerProvider
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
