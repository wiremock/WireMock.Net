// Copyright © WireMock.Net

using System.Text.Json;
using WireMock.Matchers;

namespace WireMock.Net.Tests.Matchers;

public class SystemTextJsonPartialWildcardMatcherTests
{
    [Fact]
    public void SystemTextJsonPartialWildcardMatcher_GetName()
    {
        // Assign
        var matcher = new SystemTextJsonPartialWildcardMatcher("{}");

        // Act
        var name = matcher.Name;

        // Assert
        name.Should().Be("SystemTextJsonPartialWildcardMatcher");
    }

    [Fact]
    public void SystemTextJsonPartialWildcardMatcher_GetValue()
    {
        // Assign
        var matcher = new SystemTextJsonPartialWildcardMatcher("{}");

        // Act
        var value = matcher.Value;

        // Assert
        value.Should().Be("{}");
    }

    [Fact]
    public void SystemTextJsonPartialWildcardMatcher_WithInvalidStringValue_Should_ThrowException()
    {
        // Act
        Action action = () => new SystemTextJsonPartialWildcardMatcher(MatchBehaviour.AcceptOnMatch, "{ \"Id\"");

        // Assert
        action.Should().Throw<JsonException>();
    }

    [Fact]
    public void SystemTextJsonPartialWildcardMatcher_WithInvalidObjectValue_Should_ThrowException()
    {
        // Act
        Action action = () => new SystemTextJsonPartialWildcardMatcher(MatchBehaviour.AcceptOnMatch, new MemoryStream());

        // Assert
        action.Should().Throw<Exception>();
    }

    [Fact]
    public void SystemTextJsonPartialWildcardMatcher_IsMatch_WithInvalidValue_Should_ReturnMismatch_And_Exception_ShouldBeSet()
    {
        // Assign
        var matcher = new SystemTextJsonPartialWildcardMatcher("{}");

        // Act
        using var stream = new MemoryStream();
        var result = matcher.IsMatch(stream);

        // Assert
        result.Score.Should().Be(MatchScores.Mismatch);
        result.Exception.Should().NotBeNull();
    }

    [Fact]
    public void SystemTextJsonPartialWildcardMatcher_IsMatch_ByteArray()
    {
        // Assign
        var bytes = new byte[0];
        var matcher = new SystemTextJsonPartialWildcardMatcher("{}");

        // Act
        var match = matcher.IsMatch(bytes).Score;

        // Assert
        match.Should().Be(0);
    }

    [Fact]
    public void SystemTextJsonPartialWildcardMatcher_IsMatch_NullString()
    {
        // Assign
        string? s = null;
        var matcher = new SystemTextJsonPartialWildcardMatcher("{}");

        // Act
        var match = matcher.IsMatch(s).Score;

        // Assert
        match.Should().Be(0);
    }

    [Fact]
    public void SystemTextJsonPartialWildcardMatcher_IsMatch_NullObject()
    {
        // Assign
        object? o = null;
        var matcher = new SystemTextJsonPartialWildcardMatcher("{}");

        // Act
        var match = matcher.IsMatch(o).Score;

        // Assert
        match.Should().Be(0);
    }

    [Fact]
    public void SystemTextJsonPartialWildcardMatcher_IsMatch_JsonArray()
    {
        // Assign
        var matcher = new SystemTextJsonPartialWildcardMatcher(new[] { "x", "y" });

        // Act
        var match = matcher.IsMatch("[ \"x\", \"y\" ]").Score;

        // Assert
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void SystemTextJsonPartialWildcardMatcher_IsMatch_JsonObject()
    {
        // Assign
        var matcher = new SystemTextJsonPartialWildcardMatcher(new { Id = 1, Name = "Test" });

        // Act
        var match = matcher.IsMatch("{ \"Id\" : 1, \"Name\" : \"Test\" }").Score;

        // Assert
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void SystemTextJsonPartialWildcardMatcher_IsMatch_WithIgnoreCaseTrue_JsonObject()
    {
        // Assign
        var matcher = new SystemTextJsonPartialWildcardMatcher(new { id = 1, Name = "test" }, true);

        // Act
        var match = matcher.IsMatch("{ \"Id\" : 1, \"NaMe\" : \"Test\" }").Score;

        // Assert
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void SystemTextJsonPartialWildcardMatcher_IsMatch_JsonObjectParsed()
    {
        // Assign
        var matcher = new SystemTextJsonPartialWildcardMatcher(new { Id = 1, Name = "Test" });

        // Act
        var match = matcher.IsMatch("{ \"Id\" : 1, \"Name\" : \"Test\" }").Score;

        // Assert
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void SystemTextJsonPartialWildcardMatcher_IsMatch_WithIgnoreCaseTrue_JsonObjectParsed()
    {
        // Assign
        var matcher = new SystemTextJsonPartialWildcardMatcher(new { Id = 1, Name = "TESt" }, true);

        // Act
        var match = matcher.IsMatch("{ \"Id\" : 1, \"Name\" : \"Test\" }").Score;

        // Assert
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void SystemTextJsonPartialWildcardMatcher_IsMatch_JsonArrayAsString()
    {
        // Assign
        var matcher = new SystemTextJsonPartialWildcardMatcher("[ \"x\", \"y\" ]");

        // Act
        var match = matcher.IsMatch("[ \"x\", \"y\" ]").Score;

        // Assert
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void SystemTextJsonPartialWildcardMatcher_IsMatch_JsonObjectAsString()
    {
        // Assign
        var matcher = new SystemTextJsonPartialWildcardMatcher("{ \"Id\" : 1, \"Name\" : \"Test\" }");

        // Act
        var match = matcher.IsMatch("{ \"Id\" : 1, \"Name\" : \"Test\" }").Score;

        // Assert
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void SystemTextJsonPartialWildcardMatcher_IsMatch_WithIgnoreCaseTrue_JsonObjectAsString()
    {
        // Assign
        var matcher = new SystemTextJsonPartialWildcardMatcher("{ \"Id\" : 1, \"Name\" : \"test\" }", true);

        // Act
        var match = matcher.IsMatch("{ \"Id\" : 1, \"Name\" : \"Test\" }").Score;

        // Assert
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void SystemTextJsonPartialWildcardMatcher_IsMatch_JsonObjectAsString_RejectOnMatch()
    {
        // Assign
        var matcher = new SystemTextJsonPartialWildcardMatcher(MatchBehaviour.RejectOnMatch, "{ \"Id\" : 1, \"Name\" : \"Test\" }");

        // Act
        var match = matcher.IsMatch("{ \"Id\" : 1, \"Name\" : \"Test\" }").Score;

        // Assert
        Assert.Equal(0.0, match);
    }

    [Fact]
    public void SystemTextJsonPartialWildcardMatcher_IsMatch_JsonObjectWithDateTimeOffsetAsString()
    {
        // Assign
        var matcher = new SystemTextJsonPartialWildcardMatcher("{ \"preferredAt\" : \"2019-11-21T10:32:53.2210009+00:00\" }");

        // Act
        var match = matcher.IsMatch("{ \"preferredAt\" : \"2019-11-21T10:32:53.2210009+00:00\" }").Score;

        // Assert
        Assert.Equal(1.0, match);
    }

    [Theory]
    [InlineData("{\"test\":\"abc\"}", "{\"test\":\"abc\",\"other\":\"xyz\"}")]
    [InlineData("\"test\"", "\"test\"")]
    [InlineData("123", "123")]
    [InlineData("[\"test\"]", "[\"test\"]")]
    [InlineData("[\"test\"]", "[\"test\", \"other\"]")]
    [InlineData("[123]", "[123]")]
    [InlineData("[123]", "[123, 456]")]
    [InlineData("{ \"test\":\"value\" }", "{\"test\":\"value\",\"other\":123}")]
    [InlineData("{ \"test\":\"value\" }", "{\"test\":\"value\"}")]
    [InlineData("{\"test\":{\"nested\":\"value\"}}", "{\"test\":{\"nested\":\"value\"}}")]
    public void SystemTextJsonPartialWildcardMatcher_IsMatch_StringInput_IsValidMatch(string value, string input)
    {
        // Assign
        var matcher = new SystemTextJsonPartialWildcardMatcher(value);

        // Act
        var match = matcher.IsMatch(input).Score;

        // Assert
        Assert.Equal(1.0, match);
    }

    [Theory]
    [InlineData("{\"test\":\"*\"}", "{\"test\":\"xxx\",\"other\":\"xyz\"}")]
    [InlineData("\"t*t\"", "\"test\"")]
    public void SystemTextJsonPartialWildcardMatcher_IsMatch_StringInputWithWildcard_IsValidMatch(string value, string input)
    {
        // Assign
        var matcher = new SystemTextJsonPartialWildcardMatcher(value);

        // Act
        var match = matcher.IsMatch(input).Score;

        // Assert
        match.Should().Be(1.0);
    }

    [Theory]
    [InlineData("\"test\"", null)]
    [InlineData("\"test1\"", "\"test2\"")]
    [InlineData("123", "1234")]
    [InlineData("[\"test\"]", "[\"test1\"]")]
    [InlineData("[\"test\"]", "[\"test1\", \"test2\"]")]
    [InlineData("[123]", "[1234]")]
    [InlineData("{}", "\"test\"")]
    [InlineData("{ \"test\":\"value\" }", "{\"test\":\"value2\"}")]
    [InlineData("{ \"test.nested\":\"value\" }", "{\"test\":{\"nested\":\"value1\"}}")]
    [InlineData("{\"test\":{\"test1\":\"value\"}}", "{\"test\":{\"test1\":\"value1\"}}")]
    [InlineData("[{ \"test.nested\":\"value\" }]", "[{\"test\":{\"nested\":\"value1\"}}]")]
    public void SystemTextJsonPartialWildcardMatcher_IsMatch_StringInputWithInvalidMatch(string value, string? input)
    {
        // Assign
        var matcher = new SystemTextJsonPartialWildcardMatcher(value);

        // Act
        var match = matcher.IsMatch(input).Score;

        // Assert
        Assert.Equal(0.0, match);
    }

    [Theory]
    [InlineData("{ \"test.nested\":123 }", "{\"test\":{\"nested\":123}}")]
    [InlineData("{ \"test.nested\":[123, 456] }", "{\"test\":{\"nested\":[123, 456]}}")]
    [InlineData("{ \"test.nested\":\"value\" }", "{\"test\":{\"nested\":\"value\"}}")]
    [InlineData("{ \"['name.with.dot']\":\"value\" }", "{\"name.with.dot\":\"value\"}")]
    [InlineData("[{ \"test.nested\":\"value\" }]", "[{\"test\":{\"nested\":\"value\"}}]")]
    [InlineData("[{ \"['name.with.dot']\":\"value\" }]", "[{\"name.with.dot\":\"value\"}]")]
    public void SystemTextJsonPartialWildcardMatcher_IsMatch_ValueAsPathValidMatch(string value, string input)
    {
        // Assign
        var matcher = new SystemTextJsonPartialWildcardMatcher(value);

        // Act
        var match = matcher.IsMatch(input).Score;

        // Assert
        Assert.Equal(1.0, match);
    }

    [Theory]
    [InlineData("{ \"test.nested\":123 }", "{\"test\":{\"nested\":456}}")]
    [InlineData("{ \"test.nested\":[123, 456] }", "{\"test\":{\"nested\":[1, 2]}}")]
    [InlineData("{ \"test.nested\":\"value\" }", "{\"test\":{\"nested\":\"value1\"}}")]
    [InlineData("{ \"['name.with.dot']\":\"value\" }", "{\"name.with.dot\":\"value1\"}")]
    [InlineData("[{ \"test.nested\":\"value\" }]", "[{\"test\":{\"nested\":\"value1\"}}]")]
    [InlineData("[{ \"['name.with.dot']\":\"value\" }]", "[{\"name.with.dot\":\"value1\"}]")]
    public void SystemTextJsonPartialWildcardMatcher_IsMatch_ValueAsPathInvalidMatch(string value, string input)
    {
        // Assign
        var matcher = new SystemTextJsonPartialWildcardMatcher(value);

        // Act
        var match = matcher.IsMatch(input).Score;

        // Assert
        Assert.Equal(0.0, match);
    }

    [Fact]
    public void SystemTextJsonPartialWildcardMatcher_IsMatch_WithIgnoreCaseTrueAndRegexTrue_JsonObject1()
    {
        // Assign
        var matcher = new SystemTextJsonPartialWildcardMatcher(new { id = 1, Number = "^\\d+$" }, ignoreCase: true, regex: true);

        // Act
        var match = matcher.IsMatch("{ \"Id\" : 1, \"Number\" : \"42\" }").Score;

        // Assert
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void SystemTextJsonPartialWildcardMatcher_IsMatch_WithIgnoreCaseTrueAndRegexTrue_JsonObject2()
    {
        // Assign
        var matcher = new SystemTextJsonPartialWildcardMatcher(new { method = "initialize", id = "^[a-f0-9]{32}-[0-9]$" }, ignoreCase: true, regex: true);

        // Act
        var match = matcher.IsMatch("{\"jsonrpc\":\"2.0\",\"id\":\"ec475f56d4694b48bc737500ba575b35-1\",\"method\":\"initialize\",\"params\":{\"protocolVersion\":\"2024-11-05\",\"capabilities\":{},\"clientInfo\":{\"name\":\"GitHub Test\",\"version\":\"1.0.0\"}}}").Score;

        // Assert
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void SystemTextJsonPartialWildcardMatcher_IsMatch_JsonElement_ShouldMatch()
    {
        // Assign
        var matcher = new SystemTextJsonPartialWildcardMatcher(new { Id = 1, Name = "Test" });

        // Act
        var jsonElement = JsonDocument.Parse("{ \"Id\" : 1, \"Name\" : \"Test\", \"Extra\" : \"value\" }").RootElement;
        var match = matcher.IsMatch(jsonElement).Score;

        // Assert
        Assert.Equal(1.0, match);
    }
}
