// Copyright © WireMock.Net

using System.Text;
using FluentAssertions;
using WireMock.Util;

namespace WireMock.Net.Tests.Util;

public class BytesEncodingUtilsTests
{
    [Fact]
    public void TryGetEncoding_UTF32()
    {
        var result = BytesEncodingUtils.TryGetEncoding(new byte[] { 0xff, 0xfe, 0x00, 0x00 }, out var encoding);

        // Assert
        result.Should().BeTrue();
        encoding?.CodePage.Should().Be(Encoding.UTF32.CodePage);
    }

    [Fact]
    public void TryGetEncoding_Invalid()
    {
        var result = BytesEncodingUtils.TryGetEncoding(new byte[] { 0xff }, out var encoding);

        // Assert
        result.Should().BeFalse();
        encoding.Should().BeNull();
    }
}