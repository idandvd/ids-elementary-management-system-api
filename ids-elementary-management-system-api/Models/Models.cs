using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ids_elementary_management_system_api.Models
{
    public class Model
    {
        public string TableName { get; set; }
        public string ListName { get; set; }
        public int Id { get; set; }


    }

    public class Class : Model
    {
        public Class() { TableName = "classes"; ListName = "Classes"; }
        public Grade Grade { get; set; }
        public int Number { get; set; }
        public Year Year { get; set; }

        public override bool Equals(object obj)
        {
            var item = obj as Class;
            if (item == null)
            {
                return false;
            }
            return Grade.Equals(item.Grade) &&
                   Number.Equals(item.Number) &&
                   Year.Equals(item.Year);
        }
        public override int GetHashCode()
        {
            return Grade.GetHashCode() * 13 * 13 +
                   Number.GetHashCode() * 13 +
                   Year.GetHashCode();
        }
    }
    public class ClassSchedule : Model
    {
        public ClassSchedule() { TableName = "classes_schedules"; }
        public Day Day { get; set; }
        public HourInDay Hour { get; set; }
        public Lesson Lesson { get; set; }
        public Class Class { get; set; }
    }
    public class Day : Model
    {
        public Day() { TableName = "days"; }
        public string Name { get; set; }
    }
    public class Grade : Model
    {
        public Grade() { TableName = "grades"; ListName = "Grades"; }
        public string Name { get; set; }
        public int Number { get; set; }

        public override bool Equals(object obj)
        {
            var item = obj as Grade;
            if (item == null)
            {
                return false;
            }
            return Number.Equals(item.Number);
        }
        public override int GetHashCode()
        {
            return Number.GetHashCode();
        }
    }
    public class HourInDay : Model
    {
        public HourInDay() { TableName = "hours_in_day"; }
        public int HourOfSchoolDay { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan FinishTime { get; set; }
        public bool IsBreak { get; set; }
        public Year Year { get; set; }
    }
    public class LessonType : Model
    {
        public LessonType() { TableName = "lesson_types"; }
        public string Name { get; set; }
    }
    public class Lesson : Model
    {
        public Lesson() { TableName = "lessons"; }
        public string Name { get; set; }
        public Teacher Teacher { get; set; }
        public LessonType LessonType { get; set; }
        public int Priority { get; set; }
        public bool HasEvaluation { get; set; }
        public bool HasGrade { get; set; }
        public string Description { get; set; }
    }
    public class LessonEvaluation : Model
    {
        public LessonEvaluation() { TableName = "lessons_evaluations"; }
        public string Evaluation { get; set; }
        public Lesson Lesson { get; set; }
        public Student Student { get; set; }
    }
    public class LessonGrade : Model
    {
        public LessonGrade() { TableName = "lessons_grades"; }
        public float Grade { get; set; }
        public Lesson Lesson { get; set; }
        public Student Student { get; set; }
    }
    public class Parent : Model
    {
        public Parent() { TableName = "parents"; ListName = "Parents"; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Cellphone { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
    }
    public class Preference : Model
    {
        public Preference() { TableName = "preferences"; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
    public class ReceivedSms : Model
    {
        public ReceivedSms() { TableName = "received_sms"; }
        public string ReceivedText { get; set; }
        public StudentSchedule StudentSchedule { get; set; }
        public Parent Parent { get; set; }
    }
    public class SentSms : Model
    {
        public SentSms() { TableName = "sent_sms"; }
        public string SentText { get; set; }
        public int ConfirmationNumber { get; set; }
        public StudentSchedule StudentSchedule { get; set; }
        public Parent Parent { get; set; }
    }
    public class Student : Model
    {
        public Student() { TableName = "students"; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Parent Mother { get; set; }
        public Parent Father { get; set; }
        public Class Class { get; set; }
        public Year Year { get; set; }
        public string PicturePath { get; set; }
        public string HomePhone { get; set; }
        public string Settlement { get; set; }
    }
    public class StudentSchedule : Model
    {
        public StudentSchedule() { TableName = "students_schedules"; }
        public Day Day { get; set; }
        public HourInDay Hour { get; set; }
        public Lesson Lesson { get; set; }
        public Student Student { get; set; }
    }
    public class TeacherClassAccess : Model
    {
        public TeacherClassAccess() { TableName = "teacher_class_access"; ListName = "TeacherClassAccesses"; }
        public Teacher Teacher { get; set; }
        public Class Class { get; set; }
    }
    public class TeacherType : Model
    {
        public TeacherType() { TableName = "teacher_types"; ListName = "TeachersTypes"; }
        public string Name { get; set; }
    }


    public class Teacher : Model
    {
        public Teacher() { TableName = "teachers"; ListName = "Teachers"; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Cellphone { get; set; }
        public TeacherType TeacherType { get; set; }
        public User User { get; set; }
        public Year Year { get; set; }


        [FromLocalData("TeacherClassAccesses", "Teacher", "Class")]
        public List<Class> ClassAccess { get; set; }
    }
    public class UserType : Model
    {
        public UserType() { TableName = "user_types"; }
        public string Name { get; set; }
        public int AuthLevel { get; set; }
    }
    public class User : Model
    {
        public User() { TableName = "users"; }
        public string Username { get; set; }
        public string Password { get; set; }
        public UserType UserType { get; set; }
    }
    public class Year : Model
    {
        public Year() { TableName = "years"; }
        public int Name { get; set; }
        public string HebrewYear { get; set; }

        public override bool Equals(object obj)
        {
            var item = obj as Year;
            if (item == null)
            {
                return false;
            }
            return Name.Equals(item.Name);
        }
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
    public class TableInformation : Model
    {
        public TableInformation() { TableName = "days"; }
    }
    public class Controller : Model
    {
        public Controller() { TableName = ""; }
        public string Name { get; set; }
    }

    public class ClassScheduleTable : Model
    {
        public Class Class { get; set; }
        public Dictionary<string, Lesson> ClassSchedules { get; set; }
        public IEnumerable<HourInDay> HoursInDay { get; set; }
        public IEnumerable<Day> Days { get; set; }
    }

}