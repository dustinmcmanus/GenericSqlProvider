# GenericSqlProvider
An interface to Oracle.ManagedDataAccess and SQL Server for parameterized queries similar to DbProviderFactory, but with support for Dependency Injection

# Purpose
This solution is an alternative to Entity Framework (EF) for developers and teams who are familiar with SQL, want to use the IDb* interfaces from The .NET System.Data namespace, and who do not want to deal with RDMS Vendor discrepencies in the .NET interface implementations.

## The Problem
Currently this project focuses on rectifying the differences between System.Data.SqlClient and Oracle.ManagedDataAccess.Client. These issues include:
1. Parameterized queries in SQL Server require an @ prefixed to the parameter name. Oracle requires that there not be any prefix to the parameter name.
2. Parameterized queries in SQL Server require an @ prefixed to each placeholder in the IdbCommand.CommandText SQL string. Oracle requires a colon instead.
3. SQL queries in SQL Server do not require a FROM clause if you are not selecting from a table or view. Oracle always requires a FROM clause, so FROM DUAL must be included in SQL queries that do not naturally select from a database object.

The following table summarizes these differences:
|  #  | Descrepency | SQL Server  | Oracle
| --- | ----------- | ----------- | ---
|   1 | Parameterized queries (parameter name) | @ required | @ prohibited
|   2 | Parameterized queries (SQL command text) | @ required  | : required
|   3 | SELECT query without table specification | FROM clause omitted | FROM DUAL required


## Examples
...Work in progress...
...for example if you are selecting the next value from a sequence or if you have a statement like SELECT CASE WHEN EXISTS [omitted details] THEN 1 ELSE 0 END)

1. Parameterized queries (parameter name)

**SQL Server**
```
var results = new List<string>();
using (SqlConnection connection = new SqlConnection(connectionString))
{
    connection.Open();
    using (IDbCommand cmd = connection.CreateCommand())
    {
        cmd.CommandText = "SELECT USERNAME FROM USER";
        using (IDataReader rdr = cmd.ExecuteReader())
        {
            int username = rdr.GetOrdinal("USERNAME");
            while (rdr.Read())
            {
                results.Add(rdr.GetString(username));
            }
        }
    }
}
```


**Oracle**
```

```


## The Solution
...work in progress