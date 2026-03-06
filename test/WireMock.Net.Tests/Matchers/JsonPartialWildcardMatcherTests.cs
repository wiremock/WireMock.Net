// Copyright © WireMock.Net

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using WireMock.Matchers;

namespace WireMock.Net.Tests.Matchers;

public class JsonPartialWildcardMatcherTests
{
    [Fact]
    public void JsonPartialWildcardMatcher_GetName()
    {
        // Assign
        var matcher = new JsonPartialWildcardMatcher("{}");

        // Act
        var name = matcher.Name;

        // Assert
        name.Should().Be("JsonPartialWildcardMatcher");
    }

    [Fact]
    public void JsonPartialWildcardMatcher_GetValue()
    {
        // Assign
        var matcher = new JsonPartialWildcardMatcher("{}");

        // Act
        var value = matcher.Value;

        // Assert
        value.Should().Be("{}");
    }

    [Fact]
    public void JsonPartialWildcardMatcher_WithInvalidStringValue_Should_ThrowException()
    {
        // Act
        // ReSharper disable once ObjectCreationAsStatement
        Action action = () => new JsonPartialWildcardMatcher(MatchBehaviour.AcceptOnMatch, "{ \"Id\"");

        // Assert
        action.Should().Throw<JsonException>();
    }

    [Fact]
    public void JsonPartialWildcardMatcher_WithInvalidObjectValue_Should_ThrowException()
    {
        // Act
        // ReSharper disable once ObjectCreationAsStatement
        Action action = () => new JsonPartialWildcardMatcher(MatchBehaviour.AcceptOnMatch, new MemoryStream());

        // Assert
        action.Should().Throw<JsonException>();
    }

    [Fact]
    public void JsonPartialWildcardMatcher_IsMatch_WithInvalidValue_Should_ReturnMismatch_And_Exception_ShouldBeSet()
    {
        // Assign
        var matcher = new JsonPartialWildcardMatcher("");

        // Act
        var result = matcher.IsMatch(new MemoryStream());

        // Assert 
        result.Score.Should().Be(MatchScores.Mismatch);
        result.Exception.Should().BeAssignableTo<JsonException>();
    }

    [Fact]
    public void JsonPartialWildcardMatcher_IsMatch_ByteArray()
    {
        // Assign
        var bytes = new byte[0];
        var matcher = new JsonPartialWildcardMatcher("");

        // Act 
        var match = matcher.IsMatch(bytes).Score;

        // Assert 
        match.Should().Be(0);
    }

    [Fact]
    public void JsonPartialWildcardMatcher_IsMatch_NullString()
    {
        // Assign
        string? s = null;
        var matcher = new JsonPartialWildcardMatcher("");

        // Act 
        var match = matcher.IsMatch(s).Score;

        // Assert 
        match.Should().Be(0);
    }

    [Fact]
    public void JsonPartialWildcardMatcher_IsMatch_NullObject()
    {
        // Assign
        object? o = null;
        var matcher = new JsonPartialWildcardMatcher("");

        // Act 
        var match = matcher.IsMatch(o).Score;

        // Assert 
        match.Should().Be(0);
    }

    [Fact]
    public void JsonPartialWildcardMatcher_IsMatch_JArray()
    {
        // Assign 
        var matcher = new JsonPartialWildcardMatcher(new[] { "x", "y" });

        // Act 
        var jArray = new JArray
        {
            "x",
            "y"
        };
        var match = matcher.IsMatch(jArray).Score;

        // Assert 
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void JsonPartialWildcardMatcher_IsMatch_JObject()
    {
        // Assign 
        var matcher = new JsonPartialWildcardMatcher(new { Id = 1, Name = "Test" });

        // Act 
        var jObject = new JObject
        {
            { "Id", new JValue(1) },
            { "Name", new JValue("Test") }
        };
        var match = matcher.IsMatch(jObject).Score;

        // Assert 
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void JsonPartialWildcardMatcher_IsMatch_WithIgnoreCaseTrue_JObject()
    {
        // Assign 
        var matcher = new JsonPartialWildcardMatcher(new { id = 1, Name = "test" }, true);

        // Act 
        var jObject = new JObject
        {
            { "Id", new JValue(1) },
            { "NaMe", new JValue("Test") }
        };
        var match = matcher.IsMatch(jObject).Score;

        // Assert 
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void JsonPartialWildcardMatcher_IsMatch_JObjectParsed()
    {
        // Assign 
        var matcher = new JsonPartialWildcardMatcher(new { Id = 1, Name = "Test" });

        // Act 
        var jObject = JObject.Parse("{ \"Id\" : 1, \"Name\" : \"Test\" }");
        var match = matcher.IsMatch(jObject).Score;

        // Assert 
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void JsonPartialWildcardMatcher_IsMatch_WithIgnoreCaseTrue_JObjectParsed()
    {
        // Assign 
        var matcher = new JsonPartialWildcardMatcher(new { Id = 1, Name = "TESt" }, true);

        // Act 
        var jObject = JObject.Parse("{ \"Id\" : 1, \"Name\" : \"Test\" }");
        var match = matcher.IsMatch(jObject).Score;

        // Assert 
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void JsonPartialWildcardMatcher_IsMatch_JArrayAsString()
    {
        // Assign 
        var matcher = new JsonPartialWildcardMatcher("[ \"x\", \"y\" ]");

        // Act 
        var jArray = new JArray
        {
            "x",
            "y"
        };
        var match = matcher.IsMatch(jArray).Score;

        // Assert 
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void JsonPartialWildcardMatcher_IsMatch_JObjectAsString()
    {
        // Assign 
        var matcher = new JsonPartialWildcardMatcher("{ \"Id\" : 1, \"Name\" : \"Test\" }");

        // Act 
        var jObject = new JObject
        {
            { "Id", new JValue(1) },
            { "Name", new JValue("Test") }
        };
        var match = matcher.IsMatch(jObject).Score;

        // Assert 
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void JsonPartialWildcardMatcher_IsMatch_WithIgnoreCaseTrue_JObjectAsString()
    {
        // Assign 
        var matcher = new JsonPartialWildcardMatcher("{ \"Id\" : 1, \"Name\" : \"test\" }", true);

        // Act 
        var jObject = new JObject
        {
            { "Id", new JValue(1) },
            { "Name", new JValue("Test") }
        };
        var match = matcher.IsMatch(jObject).Score;

        // Assert 
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void JsonPartialWildcardMatcher_IsMatch_JObjectAsString_RejectOnMatch()
    {
        // Assign 
        var matcher = new JsonPartialWildcardMatcher(MatchBehaviour.RejectOnMatch, "{ \"Id\" : 1, \"Name\" : \"Test\" }");

        // Act 
        var jObject = new JObject
        {
            { "Id", new JValue(1) },
            { "Name", new JValue("Test") }
        };
        var match = matcher.IsMatch(jObject).Score;

        // Assert 
        Assert.Equal(0.0, match);
    }

    [Fact]
    public void JsonPartialWildcardMatcher_IsMatch_JObjectWithDateTimeOffsetAsString()
    {
        // Assign 
        var matcher = new JsonPartialWildcardMatcher("{ \"preferredAt\" : \"2019-11-21T10:32:53.2210009+00:00\" }");

        // Act 
        var jObject = new JObject
        {
            { "preferredAt", new JValue("2019-11-21T10:32:53.2210009+00:00") }
        };
        var match = matcher.IsMatch(jObject).Score;

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
    public void JsonPartialWildcardMatcher_IsMatch_StringInput_IsValidMatch(string value, string input)
    {
        // Assign
        var matcher = new JsonPartialWildcardMatcher(value);

        // Act
        var match = matcher.IsMatch(input).Score;

        // Assert
        Assert.Equal(1.0, match);
    }

    [Theory]
    [InlineData("{\"test\":\"*\"}", "{\"test\":\"xxx\",\"other\":\"xyz\"}")]
    [InlineData("\"t*t\"", "\"test\"")]
    public void JsonPartialWildcardMatcher_IsMatch_StringInputWithWildcard_IsValidMatch(string value, string input)
    {
        // Assign
        var matcher = new JsonPartialWildcardMatcher(value);

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
    public void JsonPartialWildcardMatcher_IsMatch_StringInputWithInvalidMatch(string value, string? input)
    {
        // Assign
        var matcher = new JsonPartialWildcardMatcher(value);

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
    public void JsonPartialWildcardMatcher_IsMatch_ValueAsJPathValidMatch(string value, string input)
    {
        // Assign
        var matcher = new JsonPartialWildcardMatcher(value);

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
    public void JsonPartialWildcardMatcher_IsMatch_ValueAsJPathInvalidMatch(string value, string input)
    {
        // Assign
        var matcher = new JsonPartialWildcardMatcher(value);

        // Act
        var match = matcher.IsMatch(input).Score;

        // Assert
        Assert.Equal(0.0, match);
    }

    [Fact]
    public void JsonPartialWildcardMatcher_IsMatch_WithIgnoreCaseTrueAndRegexTrue_JObject1()
    {
        // Assign 
        var matcher = new JsonPartialWildcardMatcher(new { id = 1, Number = "^\\d+$" }, ignoreCase: true, regex: true);

        // Act 
        var jObject = new JObject
        {
            { "Id", new JValue(1) },
            { "Number", new JValue(1) }
        };
        var match = matcher.IsMatch(jObject).Score;

        // Assert 
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void JsonPartialWildcardMatcher_IsMatch_WithIgnoreCaseTrueAndRegexTrue_JObject2()
    {
        // Assign 
        var matcher = new JsonPartialWildcardMatcher(new { method = "initialize", id = "^[a-f0-9]{32}-[0-9]$" }, ignoreCase: true, regex: true);

        // Act 
        var jObject = JsonConvert.DeserializeObject(
            "{\"jsonrpc\":\"2.0\",\"id\":\"ec475f56d4694b48bc737500ba575b35-1\",\"method\":\"initialize\",\"params\":{\"protocolVersion\":\"2024-11-05\",\"capabilities\":{},\"clientInfo\":{\"name\":\"GitHub Test\",\"version\":\"1.0.0\"}}}");
        var match = matcher.IsMatch(jObject).Score;

        // Assert 
        Assert.Equal(1.0, match);
    }
}
