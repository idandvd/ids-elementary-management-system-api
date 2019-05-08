using ids_elementary_management_system_api.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web;
using System.Web.Http;

namespace ids_elementary_management_system_api.Controllers
{
    public class BaseApiController : ApiController
    {
        protected Type modelType = typeof(Model);

        [HttpPost]
        public IHttpActionResult PostItem(JObject itemm)
        {
            Model item = (Model)itemm.ToObject(modelType);

            if (item.Id == 0)
            {
                int newID = BusinessLayer.AddModel(item);
                if (newID == 0)
                    return Conflict();
                else if (newID == -1)
                    return InternalServerError();
                item.Id = newID;
            }
            else
            {
                bool editSucceeded = BusinessLayer.EditModel(item);
                if (!editSucceeded)
                    return InternalServerError();
            }
            return Ok();
        }

        public IHttpActionResult SaveItem(Model item)
        {
            if (item.Id == 0)
            {
                int newID = BusinessLayer.AddModel(item);
                if (newID == 0)
                    return Conflict();
                else if (newID == -1)
                    return InternalServerError();
                item.Id = newID;
            }
            else
            {
                bool editSucceeded = BusinessLayer.EditModel(item);
                if (!editSucceeded)
                    return InternalServerError();
            }
            return Ok();
        }

    }

    public class StudentsController : BaseApiController
    {
        public IHttpActionResult GetStudent(int id)
        {
            Student result = BusinessLayer.GetRow<Student>("Students", id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        public IEnumerable<Student> GetAllStudents()
        {
            return BusinessLayer.GetTable<Student>("Students");
        }

        [HttpPost, Route("api/Students/Import")]
        public IHttpActionResult PostStudent()
        {
            if (HttpContext.Current.Request.Files.Count > 0)
            {
                var file = HttpContext.Current.Request.Files[0];
                using (var excel = new ExcelPackage(file.InputStream))
                {
                    BusinessLayer.ImportStudents(excel);
                }
            }
            return Ok();
        }

        [HttpPost]
        public IHttpActionResult PostStudent(Student item)
        {
            if (item.Id == 0)
            {
                int newID = BusinessLayer.AddModel(item);
                if (newID == 0)
                    return Conflict();
                else if (newID == -1)
                    return InternalServerError();
            }
            else
            {
                bool editSucceeded = BusinessLayer.EditModel(item);
                if (!editSucceeded)
                    return InternalServerError();
            }
            return Ok();
        }
    }

    public class ClassesController : ApiController
    {
        public IHttpActionResult GetClass(int id)
        {
            Class result = BusinessLayer.GetRow<Class>("Classes", id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        public IEnumerable<Class> GetAllClasses()
        {
            return BusinessLayer.GetTable<Class>("Classes");
        }
    }

    public class ClassSchedulesController : ApiController
    {

        public IHttpActionResult GetClassSchedule(int id)
        {
            ClassSchedule result = BusinessLayer.GetRow<ClassSchedule>("Classes_Schedules", id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        public IEnumerable<ClassSchedule> GetAllClassesSchedules()
        {
            return BusinessLayer.GetTable<ClassSchedule>("Classes_Schedules");
        }



    }

    public class DaysController : ApiController
    {
        public IHttpActionResult GetStudent(int id)
        {
            Day result = BusinessLayer.GetRow<Day>("Days", id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        public IEnumerable<Day> GetAllStudents()
        {
            return BusinessLayer.GetTable<Day>("Days");
        }
    }

    public class GradesController : ApiController
    {
        public IHttpActionResult GetGrades(int id)
        {
            Grade result = BusinessLayer.GetRow<Grade>("Grades", id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        public IEnumerable<Grade> GetAllGrades()
        {
            return BusinessLayer.GetTable<Grade>("Grades");
        }
    }

    public class HoursInDayController : ApiController
    {
        public IHttpActionResult GetHoursInDay(int id)
        {
            HourInDay result = BusinessLayer.GetRow<HourInDay>("Hours_In_Day", id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        public IEnumerable<HourInDay> GetAllHoursInDay()
        {
            return BusinessLayer.GetTable<HourInDay>("Hours_In_Day");
        }
    }

    public class LessonTypesController : ApiController
    {
        public IHttpActionResult GetLessonType(int id)
        {
            LessonType result = BusinessLayer.GetRow<LessonType>("Lesson_types", id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        public IEnumerable<LessonType> GetAllLessonTypes()
        {
            return BusinessLayer.GetTable<LessonType>("Lesson_types");
        }
    }

    public class LessonsController : ApiController
    {
        public IHttpActionResult GetLesson(int id)
        {
            Lesson result = BusinessLayer.GetRow<Lesson>("Lessons", id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        public IEnumerable<Lesson> GetAllLessons()
        {
            return BusinessLayer.GetTable<Lesson>("Lessons");
        }

        [HttpPost]
        public IHttpActionResult PostLesson(Lesson item)
        {
            int newID = BusinessLayer.AddModel(item);
            if (newID == 0)
                return Conflict();
            return Ok();
        }

    }

    public class LessonEvaluationsController : ApiController
    {
        public IHttpActionResult GetStudent(int id)
        {
            LessonEvaluation result = BusinessLayer.GetRow<LessonEvaluation>("Lesson_Evaluations", id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        public IEnumerable<LessonEvaluation> GetAllStudents()
        {
            return BusinessLayer.GetTable<LessonEvaluation>("Lesson_Evaluations");
        }
    }

    public class LessonGradesController : ApiController
    {
        public IHttpActionResult GetLessonGrade(int id)
        {
            LessonGrade result = BusinessLayer.GetRow<LessonGrade>("Lesson_Grades", id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        public IEnumerable<LessonGrade> GetAllLessonGrades()
        {
            return BusinessLayer.GetTable<LessonGrade>("Lesson_Grades");
        }
    }

    public class ParentsController : ApiController
    {
        public IHttpActionResult GetParent(int id)
        {
            Parent result = BusinessLayer.GetRow<Parent>("Parents", id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        public IEnumerable<Parent> GetAllParents()
        {
            return BusinessLayer.GetTable<Parent>("Parents");
        }
    }

    public class PreferencesController : ApiController
    {
        public IHttpActionResult GetPreference(int id)
        {
            Preference result = BusinessLayer.GetRow<Preference>("Preferences", id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        public IEnumerable<Preference> GetAllPreferences()
        {
            return BusinessLayer.GetTable<Preference>("Preferences");
        }
    }

    public class ReceivedSmsController : ApiController
    {
        public IHttpActionResult GetReceivedSms(int id)
        {
            ReceivedSms result = BusinessLayer.GetRow<ReceivedSms>("Received_Sms", id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        public IEnumerable<ReceivedSms> GetAllReceivedSms()
        {
            return BusinessLayer.GetTable<ReceivedSms>("Received_Sms");
        }
    }

    public class SentSmsController : ApiController
    {
        public IHttpActionResult GetSentSms(int id)
        {
            SentSms result = BusinessLayer.GetRow<SentSms>("Sent_Sms", id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        public IEnumerable<SentSms> GetAllSentSms()
        {
            return BusinessLayer.GetTable<SentSms>("Sent_Sms");
        }
    }

    public class StudentsSchedulesController : ApiController
    {
        public IHttpActionResult GetStudentSchedule(int id)
        {
            StudentSchedule result = BusinessLayer.GetRow<StudentSchedule>("Students_Schedules", id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        public IEnumerable<StudentSchedule> GetAllStudentsSchedules()
        {
            return BusinessLayer.GetTable<StudentSchedule>("Students_Schedules");
        }
    }

    public class TeacherClassAccessController : ApiController
    {
        public IHttpActionResult GetTeacherClassAccess(int id)
        {
            TeacherClassAccess result = BusinessLayer.GetRow<TeacherClassAccess>("Teacher_Class_Access", id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        public IEnumerable<TeacherClassAccess> GetAllTeacherClassAccess()
        {
            return BusinessLayer.GetTable<TeacherClassAccess>("Teacher_Class_Access");
        }

        [HttpPost]
        public IHttpActionResult PostTeacherClassAccesses(List<TeacherClassAccess> teacherClassAccesses)
        {
            return Ok();
        }
    }

    public class TeacherTypesController : ApiController
    {
        public IHttpActionResult GetTeacherType(int id)
        {
            TeacherType result = BusinessLayer.GetRow<TeacherType>("Teacher_Types", id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        public IEnumerable<TeacherType> GetAllTeacherTypes()
        {
            return BusinessLayer.GetTable<TeacherType>("Teacher_Types");
        }

        [HttpPost]
        public IHttpActionResult PostTeacherType(TeacherType item)
        {

            int newID = BusinessLayer.AddModel(item);
            if (newID == 0)
                return Conflict();
            return Ok();
        }

        //public IHttpActionResult AddTeacherType(string name)
        //{
        //    TeacherType result = BusinessLayer.GetRow<TeacherType>("Teacher_Types", id);
        //    if (result == null)
        //        return NotFound();
        //    return Ok(result);
        //}
    }

    public class TeachersController : BaseApiController
    {
        private TeachersController()
        {
            modelType = typeof(Teacher);
        }

        public IHttpActionResult GetTeacher(int id)
        {
            Teacher result = BusinessLayer.GetRow<Teacher>("Teachers", id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpGet]
        public IEnumerable<Teacher> GetAllTeachers()
        {
            return BusinessLayer.GetTable<Teacher>("Teachers");
        }

        [HttpPost, Route("api/Teachers/Save")]
        public IHttpActionResult PostTeacher(Teacher teacher)
        {
            IHttpActionResult result = SaveItem(teacher);
            if(result.GetType() == typeof(System.Web.Http.Results.OkResult))
            {
                if(!BusinessLayer.SaveTeacherClassAccesses(teacher))
                {
                    return InternalServerError();
                }
            }
            else
            {
                return InternalServerError();
            }
            
            return Ok();
        }


    }

    public class UserTypesController : ApiController
    {
        public IHttpActionResult GetUserType(int id)
        {
            UserType result = BusinessLayer.GetRow<UserType>("User_Types", id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        public IEnumerable<UserType> GetAllUserTypes()
        {
            return BusinessLayer.GetTable<UserType>("User_Types");
        }
    }

    public class UsersController : ApiController
    {
        public IHttpActionResult GetUser(int id)
        {
            User result = BusinessLayer.GetRow<User>("Users", id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        public IEnumerable<User> GetAllUsers()
        {
            return BusinessLayer.GetTable<User>("Users");
        }

        [HttpPost, Route("api/CheckUserExists/{username}/{password}")]
        public IHttpActionResult CheckUserExists(string username, string password)
        {
            bool result = BusinessLayer.CheckUserExists(username, password);
            if (!result)
                return NotFound();
            return Ok();
        }

    }

    public class YearsController : ApiController
    {
        public IHttpActionResult GetYear(int id)
        {
            Year result = BusinessLayer.GetRow<Year>("Years", id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        public IEnumerable<Year> GetAllYears()
        {
            return BusinessLayer.GetTable<Year>("Years");
        }
    }

    public class TablesInformationController : ApiController
    {
        //public IHttpActionResult GetTable(string tableName)
        //{
        //    TableInformation result = BusinessLayer.GetTableInformation(tableName);
        //    if (result == null)
        //        return NotFound();
        //    return Ok(result);
        //}

        public IEnumerable<TableInformation> GetAllTables()
        {
            return BusinessLayer.GetAllTablesInformation();
        }
    }
    public class ControllersController : ApiController
    {

        //public IEnumerable<TableInformation> GetAllControllers2()
        //{
        //    //Type[]  t = GetTypesInNamespace();
        //    return BusinessLayer.GetAllTablesInformation();
        //}

        public IEnumerable<Controller> GetAllControllers()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string nameSpace = "ids_elementary_management_system_api.Controllers";
            Type[] types = assembly.GetTypes()
                                    .Where(t => string.Equals(t.Namespace, nameSpace, StringComparison.Ordinal))
                                    .ToArray();
            List<Controller> controllers = new List<Controller>();
            foreach (Type currentType in types)
            {
                if (!currentType.Name.Contains("<") && !currentType.Name.Contains("ControllersController") &&
                    currentType.GetMethods().Where(m => m.Name.Contains("GetAll")).Count() > 0)
                {
                    controllers.Add(new Controller() { Name = currentType.Name.Replace("Controller", "") });
                }
            }
            return controllers;

        }
    }
    public class ClassScheduleTableController : ApiController
    {
        public IHttpActionResult GetClassSchedule(int id)
        {
            ClassScheduleTable result = BusinessLayer.GetClassSchedule(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        //[HttpPost, Route("api/ClassScheduleTable/Save")]
        [HttpPost]
        public IHttpActionResult PostClassScheduleTable(ClassScheduleTable ClassSchedules)
        {
            BusinessLayer.SaveClassSchedule(ClassSchedules);
            return Ok();

        }
    }
}
