// Copyright © WireMock.Net

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using WireMock.FluentAssertions;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;
using Xunit;

namespace WireMock.Net.Tests.FluentAssertions;

public class WireMockAssertionsTests : IDisposable
{
    private readonly WireMockServer _server;
    private readonly HttpClient _httpClient;
    private readonly int _portUsed;

    public WireMockAssertionsTests()
    {
        _server = WireMockServer.Start();
        _server.Given(Request.Create().UsingAnyMethod()).RespondWith(Response.Create().WithSuccess());

        _portUsed = _server.Ports.First();

        _httpClient = new HttpClient { BaseAddress = new Uri(_server.Url!) };
    }

    [Fact]
    public async Task HaveReceivedNoCalls_AtAbsoluteUrl_WhenACallWasNotMadeToAbsoluteUrl_Should_BeOK()
    {
        await _httpClient.GetAsync("xxx");

        _server.Should()
            .HaveReceivedNoCalls()
            .AtAbsoluteUrl($"http://localhost:{_portUsed}/anyurl");
    }

    [Fact]
    public async Task HaveReceived0Calls_AtAbsoluteUrl_WhenACallWasNotMadeToAbsoluteUrl_Should_BeOK()
    {
        await _httpClient.GetAsync("xxx");

        _server.Should()
            .HaveReceived(0).Calls()
            .AtAbsoluteUrl($"http://localhost:{_portUsed}/anyurl");
    }

    [Fact]
    public async Task HaveReceived1Calls_AtAbsoluteUrl_WhenACallWasMadeToAbsoluteUrl_Should_BeOK()
    {
        await _httpClient.GetAsync("anyurl");

        _server.Should()
            .HaveReceived(1).Calls()
            .AtAbsoluteUrl($"http://localhost:{_portUsed}/anyurl");
    }

    [Fact]
    public async Task HaveReceived1Calls_AtAbsoluteUrl2_WhenACallWasMadeToAbsoluteUrl_Should_BeOK()
    {
        await _httpClient.GetAsync("anyurl");

        _server.Should()
            .HaveReceived(1).Calls()
            .AtAbsoluteUrl2($"http://localhost:{_portUsed}/anyurl");
    }

    [Fact]
    public async Task HaveReceived1Calls_AtAbsoluteUrlUsingPost_WhenAPostCallWasMadeToAbsoluteUrl_Should_BeOK()
    {
        await _httpClient.PostAsync("anyurl", new StringContent(""));

        _server.Should()
            .HaveReceived(1).Calls()
            .AtAbsoluteUrl($"http://localhost:{_portUsed}/anyurl")
            .And
            .UsingPost();
    }

    [Fact]
    public async Task HaveReceived2Calls_AtAbsoluteUrl_WhenACallWasMadeToAbsoluteUrl_Should_BeOK()
    {
        await _httpClient.GetAsync("anyurl");

        await _httpClient.GetAsync("anyurl");

        _server.Should()
            .HaveReceived(2).Calls()
            .AtAbsoluteUrl($"http://localhost:{_portUsed}/anyurl");
    }

    [Fact]
    public async Task HaveReceivedACall_AtAbsoluteUrl_WhenACallWasMadeToAbsoluteUrl_Should_BeOK()
    {
        await _httpClient.GetAsync("anyurl");

        _server.Should()
            .HaveReceivedACall()
            .AtAbsoluteUrl(new WildcardMatcher($"http://localhost:{_portUsed}/any*"));
    }

    [Fact]
    public async Task HaveReceivedACall_AtAbsoluteUrlWildcardMatcher_WhenACallWasMadeToAbsoluteUrl_Should_BeOK()
    {
        await _httpClient.GetAsync("anyurl");

        _server.Should()
            .HaveReceivedACall()
            .AtAbsoluteUrl($"http://localhost:{_portUsed}/anyurl");
    }

    [Fact]
    public void HaveReceivedACall_AtAbsoluteUrl_Should_ThrowWhenNoCallsWereMade()
    {
        Action act = () => _server.Should()
            .HaveReceivedACall()
            .AtAbsoluteUrl("anyurl");

        act.Should()
            .Throw<Exception>()
            .WithMessage("Expected _server to have been called at address matching the absolute url \"anyurl\", but no calls were made.");
    }

    [Fact]
    public async Task HaveReceivedACall_AtAbsoluteUrl_Should_ThrowWhenNoCallsMatchingTheAbsoluteUrlWereMade()
    {
        await _httpClient.GetAsync("");

        Action act = () => _server.Should()
            .HaveReceivedACall()
            .AtAbsoluteUrl("anyurl");

        act.Should()
            .Throw<Exception>()
            .WithMessage($"Expected _server to have been called at address matching the absolute url \"anyurl\", but didn't find it among the calls to {{\"http://localhost:{_portUsed}/\"}}.");
    }

    [Fact]
    public async Task HaveReceivedNoCalls_AtAbsolutePath_WhenACallWasNotMadeToAbsolutePath_Should_BeOK()
    {
        await _httpClient.GetAsync("xxx");

        _server.Should()
            .HaveReceivedNoCalls()
            .AtAbsolutePath("anypath");
    }

    [Fact]
    public async Task HaveReceived0Calls_AtAbsolutePath_WhenACallWasNotMadeToAbsolutePath_Should_BeOK()
    {
        await _httpClient.GetAsync("xxx");

        _server.Should()
            .HaveReceived(0).Calls()
            .AtAbsolutePath("anypath");
    }

    [Fact]
    public async Task HaveReceived1Calls_AtAbsolutePath_WhenACallWasMadeToAbsolutePath_Should_BeOK()
    {
        await _httpClient.GetAsync("anypath");

        _server.Should()
            .HaveReceived(1).Calls()
            .AtAbsolutePath("/anypath");
    }

    [Fact]
    public async Task HaveReceived1Calls_AtAbsolutePathUsingPost_WhenAPostCallWasMadeToAbsolutePath_Should_BeOK()
    {
        await _httpClient.PostAsync("anypath", new StringContent(""));

        _server.Should()
            .HaveReceived(1).Calls()
            .AtAbsolutePath("/anypath")
            .And
            .UsingPost();
    }

    [Fact]
    public async Task HaveReceived2Calls_AtAbsolutePath_WhenACallWasMadeToAbsolutePath_Should_BeOK()
    {
        await _httpClient.GetAsync("anypath");

        await _httpClient.GetAsync("anypath");

        _server.Should()
            .HaveReceived(2).Calls()
            .AtAbsolutePath("/anypath");
    }

    [Fact]
    public async Task HaveReceivedACall_AtAbsolutePath_WhenACallWasMadeToAbsolutePath_Should_BeOK()
    {
        await _httpClient.GetAsync("anypath");

        _server.Should()
            .HaveReceivedACall()
            .AtAbsolutePath(new WildcardMatcher("/any*"));
    }

    [Fact]
    public async Task HaveReceivedACall_AtAbsolutePathWildcardMatcher_WhenACallWasMadeToAbsolutePath_Should_BeOK()
    {
        await _httpClient.GetAsync("anypath");

        _server.Should()
            .HaveReceivedACall()
            .AtAbsolutePath("/anypath");
    }

    [Fact]
    public void HaveReceivedACall_AtAbsolutePath_Should_ThrowWhenNoCallsWereMade()
    {
        Action act = () => _server.Should()
            .HaveReceivedACall()
            .AtAbsolutePath("anypath");

        act.Should()
            .Throw<Exception>()
            .WithMessage("Expected _server to have been called at address matching the absolute path \"anypath\", but no calls were made.");
    }

    [Fact]
    public async Task HaveReceivedACall_AtAbsolutePath_Should_ThrowWhenNoCallsMatchingTheAbsolutePathWereMade()
    {
        await _httpClient.GetAsync("");

        Action act = () => _server.Should()
            .HaveReceivedACall()
            .AtAbsolutePath("/anypath");

        act.Should()
            .Throw<Exception>()
            .WithMessage($"Expected _server to have been called at address matching the absolute path \"/anypath\", but didn't find it among the calls to {{\"/\"}}.");
    }

    [Fact]
    public async Task HaveReceivedACall_WithHeader_WhenACallWasMadeWithExpectedHeader_Should_BeOK()
    {
        _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer a");
        await _httpClient.GetAsync("");

        _server.Should()
            .HaveReceivedACall()
            .WitHeaderKey("Authorization").Which.Should().StartWith("A");
    }

    [Fact]
    public async Task HaveReceivedACall_WithHeader_WhenACallWasMadeWithExpectedHeaderWithValue_Should_BeOK()
    {
        _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer a");
        await _httpClient.GetAsync("");

        _server.Should()
            .HaveReceivedACall()
            .WithHeader("Authorization", "Bearer a");
    }

    [Fact]
    public async Task HaveReceivedACall_WithHeader_WhenMultipleCallsWereMadeWithExpectedHeaderAmongMultipleHeaderValues_Should_BeOK()
    {
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        await _httpClient.GetAsync("1");

        _httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("EN"));
        await _httpClient.GetAsync("2");

        _server.Should()
            .HaveReceivedACall()
            .WithHeader("Accept", ["application/xml", "application/json"])
            .And
            .WithHeader("Accept-Language", ["EN"]);
    }

    [Fact]
    public async Task HaveReceivedACall_WithHeader_Should_ThrowWhenNoCallsMatchingTheHeaderNameWereMade()
    {
        await _httpClient.GetAsync("");

        Action act = () => _server.Should()
            .HaveReceivedACall()
            .WithHeader("Authorization", "value");

        act.Should()
            .Throw<Exception>()
            .WithMessage("*\"Authorization\"*");
    }

    [Fact]
    public async Task HaveReceivedACall_WithHeader_Should_ThrowWhenNoCallsMatchingTheHeaderValuesWereMade()
    {
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        await _httpClient.GetAsync("");

        Action act = () => _server.Should()
            .HaveReceivedACall()
            .WithHeader("Accept", "missing-value");

        act.Should()
            .Throw<Exception>()
            .WithMessage("Expected _server to have been called with Header \"Accept\" and Values {\"missing-value\"}, but didn't find it among the calls with Header(s)*");
    }

    [Fact]
    public async Task HaveReceivedACall_WithHeader_Should_ThrowWhenNoCallsMatchingTheHeaderWithMultipleValuesWereMade()
    {
        using var httpClient = new HttpClient { BaseAddress = new Uri(_server.Url!) };
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        await httpClient.GetAsync("");

        Action act = () => _server.Should()
            .HaveReceivedACall()
            .WithHeader("Accept", ["missing-value1", "missing-value2"]);

        act.Should()
            .Throw<Exception>()
            .WithMessage("Expected _server to have been called with Header \"Accept\" and Values {\"missing-value1\", \"missing-value2\"}, but didn't find it among the calls with Header(s)*");
    }

    [Fact]
    public async Task HaveReceivedACall_WithHeader_ShouldCheckAllRequests()
    {
        // Arrange
        using var server = WireMockServer.Start();
        using var client1 = server.CreateClient();

        var handler = new HttpClientHandler();
        using var client2 = server.CreateClient(handler);

        // Act 1
        var task1 = client1.SendAsync(new HttpRequestMessage(HttpMethod.Get, "/")
        {
            Headers =
            {
                Authorization = new AuthenticationHeaderValue("Bearer", "invalidToken")
            }
        });

        // Act 2
        var task2 = client2.SendAsync(new HttpRequestMessage(HttpMethod.Get, "/")
        {
            Headers =
            {
                Authorization = new AuthenticationHeaderValue("Bearer", "validToken")
            }
        });

        await Task.WhenAll(task1, task2);

        // Assert
        server.Should()
            .HaveReceivedACall()
            .WithHeader("Authorization", "Bearer invalidToken").And.WithoutHeader("x", "y").And.WithoutHeaderKey("a");

        server.Should().
            HaveReceivedACall()
            .WithHeader("Authorization", "Bearer validToken").And.WithoutHeader("Authorization", "y");
    }

    [Fact]
    public async Task HaveReceivedACall_AtUrl_WhenACallWasMadeToUrl_Should_BeOK()
    {
        await _httpClient.GetAsync("anyurl");

        _server.Should()
            .HaveReceivedACall()
            .AtUrl($"http://localhost:{_portUsed}/anyurl");
    }

    [Fact]
    public async Task HaveReceivedACall_AtUrlWildcardMatcher_WhenACallWasMadeToUrl_Should_BeOK()
    {
        await _httpClient.GetAsync("anyurl");

        _server.Should()
            .HaveReceivedACall()
            .AtUrl(new WildcardMatcher($"http://localhost:{_portUsed}/AN*", true));
    }

    [Fact]
    public void HaveReceivedACall_AtUrl_Should_ThrowWhenNoCallsWereMade()
    {
        Action act = () => _server.Should()
            .HaveReceivedACall()
            .AtUrl("anyurl");

        act.Should()
            .Throw<Exception>()
            .WithMessage("Expected _server to have been called at address matching the url \"anyurl\", but no calls were made.");
    }

    [Fact]
    public async Task HaveReceivedACall_AtUrl_Should_ThrowWhenNoCallsMatchingTheUrlWereMade()
    {
        await _httpClient.GetAsync("");

        Action act = () => _server.Should()
            .HaveReceivedACall()
            .AtUrl("anyurl");

        act.Should()
            .Throw<Exception>()
            .WithMessage($"Expected _server to have been called at address matching the url \"anyurl\", but didn't find it among the calls to {{\"http://localhost:{_portUsed}/\"}}.");
    }

    [Fact]
    public async Task HaveReceivedACall_WithProxyUrl_WhenACallWasMadeWithProxyUrl_Should_BeOK()
    {
        _server.ResetMappings();
        _server.Given(Request.Create().UsingAnyMethod())
            .RespondWith(Response.Create().WithProxy(new ProxyAndRecordSettings { Url = "http://localhost:9999" }));

        await _httpClient.GetAsync("");

        _server.Should()
            .HaveReceivedACall()
            .WithProxyUrl("http://localhost:9999");
    }

    [Fact]
    public void HaveReceivedACall_WithProxyUrl_Should_ThrowWhenNoCallsWereMade()
    {
        _server.ResetMappings();
        _server.Given(Request.Create().UsingAnyMethod())
            .RespondWith(Response.Create().WithProxy(new ProxyAndRecordSettings { Url = "http://localhost:9999" }));

        Action act = () => _server.Should()
            .HaveReceivedACall()
            .WithProxyUrl("anyurl");

        act.Should()
            .Throw<Exception>()
            .WithMessage("Expected _server to have been called with proxy url \"anyurl\", but no calls were made.");
    }

    [Fact]
    public async Task HaveReceivedACall_WithProxyUrl_Should_ThrowWhenNoCallsWithTheProxyUrlWereMade()
    {
        _server.ResetMappings();
        _server.Given(Request.Create().UsingAnyMethod())
            .RespondWith(Response.Create().WithProxy(new ProxyAndRecordSettings { Url = "http://localhost:9999" }));

        await _httpClient.GetAsync("");

        Action act = () => _server.Should()
            .HaveReceivedACall()
            .WithProxyUrl("anyurl");

        act.Should()
            .Throw<Exception>()
            .WithMessage("Expected _server to have been called with proxy url \"anyurl\", but didn't find it among the calls with {\"http://localhost:9999\"}.");
    }

    [Fact]
    public async Task HaveReceivedACall_FromClientIP_whenACallWasMadeFromClientIP_Should_BeOK()
    {
        await _httpClient.GetAsync("");
        var clientIP = _server.LogEntries.Last().RequestMessage.ClientIP;

        _server.Should()
            .HaveReceivedACall()
            .FromClientIP(clientIP);
    }

    [Fact]
    public void HaveReceivedACall_FromClientIP_Should_ThrowWhenNoCallsWereMade()
    {
        Action act = () => _server.Should()
            .HaveReceivedACall()
            .FromClientIP("different-ip");

        act.Should()
            .Throw<Exception>()
            .WithMessage("Expected _server to have been called from client IP \"different-ip\", but no calls were made.");
    }

    [Fact]
    public async Task HaveReceivedACall_FromClientIP_Should_ThrowWhenNoCallsFromClientIPWereMade()
    {
        await _httpClient.GetAsync("");
        var clientIP = _server.LogEntries.Last().RequestMessage.ClientIP;

        Action act = () => _server.Should()
            .HaveReceivedACall()
            .FromClientIP("different-ip");

        act.Should()
            .Throw<Exception>()
            .WithMessage($"Expected _server to have been called from client IP \"different-ip\", but didn't find it among the calls from IP(s) {{\"{clientIP}\"}}.");
    }

    [Fact]
    public async Task HaveReceivedNoCalls_UsingPost_WhenACallWasNotMadeUsingPost_Should_BeOK()
    {
        await _httpClient.GetAsync("anyurl");

        _server.Should()
            .HaveReceivedNoCalls()
            .UsingPost();
    }

    [Fact]
    public async Task HaveReceived2Calls_UsingDelete_WhenACallWasMadeUsingDelete_Should_BeOK()
    {
        var tasks = new[]
        {
            _httpClient.DeleteAsync("anyurl"),
            _httpClient.DeleteAsync("anyurl"),
            _httpClient.GetAsync("anyurl")
        };

        await Task.WhenAll(tasks);

        _server.Should()
            .HaveReceived(2).Calls()
            .UsingDelete();
    }

    [Fact]
    public void HaveReceivedACall_UsingPatch_Should_ThrowWhenNoCallsWereMade()
    {
        Action act = () => _server.Should()
            .HaveReceivedACall()
            .UsingPatch();

        act.Should()
            .Throw<Exception>()
            .WithMessage("Expected _server to have been called using method \"PATCH\", but no calls were made.");
    }

    [Fact]
    public async Task HaveReceivedACall_UsingOptions_Should_ThrowWhenCallsWereNotMadeUsingOptions()
    {
        await _httpClient.PostAsync("anyurl", new StringContent("anycontent"));

        Action act = () => _server.Should()
            .HaveReceivedACall()
            .UsingOptions();

        act.Should()
            .Throw<Exception>()
            .WithMessage("Expected _server to have been called using method \"OPTIONS\", but didn't find it among the methods {\"POST\"}.");
    }

#if !NET452
    [Fact]
    public async Task HaveReceivedACall_UsingConnect_WhenACallWasMadeUsingConnect_Should_BeOK()
    {
        _server.ResetMappings();
        _server.Given(Request.Create().UsingAnyMethod())
            .RespondWith(Response.Create().WithStatusCode(HttpStatusCode.Found));

        _httpClient.DefaultRequestHeaders.Add("Host", new Uri(_server.Urls[0]).Authority);

        await _httpClient.SendAsync(new HttpRequestMessage(new HttpMethod("CONNECT"), "anyurl"));

        _server.Should()
            .HaveReceivedACall()
            .UsingConnect();
    }
#endif

    [Fact]
    public async Task HaveReceivedACall_UsingDelete_WhenACallWasMadeUsingDelete_Should_BeOK()
    {
        await _httpClient.SendAsync(new HttpRequestMessage(new HttpMethod("DELETE"), "anyurl"));

        _server.Should()
            .HaveReceivedACall()
            .UsingDelete();
    }

    [Fact]
    public async Task HaveReceivedACall_UsingGet_WhenACallWasMadeUsingGet_Should_BeOK()
    {
        await _httpClient.SendAsync(new HttpRequestMessage(new HttpMethod("GET"), "anyurl"));

        _server.Should()
            .HaveReceivedACall()
            .UsingGet();
    }

    [Fact]
    public async Task HaveReceivedACall_UsingHead_WhenACallWasMadeUsingHead_Should_BeOK()
    {
        await _httpClient.SendAsync(new HttpRequestMessage(new HttpMethod("HEAD"), "anyurl"));

        _server.Should()
            .HaveReceivedACall()
            .UsingHead();
    }

    [Fact]
    public async Task HaveReceivedACall_UsingOptions_WhenACallWasMadeUsingOptions_Should_BeOK()
    {
        await _httpClient.SendAsync(new HttpRequestMessage(new HttpMethod("OPTIONS"), "anyurl"));

        _server.Should()
            .HaveReceivedACall()
            .UsingOptions();
    }

    [Theory]
    [InlineData("POST")]
    [InlineData("Post")]
    public async Task HaveReceivedACall_UsingPost_WhenACallWasMadeUsingPost_Should_BeOK(string method)
    {
        await _httpClient.SendAsync(new HttpRequestMessage(new HttpMethod(method), "anyurl"));

        _server.Should()
            .HaveReceivedACall()
            .UsingPost();
    }

    [Fact]
    public async Task HaveReceived1Calls_AtAbsoluteUrlUsingPost_ShouldChain()
    {
        // Arrange
        var server = WireMockServer.Start();

        server
            .Given(Request.Create().WithPath("/a").UsingGet())
            .RespondWith(Response.Create().WithBody("A response").WithStatusCode(HttpStatusCode.OK));

        server
            .Given(Request.Create().WithPath("/b").UsingPost())
            .RespondWith(Response.Create().WithBody("B response").WithStatusCode(HttpStatusCode.OK));

        server
            .Given(Request.Create().WithPath("/c").UsingPost())
            .RespondWith(Response.Create().WithBody("C response").WithStatusCode(HttpStatusCode.OK));

        // Act
        var httpClient = new HttpClient();

        var tasks = new[]
        {
            httpClient.GetAsync($"{server.Url}/a"),
            httpClient.PostAsync($"{server.Url}/b", new StringContent("B")),
            httpClient.PostAsync($"{server.Url}/c", new StringContent("C"))
        };

        await Task.WhenAll(tasks);

        // Assert
        server
            .Should()
            .HaveReceived(1)
            .Calls()
            .AtUrl($"{server.Url}/a")
            .And
            .UsingGet();

        server
            .Should()
            .HaveReceived(1)
            .Calls()
            .AtUrl($"{server.Url}/b")
            .And
            .UsingPost();

        server
            .Should()
            .HaveReceived(1)
            .Calls()
            .AtUrl($"{server.Url}/c")
            .And
            .UsingPost();

        server
            .Should()
            .HaveReceived(3)
            .Calls();

        server
            .Should()
            .HaveReceived(1)
            .Calls()
            .UsingGet();

        server
            .Should()
            .HaveReceived(2)
            .Calls()
            .UsingPost();

        server.Stop();
    }

    [Fact]
    public async Task HaveReceivedACall_UsingPatch_WhenACallWasMadeUsingPatch_Should_BeOK()
    {
        await _httpClient.SendAsync(new HttpRequestMessage(new HttpMethod("PATCH"), "anyurl"));

        _server.Should()
            .HaveReceivedACall()
            .UsingPatch();
    }

    [Fact]
    public async Task HaveReceivedACall_UsingPut_WhenACallWasMadeUsingPut_Should_BeOK()
    {
        await _httpClient.SendAsync(new HttpRequestMessage(new HttpMethod("PUT"), "anyurl"));

        _server.Should()
            .HaveReceivedACall()
            .UsingPut();
    }

    [Fact]
    public async Task HaveReceivedACall_UsingTrace_WhenACallWasMadeUsingTrace_Should_BeOK()
    {
        await _httpClient.SendAsync(new HttpRequestMessage(new HttpMethod("TRACE"), "anyurl"));

        _server.Should()
            .HaveReceivedACall()
            .UsingTrace();
    }

    [Fact]
    public async Task HaveReceivedACall_UsingAnyMethod_WhenACallWasMadeUsingGet_Should_BeOK()
    {
        await _httpClient.SendAsync(new HttpRequestMessage(new HttpMethod("GET"), "anyurl"));

        _server.Should()
            .HaveReceivedACall()
            .UsingAnyMethod();
    }

    [Fact]
    public void HaveReceivedNoCalls_UsingAnyMethod_WhenNoCallsWereMade_Should_BeOK()
    {
        _server
            .Should()
            .HaveReceived(0)
            .Calls()
            .UsingAnyMethod();

        _server
            .Should()
            .HaveReceivedNoCalls()
            .UsingAnyMethod();
    }

    [Fact]
    public void HaveReceivedNoCalls_AtUrl_WhenNoCallsWereMade_Should_BeOK()
    {
        _server.Should()
            .HaveReceived(0)
            .Calls()
            .AtUrl(_server.Url ?? string.Empty);

        _server.Should()
            .HaveReceivedNoCalls()
            .AtUrl(_server.Url ?? string.Empty);
    }

    [Fact]
    public async Task HaveReceived1Call_WithBodyAsString()
    {
        // Arrange
        var server = WireMockServer.Start();

        server
            .Given(Request.Create().WithPath("/a").UsingPost().WithBody("x"))
            .RespondWith(Response.Create().WithBody("A response"));

        // Act
        var httpClient = new HttpClient();

        await httpClient.PostAsync($"{server.Url}/a", new StringContent("x"));

        // Assert
        server
            .Should()
            .HaveReceived(1)
            .Calls()
            .WithBody("*")
            .And
            .UsingPost();

        server
            .Should()
            .HaveReceived(1)
            .Calls()
            .WithBody("x")
            .And
            .UsingPost();

        server
            .Should()
            .HaveReceived(0)
            .Calls()
            .WithBody("")
            .And
            .UsingPost();

        server
            .Should()
            .HaveReceived(0)
            .Calls()
            .WithBody("y")
            .And
            .UsingPost();

        server.Stop();
    }

    [Fact]
    public async Task HaveReceived1Call_WithBodyAsJson()
    {
        // Arrange
        var server = WireMockServer.Start();

        server
            .Given(Request.Create().WithPath("/a").UsingPost().WithBodyAsJson(new { x = "y" }))
            .RespondWith(Response.Create().WithBody("A response"));

        // Act
        var httpClient = new HttpClient();

        var requestBody = new
        {
            x = "y"
        };
        await httpClient.PostAsJsonAsync($"{server.Url}/a", requestBody);

        // Assert
        server
            .Should()
            .HaveReceived(1)
            .Calls()
            .WithBodyAsJson(new { x = "y" })
            .And
            .UsingPost();

        server
            .Should()
            .HaveReceived(1)
            .Calls()
            .WithBodyAsJson(@"{ ""x"": ""y"" }")
            .And
            .UsingPost();

        server
            .Should()
            .HaveReceived(0)
            .Calls()
            .WithBodyAsJson(new { x = "?" })
            .And
            .UsingPost();

        server
            .Should()
            .HaveReceived(0)
            .Calls()
            .WithBodyAsJson(@"{ ""x"": 1234 }")
            .And
            .UsingPost();

        server.Stop();
    }

    [Fact]
    public async Task WithBodyAsJson_When_NoMatch_ShouldHaveCorrectErrorMessage()
    {
        // Arrange
        var server = WireMockServer.Start();

        server
            .Given(Request.Create().WithPath("/a").UsingPost())
            .RespondWith(Response.Create().WithBody("A response"));

        // Act
        var httpClient = new HttpClient();

        var requestBody = new
        {
            x = "123"
        };
        await httpClient.PostAsJsonAsync($"{server.Url}/a", requestBody);

        // Assert
        Action act = () => server
            .Should()
            .HaveReceived(1)
            .Calls()
            .WithBodyAsJson(new { x = "y" })
            .And
            .UsingPost();

        act.Should()
            .Throw<Exception>()
            .WithMessage("""Expected wiremockserver to have been called using body "{"x":"y"}", but didn't find it among the body/bodies "{"x":"123"}".""");

        server.Stop();
    }

    [Fact]
    public async Task WithBodyAsString_When_NoMatch_ShouldHaveCorrectErrorMessage()
    {
        // Arrange
        var server = WireMockServer.Start();

        server
            .Given(Request.Create().WithPath("/a").UsingPost())
            .RespondWith(Response.Create().WithBody("A response"));

        // Act
        var httpClient = new HttpClient();

        await httpClient.PostAsync($"{server.Url}/a", new StringContent("123"));

        // Assert
        Action act = () => server
            .Should()
            .HaveReceived(1)
            .Calls()
            .WithBody("abc")
            .And
            .UsingPost();

        act.Should()
            .Throw<Exception>()
            .WithMessage("""Expected wiremockserver to have been called using body "abc", but didn't find it among the body/bodies "123".""");

        server.Stop();
    }

    [Fact]
    public async Task WithBodyAsBytes_When_NoMatch_ShouldHaveCorrectErrorMessage()
    {
        // Arrange
        var server = WireMockServer.Start();

        server
            .Given(Request.Create().WithPath("/a").UsingPost())
            .RespondWith(Response.Create().WithBody("A response"));

        // Act
        var httpClient = new HttpClient();

        await httpClient.PostAsync($"{server.Url}/a", new ByteArrayContent([5]));

        // Assert
        Action act = () => server
            .Should()
            .HaveReceived(1)
            .Calls()
            .WithBodyAsBytes([1])
            .And
            .UsingPost();

        act.Should()
            .Throw<Exception>()
            .WithMessage("""Expected wiremockserver to have been called using body "byte[1] {...}", but didn't find it among the body/bodies "byte[1] {...}".""");

        server.Stop();
    }

    [Fact]
    public async Task HaveReceived1Call_WithBodyAsBytes()
    {
        // Arrange
        var server = WireMockServer.Start();

        server
            .Given(Request.Create().WithPath("/a").UsingPut().WithBody([100]))
            .RespondWith(Response.Create().WithBody("A response"));

        // Act
        var httpClient = new HttpClient();

        await httpClient.PutAsync($"{server.Url}/a", new ByteArrayContent([100]));

        // Assert
        server
            .Should()
            .HaveReceived(1)
            .Calls()
            .WithBodyAsBytes([100])
            .And
            .UsingPut();

        server
            .Should()
            .HaveReceived(0)
            .Calls()
            .WithBodyAsBytes([])
            .And
            .UsingPut();

        server
            .Should()
            .HaveReceived(0)
            .Calls()
            .WithBodyAsBytes([42])
            .And
            .UsingPut();

        server.Stop();
    }

    [Fact]
    public async Task HaveReceived1Call_WithBodyAsString_UsingStringMatcher()
    {
        // Arrange
        var server = WireMockServer.Start();

        server
            .Given(Request.Create().WithPath("/a").UsingPost().WithBody("x"))
            .RespondWith(Response.Create().WithBody("A response"));

        // Act
        var httpClient = new HttpClient();

        await httpClient.PostAsync($"{server.Url}/a", new StringContent("x"));

        // Assert
        server
            .Should()
            .HaveReceived(1)
            .Calls()
            .WithBody(new ExactMatcher("x"))
            .And
            .UsingPost();

        server
            .Should()
            .HaveReceived(0)
            .Calls()
            .WithBody(new ExactMatcher(""))
            .And
            .UsingPost();

        server
            .Should()
            .HaveReceived(0)
            .Calls()
            .WithBody(new ExactMatcher("y"))
            .And
            .UsingPost();

        server.Stop();
    }

    [Fact]
    public async Task HaveReceivedACall_WithHeader_Should_ThrowWhenHttpMethodDoesNotMatch()
    {
        // Arrange
        var server = WireMockServer.Start();

        // Act : HTTP GET
        using var httpClient = new HttpClient();
        await httpClient.GetAsync(server.Url!);

        // Act : HTTP POST
        var request = new HttpRequestMessage(HttpMethod.Post, server.Url!);
        request.Headers.Add("TestHeader", ["Value", "Value2"]);

        await httpClient.SendAsync(request);

        // Assert
        server.Should().HaveReceivedACall().UsingPost().And.WithHeader("TestHeader", ["Value", "Value2"]);

        Action act = () => server.Should().HaveReceivedACall().UsingGet().And.WithHeader("TestHeader", "Value");
        act.Should()
            .Throw<Exception>()
            .WithMessage("Expected server to have been called with Header \"TestHeader\" and Values {\"Value\"}, but didn't find it among the calls with Header(s)*");
    }

    [Fact]
    public async Task HaveReceivedACall_WithHeaderKey_Should_ThrowWhenHttpMethodDoesNotMatch()
    {
        // Arrange
        var server = WireMockServer.Start();

        // Act : HTTP GET
        using var httpClient = new HttpClient();
        await httpClient.GetAsync(server.Url!);

        // Act : HTTP POST
        var request = new HttpRequestMessage(HttpMethod.Post, server.Url!);
        request.Headers.Add("TestHeader", ["Value", "Value2"]);

        await httpClient.SendAsync(request);

        // Assert
        server.Should().HaveReceivedACall().UsingPost().And.WitHeaderKey("TestHeader");

        Action act = () => server.Should().HaveReceivedACall().UsingGet().And.WitHeaderKey("TestHeader");
        act.Should()
            .Throw<Exception>()
            .WithMessage("Expected server to have been called with Header \"TestHeader\", but didn't find it among the calls with Header(s)*");
    }

    public void Dispose()
    {
        _server?.Stop();
        _server?.Dispose();
        _httpClient?.Dispose();
    }
}