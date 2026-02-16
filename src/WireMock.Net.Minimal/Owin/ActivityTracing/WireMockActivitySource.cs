// Copyright © WireMock.Net

using System.Diagnostics;
using System.Net.WebSockets;
using WireMock.Logging;
using WireMock.Settings;
using WireMock.WebSockets;

namespace WireMock.Owin.ActivityTracing;

/// <summary>
/// Provides an ActivitySource for WireMock.Net distributed tracing.
/// </summary>
internal static class WireMockActivitySource
{
    /// <summary>
    /// The name of the ActivitySource used by WireMock.Net.
    /// </summary>
    internal const string SourceName = "WireMock.Net";

    /// <summary>
    /// The ActivitySource instance used for creating tracing activities.
    /// </summary>
    public static readonly ActivitySource Source = new(SourceName, GetVersion());

    private static string GetVersion()
    {
        return typeof(WireMockActivitySource).Assembly.GetName().Version?.ToString() ?? "1.0.0";
    }

    /// <summary>
    /// Starts a new activity for a WireMock request.
    /// </summary>
    /// <param name="requestMethod">The HTTP method of the request.</param>
    /// <param name="requestPath">The path of the request.</param>
    /// <returns>The started activity, or null if tracing is not enabled.</returns>
    internal static Activity? StartRequestActivity(string requestMethod, string requestPath)
    {
        if (!Source.HasListeners())
        {
            return null;
        }

        var activity = Source.StartActivity(
            $"WireMock {requestMethod} {requestPath}",
            ActivityKind.Server
        );

        return activity;
    }

    /// <summary>
    /// Enriches an activity with request information.
    /// </summary>
    internal static void EnrichWithRequest(Activity? activity, IRequestMessage request, ActivityTracingOptions? options = null)
    {
        if (activity == null)
        {
            return;
        }

        activity.SetTag(WireMockSemanticConventions.HttpMethod, request.Method);
        activity.SetTag(WireMockSemanticConventions.HttpUrl, request.Url);
        activity.SetTag(WireMockSemanticConventions.HttpPath, request.Path);
        activity.SetTag(WireMockSemanticConventions.HttpHost, request.Host);

        if (request.ClientIP != null)
        {
            activity.SetTag(WireMockSemanticConventions.ClientAddress, request.ClientIP);
        }

        // Record request body if enabled
        if (options?.RecordRequestBody == true && request.Body != null)
        {
            activity.SetTag(WireMockSemanticConventions.RequestBody, request.Body);
        }
    }

    /// <summary>
    /// Enriches an activity with response information.
    /// </summary>
    internal static void EnrichWithResponse(Activity? activity, IResponseMessage? response, ActivityTracingOptions? options = null)
    {
        if (activity == null || response == null)
        {
            return;
        }

        // StatusCode can be int, HttpStatusCode, or string
        var statusCode = response.StatusCode;
        int? statusCodeInt = statusCode switch
        {
            int i => i,
            System.Net.HttpStatusCode hsc => (int)hsc,
            string s when int.TryParse(s, out var parsed) => parsed,
            _ => null
        };

        if (statusCodeInt.HasValue)
        {
            activity.SetTag(WireMockSemanticConventions.HttpStatusCode, statusCodeInt.Value);
            activity.SetTag("otel.status_description", $"HTTP {statusCodeInt.Value}");

            // Set status based on HTTP status code (using standard otel.status_code tag)
            if (statusCodeInt.Value >= 400)
            {
                activity.SetTag("otel.status_code", "ERROR");
            }
            else
            {
                activity.SetTag("otel.status_code", "OK");
            }
        }

        // Record response body if enabled
        if (options?.RecordResponseBody == true && response.BodyData?.BodyAsString != null)
        {
            activity.SetTag(WireMockSemanticConventions.ResponseBody, response.BodyData.BodyAsString);
        }
    }

    /// <summary>
    /// Enriches an activity with mapping match information.
    /// </summary>
    internal static void EnrichWithMappingMatch(
        Activity? activity, 
        Guid? mappingGuid, 
        string? mappingTitle,
        bool isPerfectMatch,
        double? matchScore)
    {
        if (activity == null)
        {
            return;
        }

        activity.SetTag(WireMockSemanticConventions.MappingMatched, isPerfectMatch);

        if (mappingGuid.HasValue)
        {
            activity.SetTag(WireMockSemanticConventions.MappingGuid, mappingGuid.Value.ToString());
        }

        if (!string.IsNullOrEmpty(mappingTitle))
        {
            activity.SetTag(WireMockSemanticConventions.MappingTitle, mappingTitle);
        }

        if (matchScore.HasValue)
        {
            activity.SetTag(WireMockSemanticConventions.MatchScore, matchScore.Value);
        }
    }

    /// <summary>
    /// Enriches an activity with log entry information (includes response and mapping match info).
    /// </summary>
    internal static void EnrichWithLogEntry(Activity? activity, ILogEntry logEntry, ActivityTracingOptions? options = null)
    {
        if (activity == null)
        {
            return;
        }

        // Enrich with response
        EnrichWithResponse(activity, logEntry.ResponseMessage, options);

        // Enrich with mapping match (if enabled)
        if (options?.RecordMatchDetails != false)
        {
            EnrichWithMappingMatch(
                activity,
                logEntry.MappingGuid,
                logEntry.MappingTitle,
                logEntry.RequestMatchResult?.IsPerfectMatch ?? false,
                logEntry.RequestMatchResult?.TotalScore);
        }

        // Set request GUID
        activity.SetTag(WireMockSemanticConventions.RequestGuid, logEntry.Guid.ToString());
    }

    /// <summary>
    /// Records an exception on the activity.
    /// </summary>
    internal static void RecordException(Activity? activity, Exception exception)
    {
        if (activity == null)
        {
            return;
        }

        // Use standard OpenTelemetry exception semantic conventions
        activity.SetTag("otel.status_code", "ERROR");
        activity.SetTag("otel.status_description", exception.Message);
        activity.SetTag("exception.type", exception.GetType().FullName);
        activity.SetTag("exception.message", exception.Message);
        activity.SetTag("exception.stacktrace", exception.ToString());
    }

    /// <summary>
    /// Starts a new activity for a WebSocket message.
    /// </summary>
    /// <param name="direction">The direction of the message.</param>
    /// <param name="mappingGuid">The GUID of the mapping handling the WebSocket.</param>
    /// <returns>The started activity, or null if tracing is not enabled.</returns>
    internal static Activity? StartWebSocketMessageActivity(WebSocketMessageDirection direction, Guid mappingGuid)
    {
        if (!Source.HasListeners())
        {
            return null;
        }

        var activity = Source.StartActivity(
            $"WireMock WebSocket {direction.ToString().ToLowerInvariant()}",
            ActivityKind.Server
        );

        if (activity != null)
        {
            activity.SetTag(WireMockSemanticConventions.MappingGuid, mappingGuid.ToString());
        }

        return activity;
    }

    /// <summary>
    /// Enriches an activity with WebSocket message information.
    /// </summary>
    internal static void EnrichWithWebSocketMessage(
        Activity? activity,
        WebSocketMessageType messageType,
        int messageSize,
        bool endOfMessage,
        string? textContent = null,
        ActivityTracingOptions? options = null)
    {
        if (activity == null)
        {
            return;
        }

        activity.SetTag(WireMockSemanticConventions.WebSocketMessageType, messageType.ToString());
        activity.SetTag(WireMockSemanticConventions.WebSocketMessageSize, messageSize);
        activity.SetTag(WireMockSemanticConventions.WebSocketEndOfMessage, endOfMessage);

        // Record message content if enabled and it's text
        if (options?.RecordRequestBody == true && messageType == WebSocketMessageType.Text && textContent != null)
        {
            activity.SetTag(WireMockSemanticConventions.WebSocketMessageContent, textContent);
        }

        activity.SetTag("otel.status_code", "OK");
    }
}