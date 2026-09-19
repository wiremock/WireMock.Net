// Copyright © WireMock.Net

using WireMock.Logging;
using WireMock.Net.StandAlone;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Settings;

namespace WireMock.Net.Console.RequestLogTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var server = StandAloneApp.Start(new WireMockServerSettings
            {
                Port = 19019,
                StartAdminInterface = true,
                StartTimeout = 1000,
                MaxRequestLogCount = 100,
                RequestLogExpirationDuration = 6,
                Logger = new WireMockConsoleLogger()
            });

            server
                .Given(Request
                    .Create()
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithHeader("Content-Type", "application/json")
                    .WithBodyAsJson(new { result = "x" }));

            await Task.Delay(2000);

            var client = new HttpClient();
            client.BaseAddress = new Uri(server.Urls[0]);

            var options = new ParallelOptions()
            {
                MaxDegreeOfParallelism = 2
            };

            var list = Enumerable.Range(1, 1000);
            Parallel.ForEach(list, options, async i =>
            {
                string result = await client.GetStringAsync("/x");
                System.Console.WriteLine(result);
            });

            //for (int i = 0; i < 1000; i++)
            //{
            //    string result = await client.GetStringAsync("/x");
            //    System.Console.WriteLine(result);
            //}

            System.Console.WriteLine("Press any key to stop the server");
            System.Console.ReadKey();
            server.Stop();
        }
    }
}