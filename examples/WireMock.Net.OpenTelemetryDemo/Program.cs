// Copyright © WireMock.Net
// OpenTelemetry Tracing Demo for WireMock.Net
// This demo uses the Console Exporter to visualize traces in the terminal.

using OpenTelemetry;
using OpenTelemetry.Trace;
using WireMock.Server;
using WireMock.Settings;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

Console.WriteLine("=== WireMock.Net OpenTelemetry Tracing Demo ===\n");

// Note: Each TracerProvider has its own set of sources/instrumentations it listens to.
// WireMock.Net's internal TracerProvider (when OpenTelemetryOptions.Enabled = true) uses OTLP exporter.
// To see traces in the console, we need to configure our own TracerProvider with the sources we want to capture.
using var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .AddSource("WireMock.Net")           // WireMock-specific traces with mapping info
    .AddAspNetCoreInstrumentation()      // ASP.NET Core HTTP server traces
    .AddHttpClientInstrumentation()      // HTTP client traces (for our test requests)
    .AddConsoleExporter()                // Export traces to console for demo purposes
    .Build();

Console.WriteLine("Console Exporter configured to visualize:");
Console.WriteLine("  - WireMock.Net traces (wiremock.* tags)");
Console.WriteLine("  - ASP.NET Core server traces");
Console.WriteLine("  - HTTP client traces\n");

// Start WireMock server with OpenTelemetry enabled
var server = WireMockServer.Start(new WireMockServerSettings
{
    Port = 0, // Use random available port
    OpenTelemetryOptions = new OpenTelemetryOptions
    {
        Enabled = true,
        ExcludeAdminRequests = true
    }
});

Console.WriteLine($"WireMock server started at: {string.Join(", ", server.Urls)}\n");

// Configure some mock mappings
server
    .Given(Request.Create()
        .WithPath("/api/hello")
        .UsingGet())
    .RespondWith(Response.Create()
        .WithStatusCode(200)
        .WithBody("Hello from WireMock!"));

server
    .Given(Request.Create()
        .WithPath("/api/user/*")
        .UsingGet())
    .RespondWith(Response.Create()
        .WithStatusCode(200)
        .WithHeader("Content-Type", "application/json")
        .WithBody(@"{""name"": ""John Doe"", ""email"": ""john@example.com""}"));

server
    .Given(Request.Create()
        .WithPath("/api/error")
        .UsingGet())
    .RespondWith(Response.Create()
        .WithStatusCode(500)
        .WithBody("Internal Server Error"));

Console.WriteLine("Mock mappings configured:");
Console.WriteLine("  GET /api/hello     -> 200 OK");
Console.WriteLine("  GET /api/user/*    -> 200 OK (JSON)");
Console.WriteLine("  GET /api/error     -> 500 Error");
Console.WriteLine();

// Make some test requests to generate traces
using var httpClient = new HttpClient { BaseAddress = new Uri(server.Urls[0]) };

Console.WriteLine("Making test requests to generate traces...\n");
Console.WriteLine("─────────────────────────────────────────────────────────────────");

// Request 1: Successful request
Console.WriteLine("\n>>> Request 1: GET /api/hello");
var response1 = await httpClient.GetAsync("/api/hello");
Console.WriteLine($"<<< Response: {(int)response1.StatusCode} {response1.StatusCode}");
Console.WriteLine($"    Body: {await response1.Content.ReadAsStringAsync()}");

await Task.Delay(500); // Small delay to let trace export complete

// Request 2: Another successful request with path parameter
Console.WriteLine("\n>>> Request 2: GET /api/user/123");
var response2 = await httpClient.GetAsync("/api/user/123");
Console.WriteLine($"<<< Response: {(int)response2.StatusCode} {response2.StatusCode}");
Console.WriteLine($"    Body: {await response2.Content.ReadAsStringAsync()}");

await Task.Delay(500);

// Request 3: Error response
Console.WriteLine("\n>>> Request 3: GET /api/error");
var response3 = await httpClient.GetAsync("/api/error");
Console.WriteLine($"<<< Response: {(int)response3.StatusCode} {response3.StatusCode}");
Console.WriteLine($"    Body: {await response3.Content.ReadAsStringAsync()}");

await Task.Delay(500);

// Request 4: No matching mapping (404)
Console.WriteLine("\n>>> Request 4: GET /api/notfound");
var response4 = await httpClient.GetAsync("/api/notfound");
Console.WriteLine($"<<< Response: {(int)response4.StatusCode} {response4.StatusCode}");

await Task.Delay(500);

Console.WriteLine("\n─────────────────────────────────────────────────────────────────");
Console.WriteLine("\nTraces above show OpenTelemetry activities from WireMock.Net!");
Console.WriteLine("Look for 'Activity.TraceId', 'Activity.SpanId', and custom tags like:");
Console.WriteLine("  - http.request.method");
Console.WriteLine("  - url.path");
Console.WriteLine("  - http.response.status_code");
Console.WriteLine("  - wiremock.mapping.matched");
Console.WriteLine("  - wiremock.mapping.guid");
Console.WriteLine();

// Cleanup
server.Stop();
Console.WriteLine("WireMock server stopped. Demo complete!");
