using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models;

namespace Tema_4.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StudentsController : ControllerBase
    {
        private IStudentRepo _studentRepo;

        public StudentsController(IStudentRepo studentRepo)
        {
            _studentRepo = studentRepo;
        }
        
        [HttpGet]
        public async Task<IEnumerable<Student>> Get()
        {
            return await _studentRepo.GetAllStudents();
        }

        [HttpPost]
        public async Task Post([FromBody] Student student)
        {
            try{
                await _studentRepo.CreateStudent(student);
            }
            catch (System.Exception){
                throw;
            }
        }

        [HttpPut]
        public async Task Put([FromBody] Student student)
        {
            try{
                await _studentRepo.UpdateStudent(student);
            }
            catch (System.Exception){
                throw;
            }
        }

        [HttpDelete("{rowKey}")]
        public async Task Delete(string partitionKey, string rowKey)
        {
            try{
                await _studentRepo.DeleteStudent(partitionKey, rowKey);
            }
            catch (System.Exception){
                throw;
            }
        }
    }
}