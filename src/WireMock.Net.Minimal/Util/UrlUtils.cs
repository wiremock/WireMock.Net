// Copyright © WireMock.Net

using Microsoft.AspNetCore.Http;
using WireMock.Models;
using Stef.Validation;

namespace WireMock.Util;

internal static class UrlUtils
{
    public static UrlDetails Parse(Uri uri, PathString pathBase)
    {
        Guard.NotNull(uri);

        if (!pathBase.HasValue)
        {
            return new UrlDetails(uri, uri);
        }

        var builder = new UriBuilder(uri);
        builder.Path = RemoveFirst(builder.Path, pathBase.Value);

        return new UrlDetails(uri, builder.Uri);
    }

    private static string RemoveFirst(string text, string search)
    {
        int pos = text.IndexOf(search, StringComparison.Ordinal);
        if (pos < 0)
        {
            return text;
        }

        return text.Substring(0, pos) + text.Substring(pos + search.Length);
    }
}