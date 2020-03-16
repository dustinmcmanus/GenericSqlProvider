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

### Parameterized queries (parameter name and SQL command text):

**SQL Server**
```
using (IDbConnection connection = new SqlConnection(connectionString))
{
    connection.Open();
    using (IDbCommand command = connection.CreateCommand())
    {
        command.CommandText = @"INSERT INTO USERS (NAME) VALUES (@NAME)";
        var param = command.CreateParameter();
        param.ParameterName = "@NAME";
        param.Value = "Bob";
        command.Parameters.Add(param);
        command.ExecuteNonQuery();
    }
}
```

**Oracle**
```
using (IDbConnection connection = new OracleConnection(connectionString))
{
    connection.Open();
    using (IDbCommand command = connection.CreateCommand())
    {
        command.CommandText = @"INSERT INTO USERS (NAME) VALUES (:NAME)";
        var param = command.CreateParameter();
        param.ParameterName = "NAME";
        param.Value = "Bob";
        command.Parameters.Add(param);
        command.ExecuteNonQuery();
    }
}
```
### FROM DUAL clause

**SQL Server**
```
long sequenceValue;
using (IDbConnection connection = new SqlConnection(connectionString))
{
    connection.Open();
    using (IDbCommand command = connection.CreateCommand())
    {
        command.CommandText = "SELECT NEXT VALUE FOR USERS_SEQUENCE";
        sequenceValue = Convert.ToInt64(command.ExecuteScalar());
    }
}
```

**Oracle**
```
using (IDbConnection connection = new OracleConnection(connectionString))
{
    connection.Open();
    using (IDbCommand command = connection.CreateCommand())
    {
        command.CommandText = @"INSERT INTO USERS (NAME) VALUES (:NAME)";
        var param = command.CreateParameter();
        param.ParameterName = "NAME";
        param.Value = "Bob";
        command.Parameters.Add(param);
        command.ExecuteNonQuery();
    }
}
```

## The Solution
...work in progress