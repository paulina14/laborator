using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models;
using Repositories;

namespace Lab2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StudentsController : ControllerBase
    {

        [HttpGet]
        public IEnumerable<Student> Get()
        {
            return StudentsRepo.Students;
        }

        [HttpGet("{id}")]
        public Student GetStudent(int id)
        {
            return StudentsRepo.Students.FirstOrDefault(s => s.Id == id);
        }

        [HttpPost]
        public void Post([FromBody] Student student)
        {
            bool exist = false;
            foreach (Student s in StudentsRepo.Students)
            {
                if (student.Id == s.Id)
                exist = true;
            }
            if(!exist)
            {
                if (student.Id != 0 && student.Name.CompareTo("null") != 0 && student.Faculty.CompareTo("null") != 0 && student.Year != 0)
                StudentsRepo.Students.Add(student);
            }  
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            foreach (Student s in StudentsRepo.Students)
            {
                if(s.Id == id)
                {
                    StudentsRepo.Students.Remove(s);
                    break;
                }
            }
        }

        [HttpPut]
        public void Put([FromBody] Student student)
        {
            foreach (Student s in StudentsRepo.Students)
            {
                if (student.Id == s.Id)
                {
                    s.Name = student.Name;
                    s.Faculty = student.Faculty;
                    s.Year = student.Year;
                    break;
                }
            }
        }
    }
}