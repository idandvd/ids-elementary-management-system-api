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

        public static bool CheckUserExists(string username,string password)
        {
            DBConnection db = DBConnection.Instance;
            string sql_username = username.Replace("'", "''");
            string sql_password = password.Replace("'", "''");
            string sql_query = "select * from users " +
                    " where username = '" + sql_username + "' " +
                    " and password = '" + sql_password + "'";
            DataTable table = db.GetDataTableByQuery(sql_query);

            if (table == null || table.Rows.Count != 1)
                return false;
            return true;
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

        public static int AddModel(Model newItem)
        {
            DBConnection db = DBConnection.Instance;
            Dictionary<string, string> columns = GetColumns(newItem);
            string columnsNames = string.Join(",", columns.Keys);
            string columnsValues = string.Join(",", columns.Values);
            int newID = db.InsertData("insert into " + newItem.TableName+"("+ columnsNames + ") values(" + columnsValues + ")");
            return newID;
        }

        private static Dictionary<string, string> GetColumns(Model model)
        {
            PropertyInfo[] properies = model.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            Dictionary<string, string> result = new Dictionary<string, string>();
            for (int currentPropertyIndex = 0; currentPropertyIndex < properies.Length; currentPropertyIndex++)
            {
                PropertyInfo currentProperty = properies[currentPropertyIndex];
                if (currentProperty.Name == "Id" || currentProperty.Name == "TableName") continue;
                string columnName = GetColumnName(currentProperty.Name);
                if (currentProperty.PropertyType.IsSubclassOf(typeof(Model)))
                {
                    columnName += "_id";
                    Model subclass = (Model)currentProperty.GetValue(model);
                    PropertyInfo idProperty = subclass.GetType().GetProperty("Id", BindingFlags.Public | BindingFlags.Instance);
                    string value = idProperty.GetValue(subclass).ToString();
                    result[columnName] = value;
                }
                else
                {
                    if (currentProperty.GetValue(model).GetType() == typeof(string))
                        result[columnName] = "'" + currentProperty.GetValue(model).ToString().Replace("'", "''") + "'";
                    else if (currentProperty.GetValue(model).GetType() == typeof(bool))
                        result[columnName] = ((bool)currentProperty.GetValue(model)) ?  "b'1'":"b'0'";
                    else
                        result[columnName] = currentProperty.GetValue(model).ToString();
                }
            }
            return result;
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
                    {
                        if (property.PropertyType.IsSubclassOf(typeof(Model)))
                        {

                            Model subModel = (Model)Activator.CreateInstance(property.PropertyType);

                            string TableName = subModel.GetType().GetProperty("TableName").GetValue(subModel).ToString();
                            subModel = (Model)
                            typeof(BusinessLayer).GetMethod("GetRow")
                                .MakeGenericMethod(property.PropertyType)
                                .Invoke(null, new object[] { TableName, currentRow[currentColumn.Key] });


                            property.SetValue(model, currentRow[currentColumn.Key] == DBNull.Value ? null :
                                    Convert.ChangeType(subModel, property.PropertyType));
                        }
                        else
                        {
                            property.SetValue(model, currentRow[currentColumn.Key] == DBNull.Value ? null :
                                            Convert.ChangeType(currentRow[currentColumn.Key], property.PropertyType));
                        }
                    }
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
            if (modelName.EndsWith("Id") && modelName.Length>2)
                modelName = modelName.Substring(0, modelName.Length - 2);
            return modelName;
        }

        private static string GetColumnName(string propertyName)
        {
            string modelName = char.ToLower(propertyName[0]).ToString();
            for (int currentCharIndex = 1; currentCharIndex < propertyName.Length; currentCharIndex++)
            {
                if (char.IsUpper(propertyName[currentCharIndex]))
                    modelName += "_" + char.ToLower(propertyName[currentCharIndex]);
                else
                    modelName += propertyName[currentCharIndex];
            }
            return modelName;
        }

        public static ClassScheduleTable  GetClassSchedule(int id)
        {
            Class cls = GetRow<Class>("classes", id);
            IEnumerable<HourInDay> hours = GetTable<HourInDay>("Hours_In_Day");
            IEnumerable<Day> days = GetTable<Day>("days");
            DBConnection db = DBConnection.Instance;
            List<ClassSchedule> classSchedule = DataTableToModel<ClassSchedule>(db.GetClassSchedule(id));
            Dictionary<string, Lesson> classScheduleLessons = new Dictionary<string, Lesson>();

            foreach (ClassSchedule currentClassSchedule in classSchedule)
            {
                classScheduleLessons[currentClassSchedule.Day.Id + "$" +
                                     currentClassSchedule.Hour.Id] = currentClassSchedule.Lesson;
            }
            ClassScheduleTable result = new ClassScheduleTable()
            {
                Class = cls,
                HoursInDay = hours,
                Days = days,
                ClassSchedules = classScheduleLessons
            };


            return result;
        }
    }
}