using ids_elementary_management_system_api.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace ids_elementary_management_system_api
{
    public class BusinessLayer
    {
        public static T GetRow<T>(string tableName, int id)
        {
            DBConnection db = DBConnection.Instance;
            DataTable table = db.GetDataTableByQuery("select * from " + tableName + " where id = " + id);
            if (table == null || table.Rows.Count == 0)
                return default(T);
            T result = DataTableToModel<T>(table).FirstOrDefault();
            return result;
        }

        public static IEnumerable<T> GetTable<T>(string tableName)
        {

            DBConnection db = DBConnection.Instance;
            DataTable table = db.GetDataTableByQuery("select * from " + tableName);
            if (table == null)
                return null;

            List<T> result = DataTableToModel<T>(table);
            return result;
        }

        public static int AddTeacherType(string name)
        {
            DBConnection db = DBConnection.Instance;
            int newID = db.InsertData("insert into teacher_types(name) values('" + name + "')");
            return newID;
        }

        public static IEnumerable<TableInformation> GetAllTablesInformation()
        {

            DBConnection db = DBConnection.Instance;
            DataTable table = db.GetDataTableByQuery("select * from information_schema.tables where table_schema = 'reshit'");
            if (table == null)
                return null;

            List<TableInformation> result = DataTableToModel<TableInformation>(table);
            return result;
        }

        

        //public static IEnumerable<object> GetTable(string tableName)
        //{
        //    string typeName = tableName.Substring(0, tableName.Length - 1);
        //    if(typeName[typeName.Length-1] == 'e')
        //        typeName = typeName.Substring(0, typeName.Length - 1);
        //    Type type = Type.GetType("ids_elementary_management_system_api.Models." + typeName);

        //    DBConnection db = DBConnection.Instance;
        //    DataTable table = db.GetDataTableByQuery("select * from " + tableName);
        //    if (table == null)
        //        return null;

        //    MethodInfo method = typeof(BusinessLayer).GetMethod("DataTableToModel");
        //    MethodInfo generic = method.MakeGenericMethod(type);

        //    List<object> result  = ((IList) generic.Invoke(null, new object[] { table })).Cast<object>().ToList< object>();

        //    return result;
        //}

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

        public static List<T> DataTableToModel<T>(DataTable table)
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
                    if (property != null)
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
                        modelName += char.ToLower(columnName[currentCharIndex]);
            }
            return modelName;
        }
    }
}