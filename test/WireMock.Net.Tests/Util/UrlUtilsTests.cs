// Copyright © WireMock.Net

#if NET452
using Microsoft.Owin;
#else
using Microsoft.AspNetCore.Http;
#endif
using NFluent;
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
        Check.That(result.Url.ToString()).Equals("https://localhost:1234/b?x=0");
        Check.That(result.AbsoluteUrl.ToString()).Equals("https://localhost:1234/a/b?x=0");
    }

    [Fact]
    public void UriUtils_CreateUri_WithEmptyPathString()
    {
        // Assign
        Uri uri = new Uri("https://localhost:1234/a/b?x=0");

        // Act
        var result = UrlUtils.Parse(uri, new PathString());

        // Assert
        Check.That(result.Url.ToString()).Equals("https://localhost:1234/a/b?x=0");
        Check.That(result.AbsoluteUrl.ToString()).Equals("https://localhost:1234/a/b?x=0");
    }

    [Fact]
    public void UriUtils_CreateUri_WithDifferentPathString()
    {
        // Assign
        Uri uri = new Uri("https://localhost:1234/a/b?x=0");

        // Act
        var result = UrlUtils.Parse(uri, new PathString("/test"));

        // Assert
        Check.That(result.Url.ToString()).Equals("https://localhost:1234/a/b?x=0");
        Check.That(result.AbsoluteUrl.ToString()).Equals("https://localhost:1234/a/b?x=0");
    }
}