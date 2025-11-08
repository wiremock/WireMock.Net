// Copyright © WireMock.Net

using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using WireMock.Validators;
using Xunit;

namespace WireMock.Net.Tests.Validators;

[ExcludeFromCodeCoverage]
public class PathValidatorTests
{
    [Fact]
    public void ValidateAndThrow_ValidPath_DoesNotThrow()
    {
        Action act = () => PathValidator.ValidateAndThrow("/valid/path");
        act.Should().NotThrow();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("\r")]
    [InlineData("\n")]
    [InlineData("\t")]
    public void ValidateAndThrow_InvalidPath_ThrowsArgumentException_WithDefaultParamName(string? path)
    {
        Action act = () => PathValidator.ValidateAndThrow(path);
        var ex = act.Should().Throw<ArgumentException>().Which;
        ex.Message.Should().StartWith("Path must start with a '/' and cannot be null, empty or whitespace.");
        ex.ParamName.Should().Be("path");
    }

    [Fact]
    public void ValidateAndThrow_NoLeadingSlash_ThrowsArgumentException_WithProvidedParamName()
    {
        Action act = () => PathValidator.ValidateAndThrow("noSlash", "myParam");
        var ex = act.Should().Throw<ArgumentException>().Which;
        ex.ParamName.Should().Be("myParam");
    }
}