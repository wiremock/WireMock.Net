// Copyright © WireMock.Net

using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using NFluent;
using WireMock.Matchers;

namespace WireMock.Net.Tests.Matchers;

public class JsonMatcherTests
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum EnumWithJsonConverter
    {
        Type1
    }

    public enum NormalEnum
    {
        Abc
    }

    public class Test1
    {
        public NormalEnum NormalEnum { get; set; }
    }

    public class Test2
    {
        public EnumWithJsonConverter EnumWithJsonConverter { get; set; }
    }

    [Fact]
    public void JsonMatcher_GetName()
    {
        // Assign
        var matcher = new JsonMatcher("{}");

        // Act
        var name = matcher.Name;

        // Assert
        Check.That(name).Equals("JsonMatcher");
    }

    [Fact]
    public void JsonMatcher_GetValue()
    {
        // Assign
        var matcher = new JsonMatcher("{}");

        // Act
        var value = matcher.Value;

        // Assert
        Check.That(value).Equals("{}");
    }

    [Fact]
    public void JsonMatcher_WithInvalidStringValue_Should_ThrowException()
    {
        // Act
        // ReSharper disable once ObjectCreationAsStatement
        Action action = () => new JsonMatcher(MatchBehaviour.AcceptOnMatch, "{ \"Id\"");

        // Assert
        action.Should().Throw<JsonException>();
    }

    [Fact]
    public void JsonMatcher_WithInvalidObjectValue_Should_ThrowException()
    {
        // Act
        // ReSharper disable once ObjectCreationAsStatement
        Action action = () => new JsonMatcher(MatchBehaviour.AcceptOnMatch, new MemoryStream());

        // Assert
        action.Should().Throw<JsonException>();
    }

    [Fact]
    public void JsonMatcher_IsMatch_WithInvalidValue_Should_ReturnMismatch_And_Exception_ShouldBeSet()
    {
        // Assign
        var matcher = new JsonMatcher("");

        // Act
        var result = matcher.IsMatch(new MemoryStream());

        // Assert
        result.Score.Should().Be(MatchScores.Mismatch);
        result.Exception.Should().BeAssignableTo<JsonException>();
    }

    [Fact]
    public void JsonMatcher_IsMatch_ByteArray()
    {
        // Assign
        var bytes = new byte[0];
        var matcher = new JsonMatcher("");

        // Act
        var match = matcher.IsMatch(bytes).Score;

        // Assert
        Check.That(match).IsEqualTo(0);
    }

    [Fact]
    public void JsonMatcher_IsMatch_NullString()
    {
        // Assign
        string? s = null;
        var matcher = new JsonMatcher("");

        // Act
        var match = matcher.IsMatch(s).Score;

        // Assert
        Check.That(match).IsEqualTo(0);
    }

    [Fact]
    public void JsonMatcher_IsMatch_NullObject()
    {
        // Assign
        object? o = null;
        var matcher = new JsonMatcher("");

        // Act
        var match = matcher.IsMatch(o).Score;

        // Assert
        Check.That(match).IsEqualTo(0);
    }

    [Fact]
    public void JsonMatcher_IsMatch_JArray()
    {
        // Assign
        var matcher = new JsonMatcher(new[] { "x", "y" });

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
    public void JsonMatcher_IsMatch_JObject_ShouldMatch()
    {
        // Assign
        var matcher = new JsonMatcher(new { Id = 1, Name = "Test" });

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
    public void JsonMatcher_IsMatch_JObject_ShouldNotMatch()
    {
        // Assign
        var matcher = new JsonMatcher(new { Id = 1, Name = "Test" });

        // Act
        var jObject = new JObject
        {
            { "Id", new JValue(1) },
            { "Name", new JValue("Test") },
            { "Other", new JValue("abc") }
        };
        var score = matcher.IsMatch(jObject).Score;

        // Assert
        Assert.Equal(MatchScores.Mismatch, score);
    }

    [Fact]
    public void JsonMatcher_IsMatch_WithIgnoreCaseTrue_JObject()
    {
        // Assign
        var matcher = new JsonMatcher(new { id = 1, Name = "test" }, true);

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
    public void JsonMatcher_IsMatch_JObjectParsed()
    {
        // Assign
        var matcher = new JsonMatcher(new { Id = 1, Name = "Test" });

        // Act
        var jObject = JObject.Parse("{ \"Id\" : 1, \"Name\" : \"Test\" }");
        var match = matcher.IsMatch(jObject).Score;

        // Assert
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void JsonMatcher_IsMatch_WithIgnoreCaseTrue_JObjectParsed()
    {
        // Assign
        var matcher = new JsonMatcher(new { Id = 1, Name = "TESt" }, true);

        // Act
        var jObject = JObject.Parse("{ \"Id\" : 1, \"Name\" : \"Test\" }");
        var match = matcher.IsMatch(jObject).Score;

        // Assert
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void JsonMatcher_IsMatch_JArrayAsString()
    {
        // Assign
        var matcher = new JsonMatcher("[ \"x\", \"y\" ]");

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
    public void JsonMatcher_IsMatch_JObjectAsString()
    {
        // Assign
        var matcher = new JsonMatcher("{ \"Id\" : 1, \"Name\" : \"Test\" }");

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
    public void JsonMatcher_IsMatch_WithIgnoreCaseTrue_JObjectAsString()
    {
        // Assign
        var matcher = new JsonMatcher("{ \"Id\" : 1, \"Name\" : \"test\" }", true);

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
    public void JsonMatcher_IsMatch_JObjectAsString_RejectOnMatch()
    {
        // Assign
        var matcher = new JsonMatcher(MatchBehaviour.RejectOnMatch, "{ \"Id\" : 1, \"Name\" : \"Test\" }");

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
    public void JsonMatcher_IsMatch_JObjectWithDateTimeOffsetAsString()
    {
        // Assign
        var matcher = new JsonMatcher("{ \"preferredAt\" : \"2019-11-21T10:32:53.2210009+00:00\" }");

        // Act
        var jObject = new JObject
        {
            { "preferredAt", new JValue("2019-11-21T10:32:53.2210009+00:00") }
        };
        var match = matcher.IsMatch(jObject).Score;

        // Assert
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void JsonMatcher_IsMatch_NormalEnum()
    {
        // Assign
        var matcher = new JsonMatcher(new Test1 { NormalEnum = NormalEnum.Abc });

        // Act
        var jObject = new JObject
        {
            { "NormalEnum", new JValue(0) }
        };
        var match = matcher.IsMatch(jObject).Score;

        // Assert
        match.Should().Be(1.0);
    }

    [Fact]
    public void JsonMatcher_IsMatch_EnumWithJsonConverter()
    {
        // Assign
        var matcher = new JsonMatcher(new Test2 { EnumWithJsonConverter = EnumWithJsonConverter.Type1 });

        // Act
        var jObject = new JObject
        {
            { "EnumWithJsonConverter", new JValue("Type1") }
        };
        var match = matcher.IsMatch(jObject).Score;

        // Assert
        match.Should().Be(1.0);
    }

    [Fact]
    public void JsonMatcher_IsMatch_WithRegexTrue_ShouldMatch()
    {
        // Assign
        var matcher = new JsonMatcher(new { Id = "^\\d+$", Name = "Test" }, regex: true);

        // Act
        var jObject = new JObject
        {
            { "Id", new JValue(42) },
            { "Name", new JValue("Test") }
        };
        var score = matcher.IsMatch(jObject).Score;

        // Assert
        Assert.Equal(1.0, score);
    }

    [Fact]
    public void JsonMatcher_IsMatch_WithRegexTrue_Complex_ShouldMatch()
    {
        // Assign
        var matcher = new JsonMatcher(new
        {
            Complex = new
            {
                Id = "^\\d+$",
                Name = ".*"
            }
        }, regex: true);

        // Act
        var jObject = new JObject
        {
            {
                "Complex", new JObject
                {
                    { "Id", new JValue(42) },
                    { "Name", new JValue("Test") }
                }
            }
        };
        var score = matcher.IsMatch(jObject).Score;

        // Assert
        Assert.Equal(1.0, score);
    }

    [Fact]
    public void JsonMatcher_IsMatch_WithRegexTrue_Complex_ShouldNotMatch()
    {
        // Assign
        var matcher = new JsonMatcher(new
        {
            Complex = new
            {
                Id = "^\\d+$",
                Name = ".*"
            }
        }, regex: true);

        // Act
        var jObject = new JObject
        {
            {
                "Complex", new JObject
                {
                    { "Id", new JValue(42) },
                    { "Name", new JValue("Test") },
                    { "Other", new JValue("Other") }
                }
            }
        };
        var score = matcher.IsMatch(jObject).Score;

        // Assert
        Assert.Equal(MatchScores.Mismatch, score);
    }

    [Fact]
    public void JsonMatcher_IsMatch_WithRegexTrue_Array_ShouldMatch()
    {
        // Assign
        var matcher = new JsonMatcher(new
        {
            Array = new[]
            {
                "^\\d+$",
                ".*"
            }
        }, regex: true);

        // Act
        var jObject = new JObject
        {
            { "Array", new JArray("42", "test") }
        };
        var score = matcher.IsMatch(jObject).Score;

        // Assert
        Assert.Equal(1.0, score);
    }

    [Fact]
    public void JsonMatcher_IsMatch_WithRegexTrue_Array_ShouldNotMatch()
    {
        // Assign
        var matcher = new JsonMatcher(new
        {
            Array = new[]
            {
                "^\\d+$",
                ".*"
            }
        }, regex: true);

        // Act
        var jObject = new JObject
        {
            { "Array", new JArray("42", "test", "other") }
        };
        var score = matcher.IsMatch(jObject).Score;

        // Assert
        Assert.Equal(MatchScores.Mismatch, score);
    }

    [Fact]
    public void JsonMatcher_IsMatch_GuidAndString()
    {
        // Assign
        var id = Guid.NewGuid();
        var idAsString = id.ToString();
        var matcher = new JsonMatcher(new { Id = id });

        // Act
        var jObject = new JObject
        {
            { "Id", new JValue(idAsString) }
        };
        var score = matcher.IsMatch(jObject).Score;

        // Assert
        Assert.Equal(1.0, score);
    }

    [Fact]
    public void JsonMatcher_IsMatch_StringAndGuid()
    {
        // Assign
        var id = Guid.NewGuid();
        var idAsString = id.ToString();
        var matcher = new JsonMatcher(new { Id = idAsString });

        // Act
        var jObject = new JObject
        {
            { "Id", new JValue(id) }
        };
        var score = matcher.IsMatch(jObject).Score;

        // Assert
        Assert.Equal(1.0, score);
    }
}