// Copyright © WireMock.Net

using AwesomeAssertions;
using Newtonsoft.Json.Linq;
using NFluent;
using WireMock.Matchers;

namespace WireMock.Net.Tests.Matchers;

public class JmesPathMatcherTests
{
    [Fact]
    public void JmesPathMatcher_GetName()
    {
        // Assign
        var matcher = new JmesPathMatcher("X");

        // Act
        string name = matcher.Name;

        // Assert
        Check.That(name).Equals("JmesPathMatcher");
    }

    [Fact]
    public void JmesPathMatcher_GetPatterns()
    {
        // Assign
        var matcher = new JmesPathMatcher("X");

        // Act
        var patterns = matcher.GetPatterns();

        // Assert
        Check.That(patterns).ContainsExactly("X");
    }

    [Fact]
    public void JmesPathMatcher_IsMatch_ByteArray()
    {
        // Assign
        var bytes = new byte[0];
        var matcher = new JmesPathMatcher("");

        // Act 
        double match = matcher.IsMatch(bytes).Score;

        // Assert 
        Check.That(match).IsEqualTo(0);
    }

    [Fact]
    public void JmesPathMatcher_IsMatch_NullString()
    {
        // Assign
        var matcher = new JmesPathMatcher("");

        // Act 
        double match = matcher.IsMatch(null).Score;

        // Assert 
        Check.That(match).IsEqualTo(0);
    }

    [Fact]
    public void JmesPathMatcher_IsMatch_EmptyString()
    {
        // Assign
        var matcher = new JmesPathMatcher("");

        // Act 
        double match = matcher.IsMatch(string.Empty).Score;

        // Assert 
        Check.That(match).IsEqualTo(0);
    }

    [Fact]
    public void JmesPathMatcher_IsMatch_NullObject()
    {
        // Assign
        object? o = null;
        var matcher = new JmesPathMatcher("");

        // Act 
        double match = matcher.IsMatch(o).Score;

        // Assert 
        Check.That(match).IsEqualTo(0);
    }

    [Fact]
    public void JmesPathMatcher_IsMatch_String_Exception_Mismatch()
    {
        // Assign
        var matcher = new JmesPathMatcher("xxx");

        // Act 
        double match = matcher.IsMatch("").Score;

        // Assert 
        Check.That(match).IsEqualTo(0);
    }

    [Fact]
    public void JmesPathMatcher_IsMatch_Object_Exception_Mismatch()
    {
        // Assign
        var matcher = new JmesPathMatcher("");

        // Act 
        double match = matcher.IsMatch("x").Score;

        // Assert 
        Check.That(match).IsEqualTo(0);
    }

    [Fact]
    public void JmesPathMatcher_IsMatch_AnonymousObject()
    {
        // Assign 
        var matcher = new JmesPathMatcher("things.name == 'RequiredThing'");

        // Act
        double match = matcher.IsMatch(new { things = new { name = "RequiredThing" } }).Score;

        // Assert 
        Check.That(match).IsEqualTo(1);
    }

    [Fact]
    public void JmesPathMatcher_IsMatch_JObject()
    {
        // Assign 
        string[] patterns = { "things.x == 'RequiredThing'" };
        var matcher = new JmesPathMatcher(patterns);

        // Act
        var sub = new JObject
        {
            { "x", new JValue("RequiredThing") }
        };
        var jobject = new JObject
        {
            { "Id", new JValue(1) },
            { "things", sub }
        };
        double match = matcher.IsMatch(jobject).Score;

        // Assert 
        Check.That(match).IsEqualTo(1);
    }

    [Fact]
    public void JmesPathMatcher_IsMatch_JObject_Parsed()
    {
        // Assign 
        var matcher = new JmesPathMatcher("things.x == 'RequiredThing'");

        // Act 
        double match = matcher.IsMatch(JObject.Parse("{ \"things\": { \"x\": \"RequiredThing\" } }")).Score;

        // Assert 
        Check.That(match).IsEqualTo(1);
    }

    [Fact]
    public void JmesPathMatcher_IsMatch_RejectOnMatch()
    {
        // Assign
        var matcher = new JmesPathMatcher(MatchBehaviour.RejectOnMatch, MatchOperator.Or, "things.x == 'RequiredThing'");

        // Act
        double match = matcher.IsMatch(JObject.Parse("{ \"things\": { \"x\": \"RequiredThing\" } }")).Score;

        // Assert
        Check.That(match).IsEqualTo(0.0);
    }

    [Fact]
    public void JmesPathMatcher_IsMatch_MultiplePatternsUsingMatchOperatorAnd()
    {
        // Assign 
        var matcher = new JmesPathMatcher(MatchOperator.And, "things.x == 'RequiredThing'", "things.x == 'abc'");

        // Act 
        double score = matcher.IsMatch(JObject.Parse("{ \"things\": { \"x\": \"RequiredThing\" } }")).Score;

        // Assert 
        score.Should().Be(0);
    }

    [Fact]
    public void JmesPathMatcher_IsMatch_MultiplePatternsUsingMatchOperatorOr()
    {
        // Assign 
        var matcher = new JmesPathMatcher(MatchOperator.Or, "things.x == 'RequiredThing'", "things.x == 'abc'");

        // Act 
        double score = matcher.IsMatch(JObject.Parse("{ \"things\": { \"x\": \"RequiredThing\" } }")).Score;

        // Assert 
        score.Should().Be(1);
    }
}