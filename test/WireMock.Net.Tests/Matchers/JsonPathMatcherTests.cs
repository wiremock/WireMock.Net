// Copyright © WireMock.Net

using Newtonsoft.Json.Linq;
using WireMock.Matchers;

namespace WireMock.Net.Tests.Matchers;

public class JsonPathMatcherTests
{
    [Fact]
    public void JsonPathMatcher_GetName()
    {
        // Arrange
        var matcher = new JsonPathMatcher("X");

        // Act
        string name = matcher.Name;

        // Assert
        name.Should().Be("JsonPathMatcher");
    }

    [Fact]
    public void JsonPathMatcher_GetPatterns()
    {
        // Arrange
        var matcher = new JsonPathMatcher("X");

        // Act
        var patterns = matcher.GetPatterns();

        // Assert
        patterns.Should().ContainSingle("X");
    }

    [Fact]
    public void JsonPathMatcher_IsMatch_ByteArray()
    {
        // Arrange
        var bytes = new byte[0];
        var matcher = new JsonPathMatcher("");

        // Act 
        double match = matcher.IsMatch(bytes).Score;

        // Assert 
        match.Should().Be(0);
    }

    [Fact]
    public void JsonPathMatcher_IsMatch_NullString()
    {
        // Arrange
        var matcher = new JsonPathMatcher("");

        // Act 
        double match = matcher.IsMatch(null).Score;

        // Assert 
        match.Should().Be(0);
    }

    [Fact]
    public void JsonPathMatcher_IsMatch_EmptyString()
    {
        // Arrange
        var matcher = new JsonPathMatcher("");

        // Act 
        double match = matcher.IsMatch(string.Empty).Score;

        // Assert 
        match.Should().Be(0);
    }

    [Fact]
    public void JsonPathMatcher_IsMatch_NullObject()
    {
        // Arrange
        object? o = null;
        var matcher = new JsonPathMatcher("");

        // Act 
        double match = matcher.IsMatch(o).Score;

        // Assert 
        match.Should().Be(0);
    }

    [Fact]
    public void JsonPathMatcher_IsMatch_String_Exception_Mismatch()
    {
        // Arrange
        var matcher = new JsonPathMatcher("xxx");

        // Act 
        double match = matcher.IsMatch("").Score;

        // Assert 
        match.Should().Be(0);
    }

    [Fact]
    public void JsonPathMatcher_IsMatch_Object_Exception_Mismatch()
    {
        // Arrange
        var matcher = new JsonPathMatcher("");

        // Act 
        double match = matcher.IsMatch("x").Score;

        // Assert 
        match.Should().Be(0);
    }

    [Fact]
    public void JsonPathMatcher_IsMatch_AnonymousObject()
    {
        // Arrange 
        var matcher = new JsonPathMatcher("$..[?(@.Id == 1)]");

        // Act 
        double match = matcher.IsMatch(new { Id = 1, Name = "Test" }).Score;

        // Assert 
        match.Should().Be(1);
    }

    [Fact]
    public void JsonPathMatcher_IsMatch_AnonymousObject_WithNestedObject()
    {
        // Arrange
        var matcher = new JsonPathMatcher("$.things[?(@.name == 'x')]");

        // Act 
        double match = matcher.IsMatch(new { things = new { name = "x" } }).Score;

        // Assert 
        match.Should().Be(1);
    }

    [Fact]
    public void JsonPathMatcher_IsMatch_String_WithNestedObject()
    {
        // Arrange
        var json = "{ \"things\": { \"name\": \"x\" } }";
        var matcher = new JsonPathMatcher("$.things[?(@.name == 'x')]");

        // Act 
        double match = matcher.IsMatch(json).Score;

        // Assert 
        match.Should().Be(1);
    }

    [Fact]
    public void JsonPathMatcher_IsNoMatch_String_WithNestedObject()
    {
        // Arrange
        var json = "{ \"things\": { \"name\": \"y\" } }";
        var matcher = new JsonPathMatcher("$.things[?(@.name == 'x')]");

        // Act 
        double match = matcher.IsMatch(json).Score;

        // Assert 
        match.Should().Be(0);
    }

    [Fact]
    public void JsonPathMatcher_IsMatch_JObject()
    {
        // Arrange 
        string[] patterns = { "$..[?(@.Id == 1)]" };
        var matcher = new JsonPathMatcher(patterns);

        // Act 
        var jobject = new JObject
        {
            { "Id", new JValue(1) },
            { "Name", new JValue("Test") }
        };
        double match = matcher.IsMatch(jobject).Score;

        // Assert 
        match.Should().Be(1);
    }

    [Fact]
    public void JsonPathMatcher_IsMatch_JObject_Parsed()
    {
        // Arrange 
        var matcher = new JsonPathMatcher("$..[?(@.Id == 1)]");

        // Act 
        double match = matcher.IsMatch(JObject.Parse("{\"Id\":1,\"Name\":\"Test\"}")).Score;

        // Assert 
        match.Should().Be(1);
    }

    [Fact]
    public void JsonPathMatcher_IsMatch_RejectOnMatch()
    {
        // Arrange
        var matcher = new JsonPathMatcher(MatchBehaviour.RejectOnMatch, MatchOperator.Or, "$..[?(@.Id == 1)]");

        // Act
        double match = matcher.IsMatch(JObject.Parse("{\"Id\":1,\"Name\":\"Test\"}")).Score;

        // Assert
        match.Should().Be(0.0);
    }

    [Fact]
    public void JsonPathMatcher_IsMatch_ArrayOneLevel()
    {
        // Arrange 
        var matcher = new JsonPathMatcher("$.arr[0].line1");

        // Act 
        double match = matcher.IsMatch(JObject.Parse(@"{
            ""name"": ""PathSelectorTest"",
            ""test"": ""test"",
            ""test2"": ""test2"",
            ""arr"": [{
                ""line1"": ""line1"",
            }]
        }")).Score;

        // Assert
        match.Should().Be(1.0);
    }

    [Fact]
    public void JsonPathMatcher_IsMatch_ObjectMatch()
    {
        // Arrange 
        var matcher = new JsonPathMatcher("$.test");

        // Act 
        double match = matcher.IsMatch(JObject.Parse(@"{
            ""name"": ""PathSelectorTest"",
            ""test"": ""test"",
            ""test2"": ""test2"",
            ""arr"": [
                {
                    ""line1"": ""line1"",
                }
            ]
        }")).Score;

        // Assert 
        match.Should().Be(1.0);
    }

    [Fact]
    public void JsonPathMatcher_IsMatch_DoesntMatch()
    {
        // Arrange 
        var matcher = new JsonPathMatcher("$.test3");

        // Act 
        double match = matcher.IsMatch(JObject.Parse(@"{
            ""name"": ""PathSelectorTest"",
            ""test"": ""test"",
            ""test2"": ""test2"",
            ""arr"": [
                {
                    ""line1"": ""line1"",
                }
            ]
        }")).Score;

        // Assert 
        match.Should().Be(0.0);
    }

    [Fact]
    public void JsonPathMatcher_IsMatch_DoesntMatchInArray()
    {
        // Arrange 
        var matcher = new JsonPathMatcher("$arr[0].line1");

        // Act 
        double match = matcher.IsMatch(JObject.Parse(@"{
            ""name"": ""PathSelectorTest"",
            ""test"": ""test"",
            ""test2"": ""test2"",
            ""arr"": []
        }")).Score;

        // Assert 
        match.Should().Be(0.0);
    }

    [Fact]
    public void JsonPathMatcher_IsMatch_DoesntMatchNoObjectsInArray()
    {
        // Arrange 
        var matcher = new JsonPathMatcher("$arr[2].line1");

        // Act 
        double match = matcher.IsMatch(JObject.Parse(@"{
            ""name"": ""PathSelectorTest"",
            ""test"": ""test"",
            ""test2"": ""test2"",
            ""arr"": []
        }")).Score;

        // Assert 
        match.Should().Be(0.0);
    }

    [Fact]
    public void JsonPathMatcher_IsMatch_NestedArrays()
    {
        // Arrange 
        var matcher = new JsonPathMatcher("$.arr[0].sub[0].subline1");

        // Act 
        double match = matcher.IsMatch(JObject.Parse(@"{
            ""name"": ""PathSelectorTest"",
            ""test"": ""test"",
            ""test2"": ""test2"",
            ""arr"": [{
                ""line1"": ""line1"",
                ""sub"":[
                {
                    ""subline1"":""subline1""
                }]
            }]
        }")).Score;

        // Assert 
        match.Should().Be(1.0);
    }

    [Fact]
    public void JsonPathMatcher_IsMatch_MultiplePatternsUsingMatchOperatorAnd()
    {
        // Assign 
        var matcher = new JsonPathMatcher(MatchBehaviour.AcceptOnMatch, MatchOperator.And, "$.arr[0].sub[0].subline1", "$.arr[0].line2");

        // Act 
        double match = matcher.IsMatch(JObject.Parse(@"{
            ""name"": ""PathSelectorTest"",
            ""test"": ""test"",
            ""test2"": ""test2"",
            ""arr"": [{
                ""line1"": ""line1"",
                ""sub"":[
                {
                    ""subline1"":""subline1""
                }]
            }]
        }")).Score;

        // Assert 
        match.Should().Be(0);
    }

    [Fact]
    public void JsonPathMatcher_IsMatch_MultiplePatternsUsingMatchOperatorOr()
    {
        // Assign 
        var matcher = new JsonPathMatcher(MatchBehaviour.AcceptOnMatch, MatchOperator.Or, "$.arr[0].sub[0].subline2", "$.arr[0].line1");

        // Act 
        double match = matcher.IsMatch(JObject.Parse(@"{
            ""name"": ""PathSelectorTest"",
            ""test"": ""test"",
            ""test2"": ""test2"",
            ""arr"": [{
                ""line1"": ""line1"",
                ""sub"":[
                {
                    ""subline1"":""subline1""
                }]
            }]
        }")).Score;

        // Assert 
        match.Should().Be(1);
    }
}
