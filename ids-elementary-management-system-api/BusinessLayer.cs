using ids_elementary_management_system_api.Models;
using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;

namespace ids_elementary_management_system_api
{
    public class FromLocalData : Attribute
    {
        public string LocalDataListName { get; set; }
        public string ForeignKey { get; set; }
        public string ColumnName { get; set; }

        public FromLocalData(string localDataListName, string foreignKey, string columnName)
        {
            LocalDataListName = localDataListName;
            ForeignKey = foreignKey;
            ColumnName = columnName;
        }
    }

    public class BusinessLayer
    {
        #region Members

        private static List<ClassSchedule> classSchedules = null;
        private static List<StudentSchedule> studentSchedules = null;
        private static List<Class> classes = null;
        private static List<Grade> grades = null;
        private static List<Parent> parents = null;
        private static List<Teacher> teachers = null;
        private static List<TeacherType> teacherTypes = null;
        private static List<TeacherClassAccess> teacherClassAccesses = null;
        private static List<LessonType> lessonTypes = null;
        private static Year currentYear = null;

        #endregion

        #region Properties

        public static List<ClassSchedule> ClassSchedules
        {
            get
            {
                if (classSchedules == null)
                    classSchedules = GetTable<ClassSchedule>(true).ToList();
                return classSchedules;
            }
            set { classSchedules = null; }

        }
        public static List<StudentSchedule> StudentSchedules
        {
            get
            {
                if (studentSchedules == null)
                    studentSchedules = GetTable<StudentSchedule>(true).ToList();
                return studentSchedules;
            }
            set { studentSchedules = null; }

        }
        public static List<Class> Classes
        {
            get
            {
                if (classes == null)
                    classes = GetTable<Class>(true).ToList();
                return classes;
            }
            set { classes = null; }

        }
        public static List<Grade> Grades
        {
            get
            {
                if (grades == null)
                    grades = GetTable<Grade>(true).ToList();
                return grades;
            }
            set { grades = null; }

        }
        public static List<Parent> Parents
        {
            get
            {
                if (parents == null)
                    parents = GetTable<Parent>(true).ToList();
                return parents;
            }
            set { parents = null; }

        }
        public static List<Teacher> Teachers
        {
            get
            {
                if (teachers == null)
                {
                    teachers = GetTable<Teacher>(true).ToList();
                    FillInnerRelation(teachers.ConvertAll(teacher => (Model)teacher), typeof(Teacher));
                }
                return teachers;
            }
            set { teachers = null; }
        }
        public static List<TeacherType> TeachersTypes
        {
            get
            {
                if (teacherTypes == null)
                {
                    teacherTypes = GetTable<TeacherType>(true).ToList();
                }
                return teacherTypes;
            }
            set { teacherTypes = null; }
        }
        public static List<TeacherClassAccess> TeacherClassAccesses
        {
            get
            {
                if (teacherClassAccesses == null)
                    teacherClassAccesses = GetTable<TeacherClassAccess>(true).ToList();
                return teacherClassAccesses;
            }
            set { teacherClassAccesses = null; }
        }
        public static List<LessonType> LessonTypes
        {
            get
            {
                if (lessonTypes == null)
                    lessonTypes = GetTable<LessonType>(true).ToList();
                return lessonTypes;
            }
            set { lessonTypes = null; }
        }
        public static Year CurrentYear
        {
            get
            {
                if (currentYear == null)
                    currentYear = GetCurrentYear();
                return currentYear;
            }
        }

        #endregion

        #region Public

        #region Getters

        public static T GetRow<T>(int id)
        {
            IEnumerable<T> localDataList = TryGetLocalData<T>();
            if (localDataList != null)
                return localDataList.Cast<Model>().Where(item => item.Id == id).Cast<T>().FirstOrDefault();
            string tableName = GetStringProperty(typeof(T), "TableName");

            DBConnection db = DBConnection.Instance;
            DataTable table = db.GetDataTableByQuery("select * from " + tableName + " where id = " + id);
            if (table == null || table.Rows.Count == 0)
                return default(T);
            T result = DataTableToModel<T>(table).FirstOrDefault();
            return result;
        }

        public static IEnumerable<T> GetTable<T>(bool isFromProperty = false)
        {
            if (!isFromProperty)
            {
                IEnumerable<T> localDataList = TryGetLocalData<T>();
                if (localDataList != null)
                    return localDataList;
            }
            string tableName = GetStringProperty(typeof(T), "TableName");
            DBConnection db = DBConnection.Instance;
            DataTable table = db.GetDataTableByQuery("select * from " + tableName);
            if (table == null)
                return null;
            List<T> result = DataTableToModel<T>(table);
            return result;

        }

        public static Year GetCurrentYear()
        {
            int yearId = Convert.ToInt32(DBConnection.Instance.GetPreference("current_year_id"));
            return GetRow<Year>(yearId);
        }

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

        public static IEnumerable<TableInformation> GetAllTablesInformation()
        {

            DBConnection db = DBConnection.Instance;
            DataTable table = db.GetDataTableByQuery("select * from information_schema.tables where table_schema = 'reshit'");
            if (table == null)
                return null;

            List<TableInformation> result = DataTableToModel<TableInformation>(table);
            return result;
        }

        public static ClassScheduleTable GetClassSchedule(int id)
        {
            Class cls = GetRow<Class>(id);
            IEnumerable<HourInDay> hours = GetTable<HourInDay>();
            IEnumerable<Day> days = GetTable<Day>();
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

        public static List<Class> GetTeacherClass(int teacherId)
        {
            return TeacherClassAccesses.Where(classAccess => classAccess.Teacher.Id == teacherId).
                                        Select(classAccess=> classAccess.Class).ToList();
        }

        public static List<ClassSchedule> Conflicts(int DayId, int HourId,
                                                    int TeacherId, int ClassId)
        {

            return ClassSchedules.Where(clsSched => clsSched.Day.Id == DayId &&
                                        clsSched.Hour.Id == HourId &&
                                        clsSched.Lesson.Teacher.Id == TeacherId &&
                                        clsSched.Class.Id != ClassId
                                        ).ToList();
        }

        #endregion

        public static bool CheckUserExists(string username, string password)
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

        public static Teacher Authenticate(string username, string password)
        {
            DBConnection db = DBConnection.Instance;
            string sql_username = username.Replace("'", "''");
            string sql_password = password.Replace("'", "''");
            string sql_query = "select * from users " +
                    " where username = '" + sql_username + "' " +
                    " and password = '" + sql_password + "'";
            DataTable table = db.GetDataTableByQuery(sql_query);

            if (table == null || table.Rows.Count != 1)
                return null;

            User AuthenticatedUser = DataTableToModel<User>(table)[0];
            Teacher AuthenticatedTeacher = Teachers.FirstOrDefault(teacher => teacher.User.Id == AuthenticatedUser.Id);
            return AuthenticatedTeacher;
        }

        public static TeacherScheduleTable GetTeacherSchedule(int teacherId)
        {
            Teacher teacher = GetRow<Teacher>(teacherId);
            IEnumerable<HourInDay> hours = GetTable<HourInDay>();
            IEnumerable<Day> days = GetTable<Day>();

            List<ClassSchedule> teacherClassSchedule = ClassSchedules.Where(clsSched => clsSched.Lesson.Teacher.Id == teacherId).ToList();
            List<StudentSchedule> teacherStudentSchedule =
                StudentSchedules.Where(studSched => studSched.Lesson.Teacher.Id == teacherId).ToList();

            Dictionary < int, Dictionary<int, string>> teacherScheduleClasses =
                   new Dictionary<int, Dictionary<int, string>>();
            foreach (ClassSchedule currentLesson in teacherClassSchedule)
            {
                int dayId = currentLesson.Day.Id;
                int hourId = currentLesson.Hour.Id;
                if (!teacherScheduleClasses.ContainsKey(dayId))
                    teacherScheduleClasses[dayId] = new Dictionary<int, string>();
                if (!teacherScheduleClasses[dayId].ContainsKey(hourId))
                    teacherScheduleClasses[dayId][hourId] = "";
                else
                    teacherScheduleClasses[dayId][hourId] += Environment.NewLine;

                teacherScheduleClasses[dayId][hourId] += currentLesson.Lesson.Name + " - " +
                                                         currentLesson.Class.Grade.Name + "'" + currentLesson.Class.Number;
            }

            foreach (StudentSchedule currentLesson in teacherStudentSchedule)
            {
                int dayId = currentLesson.Day.Id;
                int hourId = currentLesson.Hour.Id;
                if (!teacherScheduleClasses.ContainsKey(dayId))
                    teacherScheduleClasses[dayId] = new Dictionary<int, string>();
                if (!teacherScheduleClasses[dayId].ContainsKey(hourId))
                    teacherScheduleClasses[dayId][hourId] = "";
                else
                    teacherScheduleClasses[dayId][hourId] += Environment.NewLine;

                teacherScheduleClasses[dayId][hourId] += currentLesson.Lesson.Name ;
            }
            TeacherScheduleTable teacherScheduleTable = new TeacherScheduleTable()
            {
                Teacher = teacher,
                HoursInDay = hours,
                Days = days,
                TeacherScheduleClasses = teacherScheduleClasses
            };
            return teacherScheduleTable;
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
                SetListToUpdate(newItem);
                newItem.Id = newID;
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
                string setString = string.Join(",", columns.Select(col => col.Key + "=" + col.Value).ToArray());

                SetListToUpdate(model);
                return db.UpdateData("update " + model.TableName + " set " + setString + " where id = " + model.Id);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool DeleteModel(Model model)
        {
            Type[] types = GetTypesByNamespace("ids_elementary_management_system_api.Models");
            foreach (Type currentType in types)
            {
                PropertyInfo[] properties = currentType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo property in properties)
                {
                    if (model.GetType() == property.PropertyType)
                    {
                        IList fatherList = (IList)typeof(BusinessLayer).GetMethod("GetTable")
                            .MakeGenericMethod(currentType).Invoke(null, new object[] { false });
                        foreach (Model currentFather in fatherList)
                        {
                            if (((Model)property.GetValue(currentFather)).Id == model.Id)
                                DeleteModel(currentFather);
                        }
                    }
                }
            }
            SetListToUpdate(model);
            return DBConnection.Instance.Delete(model.TableName, model.Id); ;
        }

        public static Type[] GetTypesByNamespace(string nameSpace)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] types = assembly.GetTypes().Where(t => string.Equals(t.Namespace, nameSpace, StringComparison.Ordinal)).ToArray();
            return types;
        }


        private static void SetListToUpdate(Model model)
        {
            if (model.ListName != null && model.ListName != "")
            {
                PropertyInfo piLocalDataList = typeof(BusinessLayer).GetProperty(model.ListName);
                piLocalDataList.SetValue(null, null);
            }
        }

        public static bool SaveClassSchedule(ClassScheduleTable classScheduleTable)
        {
            string query = "delete from classes_schedules where class_id = " + classScheduleTable.Class.Id + ";";
            query += "insert into classes_schedules(day_id,hour_id,lesson_id,class_id) values( ";

            foreach (KeyValuePair<string, Lesson> classSchedule in classScheduleTable.ClassSchedules)
            {
                if (classSchedule.Value.Id != 0)
                {
                    string dayId = classSchedule.Key.Substring(3, 1);
                    string hourId = classSchedule.Key.Substring(9);

                    query += dayId + "," + hourId + "," + classSchedule.Value.Id + "," + classScheduleTable.Class.Id + "),(";

                }
                else
                {
                    Console.WriteLine("test");
                }
            }
            query = query.Substring(0, query.Length - 2);
            ClassSchedules = null;
            return DBConnection.Instance.RunQuery(query);
        }

        public static bool SaveTeacherClassAccesses(Teacher teacher)
        {
            string query = "delete from teacher_class_access where teacher_id = " + teacher.Id + "; ";
            if (teacher.ClassAccess.Count > 0)
            {
                query += "insert into teacher_class_access(teacher_id,class_id) values(";
                foreach (Class classAccess in teacher.ClassAccess)
                {
                    query += teacher.Id + "," + classAccess.Id + "),(";

                }
                query = query.Substring(0, query.Length - 2);
                query += ";";
            }
            bool result = DBConnection.Instance.RunQuery(query);
            if (result)
            {
                TeacherClassAccesses = null;
            }
            return result;

        }

        #region Imports

        public static void ImportStudents(ExcelPackage excel)
        {
            ExcelWorksheet ws = excel.Workbook.Worksheets.First();
            int rowCnt = ws.Dimension.End.Row;
            for (int currentRowIndex = 2; currentRowIndex <= rowCnt; currentRowIndex++)
            {
                ExcelRange wsRow = ws.Cells[currentRowIndex, 1, currentRowIndex, ws.Dimension.End.Column];
                Class studentClass = ImportClass(wsRow, currentRowIndex, 5, 6);
                Parent studentMother = ImportParent(wsRow, currentRowIndex, 7, 8, 9, 10, "female");
                Parent studentFather = ImportParent(wsRow, currentRowIndex, 11, 12, 13, 14, "male");
                Student newStudent = new Student()
                {
                    FirstName = (wsRow[currentRowIndex, 1].Value ?? string.Empty).ToString().Trim(),
                    LastName = (wsRow[currentRowIndex, 2].Value ?? string.Empty).ToString().Trim(),
                    HomePhone = (wsRow[currentRowIndex, 3].Value ?? string.Empty).ToString().Trim(),
                    Settlement = (wsRow[currentRowIndex, 4].Value ?? string.Empty).ToString().Trim(),
                    Class = studentClass,
                    Mother = studentMother,
                    Father = studentFather,
                    Year = CurrentYear,
                    PicturePath = "",
                };
                AddModel(newStudent);
            }
        }

        public static void ImportTeachers(ExcelPackage excel)
        {
            ExcelWorksheet ws = excel.Workbook.Worksheets.First();
            int rowCnt = ws.Dimension.End.Row;
            ImportTeacherTypes(ws, 5);
            ImportClasses(ws, 6, 7);
            int newUserNumber = Convert.ToInt32(DBConnection.Instance.GetPreference("current_new_user_number"));
            UserType userType = GetRow<UserType>(2);
            for (int currentRowIndex = 2; currentRowIndex <= rowCnt; currentRowIndex++)
            {
                ExcelRange wsRow = ws.Cells[currentRowIndex, 1, currentRowIndex, ws.Dimension.End.Column];
                User newUser = new User()
                {
                    Username = "user" + newUserNumber,
                    Password = "user" + newUserNumber,
                    UserType = userType
                };
                newUserNumber++;
                string teacherTypeName = (wsRow[currentRowIndex, 5].Value ?? string.Empty).ToString();

                Teacher newTeacher = new Teacher()
                {
                    FirstName = (wsRow[currentRowIndex, 2].Value ?? string.Empty).ToString().Trim(),
                    LastName = (wsRow[currentRowIndex, 3].Value ?? string.Empty).ToString().Trim(),
                    Cellphone = (wsRow[currentRowIndex, 4].Value ?? string.Empty).ToString().Trim(),
                    Year = CurrentYear,
                    TeacherType = TeachersTypes.FirstOrDefault(teacherType => teacherType.Name == teacherTypeName),
                    User = newUser

                };

                int newTeacherId = AddModel(newTeacher);
                if (newTeacherId != -1)
                {
                    AddClassAccessesToNewTeacher(wsRow, currentRowIndex, newTeacher, 6, 7, teacherTypeName);
                }
            }
            DBConnection.Instance.SetPreference("current_new_user_number", newUserNumber.ToString());
        }

        public static void ImportLessons(ExcelPackage excel)
        {
            ExcelWorksheet ws = excel.Workbook.Worksheets.First();
            int rowCnt = ws.Dimension.End.Row;
            for (int currentRowIndex = 2; currentRowIndex <= rowCnt; currentRowIndex++)
            {
                ExcelRange wsRow = ws.Cells[currentRowIndex, 1, currentRowIndex, ws.Dimension.End.Column];
                string teacherName = (wsRow[currentRowIndex, 2].Value ?? string.Empty).ToString().Trim();
                string lessonTypeName = (wsRow[currentRowIndex, 6].Value ?? string.Empty).ToString().Trim();
                Lesson newLesson = new Lesson()
                {
                    Name = (wsRow[currentRowIndex, 1].Value ?? string.Empty).ToString().Trim(),
                    Teacher = Teachers.FirstOrDefault(teacher => (teacher.FirstName + " " + teacher.LastName).Equals(teacherName)),
                    HasEvaluation = (wsRow[currentRowIndex, 3].Value ?? string.Empty).ToString() == "כן",
                    HasGrade = (wsRow[currentRowIndex, 4].Value ?? string.Empty).ToString() == "כן",
                    Description = (wsRow[currentRowIndex, 5].Value ?? string.Empty).ToString().Trim(),
                    LessonType = LessonTypes.FirstOrDefault(lessonType => lessonType.Name.Equals(lessonTypeName))
                };
                int a = AddModel(newLesson);
            }
        }

        #endregion

        #endregion

        #region Private

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

                            subModel = (Model)
                            typeof(BusinessLayer).GetMethod("GetRow")
                                .MakeGenericMethod(property.PropertyType)
                                .Invoke(null, new object[] { currentRow[currentColumn.Key] });


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


        private static IEnumerable<T> TryGetLocalData<T>()
        {
            string listName = GetStringProperty(typeof(T), "ListName");
            if (listName != null && listName != string.Empty)
            {
                PropertyInfo piLocalDataList = typeof(BusinessLayer).GetProperty(listName);
                List<T> localDataList = (List<T>)piLocalDataList.GetValue(null);
                if (localDataList != null)
                    return localDataList;
            }
            return null;
        }

        private static string GetStringProperty(Type type, string PropertyName)
        {
            object model = Activator.CreateInstance(type);
            PropertyInfo piListName = type.GetProperty(PropertyName, BindingFlags.Public | BindingFlags.Instance);
            string value = piListName?.GetValue(model)?.ToString();
            return value;
        }


        private static Dictionary<string, string> GetColumns(Model model)
        {
            PropertyInfo[] properies = model.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            Dictionary<string, string> result = new Dictionary<string, string>();
            for (int currentPropertyIndex = 0; currentPropertyIndex < properies.Length; currentPropertyIndex++)
            {
                PropertyInfo currentProperty = properies[currentPropertyIndex];
                if (currentProperty.Name == "Id" || currentProperty.Name == "TableName" ||
                    currentProperty.Name == "ListName") continue;
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
                    if (subclass == null)
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
                else if (currentProperty.CustomAttributes.Count() > 0 &&
                    currentProperty.CustomAttributes.FirstOrDefault().AttributeType == typeof(FromLocalData))
                {
                    continue;
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

        private static void FillInnerRelation(List<Model> items, Type type)
        {

            List<PropertyInfo> fromLocalDataPropertyList =
                  type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(prop => Attribute.IsDefined(prop, typeof(FromLocalData))).ToList();

            foreach (Model model in items)
            {
                foreach (PropertyInfo piFromLocalData in fromLocalDataPropertyList)
                {
                    string localDataListName = piFromLocalData.CustomAttributes.First().ConstructorArguments[0].Value.ToString();
                    string foreignKey = piFromLocalData.CustomAttributes.First().ConstructorArguments[1].Value.ToString();
                    string columnName = piFromLocalData.CustomAttributes.First().ConstructorArguments[2].Value.ToString();
                    PropertyInfo piLocalDataList = typeof(BusinessLayer).GetProperty(localDataListName);
                    IList iLocalDataList = piLocalDataList.GetValue(null) as IList;

                    //if(columnName =="All")
                    //{
                    //    property.SetValue(model, listProperty.GetValue(property));  
                    //    continue;
                    //}

                    PropertyInfo piColumn = piLocalDataList.PropertyType.GetGenericArguments()[0].GetProperty(columnName);
                    PropertyInfo piForeignKey = piLocalDataList.PropertyType.GetGenericArguments()[0].GetProperty(foreignKey);

                    Type listOfType = piFromLocalData.PropertyType.GetGenericArguments().Single();
                    Type listType = typeof(List<>).MakeGenericType(listOfType);
                    IList newList = Activator.CreateInstance(listType) as IList;
                    foreach (var listItem in iLocalDataList)
                    {
                        Model foreignKeyData = (Model)piForeignKey.GetValue(listItem);
                        if (foreignKeyData.Id == model.Id)
                        {
                            object columnData = piColumn.GetValue(listItem);
                            newList.Add(columnData);
                        }
                    }

                    piFromLocalData.SetValue(model, newList);

                }
            }
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
            if (modelName.EndsWith("Id") && modelName.Length > 2)
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


        private static void AddClassAccessesToNewTeacher(ExcelRange wsRow,
                                                         int currentRowIndex,
                                                         Teacher newTeacher,
                                                         int gradeNameColumnIndex,
                                                         int classNumberColumnIndex,
                                                         string teacherTypeName)
        {
            string gradeName = (wsRow[currentRowIndex, gradeNameColumnIndex].Value ?? string.Empty).ToString();
            if (gradeName != string.Empty)
            {
                string classNumber = (wsRow[currentRowIndex, classNumberColumnIndex].Value ?? string.Empty).ToString();
                if (classNumber != string.Empty)
                {
                    TeacherClassAccess newTeacherClassAccess = new TeacherClassAccess
                    {
                        Teacher = newTeacher,
                        Class = Classes.FirstOrDefault(cls => cls.Number == Convert.ToInt32(classNumber) && cls.Grade.Name == gradeName)
                    };
                    AddModel(newTeacherClassAccess);
                }
                else
                {
                    List<Class> fullGrade = Classes.Where(cls => cls.Grade.Name == gradeName).ToList();
                    foreach (Class currentClass in fullGrade)
                    {
                        TeacherClassAccess newTeacherClassAccess = new TeacherClassAccess
                        {
                            Teacher = newTeacher,
                            Class = currentClass
                        };
                        AddModel(newTeacherClassAccess);
                    }
                }
            }
            else if (teacherTypeName.Contains("מנהל"))
            {
                foreach (Class currentClass in Classes)
                {
                    TeacherClassAccess newTeacherClassAccess = new TeacherClassAccess
                    {
                        Teacher = newTeacher,
                        Class = currentClass
                    };
                    AddModel(newTeacherClassAccess);
                }
            }
        }

        #region Imports


        private static Class ImportClass(ExcelRange wsRow,
                                int currentRowIndex,
                                int gradeNameIndex,
                                int classNumberIndex)
        {
            string gradeName = wsRow[currentRowIndex, gradeNameIndex].Value.ToString().Trim();
            int classNumber = Convert.ToInt32(wsRow[currentRowIndex, classNumberIndex].Value.ToString().Trim());
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
                //Classes.Add(newClass);
                studentClass = newClass;
            }
            return studentClass;
        }

        private static Parent ImportParent(ExcelRange wsRow,
                                          int currentRowIndex,
                                          int firstNameIndex,
                                          int lastNameIndex,
                                          int cellphoneIndex,
                                          int emailIndex,
                                          string gender)
        {
            string firstName = (wsRow[currentRowIndex, firstNameIndex].Value ?? string.Empty).ToString().Trim();
            string lastName = (wsRow[currentRowIndex, lastNameIndex].Value ?? string.Empty).ToString().Trim();
            string cellphone = (wsRow[currentRowIndex, cellphoneIndex].Value ?? string.Empty).ToString().Trim();
            string email = (wsRow[currentRowIndex, emailIndex].Value ?? string.Empty).ToString().Trim();
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


        private static void ImportTeacherTypes(ExcelWorksheet ws, int teacherTypeColumnIndex)
        {
            int rowCnt = ws.Dimension.End.Row;
            List<string> newTeacherTypeNames = new List<string>();
            for (int currentRowIndex = 2; currentRowIndex <= rowCnt; currentRowIndex++)
            {
                ExcelRange wsRow = ws.Cells[currentRowIndex, teacherTypeColumnIndex, currentRowIndex, teacherTypeColumnIndex];
                newTeacherTypeNames.Add((wsRow[currentRowIndex, teacherTypeColumnIndex].Value ?? string.Empty).ToString().Trim());

            }
            newTeacherTypeNames = newTeacherTypeNames.Distinct().ToList();
            foreach (string newTeacherTypeName in newTeacherTypeNames)
            {
                TeacherType newTeacherType = new TeacherType()
                {
                    Name = newTeacherTypeName
                };
                AddModel(newTeacherType);
            }
        }

        private static void ImportClasses(ExcelWorksheet ws, int gradeNameColumnIndex, int classNumberColumnIndex)
        {
            int rowCnt = ws.Dimension.End.Row;
            List<Class> newClasses = new List<Class>();
            for (int currentRowIndex = 2; currentRowIndex <= rowCnt; currentRowIndex++)
            {
                ExcelRange wsRow = ws.Cells[currentRowIndex, gradeNameColumnIndex, currentRowIndex, gradeNameColumnIndex];
                string newGradeName = (wsRow[currentRowIndex, gradeNameColumnIndex].Value ?? string.Empty).ToString().Trim();
                wsRow = ws.Cells[currentRowIndex, classNumberColumnIndex, currentRowIndex, classNumberColumnIndex];
                string newClassNumberString = (wsRow[currentRowIndex, classNumberColumnIndex].Value ?? string.Empty).ToString().Trim();
                if (newGradeName != string.Empty && newClassNumberString != string.Empty)
                {
                    int newClassNumber = Convert.ToInt32(newClassNumberString);

                    newClasses.Add(new Class()
                    {
                        Number = newClassNumber,
                        Grade = Grades.FirstOrDefault(grade => grade.Name == newGradeName),
                        Year = CurrentYear
                    });
                }
            }
            newClasses = newClasses.Distinct().ToList();
            foreach (var newClass in newClasses)
            {
                AddModel(newClass);
            }
        }

        #endregion

        #endregion

        #region not used
        //private static void UpdateRealtions(Model model)
        //{
        //    PropertyInfo[] properies = model.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        //    properies = properies.Where(property => property.CustomAttributes.Count() > 0).ToArray();
        //    foreach (PropertyInfo currentProperty in properies)
        //    {
        //        CustomAttributeData attribute = currentProperty.CustomAttributes.FirstOrDefault();
        //        if (attribute.AttributeType == typeof(FromLocalData))
        //        {
        //            string localDataListName = attribute.ConstructorArguments[0].Value.ToString();
        //            string foreignKey = attribute.ConstructorArguments[1].Value.ToString();
        //            string columnName = attribute.ConstructorArguments[2].Value.ToString();
        //            PropertyInfo piLocalDataList = typeof(BusinessLayer).GetProperty(localDataListName);
        //            IList iLocalDataList = piLocalDataList.GetValue(null) as IList;
        //            piLocalDataList.PropertyType.GetGenericArguments()[0].GetType();
        //            Model relatedModel = (Model)Activator.CreateInstance(piLocalDataList.PropertyType.GetGenericArguments()[0]);
        //            PropertyInfo piForeignKey = piLocalDataList.PropertyType.GetGenericArguments()[0].GetProperty(foreignKey);
        //            string foreignKeyIdColumnName = GetColumnName(foreignKey) + "_id";
        //            string deleteQuery = "delete from " + relatedModel.TableName + " where " + foreignKeyIdColumnName + " = " + model.Id;
        //            string insertQuery = "insert into " + relatedModel.TableName + ""



        //        }
        //    }
        //}
        #endregion
    }
}