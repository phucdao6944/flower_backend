using Dapper;

namespace flower.Helpers
{
    public static class SqlForm
    {
        public static string sql = "";
        public static string GenerateSqlString(string[] fields, string table, bool isCreate, bool reset = true)
        {
            if (reset) sql = "";
            if(isCreate)
            {
                sql += "INSERT INTO " + table;
                string fieldsNeedInsert = string.Join(",", fields);
                string valueOfFields = "@" + string.Join(",@", fields);
                sql += " (" + fieldsNeedInsert + ") OUTPUT INSERTED.[id] VALUES (" + valueOfFields + ")";
            }
            else
            {
                sql += "UPDATE " + table + " SET ";
                string condition = "";
                foreach (string field in fields)
                {
                    if (string.IsNullOrEmpty(condition))
                    {
                        condition += " " + field + " = @" + field;
                    }
                    else
                    {
                        condition += " , " + field + " = @" + field;
                    }
                }
                sql += condition + " WHERE 1 = 1 ";
            }
            return sql;
        }
    }
}
