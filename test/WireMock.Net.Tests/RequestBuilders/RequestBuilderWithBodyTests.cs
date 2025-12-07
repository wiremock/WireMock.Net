// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NFluent;
using WireMock.Matchers;
using WireMock.Matchers.Request;
using WireMock.Models;
using WireMock.RequestBuilders;
using WireMock.Types;
using WireMock.Util;
using Xunit;

namespace WireMock.Net.Tests.RequestBuilders;

public class RequestBuilderWithBodyTests
{
    private const string ClientIp = "::1";

    [Fact]
    public void Request_WithBody_IMatcher()
    {
        // Assign
        var matcher = new WildcardMatcher("x");

        // Act
        var requestBuilder = (Request)Request.Create().WithBody(matcher);

        // Assert
        var matchers = requestBuilder.GetPrivateFieldValue<IList<IRequestMatcher>>("_requestMatchers");
        matchers.Should().HaveCount(1);
        ((RequestMessageBodyMatcher)matchers[0]).Matchers.Should().Contain(matcher);
    }

    [Fact]
    public void Request_WithBody_IMatchers()
    {
        // Assign
        var matcher1 = new WildcardMatcher("x");
        var matcher2 = new WildcardMatcher("y");

        // Act
        var requestBuilder = (Request)Request.Create().WithBody(new[] { matcher1, matcher2 }.Cast<IMatcher>().ToArray());

        // Assert
        var matchers = requestBuilder.GetPrivateFieldValue<IList<IRequestMatcher>>("_requestMatchers");
        matchers.Should().HaveCount(1);
        ((RequestMessageBodyMatcher)matchers[0]).Matchers.Should().Contain(new[] { matcher1, matcher2 });
    }

    [Fact]
    public void Request_WithBody_FuncString()
    {
        // Assign
        var requestBuilder = Request.Create().UsingAnyMethod().WithBody(b => b != null && b.Contains("b"));

        // Act
        var body = new BodyData
        {
            BodyAsString = "b",
            DetectedBodyType = BodyType.String
        };
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "POST", ClientIp, body);

        // Assert
        var requestMatchResult = new RequestMatchResult();
        Check.That(requestBuilder.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
    }

    [Fact]
    public void Request_WithBody_FuncObject()
    {
        // Assign
        var requestBuilder = Request.Create()
            .UsingAnyMethod()
            .WithBody(b => b != null);

        // Act
        var body = new BodyData
        {
            BodyAsJson = JObject.Parse("""{ "X": 123, "Y": "a" }"""),
            DetectedBodyType = BodyType.Json
        };
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "POST", ClientIp, body);

        // Assert
        var requestMatchResult = new RequestMatchResult();
        Check.That(requestBuilder.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
    }

    [Fact]
    public void Request_WithBody_FuncType()
    {
        // Assign
        var requestBuilder = Request.Create()
            .UsingAnyMethod()
            .WithBodyType<FuncType>(ft => ft != null && ft.X == 123 && ft.Y == "a");

        // Act
        var body = new BodyData
        {
            BodyAsJson = JObject.Parse("""{ "X": 123, "Y": "a" }"""),
            DetectedBodyType = BodyType.Json
        };
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "POST", ClientIp, body);

        // Assert
        var requestMatchResult = new RequestMatchResult();
        Check.That(requestBuilder.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
    }

    private class FuncType
    {
        public int X { get; set; } = 42;

        public string Y { get; set; } = string.Empty;
    }

    [Fact]
    public void Request_WithBody_FuncFormUrlEncoded()
    {
        // Assign
        var requestBuilder = Request.Create().UsingAnyMethod().WithBody((IDictionary<string, string>? values) => values != null);

        // Act
        var body = new BodyData
        {
            BodyAsFormUrlEncoded = new Dictionary<string, string>(),
            DetectedBodyTypeFromContentType = BodyType.FormUrlEncoded,
            DetectedBodyType = BodyType.FormUrlEncoded
        };
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "POST", ClientIp, body);

        // Assert
        var requestMatchResult = new RequestMatchResult();
        Check.That(requestBuilder.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
    }

    [Fact]
    public void Request_WithBody_FuncBodyData()
    {
        // Assign
        var requestBuilder = Request.Create().UsingAnyMethod().WithBody((IBodyData? b) => b != null);

        // Act
        var body = new BodyData
        {
            BodyAsJson = 123,
            DetectedBodyType = BodyType.Json
        };
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "POST", ClientIp, body);

        // Assert
        var requestMatchResult = new RequestMatchResult();
        Check.That(requestBuilder.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
    }

    [Fact]
    public void Request_WithBody_FuncByteArray()
    {
        // Assign
        var requestBuilder = Request.Create().UsingAnyMethod().WithBody((byte[]? b) => b != null);

        // Act
        var body = new BodyData
        {
            BodyAsBytes = new byte[0],
            DetectedBodyType = BodyType.Bytes
        };
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "POST", ClientIp, body);

        // Assert
        var requestMatchResult = new RequestMatchResult();
        Check.That(requestBuilder.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
    }

    [Fact]
    public void Request_WithBodyExactMatcher()
    {
        // Arrange
        var requestBuilder = Request.Create().UsingAnyMethod().WithBody(new ExactMatcher("cat"));

        // Act
        var body = new BodyData
        {
            BodyAsString = "cat",
            DetectedBodyType = BodyType.String
        };
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "POST", ClientIp, body);

        // Assert
        var requestMatchResult = new RequestMatchResult();
        Check.That(requestBuilder.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
    }

    [Fact]
    public void Request_WithBody_BodyDataAsString_Using_WildcardMatcher()
    {
        // Arrange
        var spec = Request.Create().WithPath("/foo").UsingAnyMethod().WithBody(new WildcardMatcher("H*o*"));

        // Act
        var body = new BodyData
        {
            BodyAsString = "Hello world!",
            DetectedBodyType = BodyType.String
        };
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "PUT", ClientIp, body);

        // Assert
        var requestMatchResult = new RequestMatchResult();
        spec.GetMatchingScore(request, requestMatchResult).Should().Be(1.0);
    }

    [Fact]
    public void Request_WithBody_BodyDataAsJson_Using_WildcardMatcher()
    {
        // Arrange
        var spec = Request.Create().WithPath("/foo").UsingAnyMethod().WithBody(new WildcardMatcher("*Hello*"));

        // Act
        var body = new BodyData
        {
            BodyAsJson = new { Hi = "Hello world!" },
            BodyAsString = "{ Hi = \"Hello world!\" }",
            DetectedBodyType = BodyType.Json
        };
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "PUT", ClientIp, body);

        // Assert
        var requestMatchResult = new RequestMatchResult();
        spec.GetMatchingScore(request, requestMatchResult).Should().Be(1.0);
    }

    [Fact]
    public void Request_WithBody_XPathMatcher_true()
    {
        // Arrange
        var spec = Request.Create().UsingAnyMethod().WithBody(new XPathMatcher("/todo-list[count(todo-item) = 3]"));

        // Act
        var body = new BodyData
        {
            BodyAsString = @"
                    <todo-list>
                        <todo-item id='a1'>abc</todo-item>
                        <todo-item id='a2'>def</todo-item>
                        <todo-item id='a3'>xyz</todo-item>
                    </todo-list>",
            DetectedBodyType = BodyType.String
        };
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "PUT", ClientIp, body);

        // Assert
        var requestMatchResult = new RequestMatchResult();
        Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
    }

    [Fact]
    public void Request_WithBody_XPathMatcher_false()
    {
        // Arrange
        var spec = Request.Create().UsingAnyMethod().WithBody(new XPathMatcher("/todo-list[count(todo-item) = 99]"));

        // Act
        var body = new BodyData
        {
            BodyAsString = @"
                    <todo-list>
                        <todo-item id='a1'>abc</todo-item>
                        <todo-item id='a2'>def</todo-item>
                        <todo-item id='a3'>xyz</todo-item>
                    </todo-list>",
            DetectedBodyType = BodyType.String
        };
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "PUT", ClientIp, body);

        // Assert
        var requestMatchResult = new RequestMatchResult();
        Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsNotEqualTo(1.0);
    }

    [Fact]
    public void Request_WithBody_JsonPathMatcher_true()
    {
        // Arrange
        var spec = Request.Create().UsingAnyMethod().WithBody(new JsonPathMatcher("$..things[?(@.name == 'RequiredThing')]"));

        // Act
        var body = new BodyData
        {
            BodyAsString = "{ \"things\": [ { \"name\": \"RequiredThing\" }, { \"name\": \"Wiremock\" } ] }",
            DetectedBodyType = BodyType.String
        };
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "PUT", ClientIp, body);

        // Assert
        var requestMatchResult = new RequestMatchResult();
        Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
    }

    [Fact]
    public void Request_WithBodyJson_PathMatcher_false()
    {
        // Arrange
        var spec = Request.Create().UsingAnyMethod().WithBody(new JsonPathMatcher("$.things[?(@.name == 'RequiredThing')]"));

        // Act
        var body = new BodyData
        {
            BodyAsString = "{ \"things\": { \"name\": \"Wiremock\" } }",
            DetectedBodyType = BodyType.String
        };
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "PUT", ClientIp, body);

        // Assert
        var requestMatchResult = new RequestMatchResult();
        Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsNotEqualTo(1.0);
    }

    [Fact]
    public void Request_WithBody_Object_JsonPathMatcher_true()
    {
        // Arrange
        var spec = Request.Create().UsingAnyMethod().WithBody(new JsonPathMatcher("$..things[?(@.name == 'RequiredThing')]"));

        // Act
        string jsonString = "{ \"things\": [ { \"name\": \"RequiredThing\" }, { \"name\": \"Wiremock\" } ] }";
        var bodyData = new BodyData
        {
            BodyAsJson = JsonConvert.DeserializeObject(jsonString),
            BodyAsString = jsonString,
            Encoding = Encoding.UTF8,
            DetectedBodyType = BodyType.Json
        };

        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "PUT", ClientIp, bodyData);

        // Assert
        var requestMatchResult = new RequestMatchResult();
        Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
    }

    [Fact]
    public void Request_WithBody_Array_JsonPathMatcher_1()
    {
        // Arrange
        var spec = Request.Create().UsingAnyMethod().WithBody(new JsonPathMatcher("$..books[?(@.price < 10)]"));

        // Act
        string jsonString = "{ \"books\": [ { \"category\": \"test1\", \"price\": 8.95 }, { \"category\": \"test2\", \"price\": 20 } ] }";
        var bodyData = new BodyData
        {
            BodyAsJson = JsonConvert.DeserializeObject(jsonString),
            BodyAsString = jsonString,
            Encoding = Encoding.UTF8,
            DetectedBodyType = BodyType.Json
        };

        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "PUT", ClientIp, bodyData);

        // Assert
        var requestMatchResult = new RequestMatchResult();
        Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
    }

    [Fact]
    public void Request_WithBody_Array_JsonPathMatcher_2()
    {
        // Arrange
        var spec = Request.Create().UsingAnyMethod().WithBody(new JsonPathMatcher("$..[?(@.Id == 1)]"));

        // Act
        string jsonString = "{ \"Id\": 1, \"Name\": \"Test\" }";
        var bodyData = new BodyData
        {
            BodyAsJson = JsonConvert.DeserializeObject(jsonString),
            BodyAsString = jsonString,
            Encoding = Encoding.UTF8,
            DetectedBodyType = BodyType.Json
        };

        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "PUT", ClientIp, bodyData);

        // Assert
        var requestMatchResult = new RequestMatchResult();
        double result = spec.GetMatchingScore(request, requestMatchResult);
        Check.That(result).IsEqualTo(1.0);
    }

    [Fact]
    public void Request_WithBody_ExactObjectMatcher_true()
    {
        // Assign
        object body = DateTime.MinValue;
        var requestBuilder = Request.Create().UsingAnyMethod().WithBody(body);

        var bodyData = new BodyData
        {
            BodyAsJson = DateTime.MinValue,
            DetectedBodyType = BodyType.Json
        };

        // Act
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "PUT", ClientIp, bodyData);

        // Assert
        var requestMatchResult = new RequestMatchResult();
        Check.That(requestBuilder.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
    }

    [Fact]
    public void Request_WithBodyAsJson_UsingObject_UsesJsonMatcher()
    {
        // Assign
        object body = new
        {
            Test = "abc"
        };
        var requestBuilder = Request.Create().UsingAnyMethod().WithBodyAsJson(body);

        var bodyData = new BodyData
        {
            BodyAsJson = body,
            DetectedBodyType = BodyType.Json
        };

        // Act
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "POST", ClientIp, bodyData);

        // Assert
        var requestMatchResult = new RequestMatchResult();
        Check.That(requestBuilder.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
    }

    [Theory]
    [InlineData(new byte[] { 1 }, BodyType.Bytes)]
    [InlineData(new byte[] { 48, 49, 50 }, BodyType.Bytes)]
    [InlineData(new byte[] { 48, 49, 50 }, BodyType.String)]
    public void Request_WithBodyAsBytes_ExactObjectMatcher_true(byte[] bytes, BodyType detectedBodyType)
    {
        // Assign
        byte[] body = bytes;
        var requestBuilder = Request.Create().UsingAnyMethod().WithBody(body);

        var bodyData = new BodyData
        {
            BodyAsBytes = bytes,
            DetectedBodyType = detectedBodyType
        };

        // Act
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "PUT", ClientIp, bodyData);

        // Assert
        var requestMatchResult = new RequestMatchResult();
        requestBuilder.GetMatchingScore(request, requestMatchResult).Should().Be(1.0);
    }
}