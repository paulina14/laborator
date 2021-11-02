using Models;
using System.Threading.Tasks;
using System.Collections.Generic;

public interface IStudentRepo
{
    Task<List<Student>> GetAllStudents();
    Task DeleteStudent(string partitionKey, string rowKey); 
    Task CreateStudent(Student student);
    Task UpdateStudent(Student student);
}