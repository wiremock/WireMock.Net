// Copyright © WireMock.Net

namespace WireMock.WebSockets;

internal class WebSocketMessageSequenceBuilder : IWebSocketMessageSequenceBuilder
{
    public WebSocketMessageSequence Build()
    {
        return new WebSocketMessageSequence();
    }
}
