// Copyright © WireMock.Net

using System.Collections.Generic;
using FluentAssertions;
using WireMock.Net.Testcontainers.Utils;
using Xunit;

namespace WireMock.Net.Tests.Testcontainers;

public class CombineUtilsTests
{
    [Fact]
    public void Combine_Lists_WithBothEmpty_ReturnsEmptyList()
    {
        // Arrange
        var oldValue = new List<string>();
        var newValue = new List<string>();

        // Act
        var result = CombineUtils.Combine(oldValue, newValue);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void Combine_Lists_WithEmptyOldValue_ReturnsNewValue()
    {
        // Arrange
        var oldValue = new List<string>();
        var newValue = new List<string> { "item1", "item2" };

        // Act
        var result = CombineUtils.Combine(oldValue, newValue);

        // Assert
        result.Should().Equal("item1", "item2");
    }

    [Fact]
    public void Combine_Lists_WithEmptyNewValue_ReturnsOldValue()
    {
        // Arrange
        var oldValue = new List<string> { "item1", "item2" };
        var newValue = new List<string>();

        // Act
        var result = CombineUtils.Combine(oldValue, newValue);

        // Assert
        result.Should().Equal("item1", "item2");
    }

    [Fact]
    public void Combine_Lists_WithBothPopulated_ReturnsConcatenatedList()
    {
        // Arrange
        var oldValue = new List<int> { 1, 2, 3 };
        var newValue = new List<int> { 4, 5, 6 };

        // Act
        var result = CombineUtils.Combine(oldValue, newValue);

        // Assert
        result.Should().Equal(1, 2, 3, 4, 5, 6);
    }

    [Fact]
    public void Combine_Lists_WithDuplicates_PreservesDuplicates()
    {
        // Arrange
        var oldValue = new List<string> { "a", "b", "c" };
        var newValue = new List<string> { "b", "c", "d" };

        // Act
        var result = CombineUtils.Combine(oldValue, newValue);

        // Assert
        result.Should().Equal("a", "b", "c", "b", "c", "d");
    }

    [Fact]
    public void Combine_Dictionaries_WithBothEmpty_ReturnsEmptyDictionary()
    {
        // Arrange
        var oldValue = new Dictionary<string, int>();
        var newValue = new Dictionary<string, int>();

        // Act
        var result = CombineUtils.Combine(oldValue, newValue);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void Combine_Dictionaries_WithEmptyOldValue_ReturnsNewValue()
    {
        // Arrange
        var oldValue = new Dictionary<string, int>();
        var newValue = new Dictionary<string, int>
        {
            { "key1", 1 },
            { "key2", 2 }
        };

        // Act
        var result = CombineUtils.Combine(oldValue, newValue);

        // Assert
        result.Should().HaveCount(2);
        result["key1"].Should().Be(1);
        result["key2"].Should().Be(2);
    }

    [Fact]
    public void Combine_Dictionaries_WithEmptyNewValue_ReturnsOldValue()
    {
        // Arrange
        var oldValue = new Dictionary<string, int>
        {
            { "key1", 1 },
            { "key2", 2 }
        };
        var newValue = new Dictionary<string, int>();

        // Act
        var result = CombineUtils.Combine(oldValue, newValue);

        // Assert
        result.Should().HaveCount(2);
        result["key1"].Should().Be(1);
        result["key2"].Should().Be(2);
    }

    [Fact]
    public void Combine_Dictionaries_WithNoOverlappingKeys_ReturnsMergedDictionary()
    {
        // Arrange
        var oldValue = new Dictionary<string, string>
        {
            { "key1", "value1" },
            { "key2", "value2" }
        };
        var newValue = new Dictionary<string, string>
        {
            { "key3", "value3" },
            { "key4", "value4" }
        };

        // Act
        var result = CombineUtils.Combine(oldValue, newValue);

        // Assert
        result.Should().HaveCount(4);
        result["key1"].Should().Be("value1");
        result["key2"].Should().Be("value2");
        result["key3"].Should().Be("value3");
        result["key4"].Should().Be("value4");
    }
}