// Copyright © WireMock.Net

using System.Collections;
using AwesomeAssertions;
using WireMock.Extensions;

namespace WireMock.Net.Tests.Extensions;

public class DictionaryExtensionsTests
{
    [Fact]
    public void TryGetStringValue_WhenKeyExistsAndValueIsString_ReturnsTrueAndStringValue()
    {
        // Arrange
        var dictionary = new Hashtable { { "key", "value" } };

        // Act
        var result = dictionary.TryGetStringValue("key", out var value);

        // Assert
        result.Should().BeTrue();
        value.Should().Be("value");
    }

    [Fact]
    public void TryGetStringValue_WhenKeyExistsAndValueIsNotString_ReturnsTrueAndStringValue()
    {
        // Arrange
        var dictionary = new Hashtable { { "key", 123 } };

        // Act
        var result = dictionary.TryGetStringValue("key", out var value);

        // Assert
        result.Should().BeTrue();
        value.Should().Be("123");
    }

    [Fact]
    public void TryGetStringValue_WhenKeyDoesNotExist_ReturnsFalseAndNull()
    {
        // Arrange
        var dictionary = new Hashtable { { "otherKey", "value" } };

        // Act
        var result = dictionary.TryGetStringValue("key", out var value);

        // Assert
        result.Should().BeFalse();
        value.Should().BeNull();
    }
}