using ids_elementary_management_system_api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ids_elementary_management_system_api.Controllers
{
    public class StudentsController : ApiController
    {
        
        public IEnumerable<Student> GetAllStudents()
        {
            return BusinessLayer.GetClassStudents(3);
        }
    }
}
