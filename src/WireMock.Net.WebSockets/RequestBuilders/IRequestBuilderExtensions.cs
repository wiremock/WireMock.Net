// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using Stef.Validation;
using WireMock.Matchers;
using WireMock.WebSockets.Matchers;

namespace WireMock.RequestBuilders;

/// <summary>
/// IRequestBuilderExtensions extensions for WebSockets.
/// </summary>
// ReSharper disable once InconsistentNaming
public static class IRequestBuilderExtensions
{
    /// <summary>
    /// Match WebSocket requests to a specific path.
    /// </summary>
    /// <param name="requestBuilder">The request builder.</param>
    /// <param name="path">The path to match.</param>
    /// <returns>The request builder.</returns>
    public static IRequestBuilder WithWebSocketPath(this IRequestBuilder requestBuilder, string path)
    {
        Guard.NotNullOrEmpty(path);
        Guard.NotNull(requestBuilder);

        var pathMatcher = new WildcardMatcher(path);
        requestBuilder.Add(new WebSocketRequestMatcher(pathMatcher));
        return requestBuilder;
    }

    /// <summary>
    /// Match WebSocket requests with specific subprotocols.
    /// </summary>
    /// <param name="requestBuilder">The request builder.</param>
    /// <param name="subProtocols">The acceptable subprotocols.</param>
    /// <returns>The request builder.</returns>
    public static IRequestBuilder WithWebSocketSubprotocol(this IRequestBuilder requestBuilder, params string[] subProtocols)
    {
        Guard.NotNullOrEmpty(subProtocols);
        Guard.NotNull(requestBuilder);

        var subProtocolList = new List<string>(subProtocols);
        requestBuilder.Add(new WebSocketRequestMatcher(null, subProtocolList));
        return requestBuilder;
    }

    /// <summary>
    /// Match WebSocket requests based on custom headers.
    /// </summary>
    /// <param name="requestBuilder">The request builder.</param>
    /// <param name="headers">The header key-value pairs to match.</param>
    /// <returns>The request builder.</returns>
    public static IRequestBuilder WithCustomHandshakeHeaders(this IRequestBuilder requestBuilder, params (string Key, string Value)[] headers)
    {
        Guard.NotNullOrEmpty(headers);
        Guard.NotNull(requestBuilder);

        // Create a predicate that checks for specific headers
        Func<WebSocketConnectRequest, bool>? predicate = wsRequest =>
        {
            foreach (var (key, expectedValue) in headers)
            {
                if (!wsRequest.Headers.TryGetValue(key, out var values) || values == null)
                {
                    return false;
                }

                var hasMatch = false;
                foreach (var value in values)
                {
                    if (value.Equals(expectedValue, StringComparison.OrdinalIgnoreCase))
                    {
                        hasMatch = true;
                        break;
                    }
                }

                if (!hasMatch)
                {
                    return false;
                }
            }

            return true;
        };

        requestBuilder.Add(new WebSocketRequestMatcher(null, null, predicate));
        return requestBuilder;
    }
}