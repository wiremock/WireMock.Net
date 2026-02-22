// Copyright © WireMock.Net

namespace WireMock.Owin.ActivityTracing;

/// <summary>
/// Semantic convention constants for WireMock.Net tracing attributes.
/// </summary>
internal static class WireMockSemanticConventions
{
    public const string OtelStatusCode = "otel.status_code";

    // Standard HTTP semantic conventions (OpenTelemetry)
    public const string HttpMethod = "http.request.method";
    public const string HttpUrl = "url.full";
    public const string HttpPath = "url.path";
    public const string HttpHost = "server.address";
    public const string HttpStatusCode = "http.response.status_code";
    public const string ClientAddress = "client.address";

    // WireMock-specific attributes
    public const string MappingMatched = "wiremock.mapping.matched";
    public const string MappingGuid = "wiremock.mapping.guid";
    public const string MappingTitle = "wiremock.mapping.title";
    public const string MatchScore = "wiremock.match.score";
    public const string PartialMappingGuid = "wiremock.partial_mapping.guid";
    public const string PartialMappingTitle = "wiremock.partial_mapping.title";
    public const string RequestGuid = "wiremock.request.guid";
    public const string RequestBody = "wiremock.request.body";
    public const string ResponseBody = "wiremock.response.body";

    // WebSocket-specific attributes
    public const string WebSocketMessageType = "wiremock.websocket.message.type";
    public const string WebSocketMessageSize = "wiremock.websocket.message.size";
    public const string WebSocketEndOfMessage = "wiremock.websocket.message.end_of_message";
    public const string WebSocketMessageContent = "wiremock.websocket.message.content";
}