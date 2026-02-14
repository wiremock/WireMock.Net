// Copyright © WireMock.Net

using AwesomeAssertions;
using WireMock.Util;

namespace WireMock.Net.Tests.Grpc;

public class ProtoDefinitionHelperTests
{
    private static readonly IProtoBufUtils ProtoBufUtils = new ProtoBufUtils();

    [Fact]
    public async Task FromDirectory_Greet_ShouldReturnModifiedProtoFiles()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        var directory = Path.Combine(Directory.GetCurrentDirectory(), "Grpc", "Test");
        var expectedFilename = "SubFolder/request.proto";
        var expectedComment = $"// {expectedFilename}";

        // Act
        var protoDefinitionData = await ProtoDefinitionDataHelper.FromDirectory(directory, cancellationToken);
        var protoDefinitions = protoDefinitionData.ToList("greet");

        // Assert
        protoDefinitions.Should().HaveCount(2);
        protoDefinitions[0].Should().StartWith("// greet.proto");
        protoDefinitions[1].Should().StartWith(expectedComment);

        // Arrange
        var resolver = new WireMockProtoFileResolver(protoDefinitions);

        // Act + Assert
        resolver.Exists(expectedFilename).Should().BeTrue();
        resolver.Exists("x").Should().BeFalse();

        // Act + Assert
        var text = await resolver.OpenText(expectedFilename).ReadToEndAsync();
        text.Should().StartWith(expectedComment);
        System.Action action = () => resolver.OpenText("x");
        action.Should().Throw<FileNotFoundException>();
    }

    [Fact]
    public async Task FromDirectory_OpenTelemetry_ShouldReturnModifiedProtoFiles()
    {
        // Arrange
        var directory = Path.Combine(Directory.GetCurrentDirectory(), "Grpc", "ot");

        // Act
        var protoDefinitionData = await ProtoDefinitionDataHelper.FromDirectory(directory, TestContext.Current.CancellationToken);
        var protoDefinitions = protoDefinitionData.ToList("trace_service");

        // Assert
        protoDefinitions.Should().HaveCount(10);

        var responseBytes = await ProtoBufUtils.GetProtoBufMessageWithHeaderAsync(
            protoDefinitions,
            "OpenTelemetry.Proto.Collector.Trace.V1.ExportTracePartialSuccess",
            new
            {
                rejected_spans = 1,
                error_message = "abc"
            },
            cancellationToken: TestContext.Current.CancellationToken
        );

        // Assert
        Convert.ToBase64String(responseBytes).Should().Be("AAAAAAcIARIDYWJj");
    }
}