using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace GenericSqlProvider.Configuration
{
    public class DatabaseProviders
    {
        static readonly ReadOnlyObservableCollection<DatabaseProviderInfo> readOnlyProviders;

        static DatabaseProviders()
        {
            var providers = new ObservableCollection<DatabaseProviderInfo>();
            providers.Add(new DatabaseProviderInfo() { IntegerValue = (int)DatabaseProviderType.Oracle, DisplayName = "Oracle", InvariantName = "Oracle.ManagedDataAccess.Client" });
            providers.Add(new DatabaseProviderInfo() { IntegerValue = (int)DatabaseProviderType.SqlServer, DisplayName = "SQL Server", InvariantName = "System.Data.SqlClient" });
            readOnlyProviders = new ReadOnlyObservableCollection<DatabaseProviderInfo>(providers);
        }

        public static ReadOnlyObservableCollection<DatabaseProviderInfo> GetSupportedProviders()
        {
            return readOnlyProviders;
        }
    }
}
