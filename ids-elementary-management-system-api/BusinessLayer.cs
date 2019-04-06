using ids_elementary_management_system_api.Models;
using OfficeOpenXml;
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
        private static List<Class> classes = null;
        private static List<Grade> grades = null;
        private static Year currentYear = null;
        private static List<Parent> parents = null;

        public static List<Class> Classes
        {
            get
            {
                if (classes == null)
                    classes = GetTable<Class>("classes").ToList();
                return classes;
            }

        }
        public static List<Grade> Grades
        {
            get
            {
                if (grades == null)
                    grades = GetTable<Grade>("grades").ToList();
                return grades;
            }

        }
        public static List<Parent> Parents
        {
            get
            {
                if (parents == null)
                    parents = GetTable<Parent>("parents").ToList();
                return parents;
            }

        }
        public static Year CurrentYear
        {
            get
            {
                if( currentYear == null)
                    currentYear = GetCurrentYear();
                return currentYear;
            }
        }

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
            try
            {
                DBConnection db = DBConnection.Instance;
                Dictionary<string, string> columns = GetColumns(newItem);
                string columnsNames = string.Join(",", columns.Keys);
                string columnsValues = string.Join(",", columns.Values);
                int newID = db.InsertData("insert into " + newItem.TableName + "(" + columnsNames + ") values(" + columnsValues + ")");
                return newID;
            }
            catch (Exception)
            {
                return -1;
            }
            
        }

        public static bool EditModel(Model model)
        {
            try
            {
                DBConnection db = DBConnection.Instance;
                Dictionary<string, string> columns = GetColumns(model);
                string setString = string.Join(",", columns.Select(col=> col.Key + "=" +col.Value).ToArray());
                return db.UpdateData("update " + model.TableName + " set " + setString + " where id = " + model.Id); ;
            }
            catch (Exception)
            {
                return false;
            }
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
                if (currentProperty.PropertyType == typeof(Year))
                {
                    columnName += "_id";
                    result[columnName] = CurrentYear.Id.ToString();
                }
                else if (currentProperty.PropertyType.IsSubclassOf(typeof(Model)))
                {
                    columnName += "_id";
                    Model subclass = (Model)currentProperty.GetValue(model);
                    if( subclass == null)
                    {
                        result[columnName] = "null";
                        continue;
                    }
                    PropertyInfo idProperty = subclass.GetType().GetProperty("Id", BindingFlags.Public | BindingFlags.Instance);
                    string value = idProperty.GetValue(subclass).ToString();
                    if (value == "0")
                        value = AddModel(subclass).ToString();
                    result[columnName] = value;
                }
                else
                {
                    if (currentProperty.PropertyType == typeof(string))
                        result[columnName] = "'" + (currentProperty.GetValue(model)?.ToString().Replace("'", "''") ?? "") + "'";
                    else if (currentProperty.PropertyType == typeof(bool))
                        result[columnName] = ((bool)currentProperty.GetValue(model)) ? "b'1'" : "b'0'";
                    else
                        result[columnName] = currentProperty.GetValue(model)?.ToString() ?? "";
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

        public static Class ImportClass(ExcelRange wsRow,
                                        int currentRowIndex,
                                        int gradeNameIndex,
                                        int classNumberIndex)
        {
            string gradeName = wsRow[currentRowIndex, gradeNameIndex].Value.ToString().Trim();
            int classNumber = Convert.ToInt32(wsRow[currentRowIndex, classNumberIndex].Value.ToString());
            Class studentClass = Classes.FirstOrDefault(c => c.Grade.Name == gradeName &&
                                                             c.Number == classNumber);
            if (studentClass == null)
            {
                Class newClass = new Class()
                {
                    Grade = Grades.FirstOrDefault(grade => grade.Name == gradeName),
                    Number = classNumber,
                    Year = CurrentYear,
                };
                newClass.Id = AddModel(newClass);
                Classes.Add(newClass);
                studentClass = newClass;
            }
            return studentClass;
        }

        public static Parent ImportParent(ExcelRange wsRow,
                                          int currentRowIndex,
                                          int firstNameIndex,
                                          int lastNameIndex,
                                          int cellphoneIndex,
                                          int emailIndex,
                                          string gender)
        {
            string firstName = (wsRow[currentRowIndex, firstNameIndex].Value ?? string.Empty).ToString();
            string lastName = (wsRow[currentRowIndex, lastNameIndex].Value ?? string.Empty).ToString();
            string cellphone = (wsRow[currentRowIndex, cellphoneIndex].Value ?? string.Empty).ToString().Trim();
            string email = (wsRow[currentRowIndex, emailIndex].Value ?? string.Empty).ToString();
            Parent studentParent = Parents.FirstOrDefault(p => p.FirstName == cellphone &&
                                                               p.LastName == cellphone &&
                                                               p.Cellphone == cellphone &&
                                                               p.Email == email &&
                                                               p.Gender == gender);
            if (studentParent == null)
            {
                Parent newParent = new Parent()
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Cellphone = cellphone,
                    Email = email,
                    Gender = gender
                };
                newParent.Id = AddModel(newParent);
                Parents.Add(newParent);
                studentParent = newParent;
            }
            return studentParent;
        }

        public static void ImportStudents(ExcelPackage excel)
        {
            ExcelWorksheet ws = excel.Workbook.Worksheets.First();
            int rowCnt = ws.Dimension.End.Row;
            for (int currentRowIndex = 2; currentRowIndex <= rowCnt; currentRowIndex++)
            {
                ExcelRange wsRow = ws.Cells[currentRowIndex, 1, currentRowIndex, ws.Dimension.End.Column];
                Class studentClass = ImportClass(wsRow, currentRowIndex, 5, 6);
                Parent studentMother = ImportParent(wsRow, currentRowIndex, 7, 8, 9, 10,"female");
                Parent studentFather = ImportParent(wsRow, currentRowIndex, 11, 12, 13, 14,"male");
                Student newStudent = new Student()
                {
                    FirstName = (wsRow[currentRowIndex, 1].Value ?? string.Empty).ToString(),
                    LastName = (wsRow[currentRowIndex, 2].Value ?? string.Empty).ToString(),
                    HomePhone = (wsRow[currentRowIndex, 3].Value ?? string.Empty).ToString(),
                    Settlement = (wsRow[currentRowIndex, 4].Value ?? string.Empty).ToString(),
                    Class = studentClass,
                    Mother = studentMother,
                    Father = studentFather,
                    Year = CurrentYear,
                    PicturePath = "",
                };
                AddModel(newStudent);
            }
        }

        public static Year GetCurrentYear()
        {
            int yearId = Convert.ToInt32(GetPreference("current_year_id"));
            return GetRow<Year>("years", yearId);
        }

        public static string GetPreference(string preferenceName)
        {
            DBConnection db = DBConnection.Instance;
            DataTable table = db.GetDataTableByQuery("select value from preferences where name='" + preferenceName + "'");
            return table.Rows[0][0].ToString();
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

                            if (currentRow[currentColumn.Key] == DBNull.Value)
                            {
                                property.SetValue(model, null);
                                continue;
                            }

                            string TableName = subModel.GetType().GetProperty("TableName").GetValue(subModel).ToString();
                            subModel = (Model)
                            typeof(BusinessLayer).GetMethod("GetRow")
                                .MakeGenericMethod(property.PropertyType)
                                .Invoke(null, new object[] { TableName, currentRow[currentColumn.Key] });


                            property.SetValue(model, Convert.ChangeType(subModel, property.PropertyType));
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
                classScheduleLessons["Day" + currentClassSchedule.Day.Id + "$Hour" +
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