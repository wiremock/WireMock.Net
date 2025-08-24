// Copyright © WireMock.Net

#if PROTOBUF
using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using WireMock.Util;
using Xunit;

namespace WireMock.Net.Tests.Grpc;

public class ProtoBufUtilsTests
{
    private static readonly IProtoBufUtils ProtoBufUtils = new ProtoBufUtils();

    [Fact]
    public async Task GetProtoBufMessageWithHeader_MultipleProtoFiles()
    {
        // Arrange
        var greet = await ReadProtoFileAsync("greet1.proto");
        var request = await ReadProtoFileAsync("request.proto");

        // Act
        var responseBytes = await ProtoBufUtils.GetProtoBufMessageWithHeaderAsync(
            [greet, request],
            "greet.HelloRequest",
            new
            {
                name = "hello"
            }
        );

        // Assert
        Convert.ToBase64String(responseBytes).Should().Be("AAAAAAcKBWhlbGxv");
    }

    private static Task<string> ReadProtoFileAsync(string filename)
    {
        return File.ReadAllTextAsync(Path.Combine(Directory.GetCurrentDirectory(), "Grpc", filename));
    }
}
#endif