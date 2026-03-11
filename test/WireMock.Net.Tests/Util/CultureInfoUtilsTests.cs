// Copyright © WireMock.Net

using System.Globalization;
using WireMock.Util;

namespace WireMock.Net.Tests.Util;

public class CultureInfoUtilsTests
{
    [Theory]
    [InlineData(null, typeof(CultureInfo))]
    [InlineData("en-US", typeof(CultureInfo))]
    [InlineData("fr-FR", typeof(CultureInfo))]
    [InlineData("es-ES", typeof(CultureInfo))]
    public void Parse_ValidInputs_ReturnsExpectedCultureInfo(string? value, Type expectedType)
    {
        // Act
        var result = CultureInfoUtils.Parse(value);

        // Assert
        result.Should().BeOfType(expectedType);
    }

    [Theory]
    [InlineData("InvalidCulture")]
    [InlineData("123456")]
    public void Parse_InvalidInputs_ReturnsCurrentCulture(string? value)
    {
        // Arrange
        var expectedCulture = CultureInfo.CurrentCulture;

        // Act
        var result = CultureInfoUtils.Parse(value);

        // Assert
        result.Should().Be(expectedCulture);
    }

    [Fact]
    public void Parse_IntegerInput_ReturnsExpectedCultureInfo()
    {
        // Arrange
        string value = "1033"; // en-US culture identifier
        var expectedCulture = new CultureInfo(1033);

        // Act
        var result = CultureInfoUtils.Parse(value);

        // Assert
        result.Should().Be(expectedCulture);
    }

    [Fact]
    public void Parse_CurrentCultureInput_ReturnsCurrentCulture()
    {
        // Arrange
        string value = nameof(CultureInfo.CurrentCulture);
        var expectedCulture = CultureInfo.CurrentCulture;

        // Act
        var result = CultureInfoUtils.Parse(value);

        // Assert
        result.Should().Be(expectedCulture);
    }

    [Fact]
    public void Parse_InvariantCultureInput_ReturnsInvariantCulture()
    {
        // Arrange
        string value = nameof(CultureInfo.InvariantCulture);
        var expectedCulture = CultureInfo.InvariantCulture;

        // Act
        var result = CultureInfoUtils.Parse(value);

        // Assert
        result.Should().Be(expectedCulture);
    }
}