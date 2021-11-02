using Microsoft.WindowsAzure.Storage.Table;

namespace Models
{
    public class Student : TableEntity
    {
        public Student(string university, string cnp)
        {
            this.PartitionKey = university;
            this.RowKey = cnp;
        }

        public Student () {}

        public string Name { get; set; }
        public string Faculty { get; set; }
        public int Year { get; set; }
        public string PhoneNumber { get; set; }
    }
}