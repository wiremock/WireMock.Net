//#if !(NET452 || NET461 || NETCOREAPP3_1)
using System;
using System.IO;
using System.Threading.Tasks;
using Moq;
using VerifyXunit;
using WireMock.Net.OpenApiParser;
using WireMock.Net.OpenApiParser.Settings;
using Xunit;

namespace WireMock.Net.Tests.OpenApiParser;

[UsesVerify]
public class WireMockOpenApiParserTests
{
    private readonly DateTime _exampleDateTime = new(2024, 6, 19, 12, 34, 56, DateTimeKind.Utc);
    private readonly Mock<IWireMockOpenApiParserExampleValues> _exampleValuesMock = new();

    private readonly WireMockOpenApiParser _sut = new();

    public WireMockOpenApiParserTests()
    {
        _exampleValuesMock.SetupGet(e => e.Boolean).Returns(true);
        _exampleValuesMock.SetupGet(e => e.Integer).Returns(42);
        _exampleValuesMock.SetupGet(e => e.Float).Returns(1.1f);
        _exampleValuesMock.SetupGet(e => e.Decimal).Returns(2.2m);
        _exampleValuesMock.SetupGet(e => e.String).Returns("example-string");
        _exampleValuesMock.SetupGet(e => e.Object).Returns("example-object");
        _exampleValuesMock.SetupGet(e => e.Bytes).Returns("Stef"u8.ToArray());
        _exampleValuesMock.SetupGet(e => e.Date).Returns(() => _exampleDateTime.Date);
        _exampleValuesMock.SetupGet(e => e.DateTime).Returns(() => _exampleDateTime);
    }

    [Fact]
    public async Task FromText_UsingYaml_ShouldReturnMappings()
    {
        // Arrange
        var settings = new WireMockOpenApiParserSettings
        {
            ExampleValues = _exampleValuesMock.Object
        };

        var openApiDocument = File.ReadAllText(Path.Combine("OpenApiParser", "payroc-openapi-spec.yaml"));

        // Act
        var mappings = _sut.FromText(openApiDocument, settings, out _);

        // Verify
        await Verifier.Verify(mappings);
    }

    [Fact]
    public async Task FromText_UsingJson_WithPlainTextExample_ShouldReturnMappings()
    {
        // Arrange
        var settings = new WireMockOpenApiParserSettings
        {
            ExampleValues = _exampleValuesMock.Object
        };

        var openApiDocument = File.ReadAllText(Path.Combine("OpenApiParser", "oas-content-example.json"));

        // Act
        var mappings = _sut.FromText(openApiDocument, settings, out _);

        // Verify
        await Verifier.Verify(mappings);
    }
}
//#endif