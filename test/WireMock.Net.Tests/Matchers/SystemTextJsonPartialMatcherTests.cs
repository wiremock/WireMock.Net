// Copyright © WireMock.Net

using System.Text.Json;
using WireMock.Matchers;

namespace WireMock.Net.Tests.Matchers;

public class SystemTextJsonPartialMatcherTests
{
    [Fact]
    public void SystemTextJsonPartialMatcher_GetName()
    {
        // Assign
        var matcher = new SystemTextJsonPartialMatcher("{}");

        // Act
        string name = matcher.Name;

        // Assert
        name.Should().Be("SystemTextJsonPartialMatcher");
    }

    [Fact]
    public void SystemTextJsonPartialMatcher_GetValue()
    {
        // Assign
        var matcher = new SystemTextJsonPartialMatcher("{}");

        // Act
        object value = matcher.Value;

        // Assert
        value.Should().Be("{}");
    }

    [Fact]
    public void SystemTextJsonPartialMatcher_WithInvalidStringValue_Should_ThrowException()
    {
        // Act
        Action action = () => new SystemTextJsonPartialMatcher(MatchBehaviour.AcceptOnMatch, "{ \"Id\"");

        // Assert
        action.Should().Throw<JsonException>();
    }

    [Fact]
    public void SystemTextJsonPartialMatcher_WithInvalidObjectValue_Should_ThrowException()
    {
        // Act
        Action action = () => new SystemTextJsonPartialMatcher(MatchBehaviour.AcceptOnMatch, new MemoryStream());

        // Assert
        action.Should().Throw<Exception>();
    }

    [Fact]
    public void SystemTextJsonPartialMatcher_IsMatch_WithInvalidValue_Should_ReturnMismatch_And_Exception_ShouldBeSet()
    {
        // Assign
        var matcher = new SystemTextJsonPartialMatcher("{}");
        using var stream = new MemoryStream();

        // Act
        var result = matcher.IsMatch(stream);

        // Assert
        result.Score.Should().Be(MatchScores.Mismatch);
        result.Exception.Should().NotBeNull();
    }

    [Fact]
    public void SystemTextJsonPartialMatcher_IsMatch_ByteArray()
    {
        // Assign
        var bytes = new byte[0];
        var matcher = new SystemTextJsonPartialMatcher("{}");

        // Act
        double match = matcher.IsMatch(bytes).Score;

        // Assert
        match.Should().Be(0);
    }

    [Fact]
    public void SystemTextJsonPartialMatcher_IsMatch_NullString()
    {
        // Assign
        string? s = null;
        var matcher = new SystemTextJsonPartialMatcher("{}");

        // Act
        double match = matcher.IsMatch(s).Score;

        // Assert
        match.Should().Be(0);
    }

    [Fact]
    public void SystemTextJsonPartialMatcher_IsMatch_NullObject()
    {
        // Assign
        object? o = null;
        var matcher = new SystemTextJsonPartialMatcher("{}");

        // Act
        double match = matcher.IsMatch(o).Score;

        // Assert
        match.Should().Be(0);
    }

    [Fact]
    public void SystemTextJsonPartialMatcher_IsMatch_JsonArray()
    {
        // Assign
        var matcher = new SystemTextJsonPartialMatcher(new[] { "x", "y" });

        // Act
        double match = matcher.IsMatch("[ \"x\", \"y\" ]").Score;

        // Assert
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void SystemTextJsonPartialMatcher_IsMatch_JsonObject()
    {
        // Assign
        var matcher = new SystemTextJsonPartialMatcher(new { Id = 1, Name = "Test" });

        // Act
        double match = matcher.IsMatch("{ \"Id\" : 1, \"Name\" : \"Test\" }").Score;

        // Assert
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void SystemTextJsonPartialMatcher_IsMatch_WithRegexTrue()
    {
        // Assign
        var matcher = new SystemTextJsonPartialMatcher(new { Id = "^\\d+$", Name = "Test" }, false, true);

        // Act
        double match = matcher.IsMatch("{ \"Id\" : \"1\", \"Name\" : \"Test\" }").Score;

        // Assert
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void SystemTextJsonPartialMatcher_IsMatch_WithRegexFalse()
    {
        // Assign
        var matcher = new SystemTextJsonPartialMatcher(new { Id = "^\\d+$", Name = "Test" });

        // Act
        double match = matcher.IsMatch("{ \"Id\" : 1, \"Name\" : \"Test\" }").Score;

        // Assert
        Assert.Equal(0.0, match);
    }

    [Fact]
    public void SystemTextJsonPartialMatcher_IsMatch_GuidAsString_UsingRegex()
    {
        var guid = new Guid("1111238e-b775-44a9-a263-95e570135c94");
        var matcher = new SystemTextJsonPartialMatcher(new
        {
            Id = 1,
            Name = "^1111[a-fA-F0-9]{4}(-[a-fA-F0-9]{4}){3}-[a-fA-F0-9]{12}$"
        }, false, true);

        // Act
        double match = matcher.IsMatch($"{{ \"Id\" : 1, \"Name\" : \"{guid}\" }}").Score;

        // Assert
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void SystemTextJsonPartialMatcher_IsMatch_WithIgnoreCaseTrue_JsonObject()
    {
        // Assign
        var matcher = new SystemTextJsonPartialMatcher(new { id = 1, Name = "test" }, true);

        // Act
        double match = matcher.IsMatch("{ \"Id\" : 1, \"NaMe\" : \"Test\" }").Score;

        // Assert
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void SystemTextJsonPartialMatcher_IsMatch_JsonObjectParsed()
    {
        // Assign
        var matcher = new SystemTextJsonPartialMatcher(new { Id = 1, Name = "Test" });

        // Act
        double match = matcher.IsMatch("{ \"Id\" : 1, \"Name\" : \"Test\" }").Score;

        // Assert
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void SystemTextJsonPartialMatcher_IsMatch_WithIgnoreCaseTrue_JsonObjectParsed()
    {
        // Assign
        var matcher = new SystemTextJsonPartialMatcher(new { Id = 1, Name = "TESt" }, true);

        // Act
        double match = matcher.IsMatch("{ \"Id\" : 1, \"Name\" : \"Test\" }").Score;

        // Assert
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void SystemTextJsonPartialMatcher_IsMatch_JsonArrayAsString()
    {
        // Assign
        var matcher = new SystemTextJsonPartialMatcher("[ \"x\", \"y\" ]");

        // Act
        double match = matcher.IsMatch("[ \"x\", \"y\" ]").Score;

        // Assert
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void SystemTextJsonPartialMatcher_IsMatch_JsonObjectAsString()
    {
        // Assign
        var matcher = new SystemTextJsonPartialMatcher("{ \"Id\" : 1, \"Name\" : \"Test\" }");

        // Act
        double match = matcher.IsMatch("{ \"Id\" : 1, \"Name\" : \"Test\" }").Score;

        // Assert
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void SystemTextJsonPartialMatcher_IsMatch_JsonObjectAsStringWithDottedPropertyName()
    {
        // Assign
        var matcher = new SystemTextJsonPartialMatcher("{ \"urn:ietf:params:scim:schemas:extension:enterprise:2.0:User\" : \"Test\" }");

        // Act
        double match = matcher.IsMatch("{ \"urn:ietf:params:scim:schemas:extension:enterprise:2.0:User\" : \"Test\" }").Score;

        // Assert
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void SystemTextJsonPartialMatcher_IsMatch_GuidAsString()
    {
        // Assign
        var guid = Guid.NewGuid();
        var matcher = new SystemTextJsonPartialMatcher(new { Id = 1, Name = guid });

        // Act
        double match = matcher.IsMatch($"{{ \"Id\" : 1, \"Name\" : \"{guid}\" }}").Score;

        // Assert
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void SystemTextJsonPartialMatcher_IsMatch_WithIgnoreCaseTrue_JsonObjectAsString()
    {
        // Assign
        var matcher = new SystemTextJsonPartialMatcher("{ \"Id\" : 1, \"Name\" : \"test\" }", true);

        // Act
        double match = matcher.IsMatch("{ \"Id\" : 1, \"Name\" : \"Test\" }").Score;

        // Assert
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void SystemTextJsonPartialMatcher_IsMatch_JsonObjectAsString_RejectOnMatch()
    {
        // Assign
        var matcher = new SystemTextJsonPartialMatcher(MatchBehaviour.RejectOnMatch, "{ \"Id\" : 1, \"Name\" : \"Test\" }");

        // Act
        double match = matcher.IsMatch("{ \"Id\" : 1, \"Name\" : \"Test\" }").Score;

        // Assert
        Assert.Equal(0.0, match);
    }

    [Fact]
    public void SystemTextJsonPartialMatcher_IsMatch_JsonObjectWithDateTimeOffsetAsString()
    {
        // Assign
        var matcher = new SystemTextJsonPartialMatcher("{ \"preferredAt\" : \"2019-11-21T10:32:53.2210009+00:00\" }");

        // Act
        double match = matcher.IsMatch("{ \"preferredAt\" : \"2019-11-21T10:32:53.2210009+00:00\" }").Score;

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
    public void SystemTextJsonPartialMatcher_IsMatch_StringInputValidMatch(string value, string input)
    {
        // Assign
        var matcher = new SystemTextJsonPartialMatcher(value);

        // Act
        double match = matcher.IsMatch(input).Score;

        // Assert
        Assert.Equal(1.0, match);
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
    public void SystemTextJsonPartialMatcher_IsMatch_StringInputWithInvalidMatch(string value, string? input)
    {
        // Assign
        var matcher = new SystemTextJsonPartialMatcher(value);

        // Act
        double match = matcher.IsMatch(input).Score;

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
    public void SystemTextJsonPartialMatcher_IsMatch_ValueAsPathValidMatch(string value, string input)
    {
        // Assign
        var matcher = new SystemTextJsonPartialMatcher(value);

        // Act
        double match = matcher.IsMatch(input).Score;

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
    public void SystemTextJsonPartialMatcher_IsMatch_ValueAsPathInvalidMatch(string value, string input)
    {
        // Assign
        var matcher = new SystemTextJsonPartialMatcher(value);

        // Act
        double match = matcher.IsMatch(input).Score;

        // Assert
        Assert.Equal(0.0, match);
    }

    [Fact]
    public void SystemTextJsonPartialMatcher_IsMatch_JsonElement_ShouldMatch()
    {
        // Assign
        var matcher = new SystemTextJsonPartialMatcher(new { Id = 1, Name = "Test" });

        // Act
        var jsonElement = JsonDocument.Parse("{ \"Id\" : 1, \"Name\" : \"Test\", \"Extra\" : \"value\" }").RootElement;
        double match = matcher.IsMatch(jsonElement).Score;

        // Assert
        Assert.Equal(1.0, match);
    }
}
