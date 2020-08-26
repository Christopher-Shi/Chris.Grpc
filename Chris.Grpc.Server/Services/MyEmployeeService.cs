using System;
using System.Linq;
using System.Threading.Tasks;
using Chris.Grpc.Server.Data;
using Grpc.Core;
using GrpcServer.Web.Protos;
using Microsoft.Extensions.Logging;

namespace Chris.Grpc.Server.Services
{
    public class MyEmployeeService : EmployeeService.EmployeeServiceBase
    {
        private readonly ILogger<MyEmployeeService> _logger;

        public MyEmployeeService(ILogger<MyEmployeeService> logger)
        {
            _logger = logger;
        }

        public override async Task<EmployeeResponse> GetByNo(GetByNoRequest request, ServerCallContext context)
        {
            var metadata = context.RequestHeaders;
            foreach (var pair in metadata)
            {
                _logger.LogInformation($"{pair.Key}: {pair.Value}");
            }

            var employee = InMemoryData.Employees.SingleOrDefault(x => x.No == request.No);
            if (employee != null)
            {
                var response = new EmployeeResponse
                {
                    Employee = employee
                };

                return await Task.FromResult(response);
            }

            throw new Exception($"Employee not found with no: {request.No}");
        }
    }
}
