// Copyright © WireMock.Net

using System.Collections.Generic;

namespace WireMock.WebSockets;

internal class WebSocketMessagesBuilder : IWebSocketMessagesBuilder
{
    internal List<WebSocketMessageBuilder> Messages { get; } = new();

    public IWebSocketMessagesBuilder AddMessage(Action<IWebSocketMessageBuilder> configure)
    {
        var messageBuilder = new WebSocketMessageBuilder();
        configure(messageBuilder);
        Messages.Add(messageBuilder);
        return this;
    }
}
