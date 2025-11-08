// Copyright © WireMock.Net

#if !(NET452 || NET461 || NETCOREAPP3_1)
using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using VerifyTests;
using VerifyXunit;
using WireMock.Matchers;
using WireMock.Net.Tests.VerifyExtensions;
using WireMock.RequestBuilders;
using WireMock.Serialization;
using WireMock.Settings;
using WireMock.Util;
using Xunit;

namespace WireMock.Net.Tests.Serialization;

[UsesVerify]
public class ProxyMappingConverterTests
{
    private static readonly VerifySettings VerifySettings = new();
    static ProxyMappingConverterTests()
    {
        VerifySettings.Init();
    }

    private readonly WireMockServerSettings _settings = new();

    private readonly MappingConverter _mappingConverter;

    private readonly ProxyMappingConverter _sut;

    public ProxyMappingConverterTests()
    {
        var guidUtilsMock = new Mock<IGuidUtils>();
        guidUtilsMock.Setup(g => g.NewGuid()).Returns(Guid.Parse("ff55ac0a-fea9-4d7b-be74-5e483a2c1305"));

        var dateTimeUtilsMock = new Mock<IDateTimeUtils>();
        dateTimeUtilsMock.SetupGet(d => d.UtcNow).Returns(new DateTime(2022, 12, 4));

        _mappingConverter = new MappingConverter(new MatcherMapper(_settings));

        _sut = new ProxyMappingConverter(_settings, guidUtilsMock.Object, dateTimeUtilsMock.Object);
    }

    [Fact]
    public Task ToMapping_UseDefinedRequestMatchers_True()
    {
        // Arrange
        var proxyAndRecordSettings = new ProxyAndRecordSettings
        {
            UseDefinedRequestMatchers = true
        };

        var request = Request.Create()
            .UsingPost()
            .WithPath("/x")
            .WithParam("p1", "p1-v")
            .WithParam("p2", "p2-v")
            .WithHeader("Content-Type", new ContentTypeMatcher("text/plain"))
            .WithCookie("c", "x")
            .WithBody(new RegexMatcher("<RequestType>Auth</RequestType>"));

        var mappingMock = new Mock<IMapping>();
        mappingMock.SetupGet(m => m.RequestMatcher).Returns(request);
        mappingMock.SetupGet(m => m.Title).Returns("my title");
        mappingMock.SetupGet(m => m.Description).Returns("my description");

        var requestMessageMock = new Mock<IRequestMessage>();

        var responseMessage = new ResponseMessage();

        // Act
        var proxyMapping = _sut.ToMapping(mappingMock.Object, proxyAndRecordSettings, requestMessageMock.Object, responseMessage)!;

        // Assert
        var model = _mappingConverter.ToMappingModel(proxyMapping);

        // Verify
        return Verifier.Verify(model, VerifySettings);
    }
}
#endif