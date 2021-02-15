using System.Data;
using System.Data.Common;

namespace GenericSqlProvider
{
    public abstract class DatabaseUtils
    {
        public static void AddParameter(IDbCommand command, string parameterName, object value)
        {
            // look into extension methods
            var param = command.CreateParameter();
            param.ParameterName = parameterName;
            param.Value = value ?? System.DBNull.Value;
            // Parameter is added during CreateParameter();
            //cmd.Parameters.Add(param);
        }

        public abstract long GetSequenceValue();
    }
}
