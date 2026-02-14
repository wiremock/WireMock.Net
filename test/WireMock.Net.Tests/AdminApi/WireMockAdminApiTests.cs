// Copyright © WireMock.Net

using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using AwesomeAssertions;
using Moq;
using NFluent;
using RestEase;
using WireMock.Admin.Mappings;
using WireMock.Admin.Scenarios;
using WireMock.Admin.Settings;
using WireMock.Client;
using WireMock.Client.Extensions;
using WireMock.Handlers;
using WireMock.Logging;
using WireMock.Matchers;
using WireMock.Net.Tests.VerifyExtensions;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;
using WireMock.Types;
using WireMock.Util;

namespace WireMock.Net.Tests.AdminApi;

public partial class WireMockAdminApiTests
{
    private static readonly VerifySettings VerifySettings = new();

    private readonly Mock<IGuidUtils> _guidUtilsMock = new();

    public WireMockAdminApiTests()
    {
        var startGuid = 1000;
        _guidUtilsMock.Setup(g => g.NewGuid()).Returns(() => new Guid($"98fae52e-76df-47d9-876f-2ee32e93{startGuid++}"));
    }

    static WireMockAdminApiTests()
    {
        VerifyNewtonsoftJson.Enable(VerifySettings);
    }

    [Fact]
    public async Task IWireMockAdminApi_WaitForHealthAsync_AndCall_GetHealthAsync_OK()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        var adminUsername = $"username_{Guid.NewGuid()}";
        var adminPassword = $"password_{Guid.NewGuid()}";
        var server = WireMockServer.Start(w =>
        {
            w.StartAdminInterface = true;
            w.AdminUsername = adminUsername;
            w.AdminPassword = adminPassword;
        });
        var api = RestClient.For<IWireMockAdminApi>(server.Urls[0])
            .WithAuthorization(adminUsername, adminPassword);

        // Act 1
        await api.WaitForHealthAsync(cancellationToken: cancellationToken);

        // Act 2
        var status = await api.GetHealthAsync(cancellationToken);
        status.Should().Be("Healthy");
    }

    [Fact]
    public async Task IWireMockAdminApi_WaitForHealthAsync_AndCall_GetHealthAsync_ThrowsException()
    {
        // Arrange
        var server = WireMockServer.Start(w =>
        {
            w.StartAdminInterface = true;
            w.AdminUsername = $"username_{Guid.NewGuid()}";
            w.AdminPassword = $"password_{Guid.NewGuid()}";
        });

        var api = RestClient.For<IWireMockAdminApi>(server.Urls[0]);

        // Act
        Func<Task> act = () => api.WaitForHealthAsync(maxRetries: 3, cancellationToken: TestContext.Current.CancellationToken);
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task IWireMockAdminApi_GetSettingsAsync()
    {
        // Arrange
        var server = WireMockServer.StartWithAdminInterface();
        var api = RestClient.For<IWireMockAdminApi>(server.Urls[0]);

        // Act
        var settings = await api.GetSettingsAsync(TestContext.Current.CancellationToken);
        Check.That(settings).IsNotNull();
    }

    [Fact]
    public async Task IWireMockAdminApi_GetSettingsAsync_ForDifferentAdminPath()
    {
        // Arrange
        var server = WireMockServer.Start(w =>
        {
            w.StartAdminInterface = true;
            w.AdminPath = "/foo/__admin";
        });
        var api = RestClient.For<IWireMockAdminApi>(server.Urls[0] + "/foo");

        // Act
        var settings = await api.GetSettingsAsync(TestContext.Current.CancellationToken);

        // Assert
        Check.That(settings).IsNotNull();

        // Cleanup
        server.Stop();
        server.Dispose();
    }

    [Fact]
    public async Task IWireMockAdminApi_PostSettingsAsync()
    {
        // Arrange
        var server = WireMockServer.StartWithAdminInterface();
        var api = RestClient.For<IWireMockAdminApi>(server.Urls[0]);

        // Act
        var settings = new SettingsModel();
        var status = await api.PostSettingsAsync(settings, TestContext.Current.CancellationToken);
        Check.That(status.Status).Equals("Settings updated");
    }

    [Fact]
    public async Task IWireMockAdminApi_PutSettingsAsync()
    {
        // Arrange
        var server = WireMockServer.StartWithAdminInterface();
        var api = RestClient.For<IWireMockAdminApi>(server.Urls[0]);

        // Act
        var settings = new SettingsModel();
        var status = await api.PutSettingsAsync(settings, TestContext.Current.CancellationToken);
        Check.That(status.Status).Equals("Settings updated");
    }

    // https://github.com/wiremock/WireMock.Net/issues/325
    [Fact]
    public async Task IWireMockAdminApi_PutMappingAsync()
    {
        // Arrange
        var server = WireMockServer.StartWithAdminInterface();
        var api = RestClient.For<IWireMockAdminApi>(server.Urls[0]);

        // Act
        var model = new MappingModel
        {
            Request = new RequestModel { Path = "/1" },
            Response = new ResponseModel { Body = "txt", StatusCode = 200 },
            Priority = 500,
            Title = "test"
        };
        var result = await api.PutMappingAsync(new Guid("a0000000-0000-0000-0000-000000000000"), model, TestContext.Current.CancellationToken);

        // Assert
        Check.That(result).IsNotNull();
        Check.That(result.Status).Equals("Mapping added or updated");
        Check.That(result.Guid).IsNotNull();

        var mapping = server.Mappings.Single(m => m.Priority == 500);
        Check.That(mapping).IsNotNull();
        Check.That(mapping.Title).Equals("test");

        server.Stop();
    }



    [Fact]
    public async Task IWireMockAdminApi_FindRequestsAsync()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        var server = WireMockServer.Start(new WireMockServerSettings
        {
            StartAdminInterface = true,
            Logger = new WireMockNullLogger()
        });
        server
            .Given(Request.Create().WithPath("/foo").UsingGet())
            .RespondWith(Response.Create());

        var serverUrl = "http://localhost:" + server.Ports[0];
        using var httpClient = new HttpClient();
        await httpClient.GetAsync(serverUrl + "/foo", cancellationToken);
        var api = RestClient.For<IWireMockAdminApi>(serverUrl);

        // Act
        var requests = await api.FindRequestsAsync(new RequestModel { Methods = new[] { "GET" } }, cancellationToken);

        // Assert
        requests.Should().HaveCount(1);
        var requestLogged = requests.First();
        requestLogged.Request.Method.Should().Be("GET");
        requestLogged.Request.Body.Should().BeNull();
        requestLogged.Request.Path.Should().Be("/foo");
    }

    [Fact]
    public async Task IWireMockAdminApi_FindRequestsByMappingGuidAsync_Found()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        var mappingGuid = Guid.NewGuid();
        var server = WireMockServer.Start(new WireMockServerSettings
        {
            StartAdminInterface = true,
            Logger = new WireMockNullLogger()
        });
        server
            .Given(Request.Create().WithPath("/foo").UsingGet())
            .WithGuid(mappingGuid)
            .RespondWith(Response.Create());

        var serverUrl = "http://localhost:" + server.Ports[0];
        using var client = new HttpClient();
        await client.GetAsync(serverUrl + "/foo", cancellationToken);
        await client.GetAsync(serverUrl + "/foo?bar=baz", cancellationToken);
        var api = RestClient.For<IWireMockAdminApi>(serverUrl);

        // Act
        var logEntryModels = await api.FindRequestsByMappingGuidAsync(mappingGuid, cancellationToken);

        // Assert
        logEntryModels.Should().HaveCount(2);
        logEntryModels[0].Should().NotBeNull();
        logEntryModels[0]!.Request.Method.Should().Be("GET");
        logEntryModels[0]!.Request.Body.Should().BeNull();
        logEntryModels[0]!.Request.Path.Should().Be("/foo");
        logEntryModels[0]!.Request.Query.Should().BeNullOrEmpty();
        logEntryModels[1].Should().NotBeNull();
        logEntryModels[1]!.Request.Method.Should().Be("GET");
        logEntryModels[1]!.Request.Body.Should().BeNull();
        logEntryModels[1]!.Request.Path.Should().Be("/foo");
        logEntryModels[1]!.Request.Query.Should().BeEquivalentTo(new Dictionary<string, WireMockList<string>>
        {
            {"bar", new WireMockList<string>("baz")}
        });
    }

    [Fact]
    public async Task IWireMockAdminApi_FindRequestsByMappingGuidAsync_NotFound()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        var server = WireMockServer.Start(new WireMockServerSettings
        {
            StartAdminInterface = true,
            Logger = new WireMockNullLogger()
        });
        server
            .Given(Request.Create().WithPath("/foo").UsingGet())
            .WithGuid(Guid.NewGuid())
            .RespondWith(Response.Create());

        var serverUrl = "http://localhost:" + server.Ports[0];
        using var httpClient = new HttpClient();
        await httpClient.GetAsync(serverUrl + "/foo", cancellationToken);
        var api = RestClient.For<IWireMockAdminApi>(serverUrl);

        // Act
        var logEntryModels = await api.FindRequestsByMappingGuidAsync(Guid.NewGuid(), cancellationToken);

        // Assert
        logEntryModels.Should().BeEmpty();
    }

    [Fact]
    public async Task IWireMockAdminApi_FindRequestsByMappingGuidAsync_Invalid_ShouldReturnBadRequest()
    {
        // Arrange
        var server = WireMockServer.Start(new WireMockServerSettings
        {
            StartAdminInterface = true,
            Logger = new WireMockNullLogger()
        });

        // Act
        var result = await server.CreateClient().GetAsync("/__admin/requests/find?mappingGuid=x", TestContext.Current.CancellationToken);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task IWireMockAdminApi_GetRequestsAsync()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        var server = WireMockServer.Start(new WireMockServerSettings
        {
            StartAdminInterface = true,
            Logger = new WireMockNullLogger()
        });
        var serverUrl = "http://localhost:" + server.Ports[0];
        using var httpClient = new HttpClient();
        await httpClient.GetAsync(serverUrl + "/foo", cancellationToken);
        var api = RestClient.For<IWireMockAdminApi>(serverUrl);

        // Act
        var requests = await api.GetRequestsAsync(cancellationToken);

        // Assert
        Check.That(requests).HasSize(1);
        var requestLogged = requests.First();
        Check.That(requestLogged.Request.Method).IsEqualTo("GET");
        Check.That(requestLogged.Request.Body).IsNull();
        Check.That(requestLogged.Request.Path).IsEqualTo("/foo");
    }

    [Fact]
    public async Task IWireMockAdminApi_GetRequestsAsync_JsonApi()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        var server = WireMockServer.Start(new WireMockServerSettings
        {
            StartAdminInterface = true,
            Logger = new WireMockNullLogger()
        });
        string serverUrl = server.Urls[0];
        string data = "{\"data\":[{\"type\":\"program\",\"attributes\":{\"alias\":\"T000001\",\"title\":\"Title Group Entity\"}}]}";
        string jsonApiAcceptHeader = "application/vnd.api+json";
        string jsonApiContentType = "application/vnd.api+json";

        var request = new HttpRequestMessage(HttpMethod.Post, serverUrl);
        request.Headers.Accept.Clear();
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(jsonApiAcceptHeader));
        request.Content = new StringContent(data);
        request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(jsonApiContentType);

        using var client = new HttpClient();
        var response = await client.SendAsync(request, cancellationToken);
        Check.That(response).IsNotNull();

        var api = RestClient.For<IWireMockAdminApi>(serverUrl);

        // Act
        var requests = await api.GetRequestsAsync(cancellationToken);

        // Assert
        Check.That(requests).HasSize(1);
        var requestLogged = requests.First();
        Check.That(requestLogged.Request.Method).IsEqualTo("POST");
        Check.That(requestLogged.Request.Body).IsNotNull();
        Check.That(requestLogged.Request.Body).Contains("T000001");
    }

    [Fact]
    public async Task IWireMockAdminApi_GetMappingAsync_WithBodyModelMatcherModel_WithoutMethods_ShouldReturnCorrectMappingModel()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        var guid = Guid.Parse("90356dba-b36c-469a-a17e-669cd84f1f05");
        var server = WireMockServer.StartWithAdminInterface();
        var api = RestClient.For<IWireMockAdminApi>(server.Url);

        // Act
        var model = new MappingModel
        {
            Guid = guid,
            Request = new RequestModel
            {
                Path = "/1",
                Body = new BodyModel
                {
                    Matcher = new MatcherModel
                    {
                        Name = "RegexMatcher",
                        Pattern = "hello",
                        IgnoreCase = true
                    }
                }
            },
            Response = new ResponseModel { Body = "world" }
        };
        var postMappingResult = await api.PostMappingAsync(model, cancellationToken);

        // Assert
        postMappingResult.Should().NotBeNull();

        var mapping = server.Mappings.FirstOrDefault(m => m.Guid == guid);
        mapping.Should().NotBeNull();

        var getMappingResult = await api.GetMappingAsync(guid, cancellationToken);

        await Verify(getMappingResult, VerifySettings).DontScrubGuids();

        server.Stop();
    }

    [Fact]
    public async Task IWireMockAdminApi_GetMappingAsync_WithProxy_And_ProxyUrlReplaceSettings()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        var guid = Guid.Parse("90356dba-b36c-469a-a17e-669cd84f1f05");
        var server = WireMockServer.StartWithAdminInterface();
        var api = RestClient.For<IWireMockAdminApi>(server.Url);

        // Act
        var model = new MappingModel
        {
            Guid = guid,
            Request = new RequestModel
            {
                Path = "/1",
                Body = new BodyModel
                {
                    Matcher = new MatcherModel
                    {
                        Name = "RegexMatcher",
                        Pattern = "hello",
                        IgnoreCase = true
                    }
                }
            },
            Response = new ResponseModel
            {
                ProxyUrl = "https://my-proxy.com",
                ProxyUrlReplaceSettings = new ProxyUrlReplaceSettingsModel
                {
                    OldValue = "x",
                    NewValue = "y",
                    IgnoreCase = true
                }
            }
        };
        var postMappingResult = await api.PostMappingAsync(model, cancellationToken);

        // Assert
        postMappingResult.Should().NotBeNull();

        var mapping = server.Mappings.FirstOrDefault(m => m.Guid == guid);
        mapping.Should().NotBeNull();

        var getMappingResult = await api.GetMappingAsync(guid, cancellationToken);

        await Verify(getMappingResult, VerifySettings).DontScrubGuids();

        server.Stop();
    }

    [Fact]
    public async Task IWireMockAdminApi_GetMappingAsync_WithProxy_And_ProxyUrlReplaceSettings_And_TransformTemplate()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        var guid = Guid.Parse("90356dba-b36c-469a-a17e-669cd84f1f06");
        var server = WireMockServer.StartWithAdminInterface();
        var api = RestClient.For<IWireMockAdminApi>(server.Url);

        // Act
        var model = new MappingModel
        {
            Guid = guid,
            Request = new RequestModel
            {
                Path = "/1",
                Body = new BodyModel
                {
                    Matcher = new MatcherModel
                    {
                        Name = "RegexMatcher",
                        Pattern = "hello",
                        IgnoreCase = true
                    }
                }
            },
            Response = new ResponseModel
            {
                ProxyUrl = "https://my-proxy.com",
                ProxyUrlReplaceSettings = new ProxyUrlReplaceSettingsModel
                {
                    TransformTemplate = "x{{this}}y"
                }
            }
        };
        var postMappingResult = await api.PostMappingAsync(model, cancellationToken);

        // Assert
        postMappingResult.Should().NotBeNull();

        var mapping = server.Mappings.FirstOrDefault(m => m.Guid == guid);
        mapping.Should().NotBeNull();

        var getMappingResult = await api.GetMappingAsync(guid, cancellationToken);

        await Verify(getMappingResult, VerifySettings).DontScrubGuids();

        server.Stop();
    }

    [Fact]
    public async Task IWireMockAdminApi_GetRequestsAsync_Json()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        var server = WireMockServer.Start(new WireMockServerSettings
        {
            StartAdminInterface = true,
            Logger = new WireMockNullLogger()
        });
        string serverUrl = server.Urls[0];
        string data = "{\"alias\": \"T000001\"}";
        string jsonAcceptHeader = "application/json";
        string jsonApiContentType = "application/json";

        var request = new HttpRequestMessage(HttpMethod.Post, serverUrl);
        request.Headers.Accept.Clear();
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(jsonAcceptHeader));
        request.Content = new StringContent(data);
        request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(jsonApiContentType);
        using var client = new HttpClient();
        var response = await client.SendAsync(request, cancellationToken);
        Check.That(response).IsNotNull();

        var api = RestClient.For<IWireMockAdminApi>(serverUrl);

        // Act
        var requests = await api.GetRequestsAsync(cancellationToken);

        // Assert
        Check.That(requests).HasSize(1);
        var requestLogged = requests.First();
        Check.That(requestLogged.Request.Method).IsEqualTo("POST");
        Check.That(requestLogged.Request.Body).IsNotNull();
        Check.That(requestLogged.Request.Body).Contains("T000001");
    }

    [Fact]
    public async Task IWireMockAdminApi_PostFileAsync_Ascii()
    {
        // Arrange
        var filesystemHandlerMock = new Mock<IFileSystemHandler>(MockBehavior.Strict);
        filesystemHandlerMock.Setup(fs => fs.GetMappingFolder()).Returns("__admin/mappings");
        filesystemHandlerMock.Setup(fs => fs.FolderExists(It.IsAny<string>())).Returns(true);
        filesystemHandlerMock.Setup(fs => fs.WriteFile(It.IsAny<string>(), It.IsAny<byte[]>()));

        var server = WireMockServer.Start(new WireMockServerSettings
        {
            UseSSL = false,
            StartAdminInterface = true,
            FileSystemHandler = filesystemHandlerMock.Object
        });

        var api = RestClient.For<IWireMockAdminApi>(server.Urls[0]);

        // Act
        var request = await api.PostFileAsync("filename.txt", "abc", TestContext.Current.CancellationToken);

        // Assert
        Check.That(request.Guid).IsNull();
        Check.That(request.Status).Contains("File");

        // Verify
        filesystemHandlerMock.Verify(fs => fs.GetMappingFolder(), Times.Once);
        filesystemHandlerMock.Verify(fs => fs.FolderExists(It.IsAny<string>()), Times.Once);
        filesystemHandlerMock.Verify(fs => fs.WriteFile(It.Is<string>(p => p == "filename.txt"), It.IsAny<byte[]>()), Times.Once);
        filesystemHandlerMock.VerifyNoOtherCalls();

        server.Stop();
    }

    [Fact]
    public async Task IWireMockAdminApi_PutFileAsync_Ascii()
    {
        // Arrange
        var filesystemHandlerMock = new Mock<IFileSystemHandler>(MockBehavior.Strict);
        filesystemHandlerMock.Setup(fs => fs.FileExists(It.IsAny<string>())).Returns(true);
        filesystemHandlerMock.Setup(fs => fs.WriteFile(It.IsAny<string>(), It.IsAny<byte[]>()));

        var server = WireMockServer.Start(new WireMockServerSettings
        {
            UseSSL = false,
            StartAdminInterface = true,
            FileSystemHandler = filesystemHandlerMock.Object
        });

        var api = RestClient.For<IWireMockAdminApi>(server.Urls[0]);

        // Act
        var request = await api.PutFileAsync("filename.txt", "abc-abc", TestContext.Current.CancellationToken);

        // Assert
        Check.That(request.Guid).IsNull();
        Check.That(request.Status).Contains("File");

        // Verify
        filesystemHandlerMock.Verify(fs => fs.WriteFile(It.Is<string>(p => p == "filename.txt"), It.IsAny<byte[]>()), Times.Once);
        filesystemHandlerMock.Verify(fs => fs.FileExists(It.Is<string>(p => p == "filename.txt")), Times.Once);
        filesystemHandlerMock.VerifyNoOtherCalls();

        server.Stop();
    }

    [Fact]
    public void IWireMockAdminApi_PutFileAsync_NotFound()
    {
        // Arrange
        var filesystemHandlerMock = new Mock<IFileSystemHandler>(MockBehavior.Strict);
        filesystemHandlerMock.Setup(fs => fs.FileExists(It.IsAny<string>())).Returns(false);

        var server = WireMockServer.Start(new WireMockServerSettings
        {
            UseSSL = false,
            StartAdminInterface = true,
            FileSystemHandler = filesystemHandlerMock.Object
        });

        var api = RestClient.For<IWireMockAdminApi>(server.Urls[0]);

        // Act and Assert
        Check.ThatCode(() => api.PutFileAsync("filename.txt", "xxx", TestContext.Current.CancellationToken)).Throws<ApiException>();

        // Verify
        filesystemHandlerMock.Verify(fs => fs.FileExists(It.Is<string>(p => p == "filename.txt")), Times.Once);
        filesystemHandlerMock.VerifyNoOtherCalls();

        server.Stop();
    }

    [Fact]
    public void IWireMockAdminApi_GetFileAsync_NotFound()
    {
        // Arrange
        var filesystemHandlerMock = new Mock<IFileSystemHandler>(MockBehavior.Strict);
        filesystemHandlerMock.Setup(fs => fs.FileExists(It.IsAny<string>())).Returns(false);
        filesystemHandlerMock.Setup(fs => fs.ReadFile(It.IsAny<string>())).Returns(Encoding.ASCII.GetBytes("Here's a string."));

        var server = WireMockServer.Start(new WireMockServerSettings
        {
            UseSSL = false,
            StartAdminInterface = true,
            FileSystemHandler = filesystemHandlerMock.Object
        });

        var api = RestClient.For<IWireMockAdminApi>(server.Urls[0]);

        // Act and Assert
        Check.ThatCode(() => api.GetFileAsync("filename.txt", TestContext.Current.CancellationToken)).Throws<ApiException>();

        // Verify
        filesystemHandlerMock.Verify(fs => fs.FileExists(It.Is<string>(p => p == "filename.txt")), Times.Once);
        filesystemHandlerMock.VerifyNoOtherCalls();

        server.Stop();
    }

    [Fact]
    public async Task IWireMockAdminApi_GetFileAsync_Found()
    {
        // Arrange
        string data = "Here's a string.";
        var filesystemHandlerMock = new Mock<IFileSystemHandler>(MockBehavior.Strict);
        filesystemHandlerMock.Setup(fs => fs.FileExists(It.IsAny<string>())).Returns(true);
        filesystemHandlerMock.Setup(fs => fs.ReadFile(It.IsAny<string>())).Returns(Encoding.ASCII.GetBytes(data));

        var server = WireMockServer.Start(new WireMockServerSettings
        {
            UseSSL = false,
            StartAdminInterface = true,
            FileSystemHandler = filesystemHandlerMock.Object
        });

        var api = RestClient.For<IWireMockAdminApi>(server.Urls[0]);

        // Act
        string file = await api.GetFileAsync("filename.txt", TestContext.Current.CancellationToken);

        // Assert
        Check.That(file).Equals(data);

        // Verify
        filesystemHandlerMock.Verify(fs => fs.FileExists(It.Is<string>(p => p == "filename.txt")), Times.Once);
        filesystemHandlerMock.Verify(fs => fs.ReadFile(It.Is<string>(p => p == "filename.txt")), Times.Once);
        filesystemHandlerMock.VerifyNoOtherCalls();

        server.Stop();
    }

    [Fact]
    public async Task IWireMockAdminApi_DeleteFileAsync_Ok()
    {
        // Arrange
        var filesystemHandlerMock = new Mock<IFileSystemHandler>(MockBehavior.Strict);
        filesystemHandlerMock.Setup(fs => fs.FileExists(It.IsAny<string>())).Returns(true);
        filesystemHandlerMock.Setup(fs => fs.DeleteFile(It.IsAny<string>()));

        var server = WireMockServer.Start(new WireMockServerSettings
        {
            UseSSL = false,
            StartAdminInterface = true,
            FileSystemHandler = filesystemHandlerMock.Object
        });

        var api = RestClient.For<IWireMockAdminApi>(server.Urls[0]);

        // Act
        await api.DeleteFileAsync("filename.txt", TestContext.Current.CancellationToken);

        // Verify
        filesystemHandlerMock.Verify(fs => fs.FileExists(It.Is<string>(p => p == "filename.txt")), Times.Once);
        filesystemHandlerMock.Verify(fs => fs.DeleteFile(It.Is<string>(p => p == "filename.txt")), Times.Once);
        filesystemHandlerMock.VerifyNoOtherCalls();

        server.Stop();
    }

    [Fact]
    public void IWireMockAdminApi_DeleteFileAsync_NotFound()
    {
        // Arrange
        var filesystemHandlerMock = new Mock<IFileSystemHandler>(MockBehavior.Strict);
        filesystemHandlerMock.Setup(fs => fs.FileExists(It.IsAny<string>())).Returns(false);
        filesystemHandlerMock.Setup(fs => fs.DeleteFile(It.IsAny<string>()));

        var server = WireMockServer.Start(new WireMockServerSettings
        {
            UseSSL = false,
            StartAdminInterface = true,
            FileSystemHandler = filesystemHandlerMock.Object
        });

        var api = RestClient.For<IWireMockAdminApi>(server.Urls[0]);

        // Act and Assert
        Check.ThatCode(() => api.DeleteFileAsync("filename.txt", TestContext.Current.CancellationToken)).Throws<ApiException>();

        // Verify
        filesystemHandlerMock.Verify(fs => fs.FileExists(It.Is<string>(p => p == "filename.txt")), Times.Once);
        filesystemHandlerMock.VerifyNoOtherCalls();

        server.Stop();
    }

    [Fact]
    public void IWireMockAdminApi_FileExistsAsync_NotFound()
    {
        // Arrange
        var filesystemHandlerMock = new Mock<IFileSystemHandler>(MockBehavior.Strict);
        filesystemHandlerMock.Setup(fs => fs.FileExists(It.IsAny<string>())).Returns(false);

        var server = WireMockServer.Start(new WireMockServerSettings
        {
            UseSSL = false,
            StartAdminInterface = true,
            FileSystemHandler = filesystemHandlerMock.Object
        });

        var api = RestClient.For<IWireMockAdminApi>(server.Urls[0]);

        // Act and Assert
        Check.ThatCode(() => api.FileExistsAsync("filename.txt", TestContext.Current.CancellationToken)).Throws<ApiException>();

        // Verify
        filesystemHandlerMock.Verify(fs => fs.FileExists(It.Is<string>(p => p == "filename.txt")), Times.Once);
        filesystemHandlerMock.VerifyNoOtherCalls();

        server.Stop();
    }

    [Fact]
    public async Task IWireMockAdminApi_DeleteScenarioUsingDeleteAsync()
    {
        // Arrange
        var name = "x";
        var server = WireMockServer.StartWithAdminInterface();
        var api = RestClient.For<IWireMockAdminApi>(server.Urls[0]);

        // Act
        var status = await api.DeleteScenarioAsync(name, TestContext.Current.CancellationToken);
        status.Status.Should().Be("No scenario found by name 'x'.");
    }

    [Fact]
    public async Task IWireMockAdminApi_DeleteScenarioUsingPostAsync()
    {
        // Arrange
        var name = "x";
        var server = WireMockServer.StartWithAdminInterface();
        var api = RestClient.For<IWireMockAdminApi>(server.Urls[0]);

        // Act
        var status = await api.ResetScenarioAsync(name, TestContext.Current.CancellationToken);
        status.Status.Should().Be("No scenario found by name 'x'.");
    }

    [Fact]
    public async Task IWireMockAdminApi_UpdateNonExistingScenarioState()
    {
        // Arrange
        using var server = WireMockServer.StartWithAdminInterface();

        var api = RestClient.For<IWireMockAdminApi>(server.Urls[0]);

        // Act
        var update = new ScenarioStateUpdateModel
        {
            State = null
        };
        var status = await api.PutScenarioStateAsync("x", update, TestContext.Current.CancellationToken);
        status.Status.Should().Be("No scenario found by name 'x'.");
    }

    [Fact]
    public async Task IWireMockAdminApi_UpdateScenarioState()
    {
        // Arrange
        using var server = WireMockServer.StartWithAdminInterface();
        server
            .Given(Request.Create()
                .WithPath("/state1")
                .UsingGet())
            .InScenario("s1")
            .WillSetStateTo("Test state 1")
            .RespondWith(Response.Create()
                .WithBody("No state msg 1"));

        server
            .Given(Request.Create()
                .WithPath("/foostate1")
                .UsingGet())
            .InScenario("s1")
            .WhenStateIs("Test state 1")
            .RespondWith(Response.Create()
                .WithBody("Test state msg 1"));

        var api = RestClient.For<IWireMockAdminApi>(server.Urls[0]);

        // Act
        var update = new ScenarioStateUpdateModel
        {
            State = null
        };
        var status = await api.PutScenarioStateAsync("s1", update, TestContext.Current.CancellationToken);
        status.Status.Should().Be("Scenario state set to ''");
    }

    [Fact]
    public async Task IWireMockAdminApi_GetMappingByGuidAsync()
    {
        // Arrange
        var guid = Guid.Parse("90356dba-b36c-469a-a17e-669cd84f1f05");
        var server = WireMockServer.StartWithAdminInterface();

        server
            .Given(
                Request.Create()
                    .WithPath("/foo1")
                    .WithParam("p1", "xyz")
                    .UsingGet()
            )
            .WithGuid(guid)
            .RespondWith(
                Response.Create()
                    .WithStatusCode(200)
                    .WithBody("1")
            );

        // Act
        var api = RestClient.For<IWireMockAdminApi>(server.Url);
        var getMappingResult = await api.GetMappingAsync(guid, TestContext.Current.CancellationToken);

        // Assert
        var mapping = server.Mappings.FirstOrDefault(m => m.Guid == guid);
        mapping.Should().NotBeNull();

        await Verify(getMappingResult, VerifySettings).DontScrubGuids();

        server.Stop();
    }

    [Fact]
    public async Task IWireMockAdminApi_GetMappingCodeByGuidAsync()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        var guid = Guid.Parse("90356dba-b36c-469a-a17e-669cd84f1f05");
        var server = WireMockServer.StartWithAdminInterface();

        server
            .Given(
                Request.Create()
                    .WithPath("/foo1")
                    .WithParam("p1", "xyz")
                    .UsingGet()
            )
            .WithGuid(guid)
            .RespondWith(
                Response.Create()
                    .WithStatusCode(200)
                    .WithBody("1")
            );

        // Act
        var api = RestClient.For<IWireMockAdminApi>(server.Url);

        var mappings = await api.GetMappingsAsync(cancellationToken);
        mappings.Should().HaveCount(1);

        var code = await api.GetMappingCodeAsync(guid, cancellationToken: cancellationToken);

        // Assert
        await Verify(code).DontScrubDateTimes().DontScrubGuids();

        server.Stop();
    }

    [Fact]
    public async Task IWireMockAdminApi_GetMappingsCode()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        var guid1 = Guid.Parse("90356dba-b36c-469a-a17e-669cd84f1f05");
        var guid2 = Guid.Parse("1b731398-4a5b-457f-a6e3-d65e541c428f");
        var guid3 = Guid.Parse("f74fd144-df53-404f-8e35-da22a640bd5f");
        var guid4 = Guid.Parse("4126DEC8-470B-4EFF-93BB-C24F83B8B1FD");
        var guid5 = Guid.Parse("c9929240-7ae8-4a5d-8ed8-0913479f6eeb");
        var server = WireMockServer.StartWithAdminInterface();

        server
            .Given(
                Request.Create()
                    .WithPath("/users/post1")
                    .UsingPost()
                    .WithBody(new JsonMatcher(new
                    {
                        city = "Amsterdam",
                        country = "The Netherlands"
                    }))
            )
            .WithGuid(guid1)
            .RespondWith(Response.Create());

        server
            .Given(
                Request.Create()
                    .WithPath("/users/post2")
                    .UsingPost()
                    .WithBody(new JsonPartialMatcher(new
                    {
                        city = "City",
                        country = "Country"
                    }))
            )
            .WithGuid(guid2)
            .RespondWith(Response.Create().WithBody("Line1\r\nSome \"value\" in Line2"));

        server
            .Given(
                Request.Create()
                    .WithPath("/foo1")
                    .WithParam("p1", "xyz")
                    .UsingGet()
            )
            .WithGuid(guid3)
            .RespondWith(
                Response.Create()
                    .WithStatusCode(200)
                    .WithBody("1")
            );

        server
            .Given(
                Request.Create()
                    .WithPath("/foo2")
                    .WithParam("p2", "abc")
                    .WithHeader("h1", "W/\"234f2q3r\"")
                    .UsingPost()
            )
            .WithGuid(guid4)
            .RespondWith(
                Response.Create()
                    .WithStatusCode("201")
                    .WithHeader("hk", "hv")
                    .WithHeader("ETag", "W/\"168d8e\"")
                    .WithBody("2")
            );

        server
            .Given(
                Request.Create()
                    .WithUrl("https://localhost/test")
                    .UsingDelete()
            )
            .WithGuid(guid5)
            .RespondWith(
                Response.Create()
                    .WithStatusCode(HttpStatusCode.Unused)
                    .WithBodyAsJson(new
                    {
                        @as = 1,
                        b = 1.2,
                        d = true,
                        e = false,
                        f = new[] { 1, 2, 3, 4 },
                        g = new
                        {
                            z1 = 1,
                            z2 = 2,
                            z3 = new[] { "a", "b", "c" },
                            z4 = new[]
                            {
                                new { a = 1, b = 2 },
                                new { a = 2, b = 3 }
                            }
                        },
                        date_field = new DateTime(2023, 05, 08, 11, 20, 19),
                        string_field_with_date = "2021-03-13T21:04:00Z",
                        multiline_text = @"This
is
multiline
text
"
                    })
            );

        // Act
        var api = RestClient.For<IWireMockAdminApi>(server.Url);

        var mappings = await api.GetMappingsAsync(cancellationToken);
        mappings.Should().HaveCount(5);

        var code = await api.GetMappingsCodeAsync(cancellationToken: cancellationToken);

        // Assert
        await Verify(code).DontScrubDateTimes().DontScrubGuids();

        server.Stop();
    }

    [Fact]
    public async Task IWireMockAdminApi_OpenApiConvert_Yml()
    {
        // Arrange
        var openApiDocument = File.ReadAllText(Path.Combine("OpenApiParser", "petstore.yml"));

        var server = WireMockServer.StartWithAdminInterface();
        var api = RestClient.For<IWireMockAdminApi>(server.Url);

        // Act
        var mappings = await api.OpenApiConvertAsync(openApiDocument, TestContext.Current.CancellationToken);

        // Assert
        server.MappingModels.Should().BeEmpty();
        mappings.Should().HaveCount(20);

        server.Stop();
    }

    [Fact]
    public async Task IWireMockAdminApi_OpenApiConvert_Json()
    {
        // Arrange
        var openApiDocument = File.ReadAllText(Path.Combine("OpenApiParser", "petstore-openapi3.json"));

        var server = WireMockServer.StartWithAdminInterface();
        var api = RestClient.For<IWireMockAdminApi>(server.Url);

        // Act
        var mappings = await api.OpenApiConvertAsync(openApiDocument, TestContext.Current.CancellationToken);

        // Assert
        server.MappingModels.Should().BeEmpty();
        mappings.Should().HaveCount(19);

        server.Stop();
    }

    [Fact]
    public async Task IWireMockAdminApi_OpenApiSave_Json()
    {
        // Arrange
        var openApiDocument = File.ReadAllText(Path.Combine("OpenApiParser", "petstore-openapi3.json"));

        var server = WireMockServer.StartWithAdminInterface();
        var api = RestClient.For<IWireMockAdminApi>(server.Url);

        // Act
        var statusModel = await api.OpenApiSaveAsync(openApiDocument, TestContext.Current.CancellationToken);

        // Assert
        statusModel.Status.Should().Be("OpenApi document converted to Mappings");
        server.MappingModels.Should().HaveCount(19);

        server.Stop();
    }

    [Fact]
    public async Task IWireMockAdminApi_OpenApiSave_Yml()
    {
        // Arrange
        var openApiDocument = File.ReadAllText(Path.Combine("OpenApiParser", "petstore.yml"));

        var server = WireMockServer.StartWithAdminInterface();
        var api = RestClient.For<IWireMockAdminApi>(server.Url);

        // Act
        var mappings = await api.OpenApiConvertAsync(openApiDocument, TestContext.Current.CancellationToken);

        // Assert
        server.MappingModels.Should().BeEmpty();
        mappings.Should().HaveCount(20);

        server.Stop();
    }

    [Fact]
    public async Task IWireMockAdminApi_ReadStaticMappingsAsync()
    {
        // Arrange
        using var server = WireMockServer.StartWithAdminInterface();
        var api = RestClient.For<IWireMockAdminApi>(server.Url);

        // Act
        var status = await api.ReloadStaticMappingsAsync(TestContext.Current.CancellationToken);

        // Assert
        status.Status.Should().Be("Static Mappings reloaded");
    }

    private static string ReadMappingFile(string filename)
    {
        return File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "__admin", "mappings", filename));
    }
}