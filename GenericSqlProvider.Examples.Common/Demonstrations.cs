using System.Collections.Generic;
using System.Data;

namespace GenericSqlProvider.Examples.Common
{
    public static class Demonstrations
    {

        public static List<string> GetSettingsWithMicrosoftDocumentationAddParameter(IDbConnection connection)
        {
            var settingNames = new List<string>();
            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = @"SELECT SETTING_NAME FROM USER_SETTING WHERE USER_NAME=@USER_NAME";

                var param = command.CreateParameter();
                param.ParameterName = "@USER_NAME";
                param.Value = "Bob";
                command.Parameters.Add(param);

                using (IDataReader rdr = command.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        settingNames.Add(rdr.GetString(0));
                    }
                }
            }

            return settingNames;
        }

        public static List<string> GetSettingsWithDatabaseUtilsAddParameter(IDbConnection connection)
        {
            var settingNames = new List<string>();
            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = @"SELECT SETTING_NAME FROM USER_SETTING WHERE USER_NAME=@USER_NAME";

                GenericSqlProvider.DatabaseUtils.AddParameter(command, "@USER_NAME", "Bob");

                using (IDataReader rdr = command.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        settingNames.Add(rdr.GetString(0));
                    }
                }
            }
            return settingNames;
        }

    }
}
