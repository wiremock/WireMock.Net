// Copyright © WireMock.Net

using System;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestEase;
using WireMock.Admin.Settings;
using WireMock.Client;
using WireMock.Client.Extensions;

namespace WireMock.Net.Client;

class Program
{
    static async Task Main(string[] args)
    {
        // Start WireMock.Net tool with Admin interface
        // dotnet-wiremock --StartAdminInterface

        // Create an implementation of the IWireMockAdminApi and pass in the base URL for the API.
        var api = RestClient.For<IWireMockAdminApi>("http://localhost:9091");

        await api.ResetMappingsAsync().ConfigureAwait(false);

        var mappingBuilder = api.GetMappingBuilder();
        mappingBuilder.Given(m => m
            .WithTitle("This is my title 1")
            .WithRequest(req => req
                .UsingGet()
                .WithPath("/bla1")
            )
            .WithResponse(rsp => rsp
                .WithBody("x1")
                .WithHeaders(h => h.Add("h1", "v1"))
            )
        );

        mappingBuilder.Given(m => m
            .WithTitle("This is my title 2")
            .WithRequest(req => req
                .UsingGet()
                .WithPath("/bla2")
            )
            .WithResponse(rsp => rsp
                .WithBody("x2")
                .WithHeaders(h => h.Add("h2", "v2"))
            )
        );

        mappingBuilder.Given(m => m
            .WithTitle("This is my title 3")
            .WithRequest(req => req
                .UsingGet()
                .WithPath("/bla3")
            )
            .WithResponse(rsp => rsp
                .WithBodyAsJson(new
                {
                    x = "test"
                }, true)
            )
        );

        mappingBuilder.Given(m => m
            .WithRequest(req => req
                .WithPath("/test1")
                .UsingPost()
                .WithBody(b => b
                    .WithJmesPathMatcher("things.name == 'RequiredThing'")
                )
            )
            .WithResponse(rsp => rsp
                .WithHeaders(h => h.Add("Content-Type", "application/json"))
                .WithDelay(TimeSpan.FromMilliseconds(50))
                .WithStatusCode(200)
                .WithBodyAsJson(new
                {
                    status = "ok"
                }, true)
            )
        );

        mappingBuilder.Given(m => m
            .WithRequest(req => req
                .WithPath("/testWithQueryParams")
                .UsingGet()
                .WithParams(p => p.WithParam("param1", pb => pb.WithExactMatcher("value1")))
            ).WithResponse(rsp => rsp
                .WithHeaders(h => h.Add("Content-Type", "application/json"))
                .WithStatusCode(200)
                .WithBodyAsJson(new
                {
                    status = "ok"
                }, true)
            )
        );

        var result = await mappingBuilder.BuildAndPostAsync().ConfigureAwait(false);
        Console.WriteLine($"result = {JsonConvert.SerializeObject(result)}");

        var mappings = await api.GetMappingsAsync();
        Console.WriteLine($"mappings = {JsonConvert.SerializeObject(mappings)}");

        // Set BASIC Auth
        var value = Convert.ToBase64String(Encoding.ASCII.GetBytes("a:b"));
        api.Authorization = new AuthenticationHeaderValue("Basic", value);

        var settings1 = await api.GetSettingsAsync();
        Console.WriteLine($"settings1 = {JsonConvert.SerializeObject(settings1)}");

        var settingsViaBuilder = new SettingsModelBuilder()
            .WithGlobalProcessingDelay(1077)
            .Build();

        settings1.GlobalProcessingDelay = 1077;
        api.PostSettingsAsync(settings1).Wait();

        var settings2 = await api.GetSettingsAsync();
        Console.WriteLine($"settings2 = {JsonConvert.SerializeObject(settings2)}");

        mappings = await api.GetMappingsAsync();
        Console.WriteLine($"mappings = {JsonConvert.SerializeObject(mappings)}");

        try
        {
            var guid = Guid.Parse("11111110-a633-40e8-a244-5cb80bc0ab66");
            var mapping = await api.GetMappingAsync(guid);
            Console.WriteLine($"mapping = {JsonConvert.SerializeObject(mapping)}");
        }
        catch (Exception e)
        {
        }

        var request = await api.GetRequestsAsync();
        Console.WriteLine($"request = {JsonConvert.SerializeObject(request)}");

        //var deleteRequestsAsync = api.DeleteRequestsAsync().Result;
        //Console.WriteLine($"DeleteRequestsAsync = {deleteRequestsAsync.Status}");

        //var resetRequestsAsync = api.ResetRequestsAsync().Result;
        //Console.WriteLine($"ResetRequestsAsync = {resetRequestsAsync.Status}");

        var scenarioStates = await api.GetScenariosAsync();
        Console.WriteLine($"GetScenariosAsync = {JsonConvert.SerializeObject(scenarioStates)}");

        var postFileResult = await api.PostFileAsync("1.cs", "C# Hello");
        Console.WriteLine($"postFileResult = {JsonConvert.SerializeObject(postFileResult)}");

        var getFileResult = await api.GetFileAsync("1.cs");
        Console.WriteLine($"getFileResult = {getFileResult}");

        Console.WriteLine("Press any key to reset mappings");
        Console.ReadKey();

        var resetMappingsAsync = await api.ResetMappingsAsync();
        Console.WriteLine($"resetMappingsAsync = {resetMappingsAsync.Status}");

        var resetMappingsAndReloadStaticMappingsAsync = await api.ResetMappingsAsync(true);
        Console.WriteLine($"resetMappingsAndReloadStaticMappingsAsync = {resetMappingsAndReloadStaticMappingsAsync.Status}");

        Console.WriteLine("Press any key to quit");
        Console.ReadKey();
    }
}