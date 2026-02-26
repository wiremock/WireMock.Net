// Copyright © WireMock.Net

using WireMock.Models;

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
        request.Method.Should().Be("posT");
    }

    [Fact]
    public void RequestMessage_ParseQuery_NoKeys()
    {
        // given
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "POST", ClientIp);

        // then
        request.GetParameter("not_there").Should().BeNull();
    }

    [Fact]
    public void RequestMessage_ParseQuery_SingleKey_SingleValue()
    {
        // Assign
        var request = new RequestMessage(new UrlDetails("http://localhost?foo=bar"), "POST", ClientIp);

        // Assert
        request.GetParameter("foo").Should().ContainSingle("bar");
    }

    [Fact]
    public void RequestMessage_ParseQuery_SingleKey_SingleValue_WithIgnoreCase()
    {
        // Assign
        var request = new RequestMessage(new UrlDetails("http://localhost?foo=bar"), "POST", ClientIp);

        // Assert
        request.GetParameter("FoO", true).Should().ContainSingle("bar");
    }

    [Fact]
    public void RequestMessage_ParseQuery_MultipleKeys_MultipleValues()
    {
        // Assign
        var request = new RequestMessage(new UrlDetails("http://localhost?key=1&key=2"), "POST", ClientIp);

        // Assert
        request.GetParameter("key").Should().Contain("1");
        request.GetParameter("key").Should().Contain("2");
    }

    [Fact]
    public void RequestMessage_ParseQuery_SingleKey_MultipleValuesCommaSeparated()
    {
        // Assign
        var request = new RequestMessage(new UrlDetails("http://localhost?key=1,2,3"), "POST", ClientIp);

        // Assert
        request.GetParameter("key").Should().Contain("1");
        request.GetParameter("key").Should().Contain("2");
        request.GetParameter("key").Should().Contain("3");
    }

    [Fact]
    public void RequestMessage_ParseQuery_SingleKey_MultipleValues()
    {
        // Assign
        var request = new RequestMessage(new UrlDetails("http://localhost?key=1,2&foo=bar&key=3"), "POST", ClientIp);

        // Assert
        request.GetParameter("key").Should().Contain("1");
        request.GetParameter("key").Should().Contain("2");
        request.GetParameter("key").Should().Contain("3");
    }
}
