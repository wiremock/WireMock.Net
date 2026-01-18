var builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<ProjectResource> apiService = builder.AddProject<Projects.AspireApp1_ApiService>("apiservice");

builder.AddProject<Projects.AspireApp1_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

await builder.Build().RunAsync();