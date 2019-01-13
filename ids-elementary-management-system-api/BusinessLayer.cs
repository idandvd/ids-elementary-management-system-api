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
                                                    " mother.cellphone , concat(mother.first_name,' ' ,mother.last_name) as mother_name, mother.email," +
                                                    " father.cellphone , concat(father.first_name,' ' ,father.last_name) as father_name, father.email," +
                                                    " students.home_phone, students.settlement" +
                                                    " from students " +
                                                    " inner join classes on classes.id = students.class_id" +
                                                                        " and classes.year_id = (select value from preferences where name = 'current_year_id')" +
                                                    " left join parents mother on mother.id = students.mother_id " +
                                                    " left join parents father on father.id = students.father_id " +
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