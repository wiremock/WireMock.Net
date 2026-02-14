// Copyright © WireMock.Net

using FluentAssertions;
using Newtonsoft.Json.Linq;
using WireMock.Util;

namespace WireMock.Net.Tests.Util;

public class CSharpFormatterTests
{
    [Fact]
    public void ConvertToAnonymousObjectDefinition_ShouldReturn_ValidValue_WhenJsonBodyIsValidJsonAsObjectString()
    {
        // Arrange
        var jsonBody = new { Key1 = "value1", Key2 = 42, F = 1.2 };
        var expectedOutput = "new\r\n    {\r\n        Key1 = \"value1\",\r\n        Key2 = 42,\r\n        F = 1.2\r\n    }";

        // Act
        var result = CSharpFormatter.ConvertToAnonymousObjectDefinition(jsonBody, 1);

        // Assert
        result.Should().Be(expectedOutput);
    }

    [Fact]
    public void ConvertToAnonymousObjectDefinition_ShouldReturn_ValidValue_WhenJsonBodyIsValidJsonAsArrayString()
    {
        // Arrange
        var jsonBody = new[] { new { test = "a" }, new { test = "b" } };
        var expectedOutput = "new []\r\n    {\r\n        new\r\n        {\r\n            test = \"a\"\r\n        },\r\n        new\r\n        {\r\n            test = \"b\"\r\n        }\r\n    }";

        // Act
        var result = CSharpFormatter.ConvertToAnonymousObjectDefinition(jsonBody, 1);

        // Assert
        result.Should().Be(expectedOutput);
    }

    [Fact]
    public void ToCSharpStringLiteral_ShouldReturn_ValidValue_WhenStringIsNotNull()
    {
        // Arrange
        var inputString = "test string";
        var expectedOutput = "\"test string\"";

        // Act
        var result = CSharpFormatter.ToCSharpStringLiteral(inputString);

        // Assert
        result.Should().Be(expectedOutput);
    }

    [Fact]
    public void ToCSharpStringLiteral_ShouldReturn_ValidValue_WhenStringContainsNewLineCharacters()
    {
        // Arrange
        var inputString = "line1\nline2\nline3";
        var expectedOutput = "@\"line1\nline2\nline3\"";

        // Action
        var result = CSharpFormatter.ToCSharpStringLiteral(inputString);

        // Assert
        result.Should().Be(expectedOutput);
    }

    [Fact]
    public void FormatPropertyName_ShouldReturn_ValidPropertyName_WhenPropertyNameIsNotReserved()
    {
        // Arrange
        var propertyName = "propertyname";
        var expectedOutput = "propertyname";

        // Action
        var result = CSharpFormatter.FormatPropertyName(propertyName);

        // Assert
        result.Should().Be(expectedOutput);
    }

    [Fact]
    public void FormatPropertyName_ShouldReturn_ValidPropertyName_WhenPropertyNameIsReserved()
    {
        // Arrange
        var propertyName = "class";
        var expectedOutput = "@class";

        // Action
        var result = CSharpFormatter.FormatPropertyName(propertyName);

        // Assert
        result.Should().Be(expectedOutput);
    }

    [Fact]
    public void ConvertJsonToAnonymousObjectDefinition_ShouldReturn_ValidObject_WhenJsonInputIsValid()
    {
        // Arrange
        var jObject = new JObject
        (
            new JProperty("Name", "John Smith"),
            new JProperty("Age", 25.1f),
            new JProperty("Gender", "Male"),
            new JProperty("address", new JObject
            (
                new JProperty("Street", "123 Main St"),
                new JProperty("City", "Anytown"),
                new JProperty("State", "CA"),
                new JProperty("Zip", "90001")
            )
        ));
        var expectedOutput = "new\r\n{\r\n    Name = \"John Smith\",\r\n    Age = 25.1,\r\n    Gender = \"Male\",\r\n    address = new\r\n    {\r\n        Street = \"123 Main St\",\r\n        City = \"Anytown\",\r\n        State = \"CA\",\r\n        Zip = \"90001\"\r\n    }\r\n}";

        // Action
        var result = CSharpFormatter.ConvertJsonToAnonymousObjectDefinition(jObject);

        // Assert
        result.Should().Be(expectedOutput);
    }
}