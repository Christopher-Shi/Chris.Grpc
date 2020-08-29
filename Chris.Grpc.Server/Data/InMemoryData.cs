using System.Collections.Generic;
using GrpcServer.Web.Protos;

namespace Chris.Grpc.Server.Data
{
    public class InMemoryData
    {
        public static List<Employee> Employees = new List<Employee>
        {
            new Employee
            {
                Id = 1,
                No = 1994,
                FirstName = "Chandler",
                LastName = "Bing",
                Salary = 2200
            },
            new Employee
            {
                Id = 2,
                No = 1999,
                FirstName = "Rachel",
                LastName = "Green",
                Salary = 2400
            },
            new Employee
            {
                Id = 3,
                No = 2004,
                FirstName = "Ross",
                LastName = "Geller",
                Salary = 3405.5f
            }
        };
    }
}
