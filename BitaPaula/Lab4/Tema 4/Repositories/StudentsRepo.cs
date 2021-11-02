using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Models;

namespace Repositories
{
    public class StudentsRepo : IStudentRepo
    {
        private CloudTableClient _tableClient;
        private CloudTable _studentsTable;
        private string _connectionString;

        public StudentsRepo(IConfiguration configuration)
        {
            _connectionString = configuration.GetValue(typeof(string), "ConnectionString").ToString();
            Task.Run(async () => { await InitializeTable(); })
                 .GetAwaiter()
                 .GetResult();
        }

        private async Task InitializeTable()
        {
            var account = CloudStorageAccount.Parse(_connectionString);
            _tableClient = account.CreateCloudTableClient();

            _studentsTable = _tableClient.GetTableReference("Studenti");

            await _studentsTable.CreateIfNotExistsAsync();
        }

        public async Task<List<Student>> GetAllStudents()
        {
            var students = new List<Student>();
            TableQuery<Student> query = new TableQuery<Student>();
            TableContinuationToken token = null;

            do
            {
                TableQuerySegment<Student> resultSegment = await _studentsTable.ExecuteQuerySegmentedAsync(query, token);

                token = resultSegment.ContinuationToken;
                students.AddRange(resultSegment.Results);
            }while(token != null);

            return students;
        }

        public async Task<Student> GetStudent(string partitionKey, string rowKey)
        {
            var operation = TableOperation.Retrieve<Student>(partitionKey, rowKey);
            TableResult result = await _studentsTable.ExecuteAsync(operation);
            return (Student)(dynamic)result.Result;
        }

        public async Task CreateStudent(Student student)
        {
            var insertOp = TableOperation.Insert(student);
            await _studentsTable.ExecuteAsync(insertOp);
        }

        public async Task UpdateStudent(Student student)
        {
            var operation = TableOperation.InsertOrReplace(student);
            await _studentsTable.ExecuteAsync(operation);
        }

        public async Task DeleteStudent(string partitionKey, string rowKey)
        {
            Student student = await GetStudent(partitionKey, rowKey);
            var operation = TableOperation.Delete(student);
            await _studentsTable.ExecuteAsync(operation);
        }
        
    }
}