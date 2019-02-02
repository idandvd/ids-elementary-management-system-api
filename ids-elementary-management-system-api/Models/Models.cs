using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ids_elementary_management_system_api.Models
{
    public class Class
    {
        public int Id { get; set; }
        public int GradeId { get; set; }
        public int Number { get; set; }
        public int YearId { get; set; }
    }
    public class ClassSchedule
    {
        public int Id { get; set; }
        public int DayId { get; set; }
        public int HourId { get; set; }
        public int LessonId { get; set; }
        public int ClassId { get; set; }
    }
    public class Day
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class Grade
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
    }
    public class HourInDay
    {
        public int Id { get; set; }
        public int HourOfSchoolDay { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan FinishTime { get; set; }
        public int YearId { get; set; }
    }
    public class LessonType
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class Lesson
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TeacherId { get; set; }
        public int LessonTypeId { get; set; }
        public int Priority { get; set; }
        public int HasEvaluation { get; set; }
        public int HasGrade { get; set; }
        public string Description { get; set; }
    }
    public class LessonEvaluation
    {
        public int Id { get; set; }
        public string Evaluation { get; set; }
        public int LessonId { get; set; }
        public int StudentId { get; set; }
    }
    public class LessonGrade
    {
        public int Id { get; set; }
        public float Grade { get; set; }
        public int LessonId { get; set; }
        public int StudentId { get; set; }
    }
    public class Parent
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Cellphone { get; set; }
        public string Email { get; set; }
    }
    public class Preference
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
    public class ReceivedSms
    {
        public int Id { get; set; }
        public string ReceivedText { get; set; }
        public int StudentScheduleId { get; set; }
        public int ParentId { get; set; }
    }
    public class SentSms
    {
        public int Id { get; set; }
        public string SentText { get; set; }
        public int ConfirmationNumber { get; set; }
        public int StudentScheduleId { get; set; }
        public int ParentId { get; set; }
    }
    public class Student
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? MotherId { get; set; }
        public int? FatherId { get; set; }
        public int ClassId { get; set; }
        public int YearId { get; set; }
        public string PicturePath { get; set; }
        public string HomePhone { get; set; }
        public string Settlement { get; set; }
    }
    public class StudentSchedule
    {
        public int Id { get; set; }
        public int DayId { get; set; }
        public int HourId { get; set; }
        public int LessonId { get; set; }
        public int StudentId { get; set; }
    }
    public class TeacherClassAccess
    {
        public int Id { get; set; }
        public int TeacherId { get; set; }
        public int ClassId { get; set; }
    }
    public class TeacherType
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class Teacher
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int TeacherTypeId { get; set; }
        public int UserId { get; set; }
        public int YearId { get; set; }
    }
    public class UserType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AuthLevel { get; set; }
    }
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
    public class Year
    {
        public int Id { get; set; }
        public int Name { get; set; }
        public string HebrewYear { get; set; }
    }
    public class TableInformation
    {
        public string TableName { get; set; }
    }
    public class Controller
    {
        public string Name { get; set; }
    }
}