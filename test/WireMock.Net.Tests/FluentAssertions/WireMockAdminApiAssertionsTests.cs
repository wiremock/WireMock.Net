// Copyright © WireMock.Net

using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using AwesomeAssertions;
using RestEase;
using WireMock.Client;
using WireMock.Client.AwesomeAssertions;
using WireMock.Matchers;
using WireMock.Net.Xunit;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;

namespace WireMock.Net.Tests.FluentAssertions;

public class WireMockAdminApiAssertionsTests : IDisposable
{
    private readonly CancellationToken _ct = TestContext.Current.CancellationToken;

    private readonly WireMockServer _server;
    private readonly HttpClient _httpClient;
    private readonly IWireMockAdminApi _adminApi;
    private readonly int _portUsed;
    private readonly ITestOutputHelper _testOutputHelper;

    public WireMockAdminApiAssertionsTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;

        _server = WireMockServer.Start(settings =>
        {
            settings.StartAdminInterface = true;
            settings.Logger = new TestOutputHelperWireMockLogger(testOutputHelper);
        });
        _server.Given(Request.Create().UsingAnyMethod()).RespondWith(Response.Create().WithSuccess());

        _portUsed = _server.Ports.First();
        _httpClient = _server.CreateClient();
        _adminApi = RestClient.For<IWireMockAdminApi>(_server.Url);
    }

    [Fact]
    public async Task HaveReceivedNoCalls_AtAbsoluteUrl_WhenACallWasNotMadeToAbsoluteUrl_Should_BeOK()
    {
        await _httpClient.GetAsync("xxx", _ct);

        _adminApi.Should()
            .HaveReceivedNoCalls()
            .AtAbsoluteUrl($"http://localhost:{_portUsed}/anyurl");
    }

    [Fact]
    public async Task HaveReceived0Calls_AtAbsoluteUrl_WhenACallWasNotMadeToAbsoluteUrl_Should_BeOK()
    {
        await _httpClient.GetAsync("xxx", _ct);

        _adminApi.Should()
            .HaveReceived(0).Calls()
            .AtAbsoluteUrl($"http://localhost:{_portUsed}/anyurl");
    }

    [Fact]
    public async Task HaveReceived1Call_AtAbsoluteUrl_WhenACallWasMadeToAbsoluteUrl_Should_BeOK()
    {
        await _httpClient.GetAsync("anyurl", _ct);

        _adminApi.Should()
            .HaveReceived(1).Calls()
            .AtAbsoluteUrl($"http://localhost:{_portUsed}/anyurl");
    }

    [Fact]
    public async Task HaveReceived1Call_AtAbsoluteUrl2_WhenACallWasMadeToAbsoluteUrl_Should_BeOK()
    {
        await _httpClient.GetAsync("anyurl", _ct);

        _adminApi.Should()
            .HaveReceived(1).Calls()
            .AtAbsoluteUrl($"http://localhost:{_portUsed}/anyurl");
    }

    [Fact]
    public async Task HaveReceived1Call_AtAbsoluteUrlUsingPost_WhenAPostCallWasMadeToAbsoluteUrl_Should_BeOK()
    {
        await _httpClient.PostAsync("anyurl", new StringContent(""), _ct);

        _adminApi.Should()
            .HaveReceived(1).Calls()
            .AtAbsoluteUrl($"http://localhost:{_portUsed}/anyurl")
            .And
            .UsingPost();
    }

    [Fact]
    public async Task HaveReceived2Calls_AtAbsoluteUrl_WhenACallWasMadeToAbsoluteUrl_Should_BeOK()
    {
        await _httpClient.GetAsync("anyurl", _ct);

        await _httpClient.GetAsync("anyurl", _ct);

        _adminApi.Should()
            .HaveReceived(2).Calls()
            .AtAbsoluteUrl($"http://localhost:{_portUsed}/anyurl");
    }

    [Fact]
    public async Task HaveReceivedACall_AtAbsoluteUrl_WhenACallWasMadeToAbsoluteUrl_Should_BeOK()
    {
        await _httpClient.GetAsync("anyurl", _ct);

        _adminApi.Should()
            .HaveReceivedACall()
            .AtAbsoluteUrl(new WildcardMatcher($"http://localhost:{_portUsed}/any*"));
    }

    [Fact]
    public async Task HaveReceivedACall_AtAbsoluteUrlWildcardMatcher_WhenACallWasMadeToAbsoluteUrl_Should_BeOK()
    {
        await _httpClient.GetAsync("anyurl", _ct);

        _adminApi.Should()
            .HaveReceivedACall()
            .AtAbsoluteUrl($"http://localhost:{_portUsed}/anyurl");
    }

    [Fact]
    public void HaveReceivedACall_AtAbsoluteUrl_Should_ThrowWhenNoCallsWereMade()
    {
        Action act = () => _adminApi.Should()
            .HaveReceivedACall()
            .AtAbsoluteUrl("anyurl");

        act.Should()
            .Throw<Exception>()
            .WithMessage("Expected _adminApi to have been called at address matching the absolute url \"anyurl\", but no calls were made.");
    }

    [Fact]
    public async Task HaveReceivedACall_AtAbsoluteUrl_Should_ThrowWhenNoCallsMatchingTheAbsoluteUrlWereMade()
    {
        await _httpClient.GetAsync("", _ct);

        Action act = () => _adminApi.Should()
            .HaveReceivedACall()
            .AtAbsoluteUrl("anyurl");

        act.Should()
            .Throw<Exception>()
            .WithMessage($"Expected _adminApi to have been called at address matching the absolute url \"anyurl\", but didn't find it among the calls to {{\"http://localhost:{_portUsed}/\"}}.");
    }

    [Fact]
    public async Task HaveReceivedNoCalls_AtAbsolutePath_WhenACallWasNotMadeToAbsolutePath_Should_BeOK()
    {
        await _httpClient.GetAsync("xxx", _ct);

        _adminApi.Should()
            .HaveReceivedNoCalls()
            .AtAbsolutePath("anypath");
    }

    [Fact]
    public async Task HaveReceived0Calls_AtAbsolutePath_WhenACallWasNotMadeToAbsolutePath_Should_BeOK()
    {
        await _httpClient.GetAsync("xxx", _ct);

        _adminApi.Should()
            .HaveReceived(0).Calls()
            .AtAbsolutePath("anypath");
    }

    [Fact]
    public async Task HaveReceived1Call_AtAbsolutePath_WhenACallWasMadeToAbsolutePath_Should_BeOK()
    {
        await _httpClient.GetAsync("anypath", _ct);

        _adminApi.Should()
            .HaveReceived(1).Calls()
            .AtAbsolutePath("/anypath");
    }

    [Fact]
    public async Task HaveReceived1Call_AtAbsolutePathUsingPost_WhenAPostCallWasMadeToAbsolutePath_Should_BeOK()
    {
        await _httpClient.PostAsync("anypath", new StringContent(""), _ct);

        _adminApi.Should()
            .HaveReceived(1).Calls()
            .AtAbsolutePath("/anypath")
            .And
            .UsingPost();
    }

    [Fact]
    public async Task HaveReceived2Calls_AtAbsolutePath_WhenACallWasMadeToAbsolutePath_Should_BeOK()
    {
        await _httpClient.GetAsync("anypath", _ct);

        await _httpClient.GetAsync("anypath", _ct);

        _adminApi.Should()
            .HaveReceived(2).Calls()
            .AtAbsolutePath("/anypath");
    }

    [Fact]
    public async Task HaveReceivedACall_AtAbsolutePath_WhenACallWasMadeToAbsolutePath_Should_BeOK()
    {
        await _httpClient.GetAsync("anypath", _ct);

        _adminApi.Should()
            .HaveReceivedACall()
            .AtAbsolutePath(new WildcardMatcher("/any*"));
    }

    [Fact]
    public async Task HaveReceivedACall_AtAbsolutePathWildcardMatcher_WhenACallWasMadeToAbsolutePath_Should_BeOK()
    {
        await _httpClient.GetAsync("anypath", _ct);

        _adminApi.Should()
            .HaveReceivedACall()
            .AtAbsolutePath("/anypath");
    }

    [Fact]
    public void HaveReceivedACall_AtAbsolutePath_Should_ThrowWhenNoCallsWereMade()
    {
        Action act = () => _adminApi.Should()
            .HaveReceivedACall()
            .AtAbsolutePath("anypath");

        act.Should()
            .Throw<Exception>()
            .WithMessage("Expected _adminApi to have been called at address matching the absolute path \"anypath\", but no calls were made.");
    }

    [Fact]
    public async Task HaveReceivedACall_AtAbsolutePath_Should_ThrowWhenNoCallsMatchingTheAbsolutePathWereMade()
    {
        await _httpClient.GetAsync("", _ct);

        Action act = () => _adminApi.Should()
            .HaveReceivedACall()
            .AtAbsolutePath("/anypath");

        act.Should()
            .Throw<Exception>()
            .WithMessage($"Expected _adminApi to have been called at address matching the absolute path \"/anypath\", but didn't find it among the calls to {{\"/\"}}.");
    }

    [Fact]
    public async Task HaveReceivedNoCalls_AtPath_WhenACallWasNotMadeToPath_Should_BeOK()
    {
        await _httpClient.GetAsync("xxx", _ct);

        _adminApi.Should()
            .HaveReceivedNoCalls()
            .AtPath("anypath");
    }

    [Fact]
    public async Task HaveReceived0Calls_AtPath_WhenACallWasNotMadeToPath_Should_BeOK()
    {
        await _httpClient.GetAsync("xxx", _ct);

        _adminApi.Should()
            .HaveReceived(0).Calls()
            .AtPath("anypath");
    }

    [Fact]
    public async Task HaveReceived1Call_AtPath_WhenACallWasMadeToPath_Should_BeOK()
    {
        await _httpClient.GetAsync("anypath", _ct);

        _adminApi.Should()
            .HaveReceived(1).Calls()
            .AtPath("/anypath");
    }

    [Fact]
    public async Task HaveReceived1Call_AtPathUsingPost_WhenAPostCallWasMadeToPath_Should_BeOK()
    {
        await _httpClient.PostAsync("anypath", new StringContent(""), _ct);

        _adminApi.Should()
            .HaveReceived(1).Calls()
            .AtPath("/anypath")
            .And
            .UsingPost();
    }

    [Fact]
    public async Task HaveReceived2Calls_AtPath_WhenACallWasMadeToPath_Should_BeOK()
    {
        await _httpClient.GetAsync("anypath", _ct);

        await _httpClient.GetAsync("anypath", _ct);

        _adminApi.Should()
            .HaveReceived(2).Calls()
            .AtPath("/anypath");
    }

    [Fact]
    public async Task HaveReceivedACall_AtPath_WhenACallWasMadeToPath_Should_BeOK()
    {
        await _httpClient.GetAsync("anypath", _ct);

        _adminApi.Should()
            .HaveReceivedACall()
            .AtPath(new WildcardMatcher("/any*"));
    }

    [Fact]
    public async Task HaveReceivedACall_AtPathWildcardMatcher_WhenACallWasMadeToPath_Should_BeOK()
    {
        await _httpClient.GetAsync("anypath", _ct);

        _adminApi.Should()
            .HaveReceivedACall()
            .AtPath("/anypath");
    }

    [Fact]
    public void HaveReceivedACall_AtPath_Should_ThrowWhenNoCallsWereMade()
    {
        Action act = () => _adminApi.Should()
            .HaveReceivedACall()
            .AtPath("anypath");

        act.Should()
            .Throw<Exception>()
            .WithMessage("Expected _adminApi to have been called at address matching the path \"anypath\", but no calls were made.");
    }

    [Fact]
    public async Task HaveReceivedACall_AtPath_Should_ThrowWhenNoCallsMatchingThePathWereMade()
    {
        await _httpClient.GetAsync("", _ct);

        Action act = () => _adminApi.Should()
            .HaveReceivedACall()
            .AtPath("/anypath");

        act.Should()
            .Throw<Exception>()
            .WithMessage($"Expected _adminApi to have been called at address matching the path \"/anypath\", but didn't find it among the calls to {{\"/\"}}.");
    }

    [Fact]
    public async Task HaveReceivedACall_WithHeader_WhenACallWasMadeWithExpectedHeader_Should_BeOK()
    {
        _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer a");
        await _httpClient.GetAsync("", _ct);

        _adminApi.Should()
            .HaveReceivedACall()
            .WitHeaderKey("Authorization").Which.Should().StartWith("A");
    }

    [Fact]
    public async Task HaveReceivedACall_WithHeader_WhenACallWasMadeWithExpectedHeaderWithValue_Should_BeOK()
    {
        _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer a");
        await _httpClient.GetAsync("", _ct);

        _adminApi.Should()
            .HaveReceivedACall()
            .WithHeader("Authorization", "Bearer a");
    }

    [Fact]
    public async Task HaveReceivedACall_WithHeader_WhenMultipleCallsWereMadeWithExpectedHeaderAmongMultipleHeaderValues_Should_BeOK()
    {
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        await _httpClient.GetAsync("1", _ct);

        _httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("EN"));
        await _httpClient.GetAsync("2", _ct);

        _adminApi.Should()
            .HaveReceivedACall()
            .WithHeader("Accept", ["application/xml", "application/json"])
            .And
            .WithHeader("Accept-Language", ["EN"]);
    }

    [Fact]
    public async Task HaveReceivedACall_WithHeader_Should_ThrowWhenNoCallsMatchingTheHeaderNameWereMade()
    {
        await _httpClient.GetAsync("", _ct);

        Action act = () => _adminApi.Should()
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
        await _httpClient.GetAsync("", _ct);

        Action act = () => _adminApi.Should()
            .HaveReceivedACall()
            .WithHeader("Accept", "missing-value");

        act.Should()
            .Throw<Exception>()
            .WithMessage("Expected _adminApi to have been called with Header \"Accept\" and Values {\"missing-value\"}, but didn't find it among the calls with Header(s)*");
    }

    [Fact]
    public async Task HaveReceivedACall_WithHeader_Should_ThrowWhenNoCallsMatchingTheHeaderWithMultipleValuesWereMade()
    {
        using var httpClient = new HttpClient { BaseAddress = new Uri(_server.Url!) };
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        await httpClient.GetAsync("", _ct);

        Action act = () => _adminApi.Should()
            .HaveReceivedACall()
            .WithHeader("Accept", ["missing-value1", "missing-value2"]);

        act.Should()
            .Throw<Exception>()
            .WithMessage("Expected _adminApi to have been called with Header \"Accept\" and Values {\"missing-value1\", \"missing-value2\"}, but didn't find it among the calls with Header(s)*");
    }

    [Fact]
    public async Task HaveReceivedACall_WithHeader_ShouldCheckAllRequests()
    {
        // Arrange
        using var server = WireMockServer.Start(settings =>
        {
            settings.StartAdminInterface = true;
            settings.Logger = new TestOutputHelperWireMockLogger(_testOutputHelper);
        });
        using var client1 = server.CreateClient();
        var adminAdmin = RestClient.For<IWireMockAdminApi>(server.Url);

        var handler = new HttpClientHandler();
        using var client2 = server.CreateClient(handler);

        // Act 1
        var task1 = client1.SendAsync(new HttpRequestMessage(HttpMethod.Get, "/")
        {
            Headers =
            {
                Authorization = new AuthenticationHeaderValue("Bearer", "invalidToken")
            }
        }, _ct);

        // Act 2
        var task2 = client2.SendAsync(new HttpRequestMessage(HttpMethod.Get, "/")
        {
            Headers =
            {
                Authorization = new AuthenticationHeaderValue("Bearer", "validToken")
            }
        }, _ct);

        await Task.WhenAll(task1, task2);

        // Assert
        adminAdmin.Should()
            .HaveReceivedACall()
            .WithHeader("Authorization", "Bearer invalidToken").And.WithoutHeader("x", "y").And.WithoutHeaderKey("a");

        adminAdmin.Should().
            HaveReceivedACall()
            .WithHeader("Authorization", "Bearer validToken").And.WithoutHeader("Authorization", "y");
    }

    [Fact]
    public async Task HaveReceivedACall_AtUrl_WhenACallWasMadeToUrl_Should_BeOK()
    {
        await _httpClient.GetAsync("anyurl", _ct);

        _adminApi.Should()
            .HaveReceivedACall()
            .AtUrl($"http://localhost:{_portUsed}/anyurl");
    }

    [Fact]
    public async Task HaveReceivedACall_AtUrlWildcardMatcher_WhenACallWasMadeToUrl_Should_BeOK()
    {
        await _httpClient.GetAsync("anyurl", _ct);

        _adminApi.Should()
            .HaveReceivedACall()
            .AtUrl(new WildcardMatcher($"http://localhost:{_portUsed}/AN*", true));
    }

    [Fact]
    public void HaveReceivedACall_AtUrl_Should_ThrowWhenNoCallsWereMade()
    {
        Action act = () => _adminApi.Should()
            .HaveReceivedACall()
            .AtUrl("anyurl");

        act.Should()
            .Throw<Exception>()
            .WithMessage("Expected _adminApi to have been called at address matching the url \"anyurl\", but no calls were made.");
    }

    [Fact]
    public async Task HaveReceivedACall_AtUrl_Should_ThrowWhenNoCallsMatchingTheUrlWereMade()
    {
        await _httpClient.GetAsync("", _ct);

        Action act = () => _adminApi.Should()
            .HaveReceivedACall()
            .AtUrl("anyurl");

        act.Should()
            .Throw<Exception>()
            .WithMessage($"Expected _adminApi to have been called at address matching the url \"anyurl\", but didn't find it among the calls to {{\"http://localhost:{_portUsed}/\"}}.");
    }

    [Fact]
    public async Task HaveReceivedACall_WithProxyUrl_WhenACallWasMadeWithProxyUrl_Should_BeOK()
    {
        _server.ResetMappings();
        _server.Given(Request.Create().UsingAnyMethod())
            .RespondWith(Response.Create().WithProxy(new ProxyAndRecordSettings { Url = "http://localhost:9999" }));

        await _httpClient.GetAsync("", _ct);

        _adminApi.Should()
            .HaveReceivedACall()
            .WithProxyUrl("http://localhost:9999");
    }

    [Fact]
    public void HaveReceivedACall_WithProxyUrl_Should_ThrowWhenNoCallsWereMade()
    {
        _server.ResetMappings();
        _server.Given(Request.Create().UsingAnyMethod())
            .RespondWith(Response.Create().WithProxy(new ProxyAndRecordSettings { Url = "http://localhost:9999" }));

        Action act = () => _adminApi.Should()
            .HaveReceivedACall()
            .WithProxyUrl("anyurl");

        act.Should()
            .Throw<Exception>()
            .WithMessage("Expected _adminApi to have been called with proxy url \"anyurl\", but no calls were made.");
    }

    [Fact]
    public async Task HaveReceivedACall_WithProxyUrl_Should_ThrowWhenNoCallsWithTheProxyUrlWereMade()
    {
        _server.ResetMappings();
        _server.Given(Request.Create().UsingAnyMethod())
            .RespondWith(Response.Create().WithProxy(new ProxyAndRecordSettings { Url = "http://localhost:9999" }));

        await _httpClient.GetAsync("", _ct);

        Action act = () => _adminApi.Should()
            .HaveReceivedACall()
            .WithProxyUrl("anyurl");

        act.Should()
            .Throw<Exception>()
            .WithMessage("Expected _adminApi to have been called with proxy url \"anyurl\", but didn't find it among the calls with {\"http://localhost:9999\"}.");
    }

    [Fact]
    public async Task HaveReceivedACall_FromClientIP_whenACallWasMadeFromClientIP_Should_BeOK()
    {
        await _httpClient.GetAsync("", _ct);
        var clientIP = _server.LogEntries.Last().RequestMessage.ClientIP;

        _adminApi.Should()
            .HaveReceivedACall()
            .FromClientIP(clientIP);
    }

    [Fact]
    public void HaveReceivedACall_FromClientIP_Should_ThrowWhenNoCallsWereMade()
    {
        Action act = () => _adminApi.Should()
            .HaveReceivedACall()
            .FromClientIP("different-ip");

        act.Should()
            .Throw<Exception>()
            .WithMessage("Expected _adminApi to have been called from client IP \"different-ip\", but no calls were made.");
    }

    [Fact]
    public async Task HaveReceivedACall_FromClientIP_Should_ThrowWhenNoCallsFromClientIPWereMade()
    {
        await _httpClient.GetAsync("", _ct);
        var clientIP = _server.LogEntries.Last().RequestMessage.ClientIP;

        Action act = () => _adminApi.Should()
            .HaveReceivedACall()
            .FromClientIP("different-ip");

        act.Should()
            .Throw<Exception>()
            .WithMessage($"Expected _adminApi to have been called from client IP \"different-ip\", but didn't find it among the calls from IP(s) {{\"{clientIP}\"}}.");
    }

    [Fact]
    public async Task HaveReceivedNoCalls_UsingPost_WhenACallWasNotMadeUsingPost_Should_BeOK()
    {
        await _httpClient.GetAsync("anyurl", _ct);

        _adminApi.Should()
            .HaveReceivedNoCalls()
            .UsingPost();
    }

    [Fact]
    public async Task HaveReceived2Calls_UsingDelete_WhenACallWasMadeUsingDelete_Should_BeOK()
    {
        var tasks = new[]
        {
            _httpClient.DeleteAsync("anyurl", _ct),
            _httpClient.DeleteAsync("anyurl", _ct),
            _httpClient.GetAsync("anyurl", _ct)
        };

        await Task.WhenAll(tasks);

        _adminApi.Should()
            .HaveReceived(2).Calls()
            .UsingDelete();
    }

    [Fact]
    public void HaveReceivedACall_UsingPatch_Should_ThrowWhenNoCallsWereMade()
    {
        Action act = () => _adminApi.Should()
            .HaveReceivedACall()
            .UsingPatch();

        act.Should()
            .Throw<Exception>()
            .WithMessage("Expected _adminApi to have been called using method \"PATCH\", but no calls were made.");
    }

    [Fact]
    public async Task HaveReceivedACall_UsingOptions_Should_ThrowWhenCallsWereNotMadeUsingOptions()
    {
        await _httpClient.PostAsync("anyurl", new StringContent("anycontent"), _ct);

        Action act = () => _adminApi.Should()
            .HaveReceivedACall()
            .UsingOptions();

        act.Should()
            .Throw<Exception>()
            .WithMessage("Expected _adminApi to have been called using method \"OPTIONS\", but didn't find it among the methods {\"POST\"}.");
    }

#if !NET452
    [Fact]
    public async Task HaveReceivedACall_UsingConnect_WhenACallWasMadeUsingConnect_Should_BeOK()
    {
        _server.ResetMappings();
        _server.Given(Request.Create().UsingAnyMethod())
            .RespondWith(Response.Create().WithStatusCode(HttpStatusCode.Found));

        _httpClient.DefaultRequestHeaders.Add("Host", new Uri(_server.Urls[0]).Authority);

        await _httpClient.SendAsync(new HttpRequestMessage(new HttpMethod("CONNECT"), "anyurl"), _ct);

        _adminApi.Should()
            .HaveReceivedACall()
            .UsingConnect();
    }
#endif

    [Fact]
    public async Task HaveReceivedACall_UsingDelete_WhenACallWasMadeUsingDelete_Should_BeOK()
    {
        await _httpClient.SendAsync(new HttpRequestMessage(new HttpMethod("DELETE"), "anyurl"), _ct);

        _adminApi.Should()
            .HaveReceivedACall()
            .UsingDelete();
    }

    [Fact]
    public async Task HaveReceivedACall_UsingGet_WhenACallWasMadeUsingGet_Should_BeOK()
    {
        await _httpClient.SendAsync(new HttpRequestMessage(new HttpMethod("GET"), "anyurl"), _ct);

        _adminApi.Should()
            .HaveReceivedACall()
            .UsingGet();
    }

    [Fact]
    public async Task HaveReceivedACall_UsingHead_WhenACallWasMadeUsingHead_Should_BeOK()
    {
        await _httpClient.SendAsync(new HttpRequestMessage(new HttpMethod("HEAD"), "anyurl"), _ct);

        _adminApi.Should()
            .HaveReceivedACall()
            .UsingHead();
    }

    [Fact]
    public async Task HaveReceivedACall_UsingOptions_WhenACallWasMadeUsingOptions_Should_BeOK()
    {
        await _httpClient.SendAsync(new HttpRequestMessage(new HttpMethod("OPTIONS"), "anyurl"), _ct);

        _adminApi.Should()
            .HaveReceivedACall()
            .UsingOptions();
    }

    [Theory]
    [InlineData("POST")]
    [InlineData("Post")]
    public async Task HaveReceivedACall_UsingPost_WhenACallWasMadeUsingPost_Should_BeOK(string method)
    {
        await _httpClient.SendAsync(new HttpRequestMessage(new HttpMethod(method), "anyurl"), _ct);

        _adminApi.Should()
            .HaveReceivedACall()
            .UsingPost();
    }

    [Fact]
    public async Task HaveReceived1Call_AtAbsoluteUrlUsingPost_ShouldChain()
    {
        // Arrange
        using var server = WireMockServer.Start(settings =>
        {
            settings.StartAdminInterface = true;
            settings.Logger = new TestOutputHelperWireMockLogger(_testOutputHelper);
        });
        var adminApi = RestClient.For<IWireMockAdminApi>(server.Url);

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
            httpClient.GetAsync($"{server.Url}/a", _ct),
            httpClient.PostAsync($"{server.Url}/b", new StringContent("B"), _ct),
            httpClient.PostAsync($"{server.Url}/c", new StringContent("C"), _ct)
        };

        await Task.WhenAll(tasks);

        // Assert
        adminApi
            .Should()
            .HaveReceived(1)
            .Calls()
            .AtUrl($"{server.Url}/a")
            .And
            .UsingGet();

        adminApi
            .Should()
            .HaveReceived(1)
            .Calls()
            .AtUrl($"{server.Url}/b")
            .And
            .UsingPost();

        adminApi
            .Should()
            .HaveReceived(1)
            .Calls()
            .AtUrl($"{server.Url}/c")
            .And
            .UsingPost();

        adminApi
            .Should()
            .HaveReceived(3)
            .Calls();

        adminApi
            .Should()
            .HaveReceived(1)
            .Calls()
            .UsingGet();

        adminApi
            .Should()
            .HaveReceived(2)
            .Calls()
            .UsingPost();
    }

    [Fact]
    public async Task HaveReceivedACall_UsingPatch_WhenACallWasMadeUsingPatch_Should_BeOK()
    {
        await _httpClient.SendAsync(new HttpRequestMessage(new HttpMethod("PATCH"), "anyurl"), _ct);

        _adminApi.Should()
            .HaveReceivedACall()
            .UsingPatch();
    }

    [Fact]
    public async Task HaveReceivedACall_UsingPut_WhenACallWasMadeUsingPut_Should_BeOK()
    {
        await _httpClient.SendAsync(new HttpRequestMessage(new HttpMethod("PUT"), "anyurl"), _ct);

        _adminApi.Should()
            .HaveReceivedACall()
            .UsingPut();
    }

    [Fact]
    public async Task HaveReceivedACall_UsingTrace_WhenACallWasMadeUsingTrace_Should_BeOK()
    {
        await _httpClient.SendAsync(new HttpRequestMessage(new HttpMethod("TRACE"), "anyurl"), _ct);

        _adminApi.Should()
            .HaveReceivedACall()
            .UsingTrace();
    }

    [Fact]
    public async Task HaveReceivedACall_UsingAnyMethod_WhenACallWasMadeUsingGet_Should_BeOK()
    {
        await _httpClient.SendAsync(new HttpRequestMessage(new HttpMethod("GET"), "anyurl"), _ct);

        _adminApi.Should()
            .HaveReceivedACall()
            .UsingAnyMethod();
    }

    [Fact]
    public void HaveReceivedNoCalls_UsingAnyMethod_WhenNoCallsWereMade_Should_BeOK()
    {
        _adminApi
            .Should()
            .HaveReceived(0)
            .Calls()
            .UsingAnyMethod();

        _adminApi
            .Should()
            .HaveReceivedNoCalls()
            .UsingAnyMethod();
    }

    [Fact]
    public void HaveReceivedNoCalls_AtUrl_WhenNoCallsWereMade_Should_BeOK()
    {
        _adminApi.Should()
            .HaveReceived(0)
            .Calls()
            .AtUrl(_server.Url ?? string.Empty);

        _adminApi.Should()
            .HaveReceivedNoCalls()
            .AtUrl(_server.Url ?? string.Empty);
    }

    [Fact]
    public async Task HaveReceived1Call_WithBodyAsString()
    {
        // Arrange
        using var server = WireMockServer.Start(settings =>
        {
            settings.StartAdminInterface = true;
            settings.Logger = new TestOutputHelperWireMockLogger(_testOutputHelper);
        });
        var adminApi = RestClient.For<IWireMockAdminApi>(server.Url);

        server
            .Given(Request.Create().WithPath("/a").UsingPost().WithBody("x"))
            .RespondWith(Response.Create().WithBody("A response"));

        // Act
        var httpClient = new HttpClient();

        await httpClient.PostAsync($"{server.Url}/a", new StringContent("x"), _ct);

        // Assert
        adminApi
            .Should()
            .HaveReceived(1)
            .Calls()
            .WithBody("*")
            .And
            .UsingPost();

        adminApi
            .Should()
            .HaveReceived(1)
            .Calls()
            .WithBody("x")
            .And
            .UsingPost();

        adminApi
            .Should()
            .HaveReceived(0)
            .Calls()
            .WithBody("")
            .And
            .UsingPost();

        adminApi
            .Should()
            .HaveReceived(0)
            .Calls()
            .WithBody("y")
            .And
            .UsingPost();
    }

    [Fact]
    public async Task HaveReceived1Call_WithBodyAsJson()
    {
        // Arrange
        using var server = WireMockServer.Start(settings =>
        {
            settings.StartAdminInterface = true;
            settings.Logger = new TestOutputHelperWireMockLogger(_testOutputHelper);
        });
        var adminApi = RestClient.For<IWireMockAdminApi>(server.Url);

        server
            .Given(Request.Create().WithPath("/a").UsingPost().WithBodyAsJson(new { x = "y" }))
            .RespondWith(Response.Create().WithBody("A response"));

        // Act
        using var httpClient = new HttpClient();

        var requestBody = new
        {
            x = "y"
        };
        await httpClient.PostAsJsonAsync($"{server.Url}/a", requestBody, _ct);

        // Assert
        adminApi
            .Should()
            .HaveReceived(1)
            .Calls()
            .WithBodyAsJson(new { x = "y" })
            .And
            .UsingPost();

        adminApi
            .Should()
            .HaveReceived(1)
            .Calls()
            .WithBodyAsJson(@"{ ""x"": ""y"" }")
            .And
            .UsingPost();

        adminApi
            .Should()
            .HaveReceived(0)
            .Calls()
            .WithBodyAsJson(new { x = "?" })
            .And
            .UsingPost();

        adminApi
            .Should()
            .HaveReceived(0)
            .Calls()
            .WithBodyAsJson(@"{ ""x"": 1234 }")
            .And
            .UsingPost();
    }

    [Fact]
    public async Task WithBodyAsJson_When_NoMatch_ShouldHaveCorrectErrorMessage()
    {
        // Arrange
        using var server = WireMockServer.Start(settings =>
        {
            settings.StartAdminInterface = true;
            settings.Logger = new TestOutputHelperWireMockLogger(_testOutputHelper);
        });
        var adminApi = RestClient.For<IWireMockAdminApi>(server.Url);

        server
            .Given(Request.Create().WithPath("/a").UsingPost())
            .RespondWith(Response.Create().WithBody("A response"));

        // Act
        using var httpClient = new HttpClient();

        var requestBody = new
        {
            x = "123"
        };
        await httpClient.PostAsJsonAsync($"{server.Url}/a", requestBody, _ct);

        // Assert
        Action act = () => adminApi
            .Should()
            .HaveReceived(1)
            .Calls()
            .WithBodyAsJson(new { x = "y" })
            .And
            .UsingPost();

        act.Should()
            .Throw<Exception>()
            .WithMessage("""Expected wiremockadminapi to have been called using body "{"x":"y"}", but didn't find it among the body/bodies "{"x":"123"}".""");
    }

    [Fact]
    public async Task WithBodyAsString_When_NoMatch_ShouldHaveCorrectErrorMessage()
    {
        // Arrange
        using var server = WireMockServer.Start(settings =>
        {
            settings.StartAdminInterface = true;
            settings.Logger = new TestOutputHelperWireMockLogger(_testOutputHelper);
        });
        var adminApi = RestClient.For<IWireMockAdminApi>(server.Url);

        server
            .Given(Request.Create().WithPath("/a").UsingPost())
            .RespondWith(Response.Create().WithBody("A response"));

        // Act
        using var httpClient = new HttpClient();

        await httpClient.PostAsync($"{server.Url}/a", new StringContent("123"), _ct);

        // Assert
        Action act = () => adminApi
            .Should()
            .HaveReceived(1)
            .Calls()
            .WithBody("abc")
            .And
            .UsingPost();

        act.Should()
            .Throw<Exception>()
            .WithMessage("""Expected wiremockadminapi to have been called using body "abc", but didn't find it among the body/bodies "123".""");
    }

    [Fact]
    public async Task WithBodyAsBytes_When_NoMatch_ShouldHaveCorrectErrorMessage()
    {
        // Arrange
        using var server = WireMockServer.Start(settings =>
        {
            settings.StartAdminInterface = true;
            settings.Logger = new TestOutputHelperWireMockLogger(_testOutputHelper);
        });
        var adminApi = RestClient.For<IWireMockAdminApi>(server.Url);

        server
            .Given(Request.Create().WithPath("/a").UsingPost())
            .RespondWith(Response.Create().WithBody("A response"));

        // Act
        using var httpClient = new HttpClient();

        await httpClient.PostAsync($"{server.Url}/a", new ByteArrayContent([5]), _ct);

        // Assert
        Action act = () => adminApi
            .Should()
            .HaveReceived(1)
            .Calls()
            .WithBodyAsBytes([1])
            .And
            .UsingPost();

        act.Should()
            .Throw<Exception>()
            .WithMessage("Expected wiremockadminapi to have been called using body \"byte[1] {...}\", but didn't find it among the body/bodies \"byte[1] {...}\".");
    }

    [Fact]
    public async Task HaveReceived1Call_WithBodyAsBytes()
    {
        // Arrange
        using var server = WireMockServer.Start(settings =>
        {
            settings.StartAdminInterface = true;
            settings.Logger = new TestOutputHelperWireMockLogger(_testOutputHelper);
        });
        var adminApi = RestClient.For<IWireMockAdminApi>(server.Url);

        var bytes = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };
        server
            .Given(Request.Create().WithPath("/binary").UsingPut().WithBody(bytes))
            .RespondWith(Response.Create().WithBody("A response"));

        // Act
        using var httpClient = new HttpClient();

        await httpClient.PutAsync($"{server.Url}/binary", new ByteArrayContent(bytes), _ct);

        // Assert
        adminApi
            .Should()
            .HaveReceived(1)
            .Calls()
            .WithBodyAsBytes(bytes)
            .And
            .UsingPut();

        adminApi
            .Should()
            .HaveReceived(0)
            .Calls()
            .WithBodyAsBytes([])
            .And
            .UsingPut();

        adminApi
            .Should()
            .HaveReceived(0)
            .Calls()
            .WithBodyAsBytes([42])
            .And
            .UsingPut();
    }

    [Fact]
    public async Task HaveReceived1Call_WithBodyAsString_UsingStringMatcher()
    {
        // Arrange
        using var server = WireMockServer.Start(settings =>
        {
            settings.StartAdminInterface = true;
            settings.Logger = new TestOutputHelperWireMockLogger(_testOutputHelper);
        });
        var adminApi = RestClient.For<IWireMockAdminApi>(server.Url);

        server
            .Given(Request.Create().WithPath("/a").UsingPost().WithBody("x"))
            .RespondWith(Response.Create().WithBody("A response"));

        // Act
        using var httpClient = new HttpClient();

        await httpClient.PostAsync($"{server.Url}/a", new StringContent("x"), _ct);

        // Assert
        adminApi
            .Should()
            .HaveReceived(1)
            .Calls()
            .WithBody(new ExactMatcher("x"))
            .And
            .UsingPost();

        adminApi
            .Should()
            .HaveReceived(0)
            .Calls()
            .WithBody(new ExactMatcher(""))
            .And
            .UsingPost();

        adminApi
            .Should()
            .HaveReceived(0)
            .Calls()
            .WithBody(new ExactMatcher("y"))
            .And
            .UsingPost();
    }

    [Fact]
    public async Task HaveReceivedACall_WithHeader_Should_ThrowWhenHttpMethodDoesNotMatch()
    {
        // Arrange
        using var server = WireMockServer.Start(settings =>
        {
            settings.StartAdminInterface = true;
            settings.Logger = new TestOutputHelperWireMockLogger(_testOutputHelper);
        });
        var adminApi = RestClient.For<IWireMockAdminApi>(server.Url);

        // Act : HTTP GET
        using var httpClient = new HttpClient();
        await httpClient.GetAsync(server.Url, _ct);

        // Act : HTTP POST
        var request = new HttpRequestMessage(HttpMethod.Post, server.Url);
        request.Headers.Add("TestHeader", ["Value", "Value2"]);

        await httpClient.SendAsync(request, _ct);

        // Assert
        adminApi.Should().HaveReceivedACall().UsingPost().And.WithHeader("TestHeader", ["Value", "Value2"]);

        Action act = () => adminApi.Should().HaveReceivedACall().UsingGet().And.WithHeader("TestHeader", "Value");
        act.Should()
            .Throw<Exception>()
            .WithMessage("Expected adminapi to have been called with Header \"TestHeader\" and Values {\"Value\"}, but didn't find it among the calls with Header(s)*");
    }

    [Fact]
    public async Task HaveReceivedACall_WithHeaderKey_Should_ThrowWhenHttpMethodDoesNotMatch()
    {
        // Arrange
        using var server = WireMockServer.Start(settings =>
        {
            settings.StartAdminInterface = true;
            settings.Logger = new TestOutputHelperWireMockLogger(_testOutputHelper);
        });
        var adminApi = RestClient.For<IWireMockAdminApi>(server.Url);

        // Act : HTTP GET
        using var httpClient = new HttpClient();
        await httpClient.GetAsync(server.Url, _ct);

        // Act : HTTP POST
        var request = new HttpRequestMessage(HttpMethod.Post, server.Url);
        request.Headers.Add("TestHeader", ["Value", "Value2"]);

        await httpClient.SendAsync(request, _ct);

        // Assert
        adminApi.Should().HaveReceivedACall().UsingPost().And.WitHeaderKey("TestHeader");

        Action act = () => adminApi.Should().HaveReceivedACall().UsingGet().And.WitHeaderKey("TestHeader");
        act.Should()
            .Throw<Exception>()
            .WithMessage("Expected adminapi to have been called with Header \"TestHeader\", but didn't find it among the calls with Header(s)*");
    }

    public void Dispose()
    {
        _server?.Stop();
        _server?.Dispose();
        _httpClient?.Dispose();

        GC.SuppressFinalize(this);
    }
}