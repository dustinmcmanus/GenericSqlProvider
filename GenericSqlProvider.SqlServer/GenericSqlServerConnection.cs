using System;
using System.Data;
using System.Data.SqlClient;

namespace GenericSqlProvider.SqlServer
{
    // Cannot derive from sealed type OracleConnection
    public class GenericSqlServerConnection : IDbConnection, IDisposable
    {
        private SqlConnection connection;
        private SqlTransaction transaction;

        public GenericSqlServerConnection(string connectionString)
        {
            connection = new SqlConnection(connectionString);
            connection.Open();
        }

        public string ConnectionString { get => connection.ConnectionString; set => connection.ConnectionString = value; }

        public int ConnectionTimeout => connection.ConnectionTimeout;

        public string Database => connection.Database;

        public ConnectionState State => connection.State;

        public IDbTransaction BeginTransaction()
        {
            transaction = connection.BeginTransaction();
            return transaction;
        }

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            transaction = connection.BeginTransaction(il);
            return transaction;
        }

        public void ChangeDatabase(string databaseName)
        {
            connection.ChangeDatabase(databaseName);
        }

        public void Close()
        {
            connection.Close();
        }

        public IDbCommand CreateCommand()
        {
            var command = connection.CreateCommand();
            command.Transaction = transaction;
            //command.BindByName = true; // Oracle only
            return new GenericSqlServerCommand(command);
        }

        public void Open()
        {
            // do nothing. connection is opened when created
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose transaction if exists and not already disposed?
                    connection.Dispose();
                }

                disposedValue = true;
            }
        }
        
        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}
