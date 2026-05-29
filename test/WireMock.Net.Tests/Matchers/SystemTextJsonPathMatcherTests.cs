// Copyright © WireMock.Net

using System.Text.Json.Nodes;
using WireMock.Matchers;

namespace WireMock.Net.Tests.Matchers;

public class SystemTextJsonPathMatcherTests
{
    [Fact]
    public void SystemTextJsonPathMatcher_GetName()
    {
        // Arrange
        var matcher = new SystemTextJsonPathMatcher("X");

        // Act
        string name = matcher.Name;

        // Assert
        name.Should().Be("SystemTextJsonPathMatcher");
    }

    [Fact]
    public void SystemTextJsonPathMatcher_GetPatterns()
    {
        // Arrange
        var matcher = new SystemTextJsonPathMatcher("X");

        // Act
        var patterns = matcher.GetPatterns();

        // Assert
        patterns.Should().ContainSingle("X");
    }

    [Fact]
    public void SystemTextJsonPathMatcher_IsMatch_ByteArray()
    {
        // Arrange
        var bytes = new byte[0];
        var matcher = new SystemTextJsonPathMatcher("$.Id");

        // Act
        double match = matcher.IsMatch(bytes).Score;

        // Assert
        match.Should().Be(0);
    }

    [Fact]
    public void SystemTextJsonPathMatcher_IsMatch_NullString()
    {
        // Arrange
        var matcher = new SystemTextJsonPathMatcher("$.Id");

        // Act
        double match = matcher.IsMatch(null).Score;

        // Assert
        match.Should().Be(0);
    }

    [Fact]
    public void SystemTextJsonPathMatcher_IsMatch_EmptyString()
    {
        // Arrange
        var matcher = new SystemTextJsonPathMatcher("$.Id");

        // Act
        double match = matcher.IsMatch(string.Empty).Score;

        // Assert
        match.Should().Be(0);
    }

    [Fact]
    public void SystemTextJsonPathMatcher_IsMatch_NullObject()
    {
        // Arrange
        object? o = null;
        var matcher = new SystemTextJsonPathMatcher("$.Id");

        // Act
        double match = matcher.IsMatch(o).Score;

        // Assert
        match.Should().Be(0);
    }

    [Fact]
    public void SystemTextJsonPathMatcher_IsMatch_String_Exception_Mismatch()
    {
        // Arrange
        var matcher = new SystemTextJsonPathMatcher("$.Id");

        // Act
        double match = matcher.IsMatch("not-json").Score;

        // Assert
        match.Should().Be(0);
    }

    [Fact]
    public void SystemTextJsonPathMatcher_IsMatch_AnonymousObject()
    {
        // Arrange - RFC 9535: filter expression requires an array context
        var matcher = new SystemTextJsonPathMatcher("$[?(@.Id == 1)]");

        // Act
        double match = matcher.IsMatch(new[] { new { Id = 1, Name = "Test" } }).Score;

        // Assert
        match.Should().Be(1);
    }

    [Fact]
    public void SystemTextJsonPathMatcher_IsMatch_AnonymousObject_WithNestedObject()
    {
        // Arrange
        var matcher = new SystemTextJsonPathMatcher("$.things[?(@.name == 'x')]");

        // Act
        double match = matcher.IsMatch(new { things = new { name = "x" } }).Score;

        // Assert
        match.Should().Be(1);
    }

    [Fact]
    public void SystemTextJsonPathMatcher_IsMatch_String_WithNestedObject()
    {
        // Arrange
        var json = "{ \"things\": { \"name\": \"x\" } }";
        var matcher = new SystemTextJsonPathMatcher("$.things[?(@.name == 'x')]");

        // Act
        double match = matcher.IsMatch(json).Score;

        // Assert
        match.Should().Be(1);
    }

    [Fact]
    public void SystemTextJsonPathMatcher_IsNoMatch_String_WithNestedObject()
    {
        // Arrange
        var json = "{ \"things\": { \"name\": \"y\" } }";
        var matcher = new SystemTextJsonPathMatcher("$.things[?(@.name == 'x')]");

        // Act
        double match = matcher.IsMatch(json).Score;

        // Assert
        match.Should().Be(0);
    }

    [Fact]
    public void SystemTextJsonPathMatcher_IsMatch_JsonNode()
    {
        // Arrange - RFC 9535: filter expression requires an array context
        string[] patterns = { "$[?(@.Id == 1)]" };
        var matcher = new SystemTextJsonPathMatcher(patterns);

        // Act
        var node = JsonNode.Parse("[{\"Id\":1,\"Name\":\"Test\"}]");
        double match = matcher.IsMatch(node).Score;

        // Assert
        match.Should().Be(1);
    }

    [Fact]
    public void SystemTextJsonPathMatcher_IsMatch_JsonNode_Parsed()
    {
        // Arrange - RFC 9535: filter expression requires an array context
        var matcher = new SystemTextJsonPathMatcher("$[?(@.Id == 1)]");

        // Act
        double match = matcher.IsMatch(JsonNode.Parse("[{\"Id\":1,\"Name\":\"Test\"}]")).Score;

        // Assert
        match.Should().Be(1);
    }

    [Fact]
    public void SystemTextJsonPathMatcher_IsMatch_RejectOnMatch()
    {
        // Arrange - RFC 9535: filter expression requires an array context
        var matcher = new SystemTextJsonPathMatcher(MatchBehaviour.RejectOnMatch, MatchOperator.Or, "$[?(@.Id == 1)]");

        // Act
        double match = matcher.IsMatch(JsonNode.Parse("[{\"Id\":1,\"Name\":\"Test\"}]")).Score;

        // Assert
        match.Should().Be(0.0);
    }

    [Fact]
    public void SystemTextJsonPathMatcher_IsMatch_ArrayOneLevel()
    {
        // Arrange
        var matcher = new SystemTextJsonPathMatcher("$.arr[0].line1");

        // Act
        double match = matcher.IsMatch(JsonNode.Parse(@"{
            ""name"": ""PathSelectorTest"",
            ""test"": ""test"",
            ""test2"": ""test2"",
            ""arr"": [{
                ""line1"": ""line1""
            }]
        }")).Score;

        // Assert
        match.Should().Be(1.0);
    }

    [Fact]
    public void SystemTextJsonPathMatcher_IsMatch_ObjectMatch()
    {
        // Arrange
        var matcher = new SystemTextJsonPathMatcher("$.test");

        // Act
        double match = matcher.IsMatch(JsonNode.Parse(@"{
            ""name"": ""PathSelectorTest"",
            ""test"": ""test"",
            ""test2"": ""test2"",
            ""arr"": [
                {
                    ""line1"": ""line1""
                }
            ]
        }")).Score;

        // Assert
        match.Should().Be(1.0);
    }

    [Fact]
    public void SystemTextJsonPathMatcher_IsMatch_DoesntMatch()
    {
        // Arrange
        var matcher = new SystemTextJsonPathMatcher("$.test3");

        // Act
        double match = matcher.IsMatch(JsonNode.Parse(@"{
            ""name"": ""PathSelectorTest"",
            ""test"": ""test"",
            ""test2"": ""test2"",
            ""arr"": [
                {
                    ""line1"": ""line1""
                }
            ]
        }")).Score;

        // Assert
        match.Should().Be(0.0);
    }

    [Fact]
    public void SystemTextJsonPathMatcher_IsMatch_DoesntMatchInArray()
    {
        // Arrange
        var matcher = new SystemTextJsonPathMatcher("$arr[0].line1");

        // Act
        double match = matcher.IsMatch(JsonNode.Parse(@"{
            ""name"": ""PathSelectorTest"",
            ""test"": ""test"",
            ""test2"": ""test2"",
            ""arr"": []
        }")).Score;

        // Assert
        match.Should().Be(0.0);
    }

    [Fact]
    public void SystemTextJsonPathMatcher_IsMatch_DoesntMatchNoObjectsInArray()
    {
        // Arrange
        var matcher = new SystemTextJsonPathMatcher("$arr[2].line1");

        // Act
        double match = matcher.IsMatch(JsonNode.Parse(@"{
            ""name"": ""PathSelectorTest"",
            ""test"": ""test"",
            ""test2"": ""test2"",
            ""arr"": []
        }")).Score;

        // Assert
        match.Should().Be(0.0);
    }

    [Fact]
    public void SystemTextJsonPathMatcher_IsMatch_NestedArrays()
    {
        // Arrange
        var matcher = new SystemTextJsonPathMatcher("$.arr[0].sub[0].subline1");

        // Act
        double match = matcher.IsMatch(JsonNode.Parse(@"{
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
    public void SystemTextJsonPathMatcher_IsMatch_MultiplePatternsUsingMatchOperatorAnd()
    {
        // Arrange
        var matcher = new SystemTextJsonPathMatcher(MatchBehaviour.AcceptOnMatch, MatchOperator.And, "$.arr[0].sub[0].subline1", "$.arr[0].line2");

        // Act
        double match = matcher.IsMatch(JsonNode.Parse(@"{
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
    public void SystemTextJsonPathMatcher_IsMatch_MultiplePatternsUsingMatchOperatorOr()
    {
        // Arrange
        var matcher = new SystemTextJsonPathMatcher(MatchBehaviour.AcceptOnMatch, MatchOperator.Or, "$.arr[0].sub[0].subline2", "$.arr[0].line1");

        // Act
        double match = matcher.IsMatch(JsonNode.Parse(@"{
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

    [Fact]
    public void SystemTextJsonPathMatcher_IsMatch_String_ArrayOneLevel()
    {
        // Arrange
        var matcher = new SystemTextJsonPathMatcher("$.arr[0].line1");

        // Act
        double match = matcher.IsMatch(@"{
            ""name"": ""PathSelectorTest"",
            ""arr"": [{
                ""line1"": ""line1""
            }]
        }").Score;

        // Assert
        match.Should().Be(1.0);
    }

    [Fact]
    public void SystemTextJsonPathMatcher_IsMatch_String_DoesntMatch()
    {
        // Arrange
        var matcher = new SystemTextJsonPathMatcher("$.test3");

        // Act
        double match = matcher.IsMatch(@"{ ""test"": ""test"" }").Score;

        // Assert
        match.Should().Be(0.0);
    }
}
