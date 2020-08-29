using System;
using System.Collections.Generic;
using Chris.Grpc.Server.Protos;
using Google.Protobuf.WellKnownTypes;
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
                //Salary = 2200,
                MonthSalary = new MonthSalary
                {
                    Basic = 5000f,
                    Bonus = 125.5f
                },
                Status = EmloyeeStatus.Normal,
                LastModify = Timestamp.FromDateTime(DateTime.UtcNow)
            },
            new Employee
            {
                Id = 2,
                No = 1999,
                FirstName = "Rachel",
                LastName = "Green",
                //Salary = 2400
                MonthSalary = new MonthSalary
                {
                    Basic = 1000f,
                    Bonus = 0f
                },
                Status = EmloyeeStatus.Retired,
                LastModify = Timestamp.FromDateTime(DateTime.UtcNow)
            },
            new Employee
            {
                Id = 3,
                No = 2004,
                FirstName = "Ross",
                LastName = "Geller",
                //Salary = 3405.5f,
                MonthSalary = new MonthSalary
                {
                    Basic = 8000f,
                    Bonus = 1250.9f
                },
                Status = EmloyeeStatus.Onvacation,
                LastModify = Timestamp.FromDateTime(DateTime.UtcNow)
            }
        };
    }
}
