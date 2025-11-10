// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using NFluent;
using WireMock.Admin.Mappings;
using WireMock.Http;
using WireMock.Matchers;
using WireMock.Net.Tests.Facts;
using WireMock.Net.Tests.Serialization;
using WireMock.Net.Xunit;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;
using WireMock.Types;
using WireMock.Util;
using Xunit;
using Xunit.Abstractions;

namespace WireMock.Net.Tests;

public partial class WireMockServerTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public WireMockServerTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void WireMockServer_Start()
    {
        // Act
        var server = WireMockServer.Start();

        // Assert
        server.IsStarted.Should().BeTrue();
        server.IsStartedWithAdminInterface.Should().BeFalse();

        server.Stop();
    }

    [Fact]
    public void WireMockServer_StartWithAdminInterface()
    {
        // Act
        var server = WireMockServer.StartWithAdminInterface();

        // Assert
        server.IsStarted.Should().BeTrue();
        server.IsStartedWithAdminInterface.Should().BeTrue();

        server.Stop();
    }

    [Fact]
    public async Task WireMockServer_Should_Reset_LogEntries()
    {
        // Arrange
        var server = WireMockServer.Start();

        // Act
        await server.CreateClient().GetAsync("/foo");
        server.ResetLogEntries();

        // Assert
        server.LogEntries.Should().BeEmpty();

        server.Stop();
    }

    [Fact]
    public void WireMockServer_Should_reset_mappings()
    {
        // given
        string path = $"/foo_{Guid.NewGuid()}";
        var server = WireMockServer.Start();

        server
            .Given(Request.Create()
                .WithPath(path)
                .UsingGet())
            .RespondWith(Response.Create()
                .WithBody(@"{ msg: ""Hello world!""}"));

        // when
        server.ResetMappings();

        // then
        Check.That(server.Mappings).IsEmpty();
        Check.ThatCode(() => new HttpClient().GetStringAsync("http://localhost:" + server.Ports[0] + path)).ThrowsAny();

        server.Stop();
    }

#if NET461_OR_GREATER || NET6_0_OR_GREATER
    [Fact]
    public async Task WireMockServer_Should_Support_Https()
    {
        // Arrange
        const string body = "example";
        var path = $"/foo_{Guid.NewGuid()}";
        var server = WireMockServer.Start(settings =>
        {
            settings.UseSSL = true;
        });

        server
            .Given(Request.Create()
                .WithPath(path)
                .UsingGet()
            )
            .RespondWith(Response.Create()
                .WithBody(body)
            );

        // Configure the HttpClient to trust self-signed certificates
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true
        };
        using var client = new HttpClient(handler);

        // Act
        var result = await client.GetStringAsync($"{server.Url}{path}");

        // Assert
        result.Should().Be(body);

        server.Stop();
    }
#endif

#if NET6_0_OR_GREATER
    [Fact]
    public async Task WireMockServer_When_HttpClientWithWebProxyCallsHttp_Should_Work_Correct()
    {
        // Arrange
        const string body = "example";
        var settings = new WireMockServerSettings
        {
            HostingScheme = HostingScheme.Http
        };
        var server = WireMockServer.Start(settings);

        // The response to an HTTP CONNECT method, which is used to establish a tunnel with a proxy, should typically be a 200 OK status code if the connection is successful.
        // This indicates that a tunnel has been established successfully between the client and the server via the proxy. 
        server
            .Given(Request.Create()
                .UsingConnect()
            )
            .RespondWith(Response.Create()
                .WithBody("Connection established")
            );

        server
            .Given(Request.Create()
                .UsingGet()
            )
            .RespondWith(Response.Create()
                .WithBody(body)
            );

        var httpUrl = server.Urls.First();

        // Act
        string result;
        var currentProxy = HttpClient.DefaultProxy;
        try
        {
            HttpClient.DefaultProxy = new WebProxy(httpUrl, false);

            result = await new HttpClient().GetStringAsync(httpUrl);
        }
        finally
        {
            // Revert
            HttpClient.DefaultProxy = currentProxy;
        }

        // Assert
        result.Should().Be(body);

        server.Stop();
    }
#endif

#if NET6_0_OR_GREATER
    private static string[] GetIPAddressesByFamily(AddressFamily addressFamily)
    {
        return NetworkInterface.GetAllNetworkInterfaces()
            .Where(ni => ni.OperationalStatus == OperationalStatus.Up)
            .SelectMany(ni => ni.GetIPProperties().UnicastAddresses)
            .Where(addr => addr.Address.AddressFamily == addressFamily)
            .Select(addr => addr.Address.ToString())
            .ToArray();
    }

    [IgnoreOnContinuousIntegrationFact]
    public async Task WireMockServer_WithUrl0000_Should_Listen_On_All_IPs_IPv4()
    {
        // Arrange
        var port = PortUtils.FindFreeTcpPort();
        var IPv4 = GetIPAddressesByFamily(AddressFamily.InterNetwork);
        var settings = new WireMockServerSettings
        {
            Urls = ["http://0.0.0.0:" + port]
        };
        using var server = WireMockServer.Start(settings);

        server.Given(Request.Create().WithPath("/*")).RespondWith(Response.Create().WithBody("x"));

        foreach (var address in IPv4)
        {
            // Act
            var response = await new HttpClient().GetStringAsync("http://" + address + ":" + server.Ports[0] + "/foo").ConfigureAwait(false);

            // Assert
            response.Should().Be("x");
        }
    }

#if NET8_0_OR_GREATER
    [IgnoreOnContinuousIntegrationFact]
    public async Task WireMockServer_WithUrl0000_Should_Listen_On_All_IPs_IPv6()
    {
        // Arrange
        var port = PortUtils.FindFreeTcpPort();
        var IPv6 = GetIPAddressesByFamily(AddressFamily.InterNetworkV6);
        var settings = new WireMockServerSettings
        {
            Urls = ["http://0.0.0.0:" + port]
        };
        using var server = WireMockServer.Start(settings);

        server.Given(Request.Create().WithPath("/*")).RespondWith(Response.Create().WithBody("x"));

        foreach (var address in IPv6)
        {
            // Act
            var response = await new HttpClient().GetStringAsync("http://[" + address + "]:" + server.Ports[0] + "/foo").ConfigureAwait(false);

            // Assert
            response.Should().Be("x");
        }
    }
#endif
#endif

    [Fact]
    public async Task WireMockServer_Should_respond_a_redirect_without_body()
    {
        // Assign
        string path = $"/foo_{Guid.NewGuid()}";
        string pathToRedirect = $"/bar_{Guid.NewGuid()}";

        var server = WireMockServer.Start(new WireMockServerSettings
        {
            Logger = new TestOutputHelperWireMockLogger(_testOutputHelper)
        });

        server
            .Given(Request.Create()
                .WithPath(path)
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(307)
                .WithHeader("Location", pathToRedirect));
        server
            .Given(Request.Create()
                .WithPath(pathToRedirect)
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithBody("REDIRECT SUCCESSFUL"));

        // Act
        var response = await new HttpClient().GetStringAsync($"http://localhost:{server.Ports[0]}{path}");

        // Assert
        Check.That(response).IsEqualTo("REDIRECT SUCCESSFUL");

        server.Stop();
    }

#if NETCOREAPP3_1 || NET5_0_OR_GREATER
    [Fact]
    public async Task WireMockServer_WithCorsPolicyOptions_Should_Work_Correct()
    {
        // Arrange
        var settings = new WireMockServerSettings
        {
            CorsPolicyOptions = CorsPolicyOptions.AllowAll
        };
        var server = WireMockServer.Start(settings);

        server.Given(Request.Create().WithPath("/*")).RespondWith(Response.Create().WithBody("x"));

        // Act
        var response = await new HttpClient().GetStringAsync("http://localhost:" + server.Ports[0] + "/foo");

        // Assert
        response.Should().Be("x");

        server.Stop();
    }
#endif

    [Fact]
    public async Task WireMockServer_Should_delay_responses_for_a_given_route()
    {
        // Arrange
        var server = WireMockServer.Start();

        server
            .Given(Request.Create()
                .WithPath("/*"))
            .RespondWith(Response.Create()
                .WithBody(@"{ msg: ""Hello world!""}")
                .WithDelay(TimeSpan.FromMilliseconds(200)));

        // Act
        var watch = new Stopwatch();
        watch.Start();
        await new HttpClient().GetStringAsync("http://localhost:" + server.Ports[0] + "/foo");
        watch.Stop();

        // Assert
        watch.ElapsedMilliseconds.Should().BeGreaterOrEqualTo(0);

        server.Stop();
    }

    [Fact]
    public async Task WireMockServer_Should_randomly_delay_responses_for_a_given_route()
    {
        // Arrange
        var server = WireMockServer.Start();

        server
            .Given(Request.Create()
                .WithPath("/*"))
            .RespondWith(Response.Create()
                .WithBody(@"{ msg: ""Hello world!""}")
                .WithRandomDelay(10, 1000));

        var watch = new Stopwatch();
        watch.Start();

        var httClient = new HttpClient();
        async Task<long> ExecuteTimedRequestAsync()
        {
            watch.Reset();
            await httClient.GetStringAsync("http://localhost:" + server.Ports[0] + "/foo").ConfigureAwait(false);
            return watch.ElapsedMilliseconds;
        }

        // Act
        await ExecuteTimedRequestAsync();
        await ExecuteTimedRequestAsync();
        await ExecuteTimedRequestAsync();

        server.Stop();
    }

    [Fact]
    public async Task WireMockServer_Should_delay_responses()
    {
        // Arrange
        var server = WireMockServer.Start();
        server.AddGlobalProcessingDelay(TimeSpan.FromMilliseconds(200));
        server
            .Given(Request.Create().WithPath("/*"))
            .RespondWith(Response.Create().WithBody(@"{ msg: ""Hello world!""}"));

        // Act
        var watch = new Stopwatch();
        watch.Start();
        await new HttpClient().GetStringAsync("http://localhost:" + server.Ports[0] + "/foo");
        watch.Stop();

        // Assert
        watch.ElapsedMilliseconds.Should().BeGreaterOrEqualTo(0);

        server.Stop();
    }

    //Leaving commented as this requires an actual certificate with password, along with a service that expects a client certificate
    //[Fact]
    //public async Task Should_proxy_responses_with_client_certificate()
    //{
    //    // given
    //    var _server = WireMockServer.Start();
    //    _server
    //        .Given(Request.Create().WithPath("/*"))
    //        .RespondWith(Response.Create().WithProxy("https://server-that-expects-a-client-certificate", @"\\yourclientcertificatecontainingprivatekey.pfx", "yourclientcertificatepassword"));

    //    // when
    //    var result = await new HttpClient().GetStringAsync("http://localhost:" + _server.Ports[0] + "/someurl?someQuery=someValue");

    //    // then
    //    Check.That(result).Contains("google");
    //}

    [Fact]
    public async Task WireMockServer_Should_Exclude_RestrictedResponseHeader()
    {
        // Assign
        string path = $"/foo_{Guid.NewGuid()}";
        var server = WireMockServer.Start();

        server
            .Given(Request.Create().WithPath(path).UsingGet())
            .RespondWith(Response.Create().WithHeader("Transfer-Encoding", "chunked").WithHeader("test", "t"));

        // Act
        var response = await new HttpClient().GetAsync("http://localhost:" + server.Ports[0] + path);

        // Assert
        Check.That(response.Headers.Contains("test")).IsTrue();
        Check.That(response.Headers.Contains("Transfer-Encoding")).IsFalse();

        server.Stop();
    }

    [Fact] // #720
    public async Task WireMockServer_Should_AllowResponseHeaderContentLength_For_HEAD()
    {
        // Assign
        const string length = "42";
        var path = $"/cl_{Guid.NewGuid()}";
        using var server = WireMockServer.Start();

        server
            .Given(Request.Create().WithPath(path).UsingHead())
            .RespondWith(Response.Create().WithHeader(HttpKnownHeaderNames.ContentLength, length));

        // Act
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Head, path);
        var response = await server.CreateClient().SendAsync(httpRequestMessage);

        // Assert
        response.Content.Headers.GetValues(HttpKnownHeaderNames.ContentLength).Should().Contain(length);
    }

#if !NET452 && !NET461
    [Theory]
    [InlineData("TRACE")]
    [InlineData("GET")]
    public async Task WireMockServer_Should_exclude_body_for_methods_where_body_is_definitely_disallowed(string method)
    {
        // Assign
        string content = "hello";
        var server = WireMockServer.Start();

        server
            .Given(Request.Create().WithBody((byte[] bodyBytes) => bodyBytes != null))
            .AtPriority(0)
            .RespondWith(Response.Create().WithStatusCode(400));
        server
            .Given(Request.Create())
            .AtPriority(1)
            .RespondWith(Response.Create().WithStatusCode(200));

        // Act
        var request = new HttpRequestMessage(new HttpMethod(method), "http://localhost:" + server.Ports[0] + "/");
        request.Content = new StringContent(content);
        var response = await new HttpClient().SendAsync(request);

        // Assert
        Check.That(response.StatusCode).Equals(HttpStatusCode.OK);

        server.Stop();
    }
#endif

    [Theory]
    [InlineData("POST")]
    [InlineData("PUT")]
    [InlineData("OPTIONS")]
    [InlineData("REPORT")]
    [InlineData("DELETE")]
    [InlineData("SOME-UNKNOWN-METHOD")] // default behavior for unknown methods is to allow a body (see BodyParser.ShouldParseBody)
    public async Task WireMockServer_Should_not_exclude_body_for_supported_methods(string method)
    {
        // Assign
        string content = "hello";
        var server = WireMockServer.Start();

        server
            .Given(Request.Create().WithBody(content))
            .AtPriority(0)
            .RespondWith(Response.Create().WithStatusCode(200));
        server
            .Given(Request.Create())
            .AtPriority(1)
            .RespondWith(Response.Create().WithStatusCode(400));

        // Act
        var request = new HttpRequestMessage(new HttpMethod(method), "http://localhost:" + server.Ports[0] + "/");
        request.Content = new StringContent(content);
        var response = await new HttpClient().SendAsync(request);

        // Assert
        Check.That(response.StatusCode).Equals(HttpStatusCode.OK);

        server.Stop();
    }

    [Theory]
    [InlineData("application/json")]
    [InlineData("application/json; charset=ascii")]
    [InlineData("application/json; charset=utf-8")]
    [InlineData("application/json; charset=UTF-8")]
    public async Task WireMockServer_Should_AcceptPostMappingsWithContentTypeJsonAndAnyCharset(string contentType)
    {
        // Arrange
        string message = @"{
                ""request"": {
                    ""method"": ""GET"",
                    ""url"": ""/some/thing""
                },
                ""response"": {
                    ""status"": 200,
                    ""body"": ""Hello world!"",
                    ""headers"": {
                        ""Content-Type"": ""text/plain""
                    }
                }
            }";
        var stringContent = new StringContent(message);
        stringContent.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
        var server = WireMockServer.StartWithAdminInterface();

        // Act
        var response = await new HttpClient().PostAsync($"{server.Url}/__admin/mappings", stringContent);

        // Assert
        Check.That(response.StatusCode).Equals(HttpStatusCode.Created);
        Check.That(await response.Content.ReadAsStringAsync()).Contains("Mapping added");

        server.Stop();
    }

    [Theory]
    [InlineData("gzip")]
    [InlineData("deflate")]
    public async Task WireMockServer_Should_SupportRequestGZipAndDeflate(string contentEncoding)
    {
        // Arrange
        const string body = "hello wiremock";
        byte[] compressed = CompressionUtils.Compress(contentEncoding, Encoding.UTF8.GetBytes(body));

        var server = WireMockServer.Start();
        server.Given(
                Request.Create()
                    .WithPath("/foo")
                    .WithBody("hello wiremock")
            )
            .RespondWith(
                Response.Create().WithBody("OK")
            );

        var content = new StreamContent(new MemoryStream(compressed));
        content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
        content.Headers.ContentEncoding.Add(contentEncoding);

        // Act
        var response = await new HttpClient().PostAsync($"{server.Urls[0]}/foo", content);

        // Assert
        Check.That(await response.Content.ReadAsStringAsync()).Contains("OK");

        server.Stop();
    }

#if !NET452
    [Fact]
    public async Task WireMockServer_Should_respond_to_ipv4_loopback()
    {
        // Assign
        var server = WireMockServer.Start();

        server
            .Given(Request.Create()
                .WithPath("/*"))
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithBody("from ipv4 loopback"));

        // Act
        var response = await new HttpClient().GetStringAsync($"http://127.0.0.1:{server.Ports[0]}/foo");

        // Assert
        Check.That(response).IsEqualTo("from ipv4 loopback");

        server.Stop();
    }

    [Fact]
    public async Task WireMockServer_Should_respond_to_ipv6_loopback()
    {
        // Assign
        var server = WireMockServer.Start();

        server
            .Given(Request.Create()
                .WithPath("/*"))
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithBody("from ipv6 loopback"));

        // Act
        var response = await new HttpClient().GetStringAsync($"http://[::1]:{server.Ports[0]}/foo");

        // Assert
        Check.That(response).IsEqualTo("from ipv6 loopback");

        server.Stop();
    }

    [Fact]
    public async Task WireMockServer_Using_JsonMapping_And_CustomMatcher_WithCorrectParams_ShouldMatch()
    {
        // Arrange
        var settings = new WireMockServerSettings();
        settings.WatchStaticMappings = true;
        settings.WatchStaticMappingsInSubdirectories = true;
        settings.CustomMatcherMappings = new Dictionary<string, Func<MatcherModel, IMatcher>>();
        settings.CustomMatcherMappings[nameof(CustomPathParamMatcher)] = matcherModel =>
        {
            var matcherParams = JsonConvert.DeserializeObject<CustomPathParamMatcherModel>((string)matcherModel.Pattern!)!;
            return new CustomPathParamMatcher(
                matcherModel.RejectOnMatch == true ? MatchBehaviour.RejectOnMatch : MatchBehaviour.AcceptOnMatch,
                matcherParams.Path, matcherParams.PathParams
            );
        };

        var server = WireMockServer.Start(settings);
        server.WithMapping(@"{
    ""Request"": {
        ""Path"": {
            ""Matchers"": [
                {
                    ""Name"": ""CustomPathParamMatcher"",
                    ""Pattern"": ""{\""path\"":\""/customer/{customerId}/document/{documentId}\"",\""pathParams\"":{\""customerId\"":\""^[0-9]+$\"",\""documentId\"":\""^[0-9a-zA-Z\\\\-_]+\\\\.[a-zA-Z]+$\""}}""
                }
            ]
        }
    },
    ""Response"": {
        ""StatusCode"": 200,
        ""Headers"": {
            ""Content-Type"": ""application/json""
        },
        ""Body"": ""OK""
    }
    }");

        // Act
        var response = await new HttpClient().PostAsync("http://localhost:" + server.Ports[0] + "/customer/132/document/pic.jpg", new StringContent("{ Hi = \"Hello World\" }"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        server.Stop();
    }

    [Fact]
    public async Task WireMockServer_Using_JsonMapping_And_CustomMatcher_WithIncorrectParams_ShouldNotMatch()
    {
        // Arrange
        var settings = new WireMockServerSettings();
        settings.WatchStaticMappings = true;
        settings.WatchStaticMappingsInSubdirectories = true;
        settings.CustomMatcherMappings = new Dictionary<string, Func<MatcherModel, IMatcher>>();
        settings.CustomMatcherMappings[nameof(CustomPathParamMatcher)] = matcherModel =>
        {
            var matcherParams = JsonConvert.DeserializeObject<CustomPathParamMatcherModel>((string)matcherModel.Pattern!)!;
            return new CustomPathParamMatcher(
                matcherModel.RejectOnMatch == true ? MatchBehaviour.RejectOnMatch : MatchBehaviour.AcceptOnMatch,
                matcherParams.Path, matcherParams.PathParams
            );
        };

        var server = WireMockServer.Start(settings);
        server.WithMapping(@"{
    ""Request"": {
        ""Path"": {
            ""Matchers"": [
                {
                    ""Name"": ""CustomPathParamMatcher"",
                    ""Pattern"": ""{\""path\"":\""/customer/{customerId}/document/{documentId}\"",\""pathParams\"":{\""customerId\"":\""^[0-9]+$\"",\""documentId\"":\""^[0-9a-zA-Z\\\\-_]+\\\\.[a-zA-Z]+$\""}}""
                }
            ]
        }
    },
    ""Response"": {
        ""StatusCode"": 200,
        ""Headers"": {
            ""Content-Type"": ""application/json""
        },
        ""Body"": ""OK""
    }
    }");

        // Act
        var response = await new HttpClient().PostAsync("http://localhost:" + server.Ports[0] + "/customer/132/document/pic", new StringContent("{ Hi = \"Hello World\" }"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        server.Stop();
    }
#endif
}
