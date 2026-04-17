// Copyright © WireMock.Net

using System.Text.Json;
using WireMock.Matchers;

namespace WireMock.Net.Tests.Matchers;

public class SystemTextJsonMatcherTests
{
    public enum NormalEnumStj
    {
        Abc
    }

    public class Test1Stj
    {
        public NormalEnumStj NormalEnum { get; set; }
    }

    [Fact]
    public void SystemTextJsonMatcher_GetName()
    {
        // Assign
        var matcher = new SystemTextJsonMatcher("{}");

        // Act
        var name = matcher.Name;

        // Assert
        name.Should().Be("SystemTextJsonMatcher");
    }

    [Fact]
    public void SystemTextJsonMatcher_GetValue()
    {
        // Assign
        var matcher = new SystemTextJsonMatcher("{}");

        // Act
        var value = matcher.Value;

        // Assert
        value.Should().Be("{}");
    }

    [Fact]
    public void SystemTextJsonMatcher_WithInvalidStringValue_Should_ThrowException()
    {
        // Act
        Action action = () => new SystemTextJsonMatcher(MatchBehaviour.AcceptOnMatch, "{ \"Id\"");

        // Assert
        action.Should().Throw<JsonException>();
    }

    [Fact]
    public void SystemTextJsonMatcher_WithInvalidObjectValue_Should_ThrowException()
    {
        // Act
        Action action = () => new SystemTextJsonMatcher(MatchBehaviour.AcceptOnMatch, new MemoryStream());

        // Assert
        action.Should().Throw<Exception>();
    }

    [Fact]
    public void SystemTextJsonMatcher_IsMatch_WithInvalidValue_Should_ReturnMismatch_And_Exception_ShouldBeSet()
    {
        // Assign
        var matcher = new SystemTextJsonMatcher("{}");

        // Act
        var result = matcher.IsMatch(new MemoryStream());

        // Assert
        result.Score.Should().Be(MatchScores.Mismatch);
        result.Exception.Should().NotBeNull();
    }

    [Fact]
    public void SystemTextJsonMatcher_IsMatch_ByteArray()
    {
        // Assign
        var bytes = new byte[0];
        var matcher = new SystemTextJsonMatcher("{}");

        // Act
        var match = matcher.IsMatch(bytes).Score;

        // Assert
        match.Should().Be(0);
    }

    [Fact]
    public void SystemTextJsonMatcher_IsMatch_NullString()
    {
        // Assign
        string? s = null;
        var matcher = new SystemTextJsonMatcher("{}");

        // Act
        var match = matcher.IsMatch(s).Score;

        // Assert
        match.Should().Be(0);
    }

    [Fact]
    public void SystemTextJsonMatcher_IsMatch_NullObject()
    {
        // Assign
        object? o = null;
        var matcher = new SystemTextJsonMatcher("{}");

        // Act
        var match = matcher.IsMatch(o).Score;

        // Assert
        match.Should().Be(0);
    }

    [Fact]
    public void SystemTextJsonMatcher_IsMatch_JsonArrayAsString()
    {
        // Assign
        var matcher = new SystemTextJsonMatcher("[ \"x\", \"y\" ]");

        // Act
        var jsonElement = JsonDocument.Parse("[ \"x\", \"y\" ]").RootElement;
        var match = matcher.IsMatch(jsonElement).Score;

        // Assert
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void SystemTextJsonMatcher_IsMatch_JsonObjectAsString_ShouldMatch()
    {
        // Assign
        var matcher = new SystemTextJsonMatcher("{ \"Id\" : 1, \"Name\" : \"Test\" }");

        // Act
        var jsonElement = JsonDocument.Parse("{ \"Id\" : 1, \"Name\" : \"Test\" }").RootElement;
        var match = matcher.IsMatch(jsonElement).Score;

        // Assert
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void SystemTextJsonMatcher_IsMatch_AnonymousObject_ShouldMatch()
    {
        // Assign
        var matcher = new SystemTextJsonMatcher(new { Id = 1, Name = "Test" });

        // Act
        var match = matcher.IsMatch("{ \"Id\" : 1, \"Name\" : \"Test\" }").Score;

        // Assert
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void SystemTextJsonMatcher_IsMatch_AnonymousObject_ShouldNotMatch()
    {
        // Assign
        var matcher = new SystemTextJsonMatcher(new { Id = 1, Name = "Test" });

        // Act
        var match = matcher.IsMatch("{ \"Id\" : 1, \"Name\" : \"Test\", \"Other\" : \"abc\" }").Score;

        // Assert
        Assert.Equal(MatchScores.Mismatch, match);
    }

    [Fact]
    public void SystemTextJsonMatcher_IsMatch_WithIgnoreCaseTrue_JsonObject()
    {
        // Assign
        var matcher = new SystemTextJsonMatcher(new { id = 1, Name = "test" }, true);

        // Act
        var match = matcher.IsMatch("{ \"Id\" : 1, \"NaMe\" : \"Test\" }").Score;

        // Assert
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void SystemTextJsonMatcher_IsMatch_WithIgnoreCaseTrue_JsonObjectParsed()
    {
        // Assign
        var matcher = new SystemTextJsonMatcher(new { Id = 1, Name = "TESt" }, true);

        // Act
        var match = matcher.IsMatch("{ \"Id\" : 1, \"Name\" : \"Test\" }").Score;

        // Assert
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void SystemTextJsonMatcher_IsMatch_JsonObjectAsString_RejectOnMatch()
    {
        // Assign
        var matcher = new SystemTextJsonMatcher(MatchBehaviour.RejectOnMatch, "{ \"Id\" : 1, \"Name\" : \"Test\" }");

        // Act
        var match = matcher.IsMatch("{ \"Id\" : 1, \"Name\" : \"Test\" }").Score;

        // Assert
        Assert.Equal(0.0, match);
    }

    [Fact]
    public void SystemTextJsonMatcher_IsMatch_JsonObjectWithDateTimeOffsetAsString()
    {
        // Assign
        var matcher = new SystemTextJsonMatcher("{ \"preferredAt\" : \"2019-11-21T10:32:53.2210009+00:00\" }");

        // Act
        var match = matcher.IsMatch("{ \"preferredAt\" : \"2019-11-21T10:32:53.2210009+00:00\" }").Score;

        // Assert
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void SystemTextJsonMatcher_IsMatch_NormalEnum()
    {
        // Assign
        var matcher = new SystemTextJsonMatcher(new Test1Stj { NormalEnum = NormalEnumStj.Abc });

        // Act
        var match = matcher.IsMatch("{ \"NormalEnum\" : 0 }").Score;

        // Assert
        match.Should().Be(1.0);
    }

    [Fact]
    public void SystemTextJsonMatcher_IsMatch_WithRegexTrue_ShouldMatch()
    {
        // Assign
        var matcher = new SystemTextJsonMatcher(new { Id = "^\\d+$", Name = "Test" }, regex: true);

        // Act
        var match = matcher.IsMatch("{ \"Id\" : \"42\", \"Name\" : \"Test\" }").Score;

        // Assert
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void SystemTextJsonMatcher_IsMatch_WithRegexTrue_Complex_ShouldMatch()
    {
        // Assign
        var matcher = new SystemTextJsonMatcher(new
        {
            Complex = new
            {
                Id = "^\\d+$",
                Name = ".*"
            }
        }, regex: true);

        // Act
        var match = matcher.IsMatch("{ \"Complex\" : { \"Id\" : \"42\", \"Name\" : \"Test\" } }").Score;

        // Assert
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void SystemTextJsonMatcher_IsMatch_WithRegexTrue_Complex_ShouldNotMatch()
    {
        // Assign
        var matcher = new SystemTextJsonMatcher(new
        {
            Complex = new
            {
                Id = "^\\d+$",
                Name = ".*"
            }
        }, regex: true);

        // Act
        var match = matcher.IsMatch("{ \"Complex\" : { \"Id\" : \"42\", \"Name\" : \"Test\", \"Other\" : \"Other\" } }").Score;

        // Assert
        Assert.Equal(MatchScores.Mismatch, match);
    }

    [Fact]
    public void SystemTextJsonMatcher_IsMatch_WithRegexTrue_Array_ShouldMatch()
    {
        // Assign
        var matcher = new SystemTextJsonMatcher(new
        {
            Array = new[] { "^\\d+$", ".*" }
        }, regex: true);

        // Act
        var match = matcher.IsMatch("{ \"Array\" : [ \"42\", \"test\" ] }").Score;

        // Assert
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void SystemTextJsonMatcher_IsMatch_WithRegexTrue_Array_ShouldNotMatch()
    {
        // Assign
        var matcher = new SystemTextJsonMatcher(new
        {
            Array = new[] { "^\\d+$", ".*" }
        }, regex: true);

        // Act
        var match = matcher.IsMatch("{ \"Array\" : [ \"42\", \"test\", \"other\" ] }").Score;

        // Assert
        Assert.Equal(MatchScores.Mismatch, match);
    }

    [Fact]
    public void SystemTextJsonMatcher_IsMatch_GuidAndString()
    {
        // Assign
        var id = Guid.NewGuid();
        var idAsString = id.ToString();
        var matcher = new SystemTextJsonMatcher(new { Id = id });

        // Act
        var match = matcher.IsMatch($"{{ \"Id\" : \"{idAsString}\" }}").Score;

        // Assert
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void SystemTextJsonMatcher_IsMatch_StringAndGuid()
    {
        // Assign
        var id = Guid.NewGuid();
        var idAsString = id.ToString();
        var matcher = new SystemTextJsonMatcher(new { Id = idAsString });

        // Act
        var match = matcher.IsMatch($"{{ \"Id\" : \"{id}\" }}").Score;

        // Assert
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void SystemTextJsonMatcher_IsMatch_JsonElement_ShouldMatch()
    {
        // Assign
        var matcher = new SystemTextJsonMatcher(new { Id = 1, Name = "Test" });

        // Act
        var jsonElement = JsonDocument.Parse("{ \"Id\" : 1, \"Name\" : \"Test\" }").RootElement;
        var match = matcher.IsMatch(jsonElement).Score;

        // Assert
        Assert.Equal(1.0, match);
    }
}
