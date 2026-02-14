// Copyright © WireMock.Net

using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using AwesomeAssertions;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace WireMock.Net.Tests;

public partial class WireMockServerTests
{
    public class DummyClass
    {
        public string? Hi { get; set; }
    }

    [Fact]
    public async Task WireMockServer_WithBodyAsJson_Using_PostAsJsonAsync_And_MultipleJmesPathMatchers_ShouldMatch()
    {
        // Arrange
        var server = WireMockServer.Start();
        server.Given(
            Request.Create()
                .WithPath("/a")
                .WithBody(
                    [
                        new JmesPathMatcher("requestId == '1'"),
                        new JmesPathMatcher("value == 'A'")
                    ],
                    MatchOperator.And
                )
                .UsingPost()
            )
            .RespondWith(Response.Create().WithStatusCode(HttpStatusCode.OK));

        server.Given(
                Request.Create()
                    .WithPath("/a")
                    .WithBody(
                        [
                            new JmesPathMatcher("requestId == '2'"),
                            new JmesPathMatcher("value == 'A'")
                        ],
                        MatchOperator.And
                    )
                    .UsingPost()
            )
            .RespondWith(Response.Create().WithStatusCode(HttpStatusCode.Moved));

        // Act
        var requestUri = new Uri($"http://localhost:{server.Port}/a");

        var json = new { requestId = "1", value = "A" };
        var response = await server.CreateClient().PostAsJsonAsync(requestUri, json);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        server.Stop();
    }

    [Fact]
    public async Task WireMockServer_WithBodyAsJson_Using_PostAsJsonAsync_And_MultipleJmesPathMatchers_ShouldMatch_BestMatching()
    {
        // Arrange
        var server = WireMockServer.Start();
        server.Given(
                Request.Create()
                    .WithPath("/a")
                    .WithBody(
                        [
                            new JmesPathMatcher("extra == 'X'"),
                            new JmesPathMatcher("requestId == '1'"),
                            new JmesPathMatcher("value == 'A'")
                        ],
                        MatchOperator.And
                    )
                    .UsingPost()
            )
            .AtPriority(1) // Higher priority
            .RespondWith(Response.Create().WithStatusCode(HttpStatusCode.OK));

        server.Given(
                Request.Create()
                    .WithPath("/a")
                    .WithBody(
                        [
                            new JmesPathMatcher("requestId == '1'"),
                            new JmesPathMatcher("value == 'A'")
                        ],
                        MatchOperator.And
                    )
                    .UsingPost()
            )
            .AtPriority(2)
            .RespondWith(Response.Create().WithStatusCode(HttpStatusCode.Moved));

        // Act
        var requestUri = new Uri($"http://localhost:{server.Port}/a");

        var json = new { extra = "X", requestId = "1", value = "A" };
        var response = await server.CreateClient().PostAsJsonAsync(requestUri, json);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        server.Stop();
    }

    [Fact]
    public async Task WireMockServer_WithBodyAsJson_Using_PostAsJsonAsync_And_WildcardMatcher_ShouldMatch()
    {
        // Arrange
        var server = WireMockServer.Start();
        server.Given(
                Request.Create().UsingPost().WithPath("/foo").WithBody(new WildcardMatcher("*Hello*"))
            )
            .RespondWith(
                Response.Create().WithStatusCode(200)
            );

        var jsonObject = new DummyClass
        {
            Hi = "Hello World!"
        };

        // Act
        var response = await new HttpClient().PostAsJsonAsync("http://localhost:" + server.Ports[0] + "/foo", jsonObject);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        server.Stop();
    }

    [Fact]
    public async Task WireMockServer_WithBodyAsJson_Using_PostAsync_And_WildcardMatcher_ShouldMatch()
    {
        // Arrange
        var server = WireMockServer.Start();
        server.Given(
                Request.Create().UsingPost().WithPath("/foo").WithBody(new WildcardMatcher("*Hello*"))
            )
            .RespondWith(
                Response.Create().WithStatusCode(200)
            );

        // Act
        var response = await new HttpClient().PostAsync("http://localhost:" + server.Ports[0] + "/foo", new StringContent("{ Hi = \"Hello World\" }"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        server.Stop();
    }

    [Fact]
    public async Task WireMockServer_WithBodyAsJson_Using_PostAsync_And_JsonPartialWildcardMatcher_ShouldMatch()
    {
        // Arrange
        using var server = WireMockServer.Start();

        var matcher = new JsonPartialWildcardMatcher(new { method = "initialize", id = "^[a-f0-9]{32}-[0-9]$" }, ignoreCase: true, regex: true);
        server.Given(Request.Create()
            .WithHeader("Content-Type", "application/json*")
            .UsingPost()
            .WithPath("/foo")
            .WithBody(matcher)
        )
        .RespondWith(Response.Create()
            .WithHeader("Content-Type", "application/json")
            .WithBody("""
                      {"jsonrpc":"2.0","id":"{{request.bodyAsJson.id}}","result":{"protocolVersion":"2024-11-05","capabilities":{"logging":{},"prompts":{"listChanged":true},"resources":{"subscribe":true,"listChanged":true},"tools":{"listChanged":true}},"serverInfo":{"name":"ExampleServer","version":"1.0.0"}}}
                      """)
            .WithStatusCode(200)
            .WithTransformer(true)
        );

        // Act
        var cancelationToken = TestContext.Current.CancellationToken;
        var content = "{\"jsonrpc\":\"2.0\",\"id\":\"ec475f56d4694b48bc737500ba575b35-1\",\"method\":\"initialize\",\"params\":{\"protocolVersion\":\"2024-11-05\",\"capabilities\":{},\"clientInfo\":{\"name\":\"GitHub Test\",\"version\":\"1.0.0\"}}}";
        var response = await new HttpClient()
            .PostAsync($"{server.Url}/foo", new StringContent(content, Encoding.UTF8, "application/json"), cancelationToken)
;

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseText = await response.Content.ReadAsStringAsync(cancelationToken);
        responseText.Should().Contain("ec475f56d4694b48bc737500ba575b35-1");
    }

#if NET6_0_OR_GREATER
    [Fact]
    public async Task WireMockServer_WithBodyAsJson_Using_PostAsync_And_JsonPartialWildcardMatcher_And_SystemTextJson_ShouldMatch()
    {
        // Arrange
        using var server = WireMockServer.Start(x => x.DefaultJsonSerializer = new JsonConverter.System.Text.Json.SystemTextJsonConverter() );

        var matcher = new JsonPartialWildcardMatcher(new { id = "^[a-f0-9]{32}-[0-9]$" }, ignoreCase: true, regex: true);
        server.Given(Request.Create()
            .WithHeader("Content-Type", "application/json*")
            .UsingPost()
            .WithPath("/system-text-json")
            .WithBody(matcher)
        )
        .RespondWith(Response.Create()
            .WithBody("OK")
        );

        // Act
        var content = """{"id":"ec475f56d4694b48bc737500ba575b35-1"}""";
        using var httpClient = new HttpClient();
        var response = await httpClient.PostAsync($"{server.Url}/system-text-json", new StringContent(content, Encoding.UTF8, "application/json"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseText = await response.Content.ReadAsStringAsync();
        responseText.Should().Contain("OK");
    }
#endif

    [Fact]
    public async Task WireMockServer_WithBodyAsFormUrlEncoded_Using_PostAsync_And_WithFunc()
    {
        // Arrange
        var cancelationToken = TestContext.Current.CancellationToken;
        using var server = WireMockServer.Start();
        server.Given(
            Request.Create()
                .UsingPost()
                .WithPath("/foo")
                .WithBody(values => values != null && values["key1"] == "value1")
            )
            .RespondWith(
                Response.Create()
            );

        // Act
        var content = new FormUrlEncodedContent([new KeyValuePair<string, string>("key1", "value1")]);
        var response = await new HttpClient()
            .PostAsync($"{server.Url}/foo", content, cancelationToken)
;

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        server.Stop();
    }

    [Fact]
    public async Task WireMockServer_WithBodyAsFormUrlEncoded_Using_PostAsync_And_WithExactMatcher()
    {
        // Arrange
        var server = WireMockServer.Start();
        server.Given(
                Request.Create()
                    .UsingPost()
                    .WithPath("/foo")
                    .WithHeader("Content-Type", "application/x-www-form-urlencoded")
                    .WithBody(new ExactMatcher("name=John+Doe&email=johndoe%40example.com")
                )
            )
            .RespondWith(
                Response.Create()
            );

        // Act
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("name", "John Doe"),
            new KeyValuePair<string, string>("email", "johndoe@example.com")
        });
        var response = await new HttpClient()
            .PostAsync($"{server.Url}/foo", content)
;

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        server.Stop();
    }

    [Fact]
    public async Task WireMockServer_WithBodyAsFormUrlEncoded_Using_PostAsync_And_WithFormUrlEncodedMatcher()
    {
        // Arrange
        var cancelationToken = TestContext.Current.CancellationToken;
        var matcher = new FormUrlEncodedMatcher(["email=johndoe@example.com", "name=John Doe"]);
        var server = WireMockServer.Start();
        server.Given(
            Request.Create()
                .UsingPost()
                .WithPath("/foo")
                .WithHeader("Content-Type", "application/x-www-form-urlencoded")
                .WithBody(matcher)
            )
            .RespondWith(
                Response.Create()
            );

        server.Given(
            Request.Create()
                .UsingPost()
                .WithPath("/bar")
                .WithHeader("Content-Type", "application/x-www-form-urlencoded")
                .WithBody(matcher)
            )
            .RespondWith(
                Response.Create()
            );

        // Act 1
        var contentOrdered = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("name", "John Doe"),
            new KeyValuePair<string, string>("email", "johndoe@example.com")
        });
        var responseOrdered = await new HttpClient()
            .PostAsync($"{server.Url}/foo", contentOrdered, cancelationToken)
;

        // Assert 1
        responseOrdered.StatusCode.Should().Be(HttpStatusCode.OK);


        // Act 2
        var contentUnordered = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("email", "johndoe@example.com"),
            new KeyValuePair<string, string>("name", "John Doe"),
        });
        var responseUnordered = await new HttpClient()
            .PostAsync($"{server.Url}/bar", contentUnordered, cancelationToken)
;

        // Assert 2
        responseUnordered.StatusCode.Should().Be(HttpStatusCode.OK);

        server.Stop();
    }

    [Fact]
    public async Task WireMockServer_WithSseBody()
    {
        // Arrange
        var server = WireMockServer.Start();
        server
            .WhenRequest(r => r
                .UsingGet()
                .WithPath("/sse")
            )
            .ThenRespondWith(r => r
                .WithHeader("Content-Type", "text/event-stream")
                .WithHeader("Cache-Control", "no-cache")
                .WithHeader("Connection", "keep-alive")
                .WithSseBody(async (_, queue) =>
                {
                    for (var i = 1; i <= 3; i++)
                    {
                        queue.Write($"x {i};\r\n");
                        await Task.Delay(100);
                    }

                    queue.Close();
                })
            );

        server
            .WhenRequest(r => r
                .UsingGet()
            )
            .ThenRespondWith(r => r
                .WithBody("normal")
            );

        using var client = new HttpClient();

        // Act 1
        var normal = await new HttpClient()
            .GetAsync(server.Url)
;
        (await normal.Content.ReadAsStringAsync()).Should().Be("normal");

        // Act 2
        using var response = await client.GetStreamAsync($"{server.Url}/sse");
        using var reader = new StreamReader(response);

        var data = string.Empty;
        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            data += line;
        }

        // Assert 2
        data.Should().Be("x 1;x 2;x 3;");
    }
}