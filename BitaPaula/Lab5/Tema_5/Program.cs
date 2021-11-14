using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Models;

namespace Tema_5
{
    class Program
    {
        private static CloudTableClient tableClient;
        private static CloudTable studentsTable;
        private static CloudTable metricsTable;
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");
            Task.Run(async () => { await Initialize(); })
                .GetAwaiter()
                .GetResult();
        }

        static async Task Initialize()
        {
            string connectionString = "DefaultEndpointsProtocol=https;"
                   + "AccountName=bitapaulastorage;"
                   + "AccountKey=O5gK3me4vSxjUhtjibQ5uw2BQQh9kDzJl9G6peh0Cp8jxv/kU+N5IULvo6QDe2bM07jIxS8n3UK5rpr2+YpoAw==;"
                   + "EndpointSuffix=core.windows.net";
            
            var account = CloudStorageAccount.Parse(connectionString);
            tableClient = account.CreateCloudTableClient();
            studentsTable = tableClient.GetTableReference("Studenti");
            metricsTable = tableClient.GetTableReference("Metrici");

            await metricsTable.CreateIfNotExistsAsync();

            //var students = await GetAllStudents();
            await AddNewMetric();
            /*foreach (Student s in students)
            {
                Console.WriteLine(s.PartitionKey + " " + s.Name);
            }*/
        }

        private static async Task<List<Student>> GetAllStudents()
        {
            var students = new List<Student>();
            TableQuery<Student> query = new TableQuery<Student>();
            TableContinuationToken token = null;

            do
            {
                TableQuerySegment<Student> resultSegment = await studentsTable.ExecuteQuerySegmentedAsync(query, token);

                token = resultSegment.ContinuationToken;
                students.AddRange(resultSegment.Results);

            }while(token != null);

            return students;
        }


        private static async Task AddNewMetric()
        {
            List<Student> students = await GetAllStudents();
            var DistincKeys = students.Select(x => x.PartitionKey).Distinct();
            Console.WriteLine(DateTime.Now);
            foreach (var item in DistincKeys)
            {
                var NumberOfStudents = students.Count(x=> x.PartitionKey.Equals(item.ToString()));
                Console.WriteLine(item.ToString() + ": "+ NumberOfStudents);
                
                /* insert in table metric */
                var metric = new Metric(item.ToString(), NumberOfStudents.ToString());
                var insertOperation = TableOperation.InsertOrReplace(metric);
                await metricsTable.ExecuteAsync(insertOperation);
            }
            Console.WriteLine("General: "+ students.Count.ToString());
            Console.WriteLine();
            var metricGeneral = new Metric("General", students.Count.ToString());
            var insertGeneral = TableOperation.InsertOrReplace(metricGeneral);
            await metricsTable.ExecuteAsync(insertGeneral);
        }
    }
}
