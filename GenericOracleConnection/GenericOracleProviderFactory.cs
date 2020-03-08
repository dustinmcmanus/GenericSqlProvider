using GenericSqlProvider;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericSqlProvider
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
