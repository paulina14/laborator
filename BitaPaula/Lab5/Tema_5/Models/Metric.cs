using Microsoft.WindowsAzure.Storage.Table;

namespace Models
{
    public class Metric : TableEntity
    {
        public Metric(string university, string numberOfStudents)
        {
            this.PartitionKey = university;
            this.RowKey = numberOfStudents;
        }
        public Metric () {}
    }
}