// Copyright © WireMock.Net

#if NET8_0_OR_GREATER
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using AwesomeAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WireMock.Net.Xunit;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;

namespace WireMock.Net.Tests.ResponseBuilders;

public sealed class ResponseWithProxyIntegrationTests(ITestOutputHelper output)
{
    private readonly CancellationToken _ct = TestContext.Current.CancellationToken;

    [Fact]
    public async Task Response_UsingTextPlain()
    {
        // Given
        using var server = await TestServer.New().Run();
        var port = server.GetPort();
        output.WriteLine($"Server running on port {port}");

        var settings = new WireMockServerSettings
        {
            Port = 0,
            Logger = new TestOutputHelperWireMockLogger(output)
        };
        using var mockServer = WireMockServer.Start(settings);
        mockServer.Given(Request.Create().WithPath("/zipcode").UsingPatch())
                  .RespondWith(Response.Create().WithProxy($"http://localhost:{port}"));

        using var client = new HttpClient { BaseAddress = new Uri(mockServer.Urls[0]) };
        using var content = new ByteArrayContent("0123"u8.ToArray());
        content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");

        // When
        var response = await client.PatchAsync("/zipcode", content, _ct);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.GetValues("Content-Type").Should().BeEquivalentTo("text/plain; charset=utf-8");
        var result = await response.Content.ReadAsStringAsync(_ct);
        result.Should().Be("0123");
    }

    sealed class Disposable(Action dispose) : IDisposable
    {
        public void Dispose() => dispose();
    }

    sealed class TestServer(WebApplication app) : IDisposable
    {
        private Disposable _disposable = new(() => { });

        public static TestServer New()
        {
            var builder = WebApplication.CreateBuilder();
            builder.WebHost.ConfigureKestrel(opts => opts.ListenAnyIP(0));

            var app = builder.Build();

            app.MapPatch("/zipcode", async (HttpRequest req) =>
            {
                var memory = new MemoryStream();
                await req.Body.CopyToAsync(memory);
                var content = Encoding.UTF8.GetString(memory.ToArray());
                return content;
            });
            return new(app);
        }

        public int GetPort()
            => app.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>()!.Addresses
                  .Select(x => new Uri(x).Port)
                  .First();

        public async ValueTask<TestServer> Run()
        {
            var started = new TaskCompletionSource();
            var host = app.Services.GetRequiredService<IHostApplicationLifetime>();
            host.ApplicationStarted.Register(started.SetResult);
            _ = Task.Run(() => app.RunAsync());
            await started.Task;
            _disposable = new(host.StopApplication);
            return this;
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}
#endif