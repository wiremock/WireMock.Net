// Copyright © WireMock.Net

//#if GRAPHQL
using System;
using FluentAssertions;
using WireMock.GraphQL.Models;
using Xunit;

namespace WireMock.Net.Tests.Matchers.Models;

public class WireMockCustomScalarGraphTypeTests
{
    private class MyIntScalarGraphType : WireMockCustomScalarGraphType<int> { }

    private class MyStringScalarGraphType : WireMockCustomScalarGraphType<string> { }

    [Fact]
    public void ParseValue_ShouldReturnNull_When_ValueIsNull()
    {
        // Arrange
        var intGraphType = new MyIntScalarGraphType();

        // Act
        var result = intGraphType.ParseValue(null);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ParseValue_ShouldReturnValue_When_ValueIsOfCorrectType()
    {
        // Arrange
        var intGraphType = new MyIntScalarGraphType();

        // Act
        var result = intGraphType.ParseValue(5);

        // Assert
        result.Should().Be(5);
    }

    [Theory]
    [InlineData("someString")]
    [InlineData("100")]
    public void ParseValue_ShouldThrowInvalidCastException_When_ValueIsStringAndTargetIsNotString(string stringValue)
    {
        // Arrange
        var intGraphType = new MyIntScalarGraphType();

        // Act
        Action act = () => intGraphType.ParseValue(stringValue);

        // Assert
        act.Should().Throw<InvalidCastException>()
           .WithMessage("Unable to convert value '*' of type 'System.String' to type 'System.Int32'.");
    }

    [Fact]
    public void ParseValue_ShouldConvertValue_WhenTypeIsConvertible()
    {
        // Arrange
        var intGraphType = new MyIntScalarGraphType();

        // Act
        var result = intGraphType.ParseValue(5L);

        // Assert
        result.Should().Be(5);
    }

    [Fact]
    public void ParseValue_ShouldThrowException_When_ValueIsMaxLongAndTargetIsInt()
    {
        // Arrange
        var intGraphType = new MyIntScalarGraphType();

        // Act
        Action act = () => intGraphType.ParseValue(long.MaxValue);

        // Assert
        act.Should().Throw<OverflowException>();
    }

    [Fact]
    public void ParseValue_ShouldReturnStringValue_When_TypeIsString()
    {
        // Arrange
        var stringGraphType = new MyStringScalarGraphType();

        // Act
        var result = stringGraphType.ParseValue("someString");

        // Assert
        result.Should().Be("someString");
    }
}
//#endif