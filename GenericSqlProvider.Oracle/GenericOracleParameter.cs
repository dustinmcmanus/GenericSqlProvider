using System.Data;

namespace GenericSqlProvider.Oracle
{
    public class GenericOracleParameter : IDbDataParameter
    {
        public IDbDataParameter parameter;

        public GenericOracleParameter(ref IDbDataParameter oracleParameter)
        {
            parameter = oracleParameter;
        }

        public byte Precision { get => parameter.Precision; set => parameter.Precision = value; }
        public byte Scale { get => parameter.Scale; set => parameter.Scale = value; }
        public int Size { get => parameter.Size; set => parameter.Size = value; }
        public DbType DbType { get => parameter.DbType; set => parameter.DbType = value; }
        public ParameterDirection Direction { get => parameter.Direction; set => parameter.Direction = value; }

        public bool IsNullable => parameter.IsNullable;

        public string ParameterName
        {
            get => parameter.ParameterName;
            set
            {
                parameter.ParameterName = value;

                if (!string.IsNullOrEmpty(value))
                {
                    parameter.ParameterName = value.Replace("@", "");
                }
            }
        }
        public string SourceColumn { get => parameter.SourceColumn; set => parameter.SourceColumn = value; }
        public DataRowVersion SourceVersion { get => parameter.SourceVersion; set => parameter.SourceVersion = value; }
        public object Value { get => parameter.Value; set => parameter.Value = value; }
    }
}
