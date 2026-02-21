// Copyright © WireMock.Net

using AwesomeAssertions;
using WireMock.Extensions;

namespace WireMock.Net.Tests.Extensions;

public class EnumExtensionsTests
{
    private enum TestEnum
    {
        Value1
    }

    [Fact]
    public void EnumExtensions_GetFullyQualifiedEnumValue_ShouldReturnCorrectValue()
    {
        // Arrange
        var enumValue = TestEnum.Value1;

        // Act
        var result = enumValue.GetFullyQualifiedEnumValue();

        // Assert
        result.Should().Be("WireMock.Net.Tests.Extensions.TestEnum.Value1");
    }

    [Fact]
    public void EnumExtensions_GetFullyQualifiedEnumValue_ShouldThrowArgumentException_WhenTypeIsNotEnum()
    {
        // Arrange
        int nonEnumValue = 42;

        // Act
        Action act = () => nonEnumValue.GetFullyQualifiedEnumValue();

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("T must be an enum");
    }
}