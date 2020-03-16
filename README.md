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
...for example if you are selecting the next value from a sequence or if you have a statement with SELECT CASE or COALESCE or some mathematical calculation based on values in tables where your are selecting information that goes along with a table but does not get selected FROM the table, then Oracle requires you to provide a FROM DUAL clause after the select, but SQL Server allows you to ommit this FROM DUAL clause if there is not a single table that the associated subquery is coming from.

Here is an example of an application with dynamic global settings that can be overridden Whether or not these statements can be avoided with application code or schema design changes or functions/stored procedures is outside the scope of this example.

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

CREATE TABLE APPLICATION_SETTING (
    SETTING_NAME VARCHAR(255),
    VALUE VARCHAR(255)
);

CREATE TABLE USER_SETTING (
    SETTING_NAME VARCHAR(255),
    VALUE VARCHAR(255),
    USER_NAME VARCHAR(255)
);

INSERT INTO APPLICATION_SETTING (SETTING_NAME, VALUE) VALUES ('THEME', 'DEFAULT');
INSERT INTO USER_SETTING (SETTING_NAME, VALUE, USER_NAME) VALUES ('THEME', 'DARK', 'Bob');

SELECT * FROM APPLICATION_SETTING;
SELECT * FROM USER_SETTING;


SELECT
       APPLICATION_SETTING.SETTING_NAME,
(SELECT
CASE WHEN USER_SETTING.VALUE IS NOT NULL
    THEN USER_SETTING.VALUE
ELSE
   APPLICATION_SETTING.VALUE
END) AS VALUE
FROM
APPLICATION_SETTING
JOIN USER_SETTING
    ON USER_SETTING.SETTING_NAME=APPLICATION_SETTING.SETTING_NAME
WHERE USER_SETTING.USER_NAME='Bob';

-- OR ALTERNATIVELY:
SELECT
       APPLICATION_SETTING.SETTING_NAME,
(SELECT COALESCE(USER_SETTING.VALUE, APPLICATION_SETTING.VALUE)) AS VALUE
FROM
APPLICATION_SETTING
JOIN USER_SETTING
    ON USER_SETTING.SETTING_NAME=APPLICATION_SETTING.SETTING_NAME
WHERE USER_SETTING.USER_NAME='Bob';




long sequenceValue;
using (IDbConnection connection = new SqlConnection(connectionString))
{
    connection.Open();
    using (IDbCommand command = connection.CreateCommand())
    {
        command.CommandText = @"
SELECT
(SELECT 
CASE WHEN CFG_SAMPLE_POINT_TEST.VOLUME IS NOT NULL
    THEN CFG_SAMPLE_POINT_TEST.VOLUME
ELSE 
    CASE WHEN TEST_CFG.VOLUME IS NOT NULL
        THEN TEST_CFG.VOLUME
        ELSE 0
    END
END" & fromDualClause & ") AS VOLUME";
        sequenceValue = Convert.ToInt64(command.ExecuteScalar());
    }
}


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


CREATE TABLE APPLICATION_SETTING (
    SETTING_NAME VARCHAR2(255),
    VALUE VARCHAR2(255)
);

CREATE TABLE USER_SETTING (
    SETTING_NAME VARCHAR2(255),
    VALUE VARCHAR2(255),
    USER_NAME VARCHAR2(255)
);

COMMIT;

INSERT INTO APPLICATION_SETTING (SETTING_NAME, VALUE) VALUES ('THEME', 'DEFAULT');
INSERT INTO USER_SETTING (SETTING_NAME, VALUE, USER_NAME) VALUES ('THEME', 'DARK', 'Bob');

SELECT * FROM APPLICATION_SETTING;
SELECT * FROM USER_SETTING;

SELECT
       APPLICATION_SETTING.SETTING_NAME,
(SELECT
CASE WHEN USER_SETTING.VALUE IS NOT NULL
    THEN USER_SETTING.VALUE
ELSE
   APPLICATION_SETTING.VALUE
END FROM DUAL) AS VALUE
FROM
APPLICATION_SETTING
JOIN USER_SETTING
    ON USER_SETTING.SETTING_NAME=APPLICATION_SETTING.SETTING_NAME
WHERE USER_SETTING.USER_NAME='Bob';

-- OR ALTERNATIVELY:
SELECT
       APPLICATION_SETTING.SETTING_NAME,
(SELECT COALESCE(USER_SETTING.VALUE, APPLICATION_SETTING.VALUE) FROM DUAL) AS VALUE
FROM
APPLICATION_SETTING
JOIN USER_SETTING
    ON USER_SETTING.SETTING_NAME=APPLICATION_SETTING.SETTING_NAME
WHERE USER_SETTING.USER_NAME='Bob';


```

Another alternative for a similar use case:
SQL Server:
```
CREATE TABLE APPLICATION_SETTING2 (
    THEME VARCHAR(255)
);

CREATE TABLE USER_SETTING2 (
    THEME VARCHAR(255),
    USER_NAME VARCHAR(255)
);

INSERT INTO APPLICATION_SETTING2 (THEME) VALUES ('DEFAULT');
INSERT INTO USER_SETTING2 (THEME, USER_NAME) VALUES ('DARK', 'Bob');


SELECT * FROM APPLICATION_SETTING2;
SELECT * FROM USER_SETTING2;

SELECT
(SELECT
CASE WHEN EXISTS (SELECT USER_SETTING2.THEME FROM USER_SETTING2 WHERE USER_SETTING2.USER_NAME='Bob')
    THEN (SELECT USER_SETTING2.THEME FROM USER_SETTING2 WHERE USER_SETTING2.USER_NAME='Bob')
ELSE
   APPLICATION_SETTING2.THEME
END) AS VALUE
FROM APPLICATION_SETTING2;

```

Oracle:
```
CREATE TABLE APPLICATION_SETTING2 (
    THEME VARCHAR2(255)
);

CREATE TABLE USER_SETTING2 (
    THEME VARCHAR2(255),
    USER_NAME VARCHAR2(255)
);

COMMIT;

INSERT INTO APPLICATION_SETTING2 (THEME) VALUES ('DEFAULT');
INSERT INTO USER_SETTING2 (THEME, USER_NAME) VALUES ('DARK', 'Bob');


SELECT * FROM APPLICATION_SETTING2;
SELECT * FROM USER_SETTING2;

SELECT
(SELECT
CASE WHEN EXISTS (SELECT USER_SETTING2.THEME FROM USER_SETTING2 WHERE USER_SETTING2.USER_NAME='Bob')
    THEN (SELECT USER_SETTING2.THEME FROM USER_SETTING2 WHERE USER_SETTING2.USER_NAME='Bob')
ELSE
   APPLICATION_SETTING2.THEME
END FROM DUAL) AS VALUE
FROM APPLICATION_SETTING2;

```


## The Solution
...work in progress