﻿using ids_elementary_management_system_api.Models;
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
using System.Threading;
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
            return Ok(item.Id);
        }


        public IHttpActionResult DeleteItem(int id)
        {
            Model item = Activator.CreateInstance(modelType) as Model;
            item.Id = id;
            BusinessLayer.DeleteModel(item);
            return Ok();
        }

        [NonAction]
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
        private StudentsController()
        {
            modelType = typeof(Student);
        }
        public IHttpActionResult GetStudent(int id)
        {
            Student result = BusinessLayer.GetRow<Student>(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        public IEnumerable<Student> GetAllStudents()
        {
            return BusinessLayer.GetTable<Student>();
        }

        [HttpGet, Route("api/Students/OfClass/{classId}")]
        public IEnumerable<Student> StudentsOfClass(int classId)
        {
            return BusinessLayer.GetTable<Student>().
                Where(student => classId == 0 || student.Class.Id == classId).ToList();
        }
        [HttpGet, Route("api/Students/Schedule/{studentId}")]
        public IHttpActionResult GetTeacherSchedule(int studentId)
        {
            return Ok(BusinessLayer.GetStudentSchedule(studentId));
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


    }

    public class ClassesController : ApiController
    {
        public IHttpActionResult GetClass(int id)
        {
            Class result = BusinessLayer.GetRow<Class>(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        public IEnumerable<Class> GetAllClasses()
        {
            return BusinessLayer.GetTable<Class>();
        }



        [HttpGet, Route("api/Classes/GetClassesByLesson/{lessonId}")]
        public IHttpActionResult GetClassesByLesson(int lessonId)
        {
            
            return Ok(BusinessLayer.GetClassesByLesson(lessonId));
        }
    }

    public class ClassSchedulesController : ApiController
    {

        public IHttpActionResult GetClassSchedule(int id)
        {
            ClassSchedule result = BusinessLayer.GetRow<ClassSchedule>(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        public IEnumerable<ClassSchedule> GetAllClassesSchedules()
        {
            return BusinessLayer.GetTable<ClassSchedule>();
        }

    }

    public class DaysController : ApiController
    {
        public IHttpActionResult GetDay(int id)
        {
            Day result = BusinessLayer.GetRow<Day>(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        public IEnumerable<Day> GetAllDays()
        {
            return BusinessLayer.GetTable<Day>();
        }
    }

    public class GradesController : ApiController
    {
        public IHttpActionResult GetGrades(int id)
        {
            Grade result = BusinessLayer.GetRow<Grade>(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        public IEnumerable<Grade> GetAllGrades()
        {
            return BusinessLayer.GetTable<Grade>();
        }
    }

    public class HoursInDayController : ApiController
    {
        public IHttpActionResult GetHoursInDay(int id)
        {
            HourInDay result = BusinessLayer.GetRow<HourInDay>(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        public IEnumerable<HourInDay> GetAllHoursInDay()
        {
            return BusinessLayer.GetTable<HourInDay>();
        }
    }

    public class LessonTypesController : ApiController
    {
        public IHttpActionResult GetLessonType(int id)
        {
            LessonType result = BusinessLayer.GetRow<LessonType>(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        public IEnumerable<LessonType> GetAllLessonTypes()
        {
            return BusinessLayer.GetTable<LessonType>();
        }
    }

    public class LessonsController : BaseApiController
    {
        private LessonsController()
        {
            modelType = typeof(Lesson);
        }

        public IHttpActionResult GetLesson(int id)
        {
            Lesson result = BusinessLayer.GetRow<Lesson>(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        public IEnumerable<Lesson> GetAllLessons()
        {
            return BusinessLayer.GetTable<Lesson>();
        }

        [HttpPost, Route("api/Lessons/Import")]
        public IHttpActionResult PostLesson()
        {
            if (HttpContext.Current.Request.Files.Count > 0)
            {
                var file = HttpContext.Current.Request.Files[0];
                using (var excel = new ExcelPackage(file.InputStream))
                {
                    BusinessLayer.ImportLessons(excel);
                }
            }
            return Ok();
        }

    }

    public class LessonEvaluationsController : BaseApiController
    {
        public LessonEvaluationsController() { modelType = typeof(LessonEvaluation); }

        public IHttpActionResult GetLessonEvaluation(int id)
        {
            LessonEvaluation result = BusinessLayer.GetRow<LessonEvaluation>(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        public IEnumerable<LessonEvaluation> GetAllLessonEvaluations()
        {
            return BusinessLayer.GetTable<LessonEvaluation>();
        }
    }

    public class LessonGradesController : BaseApiController
    {
        public LessonGradesController() { modelType = typeof(LessonGrade); }

        public IHttpActionResult GetLessonGrade(int id)
        {
            LessonGrade result = BusinessLayer.GetRow<LessonGrade>(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        public IEnumerable<LessonGrade> GetAllLessonGrades()
        {
            return BusinessLayer.GetTable<LessonGrade>();
        }
    }

    public class ParentsController : ApiController
    {
        public IHttpActionResult GetParent(int id)
        {
            Parent result = BusinessLayer.GetRow<Parent>(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        public IEnumerable<Parent> GetAllParents()
        {
            return BusinessLayer.GetTable<Parent>();
        }
    }

    public class PreferencesController : ApiController
    {
        public IHttpActionResult GetPreference(int id)
        {
            Preference result = BusinessLayer.GetRow<Preference>(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        public IEnumerable<Preference> GetAllPreferences()
        {
            return BusinessLayer.GetTable<Preference>();
        }
    }

    public class ReceivedSmsController : ApiController
    {
        public IHttpActionResult GetReceivedSms(int id)
        {
            ReceivedSms result = BusinessLayer.GetRow<ReceivedSms>(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        public IEnumerable<ReceivedSms> GetAllReceivedSms()
        {
            return BusinessLayer.GetTable<ReceivedSms>();
        }
    }

    public class SentSmsController : ApiController
    {
        public IHttpActionResult GetSentSms(int id)
        {
            SentSms result = BusinessLayer.GetRow<SentSms>(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        public IEnumerable<SentSms> GetAllSentSms()
        {
            return BusinessLayer.GetTable<SentSms>();
        }
    }

    public class StudentsSchedulesController : ApiController
    {
        public IHttpActionResult GetStudentSchedule(int id)
        {
            StudentSchedule result = BusinessLayer.GetRow<StudentSchedule>(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        public IEnumerable<StudentSchedule> GetAllStudentsSchedules()
        {
            return BusinessLayer.GetTable<StudentSchedule>();
        }
    }

    public class TeacherClassAccessController : ApiController
    {
        public IHttpActionResult GetTeacherClassAccess(int id)
        {
            TeacherClassAccess result = BusinessLayer.GetRow<TeacherClassAccess>(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        public IEnumerable<TeacherClassAccess> GetAllTeacherClassAccess()
        {
            return BusinessLayer.GetTable<TeacherClassAccess>();
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
            TeacherType result = BusinessLayer.GetRow<TeacherType>(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        public IEnumerable<TeacherType> GetAllTeacherTypes()
        {
            return BusinessLayer.GetTable<TeacherType>();
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
            Teacher result = BusinessLayer.GetRow<Teacher>(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpGet]
        public IEnumerable<Teacher> GetAllTeachers()
        {
            return BusinessLayer.GetTable<Teacher>();
        }

        [HttpGet, Route("api/Teachers/Schedule/{teacherId}")]
        public IHttpActionResult GetTeacherSchedule(int teacherId)
        {
            return Ok(BusinessLayer.GetTeacherSchedule(teacherId));
        }

        [HttpGet, Route("api/Teachers/Class/{teacherId}")]
        public IHttpActionResult GetTeacherClass(int teacherId)
        {
            return Ok(BusinessLayer.GetTeacherClass(teacherId));
        }

        [HttpPost, Route("api/Teachers/Save")]
        public IHttpActionResult PostTeacher(Teacher teacher)
        {
            IHttpActionResult result = SaveItem(teacher);
            if (result.GetType() == typeof(System.Web.Http.Results.OkResult))
            {
                if (!BusinessLayer.SaveTeacherClassAccesses(teacher))
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

        [HttpPost, Route("api/Teachers/Import")]
        public IHttpActionResult PostTeachers()
        {
            if (HttpContext.Current.Request.Files.Count > 0)
            {
                var file = HttpContext.Current.Request.Files[0];
                using (var excel = new ExcelPackage(file.InputStream))
                {
                    BusinessLayer.ImportTeachers(excel);
                }
            }
            return Ok();
        }



    }

    public class UserTypesController : ApiController
    {
        public IHttpActionResult GetUserType(int id)
        {
            UserType result = BusinessLayer.GetRow<UserType>(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        public IEnumerable<UserType> GetAllUserTypes()
        {
            return BusinessLayer.GetTable<UserType>();
        }
    }

    public class UsersController : BaseApiController
    {
        public UsersController() { modelType = typeof(User); }
        public IHttpActionResult GetUser(int id)
        {
            User result = BusinessLayer.GetRow<User>(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        public IEnumerable<User> GetAllUsers()
        {
            return BusinessLayer.GetTable<User>();
        }

        [HttpPost, Route("api/CheckUserExists/{Username}/{Password}")]
        public IHttpActionResult CheckUserExists(string Username, string Password)
        {
            bool result = BusinessLayer.CheckUserExists(Username, Password);
            if (!result)
                return NotFound();
            return Ok();
        }

        [HttpPost, Route("api/Users/authenticate")]
        public IHttpActionResult Authenticate(JObject User)
        {
            string username = User.GetValue("username").ToString();
            string password = User.GetValue("password").ToString();
            Teacher result = BusinessLayer.Authenticate(username, password);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

    }

    public class YearsController : ApiController
    {
        public IHttpActionResult GetYear(int id)
        {
            Year result = BusinessLayer.GetRow<Year>(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        public IEnumerable<Year> GetAllYears()
        {
            return BusinessLayer.GetTable<Year>();
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
            Type[] types = BusinessLayer.GetTypesByNamespace("ids_elementary_management_system_api.Controllers");
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

        [HttpGet, Route("api/ClassScheduleTable/HasConflict/{DayId}/{HourId}/{TeacherId}/{ClassId}")]
        public IHttpActionResult HasConflict(int DayId, int HourId,
                                             int TeacherId, int ClassId)
        {
            List<ClassSchedule> Conflicts = BusinessLayer.Conflicts(DayId, HourId, TeacherId, ClassId);
            return Ok(Conflicts);
        }

    }

    public class GroupsController : ApiController
    {

        [HttpPost]
        public IHttpActionResult SaveGroup(Group group)
        {
            Tuple<int,int,int> result = BusinessLayer.SaveGroup(group);
            if (result.Item1 == 0)
            {
                return InternalServerError();
            }
            return Ok(result);
            
        }

        [HttpGet, Route("api/Groups/GetGroupsByTime/{ClassId}/{DayId}/{HourId}")]
        public IHttpActionResult GetGroupsByTime(int ClassId, int DayId, int HourId)
        {
            List<Group> Groups = BusinessLayer.GetGroups(ClassId, DayId, HourId);
            return Ok(Groups);
        }
        [HttpGet, Route("api/Groups/GetGroupsByClass/{ClassId}")]
        public IHttpActionResult GetGroupsByClass(int ClassId)
        {
            List<Group> Groups = BusinessLayer.GetGroups(ClassId);
            return Ok(Groups);
        }

        public IHttpActionResult DeleteItem(Group group)
        {
            BusinessLayer.DeleteGroup(group);
            return Ok();
        }
    }
}