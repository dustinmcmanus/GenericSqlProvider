using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;

namespace GenericSqlProvider.Oracle
{
    // Cannot derive from sealed type OracleConnection
    public class GenericOracleConnection : IDbConnection, IDisposable
    {
        private OracleConnection connection;
        private OracleTransaction transaction;

        public GenericOracleConnection(string connectionString)
        {
            connection = new OracleConnection(connectionString);
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
            // TODO: assign transaction if exists
            var oracleCommand = connection.CreateCommand();
            oracleCommand.BindByName = true;
            oracleCommand.Transaction = transaction;
            return new GenericOracleCommand(oracleCommand);
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
