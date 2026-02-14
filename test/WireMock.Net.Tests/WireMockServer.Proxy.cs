// Copyright © WireMock.Net

using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using AwesomeAssertions;
using Moq;
using NFluent;
using WireMock.Constants;
using WireMock.Handlers;
using WireMock.Matchers;
using WireMock.Matchers.Request;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;
using WireMock.Util;

namespace WireMock.Net.Tests;

public class WireMockServerProxyTests
{
    [Fact(Skip = "Fails in Linux CI")]
    public async Task WireMockServer_ProxySSL_Should_log_proxied_requests()
    {
        // Assign
        var settings = new WireMockServerSettings
        {
            UseSSL = true,
            ProxyAndRecordSettings = new ProxyAndRecordSettings
            {
                Url = "https://www.google.com",
                SaveMapping = true,
                SaveMappingToFile = false
            }

        };
        var server = WireMockServer.Start(settings);

        // Act
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(server.Urls[0])
        };
        var httpClientHandler = new HttpClientHandler { AllowAutoRedirect = false };
        using var httpClient = new HttpClient(httpClientHandler);
        await httpClient.SendAsync(requestMessage);

        // Assert
        Check.That(server.Mappings).HasSize(2);
        Check.That(server.LogEntries).HasSize(1);
    }

    [Fact]
    public async Task WireMockServer_Proxy_AdminFalse_With_SaveMapping_Is_True_And_SaveMappingToFile_Is_False_Should_AddInternalMappingOnly()
    {
        // Assign
        var settings = new WireMockServerSettings
        {
            ProxyAndRecordSettings = new ProxyAndRecordSettings
            {
                Url = "http://www.google.com",
                SaveMapping = true,
                SaveMappingToFile = false,
                ExcludedHeaders = new[] { "Connection" } // Needed for .NET 4.5.x and 4.6.x
            }
        };
        var server = WireMockServer.Start(settings);

        // Act
        var httpClientHandler = new HttpClientHandler { AllowAutoRedirect = false };
        using var client = new HttpClient(httpClientHandler);
        for (int i = 0; i < 5; i++)
        {
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(server.Url!)
            };
            await client.SendAsync(requestMessage);
        }

        // Assert
        server.Mappings.Should().HaveCount(2);
    }

    [Fact]
    public async Task WireMockServer_Proxy_AdminTrue_With_SaveMapping_Is_True_And_SaveMappingToFile_Is_False_Should_AddInternalMappingOnly()
    {
        // Assign
        var settings = new WireMockServerSettings
        {
            ProxyAndRecordSettings = new ProxyAndRecordSettings
            {
                Url = "http://www.google.com",
                SaveMapping = true,
                SaveMappingToFile = false,
                ExcludedHeaders = ["Connection"] // Needed for .NET 4.5.x and 4.6.x
            },
            StartAdminInterface = true
        };
        var server = WireMockServer.Start(settings);

        // Act
        for (int i = 0; i < 5; i++)
        {
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(server.Url!)
            };
            var httpClientHandler = new HttpClientHandler { AllowAutoRedirect = false };
            using var httpClient = new HttpClient(httpClientHandler);
            await httpClient.SendAsync(requestMessage);
        }

        // Assert
        server.Mappings.Should().HaveCount(Constants.NumAdminMappings + 2);
    }

    [Fact]
    public async Task WireMockServer_Proxy_With_SaveMappingToFile_Is_True_ShouldSaveMappingToFile()
    {
        // Assign
        string path = $"/prx_{Guid.NewGuid()}";
        var title = "IndexFile";
        var description = "IndexFile_Test";
        var stringBody = "<pretendXml>value</pretendXml>";
        var serverForProxyForwarding = WireMockServer.Start();
        var fileSystemHandlerMock = new Mock<IFileSystemHandler>();
        fileSystemHandlerMock.Setup(f => f.GetMappingFolder()).Returns("m");

        var settings = new WireMockServerSettings
        {
            ProxyAndRecordSettings = new ProxyAndRecordSettings
            {
                Url = serverForProxyForwarding.Urls[0],
                SaveMapping = false,
                SaveMappingToFile = true
            },
            FileSystemHandler = fileSystemHandlerMock.Object
        };

        var server = WireMockServer.Start(settings);
        server
            .Given(Request.Create()
                .WithPath("/*")
                .WithBody(new RegexMatcher(stringBody))
            )
            .WithTitle(title)
            .WithDescription(description)
            .AtPriority(WireMockConstants.ProxyPriority)
            .RespondWith(Response.Create().WithProxy(new ProxyAndRecordSettings
            {
                Url = serverForProxyForwarding.Urls[0],
                SaveMapping = false,
                SaveMappingToFile = true,
                UseDefinedRequestMatchers = true,
            }));

        // Act
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"{server.Urls[0]}{path}"),
            Content = new StringContent(stringBody)
        };
        var httpClientHandler = new HttpClientHandler { AllowAutoRedirect = false };
        await new HttpClient(httpClientHandler).SendAsync(requestMessage);

        // Assert
        server.Mappings.Should().HaveCount(2);

        // Verify
        fileSystemHandlerMock.Verify(f => f.WriteMappingFile($"m{Path.DirectorySeparatorChar}Proxy Mapping for _{title}.json", It.IsRegex(stringBody)), Times.Once);
    }

    [Fact]
    public async Task WireMockServer_Proxy_With_SaveMapping_Is_False_And_SaveMappingToFile_Is_True_ShouldSaveMappingToFile()
    {
        // Assign
        var fileSystemHandlerMock = new Mock<IFileSystemHandler>();
        fileSystemHandlerMock.Setup(f => f.GetMappingFolder()).Returns("m");

        var settings = new WireMockServerSettings
        {
            ProxyAndRecordSettings = new ProxyAndRecordSettings
            {
                Url = "http://www.google.com",
                SaveMapping = false,
                SaveMappingToFile = true
            },
            FileSystemHandler = fileSystemHandlerMock.Object
        };
        var server = WireMockServer.Start(settings);

        // Act
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(server.Urls[0])
        };
        var httpClientHandler = new HttpClientHandler { AllowAutoRedirect = false };
        using var httpClient = new HttpClient(httpClientHandler);
        await httpClient.SendAsync(requestMessage);

        // Assert
        server.Mappings.Should().HaveCount(1);

        // Verify
        fileSystemHandlerMock.Verify(f => f.WriteMappingFile(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task WireMockServer_Proxy_With_SaveMappingForStatusCodePattern_Is_False_Should_Not_SaveMapping()
    {
        // Assign
        var fileSystemHandlerMock = new Mock<IFileSystemHandler>();
        fileSystemHandlerMock.Setup(f => f.GetMappingFolder()).Returns("m");

        var settings = new WireMockServerSettings
        {
            ProxyAndRecordSettings = new ProxyAndRecordSettings
            {
                Url = "http://www.google.com",
                SaveMapping = true,
                SaveMappingToFile = true,
                SaveMappingForStatusCodePattern = "999" // Just make sure that we don't want this mapping
            },
            FileSystemHandler = fileSystemHandlerMock.Object
        };
        var server = WireMockServer.Start(settings);

        // Act
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(server.Urls[0])
        };
        var httpClientHandler = new HttpClientHandler { AllowAutoRedirect = false };
        using var httpClient = new HttpClient(httpClientHandler);
        await httpClient.SendAsync(requestMessage);

        // Assert
        server.Mappings.Should().HaveCount(1);

        // Verify
        fileSystemHandlerMock.Verify(f => f.WriteMappingFile(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task WireMockServer_Proxy_With_DoNotSaveMappingForHttpMethod_Should_Not_SaveMapping()
    {
        // Assign
        var fileSystemHandlerMock = new Mock<IFileSystemHandler>();
        fileSystemHandlerMock.Setup(f => f.GetMappingFolder()).Returns("m");

        var settings = new WireMockServerSettings
        {
            ProxyAndRecordSettings = new ProxyAndRecordSettings
            {
                Url = "http://www.google.com",
                SaveMapping = true,
                SaveMappingToFile = true,
                SaveMappingSettings = new ProxySaveMappingSettings
                {
                    HttpMethods = new ProxySaveMappingSetting<string[]>(new string[] { "GET" }, MatchBehaviour.RejectOnMatch) // To make sure that we don't want this mapping
                }
            },
            FileSystemHandler = fileSystemHandlerMock.Object
        };
        var server = WireMockServer.Start(settings);

        // Act
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(server.Urls[0])
        };
        var httpClientHandler = new HttpClientHandler { AllowAutoRedirect = false };
        using var httpClient = new HttpClient(httpClientHandler);
        await httpClient.SendAsync(requestMessage);

        // Assert
        server.Mappings.Should().HaveCount(1);

        // Verify
        fileSystemHandlerMock.Verify(f => f.WriteMappingFile(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task WireMockServer_Proxy_Should_log_proxied_requests()
    {
        // Assign
        var settings = new WireMockServerSettings
        {
            ProxyAndRecordSettings = new ProxyAndRecordSettings
            {
                Url = "http://www.google.com",
                SaveMapping = true,
                SaveMappingToFile = false
            }
        };
        var server = WireMockServer.Start(settings);

        // Act
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(server.Urls[0])
        };
        var httpClientHandler = new HttpClientHandler { AllowAutoRedirect = false };
        using var httpClient = new HttpClient(httpClientHandler);
        await httpClient.SendAsync(requestMessage);

        // Assert
        server.Mappings.Should().HaveCount(2);
        server.LogEntries.Should().HaveCount(1);
    }

    [Fact]
    public async Task WireMockServer_Proxy_Should_proxy_responses()
    {
        // Assign
        string path = $"/prx_{Guid.NewGuid()}";
        var server = WireMockServer.Start();
        server
            .Given(Request.Create().WithPath(path))
            .RespondWith(Response.Create().WithProxy("http://www.google.com"));

        // Act
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"{server.Urls[0]}{path}")
        };
        var httpClientHandler = new HttpClientHandler { AllowAutoRedirect = false };
        using var httpClient = new HttpClient(httpClientHandler);
        var response = await httpClient.SendAsync(requestMessage);
        string content = await response.Content.ReadAsStringAsync();

        // Assert
        Check.That(server.Mappings).HasSize(1);
        Check.That(server.LogEntries).HasSize(1);
        Check.That(content).Contains("google");

        server.Stop();
    }

    [Fact]
    public async Task WireMockServer_Proxy_Should_preserve_content_header_in_proxied_request()
    {
        // Assign
        string path = $"/prx_{Guid.NewGuid()}";
        var serverForProxyForwarding = WireMockServer.Start();
        serverForProxyForwarding
            .Given(Request.Create().WithPath(path))
            .RespondWith(Response.Create());

        var settings = new WireMockServerSettings
        {
            ProxyAndRecordSettings = new ProxyAndRecordSettings
            {
                Url = serverForProxyForwarding.Urls[0],
                SaveMapping = true,
                SaveMappingToFile = false
            }
        };
        var server = WireMockServer.Start(settings);

        // Act
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"{server.Urls[0]}{path}"),
            Content = new StringContent("stringContent", Encoding.ASCII)
        };
        requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
        requestMessage.Content.Headers.Add("bbb", "test");
        using var httpClient = new HttpClient();
        await httpClient.SendAsync(requestMessage);

        // Assert
        var receivedRequest = serverForProxyForwarding.LogEntries.First().RequestMessage;
        Check.That(receivedRequest.BodyData.BodyAsString).IsEqualTo("stringContent");
        Check.That(receivedRequest.Headers).ContainsKey("Content-Type");
        Check.That(receivedRequest.Headers["Content-Type"].First()).Contains("text/plain");
        Check.That(receivedRequest.Headers).ContainsKey("bbb");

        // check that new proxied mapping is added
        Check.That(server.Mappings).HasSize(2);
    }

    [Fact]
    public async Task WireMockServer_Proxy_Should_preserve_Authorization_header_in_proxied_request()
    {
        // Assign
        string path = $"/prx_{Guid.NewGuid()}";
        var serverForProxyForwarding = WireMockServer.Start();
        serverForProxyForwarding
            .Given(Request.Create().WithPath(path))
            .RespondWith(Response.Create().WithCallback(x => new ResponseMessage
            {
                BodyData = new BodyData
                {
                    BodyAsString = x.Headers!["Authorization"].ToString(),
                    DetectedBodyType = Types.BodyType.String
                }
            }));

        var settings = new WireMockServerSettings
        {
            ProxyAndRecordSettings = new ProxyAndRecordSettings
            {
                Url = serverForProxyForwarding.Urls[0],
                SaveMapping = true,
                SaveMappingToFile = false
            }
        };
        var server = WireMockServer.Start(settings);

        // Act
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"{server.Urls[0]}{path}"),
            Content = new StringContent("stringContent", Encoding.ASCII)
        };
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("BASIC", "test-A");
        var result = await new HttpClient().SendAsync(requestMessage);

        // Assert
        (await result.Content.ReadAsStringAsync()).Should().Be("BASIC test-A");

        var receivedRequest = serverForProxyForwarding.LogEntries.First().RequestMessage;
        receivedRequest.Headers!["Authorization"].ToString().Should().Be("BASIC test-A");

        server.Mappings.Should().HaveCount(2);
        var authorizationRequestMessageHeaderMatcher = ((Request)server.Mappings.Single(m => !m.IsAdminInterface).RequestMatcher)
            .GetRequestMessageMatcher<RequestMessageHeaderMatcher>(x => x.Matchers.Any(m => m.GetPatterns().Contains("BASIC test-A")));
        authorizationRequestMessageHeaderMatcher.Should().NotBeNull();
    }

    [Fact]
    public async Task WireMockServer_Proxy_Should_exclude_ExcludedHeaders_in_mapping()
    {
        // Assign
        string path = $"/prx_{Guid.NewGuid()}";
        var serverForProxyForwarding = WireMockServer.Start();
        serverForProxyForwarding
            .Given(Request.Create().WithPath(path))
            .RespondWith(Response.Create());

        var settings = new WireMockServerSettings
        {
            ProxyAndRecordSettings = new ProxyAndRecordSettings
            {
                Url = serverForProxyForwarding.Urls[0],
                SaveMapping = true,
                SaveMappingToFile = false,
                ExcludedHeaders = new[] { "excluded-header-X" }
            }
        };
        var server = WireMockServer.Start(settings);
        var defaultMapping = server.Mappings.First();

        // Act
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"{server.Urls[0]}{path}"),
            Content = new StringContent("stringContent")
        };
        requestMessage.Headers.Add("foobar", "exact_match");
        requestMessage.Headers.Add("ok", "ok-value");
        using var httpClient = new HttpClient();
        await httpClient.SendAsync(requestMessage);

        // Assert
        var mapping = server.Mappings.FirstOrDefault(m => m.Guid != defaultMapping.Guid);
        Check.That(mapping).IsNotNull();
        var matchers = ((Request)mapping.RequestMatcher).GetRequestMessageMatchers<RequestMessageHeaderMatcher>().Select(m => m.Name).ToList();
        Check.That(matchers).Not.Contains("excluded-header-X");
        Check.That(matchers).Contains("ok");
    }

    [Fact]
    public async Task WireMockServer_Proxy_Should_exclude_ExcludedCookies_in_mapping()
    {
        // Assign
        string path = $"/prx_{Guid.NewGuid()}";
        var serverForProxyForwarding = WireMockServer.Start();
        serverForProxyForwarding
            .Given(Request.Create().WithPath(path))
            .RespondWith(Response.Create());

        var settings = new WireMockServerSettings
        {
            ProxyAndRecordSettings = new ProxyAndRecordSettings
            {
                Url = serverForProxyForwarding.Urls[0],
                SaveMapping = true,
                SaveMappingToFile = false,
                ExcludedCookies = new[] { "ASP.NET_SessionId" }
            }
        };
        var server = WireMockServer.Start(settings);
        var defaultMapping = server.Mappings.First();

        // Act
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"{server.Urls[0]}{path}"),
            Content = new StringContent("stringContent")
        };

        var cookieContainer = new CookieContainer(3);
        cookieContainer.Add(new Uri("http://localhost"), new Cookie("ASP.NET_SessionId", "exact_match"));
        cookieContainer.Add(new Uri("http://localhost"), new Cookie("AsP.NeT_SessIonID", "case_mismatch"));
        cookieContainer.Add(new Uri("http://localhost"), new Cookie("GoodCookie", "I_should_pass"));

        var handler = new HttpClientHandler { CookieContainer = cookieContainer };
        using var httpClient = new HttpClient(handler);
        await httpClient.SendAsync(requestMessage);

        // Assert
        var mapping = server.Mappings.FirstOrDefault(m => m.Guid != defaultMapping.Guid);
        Check.That(mapping).IsNotNull();

        var matchers = ((Request)mapping.RequestMatcher).GetRequestMessageMatchers<RequestMessageCookieMatcher>().Select(m => m.Name).ToList();
        Check.That(matchers).Not.Contains("ASP.NET_SessionId");
        Check.That(matchers).Not.Contains("AsP.NeT_SessIonID");
        Check.That(matchers).Contains("GoodCookie");
    }

    [Fact]
    public async Task WireMockServer_Proxy_Should_exclude_ExcludedParams_in_mapping()
    {
        // Assign
        string path = $"/prx_{Guid.NewGuid()}";
        var serverForProxyForwarding = WireMockServer.Start();
        serverForProxyForwarding
            .Given(Request.Create().WithPath(path))
            .RespondWith(Response.Create());

        var settings = new WireMockServerSettings
        {
            ProxyAndRecordSettings = new ProxyAndRecordSettings
            {
                Url = serverForProxyForwarding.Urls[0],
                SaveMapping = true,
                SaveMappingToFile = false,
                ExcludedParams = new[] { "timestamp" }
            }
        };
        var server = WireMockServer.Start(settings);
        var defaultMapping = server.Mappings.First();
        var param01 = "?timestamp=2023-03-23";
        var param02 = "&name=person";

        // Act
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"{server.Urls[0]}{path}{param01}{param02}"),
            Content = new StringContent("stringContent"),
        };
        using var httpClient = new HttpClient();
        await httpClient.SendAsync(requestMessage);

        // Assert
        var mapping = server.Mappings.FirstOrDefault(m => m.Guid != defaultMapping.Guid);
        Check.That(mapping).IsNotNull();
        var matchers = ((Request)mapping.RequestMatcher).GetRequestMessageMatchers<RequestMessageParamMatcher>().Select(m => m.Key).ToList();
        Check.That(matchers).Not.Contains("timestamp");
        Check.That(matchers).Contains("name");
    }

    [Fact]
    public async Task WireMockServer_Proxy_Should_replace_old_path_value_with_new_path_value_in_replace_settings()
    {
        // Assign
        var replaceSettings = new ProxyUrlReplaceSettings
        {
            OldValue = "value-to-replace",
            NewValue = "new-value"
        };
        string path = $"/prx_{Guid.NewGuid()}";
        var serverForProxyForwarding = WireMockServer.Start();
        serverForProxyForwarding
            .Given(Request.Create().WithPath($"/{replaceSettings.NewValue}{path}"))
            .RespondWith(Response.Create());

        var settings = new WireMockServerSettings
        {
            ProxyAndRecordSettings = new ProxyAndRecordSettings
            {
                Url = serverForProxyForwarding.Urls[0],
                SaveMapping = true,
                SaveMappingToFile = false,
                ReplaceSettings = replaceSettings
            }
        };
        var server = WireMockServer.Start(settings);
        var defaultMapping = server.Mappings.First();

        // Act
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"{server.Urls[0]}/{replaceSettings.OldValue}{path}"),
            Content = new StringContent("stringContent")
        };

        var handler = new HttpClientHandler();
        using var httpClient = new HttpClient(handler);
        await httpClient.SendAsync(requestMessage);

        // Assert
        var mapping = serverForProxyForwarding.Mappings.FirstOrDefault(m => m.Guid != defaultMapping.Guid);
        var score = mapping.RequestMatcher.GetMatchingScore(serverForProxyForwarding.LogEntries.First().RequestMessage,
            new RequestMatchResult());
        Check.That(score).IsEqualTo(1.0);
    }

    [Fact]
    public async Task WireMockServer_Proxy_Should_preserve_content_header_in_proxied_request_with_empty_content()
    {
        // Assign
        string path = $"/prx_{Guid.NewGuid()}";
        var serverForProxyForwarding = WireMockServer.Start();
        serverForProxyForwarding
            .Given(Request.Create().WithPath(path))
            .RespondWith(Response.Create());

        var server = WireMockServer.Start();
        server
            .Given(Request.Create().WithPath("/*"))
            .RespondWith(Response.Create().WithProxy(serverForProxyForwarding.Urls[0]));

        // Act
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"{server.Urls[0]}{path}"),
            Content = new StringContent("")
        };
        requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
        using var httpClient = new HttpClient();
        await httpClient.SendAsync(requestMessage);

        // Assert
        var receivedRequest = serverForProxyForwarding.LogEntries.First().RequestMessage;
        Check.That(receivedRequest.BodyData.BodyAsString).IsEqualTo("");
        Check.That(receivedRequest.Headers).ContainsKey("Content-Type");
        Check.That(receivedRequest.Headers["Content-Type"].First()).Contains("text/plain");
    }

    [Fact]
    public async Task WireMockServer_Proxy_Should_preserve_content_header_in_proxied_response()
    {
        // Assign
        string path = $"/prx_{Guid.NewGuid()}";
        var serverForProxyForwarding = WireMockServer.Start();
        serverForProxyForwarding
            .Given(Request.Create().WithPath(path))
            .RespondWith(Response.Create()
                .WithBody("body")
                .WithHeader("Content-Type", "text/plain"));

        var server = WireMockServer.Start();
        server
            .Given(Request.Create().WithPath(path))
            .RespondWith(Response.Create().WithProxy(serverForProxyForwarding.Urls[0]));

        // Act
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"{server.Urls[0]}{path}")
        };
        using var httpClient = new HttpClient();
        var response = await httpClient.SendAsync(requestMessage);

        // Assert
        Check.That(await response.Content.ReadAsStringAsync()).IsEqualTo("body");
        Check.That(response.Content.Headers.Contains("Content-Type")).IsTrue();
        Check.That(response.Content.Headers.GetValues("Content-Type")).ContainsExactly("text/plain");
    }

    [Fact]
    public async Task WireMockServer_Proxy_Should_change_absolute_location_header_in_proxied_response()
    {
        // Assign
        string path = $"/prx_{Guid.NewGuid()}";
        var settings = new WireMockServerSettings { AllowPartialMapping = false };

        var serverForProxyForwarding = WireMockServer.Start(settings);
        serverForProxyForwarding
            .Given(Request.Create().WithPath(path))
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.Redirect)
                .WithHeader("Location", "/testpath"));

        var server = WireMockServer.Start(settings);
        server
            .Given(Request.Create().WithPath(path).UsingAnyMethod())
            .RespondWith(Response.Create().WithProxy(serverForProxyForwarding.Urls[0]));

        // Act
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"{server.Urls[0]}{path}")
        };
        var httpClientHandler = new HttpClientHandler { AllowAutoRedirect = false };
        using var httpClient = new HttpClient(httpClientHandler);
        var response = await httpClient.SendAsync(requestMessage);

        // Assert
        Check.That(response.Headers.Contains("Location")).IsTrue();
        Check.That(response.Headers.GetValues("Location")).ContainsExactly("/testpath");
    }

    [Fact]
    public async Task WireMockServer_Proxy_Should_preserve_cookie_header_in_proxied_request()
    {
        // Assign
        string path = $"/prx_{Guid.NewGuid()}";
        var serverForProxyForwarding = WireMockServer.Start();
        serverForProxyForwarding
            .Given(Request.Create().WithPath(path))
            .RespondWith(Response.Create());

        var server = WireMockServer.Start();
        server
            .Given(Request.Create().WithPath(path))
            .RespondWith(Response.Create().WithProxy(serverForProxyForwarding.Urls[0]));

        // Act
        var requestUri = new Uri($"{server.Urls[0]}{path}");
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = requestUri
        };
        var clientHandler = new HttpClientHandler();
        clientHandler.CookieContainer.Add(requestUri, new Cookie("name", "value"));
        using var httpClient = new HttpClient(clientHandler);
        await httpClient.SendAsync(requestMessage);

        // then
        var receivedRequest = serverForProxyForwarding.LogEntries.First().RequestMessage;
        Check.That(receivedRequest.Cookies).IsNotNull();
        Check.That(receivedRequest.Cookies).ContainsPair("name", "value");
    }

    /// <summary>
    /// Send some binary content in a request through the proxy and check that the same content
    /// arrived at the target. As example a JPEG/JIFF header is used, which is not representable
    /// in UTF8 and breaks if it is not treated as binary content.
    /// </summary>
    [Fact]
    public async Task WireMockServer_Proxy_Should_preserve_binary_request_content()
    {
        // arrange
        var jpegHeader = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46, 0x00 };
        var brokenJpegHeader = new byte[]
            {0xEF, 0xBF, 0xBD, 0xEF, 0xBF, 0xBD, 0xEF, 0xBF, 0xBD, 0xEF, 0xBF, 0xBD, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46, 0x00};

        bool HasCorrectHeader(byte[]? bytes) => bytes?.SequenceEqual(jpegHeader) == true;
        bool HasBrokenHeader(byte[]? bytes) => bytes?.SequenceEqual(brokenJpegHeader) == true;

        var serverForProxyForwarding = WireMockServer.Start();
        serverForProxyForwarding
            .Given(Request.Create().WithBody(HasCorrectHeader))
            .RespondWith(Response.Create().WithSuccess());

        serverForProxyForwarding
            .Given(Request.Create().WithBody(HasBrokenHeader))
            .RespondWith(Response.Create().WithStatusCode(HttpStatusCode.InternalServerError));

        var server = WireMockServer.Start();
        server
            .Given(Request.Create())
            .RespondWith(Response.Create().WithProxy(serverForProxyForwarding.Urls[0]));

        // act
        using var httpClient = new HttpClient();
        var response = await httpClient.PostAsync(server.Urls[0], new ByteArrayContent(jpegHeader));

        // assert
        Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
    }

    [Fact]
    public async Task WireMockServer_Proxy_Should_set_BodyAsJson_in_proxied_response()
    {
        // Assign
        string path = $"/prx_{Guid.NewGuid()}";
        var serverForProxyForwarding = WireMockServer.Start();
        serverForProxyForwarding
            .Given(Request.Create().WithPath(path))
            .RespondWith(Response.Create()
                .WithBodyAsJson(new { i = 42 })
                .WithHeader("Content-Type", "application/json; charset=utf-8"));

        var server = WireMockServer.Start();
        server
            .Given(Request.Create().WithPath(path))
            .RespondWith(Response.Create().WithProxy(serverForProxyForwarding.Urls[0]));

        // Act
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"{server.Urls[0]}{path}")
        };
        using var httpClient = new HttpClient();
        var response = await httpClient.SendAsync(requestMessage);

        // Assert
        string content = await response.Content.ReadAsStringAsync();
        Check.That(content).IsEqualTo("{\"i\":42}");
        Check.That(response.Content.Headers.GetValues("Content-Type")).ContainsExactly("application/json; charset=utf-8");
    }

    [Fact]
    public async Task WireMockServer_Proxy_Should_set_Body_in_multipart_proxied_response()
    {
        // Assign
        string path = $"/prx_{Guid.NewGuid()}";
        var serverForProxyForwarding = WireMockServer.Start();
        serverForProxyForwarding
            .Given(Request.Create().WithPath(path))
            .RespondWith(Response.Create()
                .WithBodyAsJson(new { i = 42 })
            );

        var server = WireMockServer.Start();
        server
            .Given(Request.Create().WithPath(path))
            .RespondWith(Response.Create().WithProxy(serverForProxyForwarding.Urls[0]));

        // Act
        var uri = new Uri($"{server.Urls[0]}{path}");
        var form = new MultipartFormDataContent
        {
            { new StringContent("data"), "test", "test.txt" }
        };
        using var httpClient = new HttpClient();
        var response = await httpClient.PostAsync(uri, form);

        // Assert
        string content = await response.Content.ReadAsStringAsync();
        Check.That(content).IsEqualTo("{\"i\":42}");
    }

    [Fact]
    public async Task WireMockServer_Proxy_Should_Not_overrule_AdminMappings()
    {
        // Assign
        string path = $"/prx_{Guid.NewGuid()}";
        var serverForProxyForwarding = WireMockServer.Start();
        serverForProxyForwarding
            .Given(Request.Create().WithPath(path))
            .RespondWith(Response.Create().WithBody("ok"));

        var server = WireMockServer.Start(new WireMockServerSettings
        {
            StartAdminInterface = true,
            ReadStaticMappings = false,
            ProxyAndRecordSettings = new ProxyAndRecordSettings
            {
                Url = serverForProxyForwarding.Urls[0],
                SaveMapping = false,
                SaveMappingToFile = false
            }
        });

        // Act 1
        var requestMessage1 = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"{server.Urls[0]}{path}")
        };
        var response1 = await new HttpClient().SendAsync(requestMessage1);

        // Assert 1
        string content1 = await response1.Content.ReadAsStringAsync();
        Check.That(content1).IsEqualTo("ok");

        // Act 2
        var requestMessage2 = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"{server.Urls[0]}/__admin/mappings")
        };
        var response2 = await new HttpClient().SendAsync(requestMessage2);

        // Assert 2
        string content2 = await response2.Content.ReadAsStringAsync();
        Check.That(content2).IsEqualTo("[]");
    }

    // On Ubuntu latest it's : "Resource temporarily unavailable"
    // On Windows-2019 it's : "No such host is known."
    [Fact]
    public async Task WireMockServer_Proxy_WhenTargetIsNotAvailable_Should_Return_CorrectResponse()
    {
        // Assign
        var settings = new WireMockServerSettings
        {
            ProxyAndRecordSettings = new ProxyAndRecordSettings
            {
                Url = $"http://error{Guid.NewGuid()}:12345"
            }
        };
        var server = WireMockServer.Start(settings);

        // Act
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(server.Urls[0])
        };
        var httpClientHandler = new HttpClientHandler { AllowAutoRedirect = false };
        using var httpClient = new HttpClient(httpClientHandler);
        var result = await httpClient.SendAsync(requestMessage);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var content = await result.Content.ReadAsStringAsync();
        content.Should().NotBeEmpty();

        server.LogEntries.Should().HaveCount(1);
        server.Stop();
    }
}