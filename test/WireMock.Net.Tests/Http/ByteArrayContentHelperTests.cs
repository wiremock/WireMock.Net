// Copyright © WireMock.Net

using System.Net.Http.Headers;
using System.Text;
using FluentAssertions;
using WireMock.Http;

namespace WireMock.Net.Tests.Http;

public class ByteArrayContentHelperTests
{
    [Fact]
    public async Task ByteArrayContentHelperTests_Create_WithNullContentType()
    {
        // Arrange
        var content = Encoding.UTF8.GetBytes("test");

        // Act
        var result = ByteArrayContentHelper.Create(content, null);

        // Assert
        result.Headers.ContentType.Should().BeNull();
        (await result.ReadAsByteArrayAsync()).Should().BeEquivalentTo(content);
    }

    [Theory]
    [InlineData("application/octet-stream", "application/octet-stream")]
    [InlineData("application/soap+xml", "application/soap+xml")]
    [InlineData("multipart/form-data; boundary=------------------------x", "multipart/form-data; boundary=------------------------x")]
    public async Task ByteArrayContentHelperTests_Create(string test, string expected)
    {
        // Arrange
        var content = Encoding.UTF8.GetBytes("test");
        var contentType = MediaTypeHeaderValue.Parse(test);

        // Act
        var result = ByteArrayContentHelper.Create(content, contentType);

        // Assert
        result.Headers.ContentType.ToString().Should().Be(expected);
        (await result.ReadAsByteArrayAsync()).Should().BeEquivalentTo(content);
    }
}