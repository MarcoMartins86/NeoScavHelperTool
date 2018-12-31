using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace NeoScavModHelperTool
{
    public static class SqlCommandExt
    {
        public static void AddArrayParameters<T>(this SQLiteCommand cmd, string name, IEnumerable<T> values)
        {
            name = name.StartsWith("@") ? name : "@" + name;
            var names = string.Join(", ", values.Select((value, i) =>
            {
                var paramName = name + i;
                cmd.Parameters.AddWithValue(paramName, value);
                return paramName;
            }));
            cmd.CommandText = cmd.CommandText.Replace(name, names);
        }

        public static void AddMultipleAndConditions<T, Q>(this SQLiteCommand cmd, string name, IEnumerable<T> columns, IEnumerable<Q> values)
        {
            name = name.StartsWith("@") ? name : "@" + name;
            var conditions = string.Join(" and ", columns.Select((value, i) =>
            {
                var paramName = name + i;
                var condition = "`" + value + "`=" + paramName;
                cmd.Parameters.AddWithValue(paramName, values.ElementAt(i));
                return condition;
            }));

            cmd.CommandText = cmd.CommandText.Replace(name, conditions);
        }
    }
}
