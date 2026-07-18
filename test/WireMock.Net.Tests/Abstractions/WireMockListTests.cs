// Copyright © WireMock.Net

using WireMock.Types;

namespace WireMock.Net.Tests.Abstractions;

public class WireMockListTests
{
    #region String Generic Type Tests

    [Fact]
    public void WireMockListOfString_Constructor_Empty_ShouldCreateEmptyList()
    {
        // Act
        var list = new WireMockList<string>();

        // Assert
        list.Should().BeEmpty();
        list.Count.Should().Be(0);
    }

    [Fact]
    public void WireMockListOfString_Constructor_WithSingleString_ShouldCreateListWithOneElement()
    {
        // Arrange
        var value = "test";

        // Act
        var list = new WireMockList<string>(value);

        // Assert
        list.Should().HaveCount(1);
        list[0].Should().Be("test");
    }

    [Fact]
    public void WireMockListOfString_Constructor_WithMultipleStrings_ShouldCreateListWithAllElements()
    {
        // Arrange
        var values = new[] { "value1", "value2", "value3" };

        // Act
        var list = new WireMockList<string>(values);

        // Assert
        list.Should().HaveCount(3);
        list.Should().ContainInOrder("value1", "value2", "value3");
    }

    [Fact]
    public void WireMockListOfString_Constructor_WithIEnumerable_ShouldCreateListWithAllElements()
    {
        // Arrange
        var values = new List<string> { "a", "b", "c" };

        // Act
        var list = new WireMockList<string>(values);

        // Assert
        list.Should().HaveCount(3);
        list.Should().ContainInOrder("a", "b", "c");
    }

    [Fact]
    public void WireMockListOfString_ToString_WhenEmpty_ShouldReturnEmptyString()
    {
        // Arrange
        var list = new WireMockList<string>();

        // Act
        var result = list.ToString();

        // Assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void WireMockListOfString_ToString_WhenSingleElement_ShouldReturnElementValue()
    {
        // Arrange
        var list = new WireMockList<string>("singleValue");

        // Act
        var result = list.ToString();

        // Assert
        result.Should().Be("singleValue");
    }

    [Fact]
    public void WireMockListOfString_ToString_WhenMultipleElements_ShouldReturnCommaSeparatedValues()
    {
        // Arrange
        var list = new WireMockList<string>("value1", "value2", "value3");

        // Act
        var result = list.ToString();

        // Assert
        result.Should().Be("value1, value2, value3");
    }

    [Fact]
    public void WireMockListOfString_ImplicitOperator_WithSingleValue_ShouldCreateList()
    {
        // Act
        WireMockList<string> list = "testValue";

        // Assert
        list.Should().HaveCount(1);
        list[0].Should().Be("testValue");
    }

    [Fact]
    public void WireMockListOfString_ImplicitOperator_WithArray_ShouldCreateList()
    {
        // Arrange
        var values = new[] { "first", "second", "third" };

        // Act
        WireMockList<string> list = values;

        // Assert
        list.Should().HaveCount(3);
        list.Should().ContainInOrder("first", "second", "third");
    }

    #endregion

    #region Object Generic Type Tests

    [Fact]
    public void WireMockListOfObject_Constructor_Empty_ShouldCreateEmptyList()
    {
        // Act
        var list = new WireMockList<object>();

        // Assert
        list.Should().BeEmpty();
        list.Count.Should().Be(0);
    }

    [Fact]
    public void WireMockListOfObject_Constructor_WithSingleObject_ShouldCreateListWithOneElement()
    {
        // Arrange
        var obj = new { Name = "Test", Value = 123 };

        // Act
        var list = new WireMockList<object>(obj);

        // Assert
        list.Should().HaveCount(1);
        list[0].Should().Be(obj);
    }

    [Fact]
    public void WireMockListOfObject_Constructor_WithMultipleObjects_ShouldCreateListWithAllElements()
    {
        // Arrange
        var obj1 = new object();
        var obj2 = new object();
        var obj3 = new object();
        var values = new[] { obj1, obj2, obj3 };

        // Act
        var list = new WireMockList<object>(values);

        // Assert
        list.Should().HaveCount(3);
        list.Should().ContainInOrder(obj1, obj2, obj3);
    }

    [Fact]
    public void WireMockListOfObject_Constructor_WithIEnumerable_ShouldCreateListWithAllElements()
    {
        // Arrange
        var values = new List<object?>
        {
            "string",
            123,
            45.67,
            true,
            null
        };

        // Act
        var list = new WireMockList<object?>(values);

        // Assert
        list.Should().HaveCount(5);
        list.Should().ContainInOrder("string", 123, 45.67, true, null);
    }

    [Fact]
    public void WireMockListOfObject_ToString_WhenEmpty_ShouldReturnEmptyString()
    {
        // Arrange
        var list = new WireMockList<object>();

        // Act
        var result = list.ToString();

        // Assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void WireMockListOfObject_ToString_WhenSingleString_ShouldReturnString()
    {
        // Arrange
        var list = new WireMockList<object>("singleString");

        // Act
        var result = list.ToString();

        // Assert
        result.Should().Be("singleString");
    }

    [Fact]
    public void WireMockListOfObject_ToString_WhenSingleObject_ShouldReturnObjectToString()
    {
        // Arrange
        var obj = new { Name = "Test" };
        var list = new WireMockList<object>(obj);

        // Act
        var result = list.ToString();

        // Assert
        result.Should().Contain("Name");
        result.Should().Contain("Test");
    }

    [Fact]
    public void WireMockListOfObject_ToString_WhenSingleInt_ShouldReturnIntAsString()
    {
        // Arrange
        var list = new WireMockList<object>(42);

        // Act
        var result = list.ToString();

        // Assert
        result.Should().Be("42");
    }

    [Fact]
    public void WireMockListOfObject_ToString_WhenMultipleElements_ShouldReturnCommaSeparatedValues()
    {
        // Arrange
        var list = new WireMockList<object>("text", 123, 45.67);

        // Act
        var result = list.ToString();

        // Assert
        result.Should().Be($"text, 123, {45.67}");
    }

    [Fact]
    public void WireMockListOfObject_ToString_WithMixedTypes_ShouldReturnCommaSeparatedStringRepresentation()
    {
        // Arrange
        var list = new WireMockList<object>
        {
            "string",
            123,
            45.67,
            true
        };

        // Act
        var result = list.ToString();

        // Assert
        result.Should().Be($"string, 123, {45.67}, True");
    }

    [Fact]
    public void WireMockListOfObject_ToString_WithNullValue_ShouldReturnEmptyStringForNull()
    {
        // Arrange
        var list = new WireMockList<object?>
        {
            "value1",
            null,
            "value3"
        };

        // Act
        var result = list.ToString();

        // Assert
        result.Should().Be("value1, , value3");
    }

    [Fact]
    public void WireMockListOfObject_ImplicitOperator_WithSingleValue_ShouldCreateList()
    {
        // Arrange
        var obj = new { Id = 1 };

        // Act
        WireMockList<object> list = obj;

        // Assert
        list.Should().HaveCount(1);
        list[0].Should().Be(obj);
    }

    [Fact]
    public void WireMockListOfObject_ImplicitOperator_WithArray_ShouldCreateList()
    {
        // Arrange
        var values = new object[] { "first", 2, 3.0 };

        // Act
        WireMockList<object> list = values;

        // Assert
        list.Should().HaveCount(3);
        list.Should().ContainInOrder("first", 2, 3.0);
    }

    #endregion

    #region List Operations Tests

    [Fact]
    public void WireMockListOfString_Add_ShouldAddElement()
    {
        // Arrange
        var list = new WireMockList<string>("initial");

        // Act
        list.Add("new");

        // Assert
        list.Should().HaveCount(2);
        list[1].Should().Be("new");
    }

    [Fact]
    public void WireMockListOfString_Remove_ShouldRemoveElement()
    {
        // Arrange
        var list = new WireMockList<string>("value1", "value2", "value3");

        // Act
        var removed = list.Remove("value2");

        // Assert
        removed.Should().BeTrue();
        list.Should().HaveCount(2);
        list.Should().ContainInOrder("value1", "value3");
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void WireMockListOfString_ToString_WithEmptyString_ShouldReturnEmptyString()
    {
        // Arrange
        var list = new WireMockList<string>("");

        // Act
        var result = list.ToString();

        // Assert
        result.Should().Be("");
    }

    [Fact]
    public void WireMockListOfString_Constructor_WithNull_ShouldThrow()
    {
        // Act & Assert
        var act = () => new WireMockList<string?>(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void WireMockListOfObject_ImplicitOperator_WithNull_ShouldCreateListWithNullElement()
    {
        // Act
        WireMockList<object> list = (null as object)!;

        // Assert
        list.Should().HaveCount(1);
        list[0].Should().BeNull();
    }

    [Fact]
    public void WireMockListOfObject_ToString_WhenSingleNullObject_ShouldReturnEmptyString()
    {
        // Arrange
        object? nullObj = null;
        var list = new WireMockList<object>(nullObj!);

        // Act
        var result = list.ToString();

        // Assert
        result.Should().Be(string.Empty);
    }

    #endregion
}