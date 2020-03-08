using GenericSqlprovider;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericSqlProvider
{
    public interface IDbProviderFactory
    {
        IDbConnection CreateConnection();
        DatabaseProviderType GetDatabaseProvider();
    }
}
