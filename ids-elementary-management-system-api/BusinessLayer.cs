using ids_elementary_management_system_api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace ids_elementary_management_system_api
{
    public class BusinessLayer
    {
        public static IEnumerable<Student> GetClassStudents(int nClassID)
        {
            DBConnection db = DBConnection.Instance;
            DataTable dt = db.GetDataTableByQuery(" select concat(students.first_name,' ' ,students.last_name) as name," +
                                        " picture_path,concat(grades.grade_name,classes.class_number) as class," +
                                        " classes.id as class_id, students.id as student_id," +
                                        " students.mother_cellphone, students.mother_full_name," +
                                        " students.father_cellphone, students.father_full_name," +
                                        " students.home_phone, students.parents_email," +
                                        " students.settlement" +
                                        " from students " +
                                        " inner join students_classes on students_classes.student_id = students.id" +
                                        " inner join classes on classes.id = students_classes.class_id" +
                                        " inner join teachers on teachers.id = classes.teacher_id " +
                                        " and teachers.year_id = (select value from preferences where name = 'current_year_id')" +
                                        " inner join grades on grades.id = classes.grade_id" +
                                        " where classes.id = " + nClassID);
            List<Student> lst = new List<Student>();
            foreach (DataRow item in dt.Rows)
            {
                lst.Add(new Student() { Name = item["name"].ToString() });
            }
            return lst;
        }

    }
}