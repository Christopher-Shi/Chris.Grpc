using System;
using System.Collections.Generic;
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
            try
            {
                // TODO: 示例代码 模拟抛出异常
                if (true)
                {
                    var trailers = new Metadata
                    {
                        {"field", "No"},
                        {"Message", "something went wrong..."}
                    };
                    //throw new RpcException(Status.DefaultCancelled, trailers);
                    throw new RpcException(new Status(StatusCode.DataLoss,"Data is lost..."), trailers);
                }

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
            catch (RpcException re)
            {
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new RpcException(Status.DefaultCancelled, e.Message);
            }
        }

        public override async Task
            GetAll(GetAllRequest request, IServerStreamWriter<EmployeeResponse> responseStream, ServerCallContext context)
        {
            try
            {
                foreach (var employee in InMemoryData.Employees)
                {
                    await responseStream.WriteAsync(new EmployeeResponse
                    {
                        Employee = employee
                    });
                }
            }
            catch (RpcException re)
            {
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new RpcException(Status.DefaultCancelled, e.Message);
            }
        }

        public override async Task<AddPhotoResponse>
            AddPhoto(IAsyncStreamReader<AddPhotoRequest> requestStream, ServerCallContext context)
        {
            var metadata = context.RequestHeaders;

            foreach (var pair in metadata)
            {
                Console.WriteLine($"{pair.Key}: {pair.Value}");
            }

            var data = new List<byte>();
            while (await requestStream.MoveNext())
            {
                Console.WriteLine($"Received: {requestStream.Current.Data.Length} bytes");
                data.AddRange(requestStream.Current.Data);
            }

            Console.WriteLine($"Received file with {data.Count} bytes");

            return new AddPhotoResponse
            {
                IsOk = true
            };
        }

        public override async Task<EmployeeResponse> Save(EmployeeRequest request, ServerCallContext context)
        {
            try
            {
                var metadata = context.RequestHeaders;
                foreach (var pair in metadata)
                {
                    _logger.LogInformation($"{pair.Key}: {pair.Value}");
                }

                InMemoryData.Employees.Add(request.Employee);

                var response = new EmployeeResponse
                {
                    Employee = InMemoryData.Employees.SingleOrDefault(x => x.No == request.Employee.No)
                };

                Console.WriteLine("Employees:");
                foreach (var employee in InMemoryData.Employees)
                {
                    Console.WriteLine(employee);
                }

                return await Task.FromResult(response);
            }
            catch (RpcException re)
            {
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new RpcException(Status.DefaultCancelled, e.Message);
            }
        }

        public override async Task
            SaveAll(IAsyncStreamReader<EmployeeRequest> requestStream,
                IServerStreamWriter<EmployeeResponse> responseStream,
                ServerCallContext context)
        {
            try
            {
                while (await requestStream.MoveNext())
                {
                    var employee = requestStream.Current.Employee;
                    lock (this)
                    {
                        InMemoryData.Employees.Add(employee);
                    }

                    await responseStream.WriteAsync(new EmployeeResponse
                    {
                        Employee = employee
                    });
                }

                Console.WriteLine("Employees:");
                foreach (var employee in InMemoryData.Employees)
                {
                    Console.WriteLine(employee);
                }
            }
            catch (RpcException re)
            {
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new RpcException(Status.DefaultCancelled, e.Message);
            }
        }
    }
}
