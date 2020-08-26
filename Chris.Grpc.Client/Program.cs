using System;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using GrpcServer.Web.Protos;

namespace Chris.Grpc.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var metadata = new Metadata
            {
                {"username", "dave"},
                {"role", "administrator"}
            };

            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new EmployeeService.EmployeeServiceClient(channel);

            var response = await client.GetByNoAsync(new GetByNoRequest
            {
                No = 1994
            }, metadata);

            Console.WriteLine($"Response messages: {response}");
            Console.WriteLine("Press any key to exist.");
            Console.ReadKey();
        }
    }
}
