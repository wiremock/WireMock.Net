// Copyright © WireMock.Net

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using WireMock.Matchers;

namespace WireMock.Net.Tests.Matchers;

public class JsonPartialMatcherTests
{
    [Fact]
    public void JsonPartialMatcher_GetName()
    {
        // Assign
        var matcher = new JsonPartialMatcher("{}");

        // Act
        string name = matcher.Name;

        // Assert
        name.Should().Be("JsonPartialMatcher");
    }

    [Fact]
    public void JsonPartialMatcher_GetValue()
    {
        // Assign
        var matcher = new JsonPartialMatcher("{}");

        // Act
        object value = matcher.Value;

        // Assert
        value.Should().Be("{}");
    }

    [Fact]
    public void JsonPartialMatcher_WithInvalidStringValue_Should_ThrowException()
    {
        // Act
        // ReSharper disable once ObjectCreationAsStatement
        Action action = () => new JsonPartialMatcher(MatchBehaviour.AcceptOnMatch, "{ \"Id\"");

        // Assert
        action.Should().Throw<JsonException>();
    }

    [Fact]
    public void JsonPartialMatcher_WithInvalidObjectValue_Should_ThrowException()
    {
        // Act
        // ReSharper disable once ObjectCreationAsStatement
        Action action = () => new JsonPartialMatcher(MatchBehaviour.AcceptOnMatch, new MemoryStream());

        // Assert
        action.Should().Throw<JsonException>();
    }

    [Fact]
    public void JsonPartialMatcher_IsMatch_WithInvalidValue_Should_ReturnMismatch_And_Exception_ShouldBeSet()
    {
        // Assign
        var matcher = new JsonPartialMatcher("");

        // Act
        var result = matcher.IsMatch(new MemoryStream());

        // Assert 
        result.Score.Should().Be(MatchScores.Mismatch);
        result.Exception.Should().BeAssignableTo<JsonException>();
    }

    [Fact]
    public void JsonPartialMatcher_IsMatch_ByteArray()
    {
        // Assign
        var bytes = new byte[0];
        var matcher = new JsonPartialMatcher("");

        // Act 
        double match = matcher.IsMatch(bytes).Score;

        // Assert 
        match.Should().Be(0);
    }

    [Fact]
    public void JsonPartialMatcher_IsMatch_NullString()
    {
        // Assign
        string? s = null;
        var matcher = new JsonPartialMatcher("");

        // Act 
        double match = matcher.IsMatch(s).Score;

        // Assert 
        match.Should().Be(0);
    }

    [Fact]
    public void JsonPartialMatcher_IsMatch_NullObject()
    {
        // Assign
        object? o = null;
        var matcher = new JsonPartialMatcher("");

        // Act 
        double match = matcher.IsMatch(o).Score;

        // Assert 
        match.Should().Be(0);
    }

    [Fact]
    public void JsonPartialMatcher_IsMatch_JArray()
    {
        // Assign 
        var matcher = new JsonPartialMatcher(new[] { "x", "y" });

        // Act 
        var jArray = new JArray
        {
            "x",
            "y"
        };
        double match = matcher.IsMatch(jArray).Score;

        // Assert 
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void JsonPartialMatcher_IsMatch_JObject()
    {
        // Assign 
        var matcher = new JsonPartialMatcher(new { Id = 1, Name = "Test" });

        // Act 
        var jObject = new JObject
        {
            { "Id", new JValue(1) },
            { "Name", new JValue("Test") }
        };
        double match = matcher.IsMatch(jObject).Score;

        // Assert 
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void JsonPartialMatcher_IsMatch_WithRegexTrue()
    {
        // Assign 
        var matcher = new JsonPartialMatcher(new { Id = "^\\d+$", Name = "Test" }, false, true);

        // Act 
        var jObject = new JObject
        {
            { "Id", new JValue(1) },
            { "Name", new JValue("Test") }
        };
        double match = matcher.IsMatch(jObject).Score;

        // Assert 
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void JsonPartialMatcher_IsMatch_WithRegexFalse()
    {
        // Assign 
        var matcher = new JsonPartialMatcher(new { Id = "^\\d+$", Name = "Test" });

        // Act 
        var jObject = new JObject
        {
            { "Id", new JValue(1) },
            { "Name", new JValue("Test") }
        };
        double match = matcher.IsMatch(jObject).Score;

        // Assert 
        Assert.Equal(0.0, match);
    }

    [Fact]
    public void JsonPartialMatcher_IsMatch_GuidAsString_UsingRegex()
    {
        var guid = new Guid("1111238e-b775-44a9-a263-95e570135c94");
        var matcher = new JsonPartialMatcher(new
        {
            Id = 1,
            Name = "^1111[a-fA-F0-9]{4}(-[a-fA-F0-9]{4}){3}-[a-fA-F0-9]{12}$"
        }, false, true);

        // Act
        var jObject = new JObject
        {
            { "Id", new JValue(1) },
            { "Name", new JValue(guid) }
        };
        double match = matcher.IsMatch(jObject).Score;

        // Assert
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void JsonPartialMatcher_IsMatch_WithIgnoreCaseTrue_JObject()
    {
        // Assign 
        var matcher = new JsonPartialMatcher(new { id = 1, Name = "test" }, true);

        // Act 
        var jObject = new JObject
        {
            { "Id", new JValue(1) },
            { "NaMe", new JValue("Test") }
        };
        double match = matcher.IsMatch(jObject).Score;

        // Assert 
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void JsonPartialMatcher_IsMatch_JObjectParsed()
    {
        // Assign 
        var matcher = new JsonPartialMatcher(new { Id = 1, Name = "Test" });

        // Act 
        var jObject = JObject.Parse("{ \"Id\" : 1, \"Name\" : \"Test\" }");
        double match = matcher.IsMatch(jObject).Score;

        // Assert 
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void JsonPartialMatcher_IsMatch_WithIgnoreCaseTrue_JObjectParsed()
    {
        // Assign 
        var matcher = new JsonPartialMatcher(new { Id = 1, Name = "TESt" }, true);

        // Act 
        var jObject = JObject.Parse("{ \"Id\" : 1, \"Name\" : \"Test\" }");
        double match = matcher.IsMatch(jObject).Score;

        // Assert 
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void JsonPartialMatcher_IsMatch_JArrayAsString()
    {
        // Assign 
        var matcher = new JsonPartialMatcher("[ \"x\", \"y\" ]");

        // Act 
        var jArray = new JArray
        {
            "x",
            "y"
        };
        double match = matcher.IsMatch(jArray).Score;

        // Assert 
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void JsonPartialMatcher_IsMatch_JObjectAsString()
    {
        // Assign 
        var matcher = new JsonPartialMatcher("{ \"Id\" : 1, \"Name\" : \"Test\" }");

        // Act 
        var jObject = new JObject
        {
            { "Id", new JValue(1) },
            { "Name", new JValue("Test") }
        };
        double match = matcher.IsMatch(jObject).Score;

        // Assert 
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void JsonPartialMatcher_IsMatch_JObjectAsStringWithDottedPropertyName()
    {
        // Assign 
        var matcher = new JsonPartialMatcher("{ \"urn:ietf:params:scim:schemas:extension:enterprise:2.0:User\" : \"Test\" }");

        // Act 
        var jObject = new JObject
        {
            { "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User", new JValue("Test") }
        };
        var match = matcher.IsMatch(jObject).Score;

        // Assert 
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void JsonPartialMatcher_IsMatch_GuidAsString()
    {
        // Assign
        var guid = Guid.NewGuid();
        var matcher = new JsonPartialMatcher(new { Id = 1, Name = guid });

        // Act
        var jObject = new JObject
        {
            { "Id", new JValue(1) },
            { "Name", new JValue(guid.ToString()) }
        };
        double match = matcher.IsMatch(jObject).Score;

        // Assert
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void JsonPartialMatcher_IsMatch_WithIgnoreCaseTrue_JObjectAsString()
    {
        // Assign 
        var matcher = new JsonPartialMatcher("{ \"Id\" : 1, \"Name\" : \"test\" }", true);

        // Act 
        var jObject = new JObject
        {
            { "Id", new JValue(1) },
            { "Name", new JValue("Test") }
        };
        double match = matcher.IsMatch(jObject).Score;

        // Assert 
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void JsonPartialMatcher_IsMatch_JObjectAsString_RejectOnMatch()
    {
        // Assign 
        var matcher = new JsonPartialMatcher(MatchBehaviour.RejectOnMatch, "{ \"Id\" : 1, \"Name\" : \"Test\" }");

        // Act 
        var jObject = new JObject
        {
            { "Id", new JValue(1) },
            { "Name", new JValue("Test") }
        };
        double match = matcher.IsMatch(jObject).Score;

        // Assert 
        Assert.Equal(0.0, match);
    }

    [Fact]
    public void JsonPartialMatcher_IsMatch_JObjectWithDateTimeOffsetAsString()
    {
        // Assign 
        var matcher = new JsonPartialMatcher("{ \"preferredAt\" : \"2019-11-21T10:32:53.2210009+00:00\" }");

        // Act 
        var jObject = new JObject
        {
            { "preferredAt", new JValue("2019-11-21T10:32:53.2210009+00:00") }
        };
        double match = matcher.IsMatch(jObject).Score;

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
    public void JsonPartialMatcher_IsMatch_StringInputValidMatch(string value, string input)
    {
        // Assign
        var matcher = new JsonPartialMatcher(value);

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
    public void JsonPartialMatcher_IsMatch_StringInputWithInvalidMatch(string value, string? input)
    {
        // Assign
        var matcher = new JsonPartialMatcher(value);

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
    public void JsonPartialMatcher_IsMatch_ValueAsJPathValidMatch(string value, string input)
    {
        // Assign
        var matcher = new JsonPartialMatcher(value);

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
    public void JsonPartialMatcher_IsMatch_ValueAsJPathInvalidMatch(string value, string input)
    {
        // Assign
        var matcher = new JsonPartialMatcher(value);

        // Act
        double match = matcher.IsMatch(input).Score;

        // Assert
        Assert.Equal(0.0, match);
    }
}
