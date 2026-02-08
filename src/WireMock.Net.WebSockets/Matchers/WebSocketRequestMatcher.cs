// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using System.Linq;
using Stef.Validation;
using WireMock.Matchers;
using WireMock.Matchers.Request;
using WireMock.Types;

namespace WireMock.WebSockets.Matchers;

/// <summary>
/// Matcher for WebSocket upgrade requests.
/// </summary>
public class WebSocketRequestMatcher : IRequestMatcher
{
    private static string Name => nameof(WebSocketRequestMatcher);

    private readonly IStringMatcher? _pathMatcher;
    private readonly IList<string>? _subProtocols;
    private readonly Func<WebSocketConnectRequest, bool>? _customPredicate;

    /// <summary>
    /// Initializes a new instance of the <see cref="WebSocketRequestMatcher"/> class.
    /// </summary>
    /// <param name="pathMatcher">The optional path matcher.</param>
    /// <param name="subProtocols">The optional list of acceptable subprotocols.</param>
    /// <param name="customPredicate">The optional custom predicate for matching.</param>
    public WebSocketRequestMatcher(IStringMatcher? pathMatcher = null, IList<string>? subProtocols = null, Func<WebSocketConnectRequest, bool>? customPredicate = null)
    {
        _pathMatcher = pathMatcher;
        _subProtocols = subProtocols;
        _customPredicate = customPredicate;
    }

    /// <inheritdoc/>
    public double GetMatchingScore(IRequestMessage requestMessage, IRequestMatchResult requestMatchResult)
    {
        var (score, exception) = GetMatchResult(requestMessage).Expand();
        return requestMatchResult.AddScore(GetType(), score, exception);
    }
   
    private MatchResult GetMatchResult(IRequestMessage requestMessage)
    {
        Guard.NotNull(requestMessage);

        // Check if this is a WebSocket upgrade request
        if (!IsWebSocketUpgradeRequest(requestMessage))
        {
            return MatchResult.From(Name);
        }

        var matchScore = MatchScores.Perfect;

        // Match path if matcher is provided
        if (_pathMatcher != null)
        {
            var pathMatchResult = _pathMatcher.IsMatch(requestMessage.Path ?? string.Empty);
            if (pathMatchResult.Score < 1.0)
            {
                return MatchResult.From(Name);
            }
            matchScore *= pathMatchResult.Score;
        }

        // Check subprotocol if specified
        if (_subProtocols?.Count > 0)
        {
            var requestSubProtocols = GetRequestedSubProtocols(requestMessage);
            var hasValidSubProtocol = requestSubProtocols.Any(_subProtocols.Contains);
            
            if (!hasValidSubProtocol && _subProtocols.Count > 0)
            {
                return MatchResult.From(Name);
            }
        }

        // Apply custom predicate if provided
        if (_customPredicate != null)
        {
            var wsRequest = CreateWebSocketConnectRequest(requestMessage);
            if (!_customPredicate(wsRequest))
            {
                return MatchResult.From(Name);
            }
        }

        return MatchResult.From(Name, matchScore);
    }

    private static bool IsWebSocketUpgradeRequest(IRequestMessage request)
    {
        if (request.Headers == null)
        {
            return false;
        }

        var hasUpgradeHeader = request.Headers.TryGetValue("Upgrade", out var upgradeValues) &&
                              upgradeValues?.Any(v => v.Equals("websocket", StringComparison.OrdinalIgnoreCase)) == true;

        var hasConnectionHeader = request.Headers.TryGetValue("Connection", out var connectionValues) &&
                                 connectionValues?.Any(v => v.IndexOf("Upgrade", StringComparison.OrdinalIgnoreCase) >= 0) == true;

        return hasUpgradeHeader && hasConnectionHeader;
    }

    private static string[] GetRequestedSubProtocols(IRequestMessage request)
    {
        if (request.Headers?.TryGetValue("Sec-WebSocket-Protocol", out var values) == true && values != null)
        {
            return values
                .SelectMany(v => v.Split(','))
                .Select(s => s.Trim())
                .ToArray();
        }

        return [];
    }

    private static WebSocketConnectRequest CreateWebSocketConnectRequest(IRequestMessage request)
    {
        var headers = request.Headers ?? new Dictionary<string, WireMockList<string>>();
        var subProtocols = GetRequestedSubProtocols(request);
        var clientIP = request.ClientIP ?? string.Empty;

        return new WebSocketConnectRequest
        {
            Path = request.Path,
            Headers = headers,
            SubProtocols = subProtocols,
            RemoteAddress = clientIP
        };
    }   
}