// Copyright © WireMock.Net

using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;

namespace WireMock.Extensions;

internal static class HttpContextExtensions
{
    internal static bool TryGet<T>(this HttpContext httpContext, [NotNullWhen(true)] out T? value)
    {
        return httpContext.Items.TryGetValue(nameof(T), out value);
    }
}