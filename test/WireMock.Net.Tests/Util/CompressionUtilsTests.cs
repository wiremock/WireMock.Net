// Copyright © WireMock.Net

using System.Text;
using AwesomeAssertions;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;
using WireMock.Util;

namespace WireMock.Net.Tests.Util;

public class CompressionUtilsTests
{
    [Theory]
    [InlineData("gzip")]
    [InlineData("deflate")]
    public void CompressDecompress_ValidInput_ReturnsOriginalData(string contentEncoding)
    {
        // Arrange
        byte[] data = Encoding.UTF8.GetBytes("Test data for compression");

        // Act
        byte[] compressedData = CompressionUtils.Compress(contentEncoding, data);
        byte[] decompressedData = CompressionUtils.Decompress(contentEncoding, compressedData);

        // Assert
        decompressedData.Should().BeEquivalentTo(data);
    }

    [Theory]
    [InlineData("gzip")]
    [InlineData("deflate")]
    public void Compress_ValidInput_ReturnsCompressedData(string contentEncoding)
    {
        // Arrange
        var text = RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = "[0-9A-Z]{1000}" }).Generate()!;
        byte[] data = Encoding.UTF8.GetBytes(text);

        // Act
        byte[] compressedData = CompressionUtils.Compress(contentEncoding, data);

        // Assert
        compressedData.Length.Should().BeLessThan(data.Length);
    }

    [Fact]
    public void CompressDecompress_InvalidContentEncoding_ThrowsNotSupportedException()
    {
        // Arrange
        byte[] data = Encoding.UTF8.GetBytes("Test data for compression");
        string contentEncoding = "invalid";

        // Act
        Action compressAction = () => CompressionUtils.Compress(contentEncoding, data);
        Action decompressAction = () => CompressionUtils.Decompress(contentEncoding, data);

        // Assert
        compressAction.Should().Throw<NotSupportedException>()
            .WithMessage($"ContentEncoding '{contentEncoding}' is not supported.");
        decompressAction.Should().Throw<NotSupportedException>()
            .WithMessage($"ContentEncoding '{contentEncoding}' is not supported.");
    }
}