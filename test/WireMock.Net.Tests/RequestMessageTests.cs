// Copyright © WireMock.Net

using NFluent;
using WireMock.Models;
using WireMock.Util;

namespace WireMock.Net.Tests;

public class RequestMessageTests
{
    private const string ClientIp = "::1";

    [Fact]
    public void RequestMessage_Method_Should_BeSame()
    {
        // given
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "posT", ClientIp);

        // then
        Check.That(request.Method).IsEqualTo("posT");
    }

    [Fact]
    public void RequestMessage_ParseQuery_NoKeys()
    {
        // given
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "POST", ClientIp);

        // then
        Check.That(request.GetParameter("not_there")).IsNull();
    }

    [Fact]
    public void RequestMessage_ParseQuery_SingleKey_SingleValue()
    {
        // Assign
        var request = new RequestMessage(new UrlDetails("http://localhost?foo=bar"), "POST", ClientIp);

        // Assert
        Check.That(request.GetParameter("foo")).ContainsExactly("bar");
    }

    [Fact]
    public void RequestMessage_ParseQuery_SingleKey_SingleValue_WithIgnoreCase()
    {
        // Assign
        var request = new RequestMessage(new UrlDetails("http://localhost?foo=bar"), "POST", ClientIp);

        // Assert
        Check.That(request.GetParameter("FoO", true)).ContainsExactly("bar");
    }

    [Fact]
    public void RequestMessage_ParseQuery_MultipleKeys_MultipleValues()
    {
        // Assign
        var request = new RequestMessage(new UrlDetails("http://localhost?key=1&key=2"), "POST", ClientIp);

        // Assert
        Check.That(request.GetParameter("key")).Contains("1");
        Check.That(request.GetParameter("key")).Contains("2");
    }

    [Fact]
    public void RequestMessage_ParseQuery_SingleKey_MultipleValuesCommaSeparated()
    {
        // Assign
        var request = new RequestMessage(new UrlDetails("http://localhost?key=1,2,3"), "POST", ClientIp);

        // Assert
        Check.That(request.GetParameter("key")).Contains("1");
        Check.That(request.GetParameter("key")).Contains("2");
        Check.That(request.GetParameter("key")).Contains("3");
    }

    [Fact]
    public void RequestMessage_ParseQuery_SingleKey_MultipleValues()
    {
        // Assign
        var request = new RequestMessage(new UrlDetails("http://localhost?key=1,2&foo=bar&key=3"), "POST", ClientIp);

        // Assert
        Check.That(request.GetParameter("key")).Contains("1");
        Check.That(request.GetParameter("key")).Contains("2");
        Check.That(request.GetParameter("key")).Contains("3");
    }

    [Fact]
    public void RequestMessage_Constructor1_PathSegments()
    {
        // Assign
        var request = new RequestMessage(new UrlDetails("http://localhost/a/b/c"), "POST", ClientIp);

        // Assert
        Check.That(request.PathSegments).ContainsExactly("a", "b", "c");
    }

    [Fact]
    public void RequestMessage_Constructor2_PathSegments()
    {
        // Assign
        var request = new RequestMessage(new UrlDetails("http://localhost/a/b/c"), "POST", ClientIp, new BodyData());

        // Assert
        Check.That(request.PathSegments).ContainsExactly("a", "b", "c");
    }
}