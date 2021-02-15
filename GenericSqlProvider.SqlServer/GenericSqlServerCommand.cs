using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;

namespace GenericSqlProvider.SqlServer
{
    public class GenericSqlServerCommand : IDbCommand, IDisposable
    {
        private IDbCommand command;
        private IEnumerable<GenericSqlServerParameter> genericSqlServerParameters = new List<GenericSqlServerParameter>();
        //private IDbTransaction transaction; //TODO: set up automatic transactions
        IDataParameterCollection sqlServerParameterCollection = null;
        IDataParameterCollection dummyParameterCollection = null;

        public GenericSqlServerCommand(IDbCommand sqlServerCommand)
        {
            command = sqlServerCommand;
        }

        public IDbConnection Connection { get => command.Connection; set => command.Connection = value; }
        public IDbTransaction Transaction { get => command.Transaction; set => command.Transaction = value; }
        public string CommandText { get => command.CommandText;
            set
            {
                command.CommandText = value;

                if (!string.IsNullOrEmpty(value))
                {
                    command.CommandText = Regex.Replace(value, @"FROM DUAL(?!\w)", "", RegexOptions.IgnoreCase);
                }
            }
        }
        public int CommandTimeout { get => command.CommandTimeout; set => command.CommandTimeout = value; }
        public CommandType CommandType { get => command.CommandType; set => command.CommandType = value; }

        public IDataParameterCollection Parameters => dummyParameterCollection;

        public UpdateRowSource UpdatedRowSource { get => command.UpdatedRowSource; set => command.UpdatedRowSource = value; }

        public void Cancel()
        {
            command.Cancel();
        }

        public IDbDataParameter CreateParameter()
        {
            var parameter = command.CreateParameter();
            if (sqlServerParameterCollection == null)
            {
                sqlServerParameterCollection = command.Parameters;
                dummyParameterCollection = new GenericSqlServerParameterCollection(ref sqlServerParameterCollection);
            }
            var genericParameter = new GenericSqlServerParameter(ref parameter);
            sqlServerParameterCollection.Add(parameter);
            return genericParameter;
        }

        public int ExecuteNonQuery()
        {
            return command.ExecuteNonQuery();
        }

        public IDataReader ExecuteReader()
        {
            return command.ExecuteReader();
        }

        public IDataReader ExecuteReader(CommandBehavior behavior)
        {
            return command.ExecuteReader(behavior);
        }

        public object ExecuteScalar()
        {
            return command.ExecuteScalar();
        }

        public void Prepare()
        {
            command.Prepare();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    command.Dispose();
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
