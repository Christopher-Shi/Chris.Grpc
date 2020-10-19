using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Chris.Grpc.Server.Protos;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using GrpcServer.Web.Protos;
using Newtonsoft.Json;
using Serilog;

namespace Chris.Grpc.Client
{
    class Program
    {
        private static string _token;
        private static DateTime _expiration = DateTime.MinValue;

        static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();

            Log.Information("Client starting...");

            using var channel = GrpcChannel.ForAddress("https://localhost:5001",
                new GrpcChannelOptions
                {
                    LoggerFactory = new SerilogLoggerFactory()
                });
            var client = new EmployeeService.EmployeeServiceClient(channel);

            //var option = int.Parse(args[0]);
            var option = 5;
            switch (option)
            {
                case 1:
                    await GetByNoAsync(client);
                    break;
                case 2:
                    await GetAllAsync(client);
                    break;
                case 3:
                    await AddPhotoAsync(client);
                    break;
                case 4:
                    await SaveAsync(client);
                    break;
                case 5:
                    await SaveAllAsync(client);
                    break;
            }

            Console.WriteLine("Press any key to exist.");
            Console.ReadKey();

            Log.CloseAndFlush();
        }

        private static bool NeedToken() => string.IsNullOrEmpty(_token) || _expiration > DateTime.UtcNow;

        public static async Task GetByNoAsync(EmployeeService.EmployeeServiceClient client)
        {
            try
            {
                if (!NeedToken() || await GetTokenAsync(client))
                {
                    var headers = new Metadata
                    {
                        {"Authorization", $"Bearer {_token}"}
                    };

                    var response = await client.GetByNoAsync(new GetByNoRequest
                    {
                        No = 1994
                    }, headers);

                    Console.WriteLine($"Response messages: {response}");
                }
            }
            catch (RpcException e)
            {
                if (e.StatusCode == StatusCode.DataLoss)
                {
                    Log.Logger.Error($"{JsonConvert.SerializeObject(e.Trailers)}");
                }

                Log.Logger.Error(e.Message);
            }
        }

        private static async Task<bool> GetTokenAsync(EmployeeService.EmployeeServiceClient client)
        {
            var request = new TokenRequest
            {
                Username = "admin",
                Password = "1234"
            };

            var response = await client.CreateTokenAsync(request);

            if (response.Success)
            {
                _token = response.Token;
                _expiration = response.Expiration.ToDateTime();

                return true;
            }

            return false;
        }

        public static async Task GetAllAsync(EmployeeService.EmployeeServiceClient client)
        {
            try
            {
                if (!NeedToken() || await GetTokenAsync(client))
                {
                    var headers = new Metadata
                    {
                        {"Authorization", $"Bearer {_token}"}
                    };

                    using var call = client.GetAll(new GetAllRequest(), headers);
                    var responseStream = call.ResponseStream;
                    while (await responseStream.MoveNext())
                    {
                        Console.WriteLine(responseStream.Current.Employee);
                    }
                }
            }
            catch (RpcException e)
            {
                Log.Logger.Error($"{JsonConvert.SerializeObject(e.Trailers)}");
                Log.Logger.Error(e.Message);
            }
        }

        public static async Task AddPhotoAsync(EmployeeService.EmployeeServiceClient client)
        {
            try
            {
                if (!NeedToken() || await GetTokenAsync(client))
                {
                    var headers = new Metadata
                    {
                        {"Authorization", $"Bearer {_token}"}
                    };

                    var fs = File.OpenRead("logo.jpg");
                    using var call = client.AddPhoto(headers);

                    var stream = call.RequestStream;

                    while (true)
                    {
                        var buffer = new byte[1024];
                        var numberRead = await fs.ReadAsync(buffer, 0, buffer.Length);
                        if (numberRead == 0)
                        {
                            break;
                        }
                        if (numberRead < buffer.Length)
                        {
                            Array.Resize(ref buffer, numberRead);
                        }

                        await stream.WriteAsync(new AddPhotoRequest
                        {
                            Data = ByteString.CopyFrom(buffer)
                        });
                    }

                    await stream.CompleteAsync();

                    var response = await call.ResponseAsync;

                    Console.WriteLine(response.IsOk);
                }
            }
            catch (RpcException e)
            {
                Log.Logger.Error($"{JsonConvert.SerializeObject(e.Trailers)}");
                Log.Logger.Error(e.Message);
            }
        }

        public static async Task SaveAsync(EmployeeService.EmployeeServiceClient client)
        {
            try
            {
                if (!NeedToken() || await GetTokenAsync(client))
                {
                    var headers = new Metadata
                    {
                        {"Authorization", $"Bearer {_token}"}
                    };

                    var response = await client.SaveAsync(new EmployeeRequest
                    {
                        Employee = new Employee
                        {
                            No = 1314011524,
                            FirstName = "Christopher",
                            LastName = "Shi",
                            //Salary = 10000
                            MonthSalary = new MonthSalary
                            {
                                Basic = 4000f,
                                Bonus = 345.5f
                            },
                            Status = EmloyeeStatus.Normal,
                            LastModify = Timestamp.FromDateTime(DateTime.UtcNow)
                        }
                    }, headers);

                    Console.WriteLine($"Response messages: { response }");
                }
            }
            catch (RpcException e)
            {
                Log.Logger.Error($"{JsonConvert.SerializeObject(e.Trailers)}");
                Log.Logger.Error(e.Message);
            }
        }

        public static async Task SaveAllAsync(EmployeeService.EmployeeServiceClient client)
        {
            var employees = new List<Employee>
            {
                new Employee
                {
                    No = 111,
                    FirstName = "Monica",
                    LastName = "Geller",
                    //Salary = 7890.1f
                    MonthSalary = new MonthSalary
                    {
                        Basic = 600f,
                        Bonus = 45.5f
                    },
                    Status = EmloyeeStatus.Resigned,
                    LastModify = Timestamp.FromDateTime(DateTime.UtcNow)
                },
                new Employee
                {
                    No = 222,
                    FirstName = "Joey",
                    LastName = "Tribbiani",
                    //Salary = 500
                    MonthSalary = new MonthSalary
                    {
                        Basic = 560f,
                        Bonus = 155.5f
                    },
                    Status = EmloyeeStatus.Retired,
                    LastModify = Timestamp.FromDateTime(DateTime.UtcNow)
                }
            };

            try
            {
                if (!NeedToken() || await GetTokenAsync(client))
                {
                    var headers = new Metadata
                    {
                        {"Authorization", $"Bearer {_token}"}
                    };

                    using var call = client.SaveAll(headers);
                    var requestStream = call.RequestStream;
                    var responseStream = call.ResponseStream;

                    var responseTask = Task.Run(async () =>
                    {
                        while (await responseStream.MoveNext())
                        {
                            Console.WriteLine($"Saved: {responseStream.Current.Employee}");
                        }
                    });

                    foreach (var employee in employees)
                    {
                        await requestStream.WriteAsync(new EmployeeRequest
                        {
                            Employee = employee
                        });
                    }

                    await requestStream.CompleteAsync();
                    await responseTask;
                }
            }
            catch (RpcException e)
            {
                Log.Logger.Error($"{JsonConvert.SerializeObject(e.Trailers)}");
                Log.Logger.Error(e.Message);
            }
        }
    }
}
