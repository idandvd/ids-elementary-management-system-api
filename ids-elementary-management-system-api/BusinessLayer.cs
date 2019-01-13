using ids_elementary_management_system_api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace ids_elementary_management_system_api
{
    public class BusinessLayer
    {
        public static IEnumerable<Student> GetClassStudents(int classID)
        {
            DBConnection db = DBConnection.Instance;
            DataTable dt = db.GetDataTableByQuery(" select students.* " +
                                                    " from students " +
                                                    " inner join classes on classes.id = students.class_id" +
                                                                        " and classes.year_id = (select value from preferences where name = 'current_year_id')" +
                                                    " where classes.id = " + classID);
            List<Student> lst = DataTableToModel<Student>(dt);
            return lst;
        }

        private static List<T> DataTableToModel<T>(DataTable table)
        {
            List<T> result = new List<T>();
            Type type = typeof(T);
            Dictionary<string, string> columns = new Dictionary<string, string>();
            foreach (DataColumn currentColumn in table.Columns)
            {
                columns.Add(currentColumn.ColumnName, GetModelName(currentColumn.ColumnName));
            }
            foreach (DataRow currentRow in table.Rows)
            {
                T model = (T)Activator.CreateInstance(type);
                foreach (KeyValuePair<string, string> currentColumn in columns)
                {
                    PropertyInfo property = type.GetProperty(currentColumn.Value, BindingFlags.Public | BindingFlags.Instance);
                    property.SetValue(model, currentRow[currentColumn.Key] == DBNull.Value ? null : currentRow[currentColumn.Key]);

                }
                result.Add(model);
            }
            return result;
        }

        private static string GetModelName(string columnName)
        {
            string modelName = string.Empty;
            
            bool isUpper = true;
            for (int currentCharIndex = 0; currentCharIndex < columnName.Length; currentCharIndex++)
            {
                if (isUpper)
                {
                    modelName += char.ToUpper(columnName[currentCharIndex]);
                    isUpper = false;
                }
                else
                    if (columnName[currentCharIndex] == '_')
                    isUpper = true;
                else
                    modelName += columnName[currentCharIndex];
            }
            return modelName;
        }
    }
}