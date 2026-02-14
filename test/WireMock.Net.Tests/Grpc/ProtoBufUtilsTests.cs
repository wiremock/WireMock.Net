// Copyright © WireMock.Net

using AwesomeAssertions;
using WireMock.Util;

namespace WireMock.Net.Tests.Grpc;

public class ProtoBufUtilsTests
{
    private static readonly IProtoBufUtils ProtoBufUtils = new ProtoBufUtils();

    [Fact]
    public async Task GetProtoBufMessageWithHeader_MultipleProtoFiles()
    {
        // Arrange
        var greet = ReadProtoFile("greet1.proto");
        var request = ReadProtoFile("request.proto");

        // Act
        var responseBytes = await ProtoBufUtils.GetProtoBufMessageWithHeaderAsync(
            [greet, request],
            "greet.HelloRequest", new
            {
                name = "hello"
            },
            cancellationToken: TestContext.Current.CancellationToken
        );

        // Assert
        Convert.ToBase64String(responseBytes).Should().Be("AAAAAAcKBWhlbGxv");
    }

    private string ReadProtoFile(string filename)
    {
        return File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Grpc", filename));
    }
}