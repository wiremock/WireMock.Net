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

    #region Equality Operator Tests

    [Fact]
    public void WireMockListOfString_EqualityOperator_ListToSingleValue_WhenSingleElementMatchesValue_ShouldBeTrue()
    {
        // Arrange
        var list = new WireMockList<string>("hello");

        // Act & Assert
        (list == "hello").Should().BeTrue();
    }

    [Fact]
    public void WireMockListOfString_EqualityOperator_ListToSingleValue_WhenSingleElementDoesNotMatchValue_ShouldBeFalse()
    {
        // Arrange
        var list = new WireMockList<string>("hello");

        // Act & Assert
        (list == "world").Should().BeFalse();
    }

    [Fact]
    public void WireMockListOfString_EqualityOperator_ListToSingleValue_WhenMultipleElements_ShouldBeFalse()
    {
        // Arrange
        var list = new WireMockList<string>("hello", "world");

        // Act & Assert
        (list == "hello").Should().BeFalse();
    }

    [Fact]
    public void WireMockListOfString_EqualityOperator_SingleValueToList_WhenSingleElementMatchesValue_ShouldBeTrue()
    {
        // Arrange
        var list = new WireMockList<string>("hello");

        // Act & Assert
        ("hello" == list).Should().BeTrue();
    }

    [Fact]
    public void WireMockListOfString_EqualityOperator_SingleValueToList_WhenSingleElementDoesNotMatchValue_ShouldBeFalse()
    {
        // Arrange
        var list = new WireMockList<string>("hello");

        // Act & Assert
        ("world" == list).Should().BeFalse();
    }

    [Fact]
    public void WireMockListOfString_InequalityOperator_ListToSingleValue_WhenSingleElementMatchesValue_ShouldBeFalse()
    {
        // Arrange
        var list = new WireMockList<string>("hello");

        // Act & Assert
        (list != "hello").Should().BeFalse();
    }

    [Fact]
    public void WireMockListOfString_InequalityOperator_ListToSingleValue_WhenSingleElementDoesNotMatchValue_ShouldBeTrue()
    {
        // Arrange
        var list = new WireMockList<string>("hello");

        // Act & Assert
        (list != "world").Should().BeTrue();
    }

    [Fact]
    public void WireMockListOfString_InequalityOperator_SingleValueToList_WhenSingleElementMatchesValue_ShouldBeFalse()
    {
        // Arrange
        var list = new WireMockList<string>("hello");

        // Act & Assert
        ("hello" != list).Should().BeFalse();
    }

    [Fact]
    public void WireMockListOfString_InequalityOperator_SingleValueToList_WhenSingleElementDoesNotMatchValue_ShouldBeTrue()
    {
        // Arrange
        var list = new WireMockList<string>("hello");

        // Act & Assert
        ("world" != list).Should().BeTrue();
    }

    #endregion

    #region Equals and GetHashCode Tests

    [Fact]
    public void WireMockListOfString_Equals_WhenBothListsHaveSameElements_ShouldBeTrue()
    {
        // Arrange
        var list1 = new WireMockList<string>("a", "b", "c");
        var list2 = new WireMockList<string>("a", "b", "c");

        // Act & Assert
        list1.Equals(list2).Should().BeTrue();
    }

    [Fact]
    public void WireMockListOfString_Equals_WhenListsHaveDifferentElements_ShouldBeFalse()
    {
        // Arrange
        var list1 = new WireMockList<string>("a", "b");
        var list2 = new WireMockList<string>("a", "x");

        // Act & Assert
        list1.Equals(list2).Should().BeFalse();
    }

    [Fact]
    public void WireMockListOfString_Equals_WhenListsHaveDifferentCounts_ShouldBeFalse()
    {
        // Arrange
        var list1 = new WireMockList<string>("a", "b");
        var list2 = new WireMockList<string>("a");

        // Act & Assert
        list1.Equals(list2).Should().BeFalse();
    }

    [Fact]
    public void WireMockListOfString_Equals_WhenBothEmpty_ShouldBeTrue()
    {
        // Arrange
        var list1 = new WireMockList<string>();
        var list2 = new WireMockList<string>();

        // Act & Assert
        list1.Equals(list2).Should().BeTrue();
    }

    [Fact]
    public void WireMockListOfString_Equals_WhenSameReference_ShouldBeTrue()
    {
        // Arrange
        var list = new WireMockList<string>("a", "b");

        // Act & Assert
        list.Equals(list).Should().BeTrue();
    }

    [Fact]
    public void WireMockListOfString_Equals_WhenComparedToNull_ShouldBeFalse()
    {
        // Arrange
        var list = new WireMockList<string>("a");

        // Act & Assert
        list.Equals(null).Should().BeFalse();
    }

    [Fact]
    public void WireMockListOfString_Equals_WhenComparedToNonWireMockList_ShouldBeFalse()
    {
        // Arrange
        var list = new WireMockList<string>("a");

        // Act & Assert
        list.Equals(new List<string> { "a" }).Should().BeFalse();
    }

    [Fact]
    public void WireMockListOfString_GetHashCode_WhenListsHaveSameElements_ShouldBeEqual()
    {
        // Arrange
        var list1 = new WireMockList<string>("a", "b", "c");
        var list2 = new WireMockList<string>("a", "b", "c");

        // Act & Assert
        list1.GetHashCode().Should().Be(list2.GetHashCode());
    }

    [Fact]
    public void WireMockListOfString_GetHashCode_WhenListsHaveDifferentElements_ShouldNotBeEqual()
    {
        // Arrange
        var list1 = new WireMockList<string>("a", "b");
        var list2 = new WireMockList<string>("x", "y");

        // Act & Assert
        list1.GetHashCode().Should().NotBe(list2.GetHashCode());
    }

    [Fact]
    public void WireMockListOfString_GetHashCode_WhenEmpty_ShouldReturnConsistentValue()
    {
        // Arrange
        var list1 = new WireMockList<string>();
        var list2 = new WireMockList<string>();

        // Act & Assert
        list1.GetHashCode().Should().Be(list2.GetHashCode());
    }

    #endregion
}