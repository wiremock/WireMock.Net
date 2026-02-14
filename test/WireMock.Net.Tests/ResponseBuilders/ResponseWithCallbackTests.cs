// Copyright © WireMock.Net

using System.Net;
using System.Text;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using NFluent;
using WireMock.Handlers;
using WireMock.Models;
using WireMock.ResponseBuilders;
using WireMock.Settings;
using WireMock.Types;
using WireMock.Util;

namespace WireMock.Net.Tests.ResponseBuilders;

public class ResponseWithCallbackTests
{
    private const string ClientIp = "::1";

    private readonly Mock<IMapping> _mappingMock;
    private readonly WireMockServerSettings _settings = new();

    public ResponseWithCallbackTests()
    {
        _mappingMock = new Mock<IMapping>();

        var filesystemHandlerMock = new Mock<IFileSystemHandler>(MockBehavior.Strict);
        filesystemHandlerMock.Setup(fs => fs.ReadResponseBodyAsString(It.IsAny<string>())).Returns("abc");

        _settings.FileSystemHandler = filesystemHandlerMock.Object;
    }

    [Fact]
    public async Task Response_ProvideResponse_WithBody_Func()
    {
        // Assign
        var request = new RequestMessage(new UrlDetails("http://localhost/test"), "GET", ClientIp);

        var responseBuilder = Response.Create()
            .WithStatusCode(500)
            .WithHeader("H1", "X1")
            .WithHeader("H2", "X2")
            .WithBody(req => $"path: {req.Path}");

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, Mock.Of<HttpContext>(), request, _settings);

        // Assert
        Check.That(response.Message.BodyData.BodyAsString).IsEqualTo("path: /test");
        Check.That(response.Message.BodyData.BodyAsBytes).IsNull();
        Check.That(response.Message.BodyData.BodyAsJson).IsNull();
        Check.That(response.Message.BodyData.Encoding.CodePage).Equals(Encoding.UTF8.CodePage);
        Check.That(response.Message.StatusCode).IsEqualTo(500);
        Check.That(response.Message.Headers["H1"].ToString()).IsEqualTo("X1");
        Check.That(response.Message.Headers["H2"].ToString()).IsEqualTo("X2");
    }

    [Fact]
    public async Task Response_ProvideResponse_WithBody_Func_DynamicCode_Should_IsFuncUsed()
    {
        // Assign
        var request = new RequestMessage(new UrlDetails("http://localhost/test"), "GET", ClientIp);

        var data = new[] { "x", "y" };
        var i = 0;
        var responseBuilder = Response.Create()
            .WithBody(_ =>
            {
                var value = data[i];
                i++;
                return value;
            });

        // Act (2x)
        var response1 = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, Mock.Of<HttpContext>(), request, _settings);
        var response2 = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, Mock.Of<HttpContext>(), request, _settings);

        // Assert
        response1.Message.BodyData!.BodyAsString.Should().Be("x");
        response1.Message.BodyData!.IsFuncUsed.Should().Be("Func<IRequestMessage, string>");
        response2.Message.BodyData!.BodyAsString.Should().Be("y");
        response2.Message.BodyData!.IsFuncUsed.Should().Be("Func<IRequestMessage, string>");
    }

    [Fact]
    public async Task Response_ProvideResponse_WithBody_AsyncFunc_DynamicCode_Should_IsFuncUsed()
    {
        // Assign
        var request = new RequestMessage(new UrlDetails("http://localhost/test"), "GET", ClientIp);

        var data = new[] { "x", "y" };
        var i = 0;
        var responseBuilder = Response.Create()
            .WithBody(_ =>
            {
                var value = data[i];
                i++;
                return Task.FromResult(value);
            });

        // Act (2x)
        var response1 = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, Mock.Of<HttpContext>(), request, _settings);
        var response2 = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, Mock.Of<HttpContext>(), request, _settings);

        // Assert
        response1.Message.BodyData!.BodyAsString.Should().Be("x");
        response1.Message.BodyData!.IsFuncUsed.Should().Be("Func<IRequestMessage, Task<string>>");
        response2.Message.BodyData!.BodyAsString.Should().Be("y");
        response2.Message.BodyData!.IsFuncUsed.Should().Be("Func<IRequestMessage, Task<string>>");
    }

    [Fact]
    public async Task Response_ProvideResponse_WithBody_FuncAsync()
    {
        // Assign
        var request = new RequestMessage(new UrlDetails("http://localhost/test"), "GET", ClientIp);

        var responseBuilder = Response.Create()
            .WithStatusCode(500)
            .WithHeader("H1", "X1")
            .WithHeader("H2", "X2")
            .WithBody(async req =>
            {
                await Task.Delay(1).ConfigureAwait(false);
                return $"path: {req.Path}";
            });

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, Mock.Of<HttpContext>(), request, _settings);

        // Assert
        Check.That(response.Message.BodyData.BodyAsString).IsEqualTo("path: /test");
        Check.That(response.Message.BodyData.BodyAsBytes).IsNull();
        Check.That(response.Message.BodyData.BodyAsJson).IsNull();
        Check.That(response.Message.BodyData.Encoding.CodePage).Equals(Encoding.UTF8.CodePage);
        Check.That(response.Message.StatusCode).IsEqualTo(500);
        Check.That(response.Message.Headers["H1"].ToString()).IsEqualTo("X1");
        Check.That(response.Message.Headers["H2"].ToString()).IsEqualTo("X2");
    }

    [Fact]
    public async Task Response_WithCallbackAsync()
    {
        // Assign
        var requestMessage = new RequestMessage(new UrlDetails("http://localhost/foo"), "GET", "::1");
        var responseBuilder = Response.Create()
            .WithCallback(async request =>
            {
                await Task.Delay(1);

                return new ResponseMessage
                {
                    BodyData = new BodyData
                    {
                        DetectedBodyType = BodyType.String,
                        BodyAsString = request.Path + "Bar"
                    },
                    StatusCode = 302
                };
            });

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, Mock.Of<HttpContext>(), requestMessage, _settings);

        // Assert
        response.Message.BodyData.BodyAsString.Should().Be("/fooBar");
        response.Message.StatusCode.Should().Be(302);
    }

    [Fact]
    public async Task Response_WithCallback()
    {
        // Assign
        var requestMessage = new RequestMessage(new UrlDetails("http://localhost/foo"), "GET", "::1");
        var responseBuilder = Response.Create()
            .WithCallback(request => new ResponseMessage
            {
                BodyData = new BodyData
                {
                    DetectedBodyType = BodyType.String,
                    BodyAsString = request.Path + "Bar"
                },
                StatusCode = 302
            });

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, Mock.Of<HttpContext>(), requestMessage, _settings);

        // Assert
        response.Message.BodyData.BodyAsString.Should().Be("/fooBar");
        response.Message.StatusCode.Should().Be(302);
    }

    [Fact]
    public async Task Response_WithCallback_ShouldUseStatusCodeAndHeaderInTheCallback()
    {
        // Assign
        var header = "X-UserId";
        var requestMessage = new RequestMessage(new UrlDetails("http://localhost/foo"), "GET", "::1");
        var responseBuilder = Response.Create()
            .WithCallback(request => new ResponseMessage
            {
                BodyData = new BodyData
                {
                    DetectedBodyType = BodyType.String,
                    BodyAsString = request.Path + "Bar"
                },
                StatusCode = HttpStatusCode.Accepted,
                Headers = new Dictionary<string, WireMockList<string>>
                {
                    { header, new WireMockList<string>("Stef") }
                }
            });

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, Mock.Of<HttpContext>(), requestMessage, _settings);

        // Assert
        response.Message.BodyData.BodyAsString.Should().Be("/fooBar");
        response.Message.StatusCode.Should().Be(HttpStatusCode.Accepted);
        response.Message.Headers[header].Should().ContainSingle("Stef");
    }

    [Fact]
    public async Task Response_WithCallback_And_Additional_WithStatusCode_And_WithHeader_ShouldUseAdditional()
    {
        // Assign
        var header = "X-UserId";
        var requestMessage = new RequestMessage(new UrlDetails("http://localhost/foo"), "GET", "::1");
        var responseBuilder = Response.Create()
            .WithCallback(request => new ResponseMessage
            {
                BodyData = new BodyData
                {
                    DetectedBodyType = BodyType.String,
                    BodyAsString = request.Path + "Bar"
                },
                StatusCode = HttpStatusCode.NotFound,
                Headers = new Dictionary<string, WireMockList<string>>
                {
                    { header, new WireMockList<string>("NA") }
                }
            })
            .WithStatusCode(HttpStatusCode.Accepted)
            .WithHeader(header, "Stef");

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, Mock.Of<HttpContext>(), requestMessage, _settings);

        // Assert
        response.Message.BodyData.BodyAsString.Should().Be("/fooBar");
        response.Message.StatusCode.Should().Be((int)HttpStatusCode.Accepted);
        response.Message.Headers[header].Should().ContainSingle("Stef");
    }

    [Fact]
    public async Task Response_WithCallback_And_UseTransformer_Is_True()
    {
        // Assign
        var requestMessage = new RequestMessage(new UrlDetails("http://localhost/foo"), "GET", "::1");
        var responseBuilder = Response.Create()
            .WithCallback(request => new ResponseMessage
            {
                BodyData = new BodyData
                {
                    DetectedBodyType = BodyType.String,
                    BodyAsString = "{{request.Path}}Bar"
                },
                StatusCode = 302
            })
            .WithTransformer();

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, Mock.Of<HttpContext>(), requestMessage, _settings);

        // Assert
        response.Message.BodyData.BodyAsString.Should().Be("/fooBar");
        response.Message.StatusCode.Should().Be(302);
    }

    // https://github.com/wiremock/WireMock.Net/issues/898
    [Fact]
    public async Task Response_WithCallback_WithRequestHeaders_Should_BeAccessibleInCallback()
    {
        // Assign
        var headerKey = "headerKey";
        var headerValues = new[] { "abc" };
        var requestHeaders = new Dictionary<string, string[]>
        {
            { headerKey, headerValues }
        };
        var requestMessage = new RequestMessage(new UrlDetails("http://localhost/foo"), "GET", "::1", null, requestHeaders);
        var responseBuilder = Response.Create()
            .WithCallback(request =>
            {
                var headersFromRequest = request!.Headers![headerKey].ToList();
                headersFromRequest.Add("extra");

                return new ResponseMessage
                {
                    Headers = new Dictionary<string, WireMockList<string>>
                    {
                        { headerKey, new WireMockList<string>(headersFromRequest) }
                    }
                };
            });

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, Mock.Of<HttpContext>(), requestMessage, _settings);

        // Assert
        response.Message.Headers![headerKey].Should().Contain("extra");
    }
}