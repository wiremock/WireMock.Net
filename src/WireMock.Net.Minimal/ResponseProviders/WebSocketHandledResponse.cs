// Copyright © WireMock.Net

using System.Net;

namespace WireMock.ResponseProviders;

/// <summary>
/// Special response marker to indicate WebSocket has been handled
/// </summary>
internal class WebSocketHandledResponse : ResponseMessage
{
    public WebSocketHandledResponse(DateTime dateTime)
    {
        // 101 Switching Protocols
        StatusCode = (int)HttpStatusCode.SwitchingProtocols;
        DateTime = dateTime;
    }
}