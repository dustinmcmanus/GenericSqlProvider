using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericSqlProvider
{
    public abstract class DatabaseUtils
    {
        public static void AddParameter(IDbCommand command, string parameterName, object value)
        {
            // look into extension methods
            var param = command.CreateParameter();
            param.ParameterName = parameterName;
            param.Value = value;
            // Parameter is added during CreateParameter();
            //cmd.Parameters.Add(param);
        }

        public abstract long GetSequenceValue();
    }
}
