using AspireApp1.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

// IResourceBuilder<ProjectResource> apiService = builder.AddProject<Projects.AspireApp1_ApiService>("apiservice");

var mappingsPath = Path.Combine(Directory.GetCurrentDirectory(), "WireMockMappings");

IResourceBuilder<WireMockServerResource> apiService = builder
    //.AddWireMock("apiservice", WireMockServerArguments.DefaultPort)
    .AddWireMock("apiservice", ["http://*:8080", "grpc://*:9090"])
    .WithMappingsPath(mappingsPath)
    .WithReadStaticMappings()
    .WithWatchStaticMappings()
    .WithApiMappingBuilder(WeatherForecastApiMock.BuildAsync);

//var apiServiceUsedForDocs = builder
//    .AddWireMock("apiservice1", WireMockServerArguments.DefaultPort)
//    .WithApiMappingBuilder(adminApiBuilder =>
//    {
//        var summaries = new[]
//        {
//            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
//        };

//        adminApiBuilder.Given(b => b
//            .WithRequest(request => request
//                .UsingGet()
//                .WithPath("/weatherforecast2")
//            )
//            .WithResponse(response => response
//                .WithHeaders(h => h.Add("Content-Type", "application/json"))
//                .WithBodyAsJson(() => Enumerable.Range(1, 5).Select(index =>
//                        new WeatherForecast
//                        (
//                            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
//                            Random.Shared.Next(-20, 55),
//                            "WireMock.Net : " + summaries[Random.Shared.Next(summaries.Length)]
//                        ))
//                    .ToArray())
//            )
//        );

//        return Task.CompletedTask;
//    });

builder.AddProject<Projects.AspireApp1_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService);

await builder.Build().RunAsync();