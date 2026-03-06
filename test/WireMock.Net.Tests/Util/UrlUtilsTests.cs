// Copyright © WireMock.Net

using Microsoft.AspNetCore.Http;
using WireMock.Util;

namespace WireMock.Net.Tests.Util;

public class UrlUtilsTests
{
    [Fact]
    public void UriUtils_CreateUri_WithValidPathString()
    {
        // Assign
        Uri uri = new Uri("https://localhost:1234/a/b?x=0");

        // Act
        var result = UrlUtils.Parse(uri, new PathString("/a"));

        // Assert
        result.Url.ToString().Should().Be("https://localhost:1234/b?x=0");
        result.AbsoluteUrl.ToString().Should().Be("https://localhost:1234/a/b?x=0");
    }

    [Fact]
    public void UriUtils_CreateUri_WithEmptyPathString()
    {
        // Assign
        Uri uri = new Uri("https://localhost:1234/a/b?x=0");

        // Act
        var result = UrlUtils.Parse(uri, new PathString());

        // Assert
        result.Url.ToString().Should().Be("https://localhost:1234/a/b?x=0");
        result.AbsoluteUrl.ToString().Should().Be("https://localhost:1234/a/b?x=0");
    }

    [Fact]
    public void UriUtils_CreateUri_WithDifferentPathString()
    {
        // Assign
        Uri uri = new Uri("https://localhost:1234/a/b?x=0");

        // Act
        var result = UrlUtils.Parse(uri, new PathString("/test"));

        // Assert
        result.Url.ToString().Should().Be("https://localhost:1234/a/b?x=0");
        result.AbsoluteUrl.ToString().Should().Be("https://localhost:1234/a/b?x=0");
    }
}