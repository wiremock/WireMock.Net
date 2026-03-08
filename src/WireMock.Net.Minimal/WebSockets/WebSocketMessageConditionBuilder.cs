// Copyright © WireMock.Net

using WireMock.Matchers;
using Stef.Validation;

namespace WireMock.WebSockets;

internal class WebSocketMessageConditionBuilder(WebSocketBuilder parent, IMatcher matcher) : IWebSocketMessageConditionBuilder
{
    public IWebSocketBuilder ThenSendMessage(Action<IWebSocketMessageBuilder> configure)
    {
        Guard.NotNull(configure);
        var messageBuilder = new WebSocketMessageBuilder();
        configure(messageBuilder);

        return parent.AddConditionalMessage(matcher, messageBuilder);
    }

    public IWebSocketBuilder SendMessages(Action<IWebSocketMessagesBuilder> configure)
    {
        Guard.NotNull(configure);
        var messagesBuilder = new WebSocketMessagesBuilder();
        configure(messagesBuilder);

        return parent.AddConditionalMessages(matcher, messagesBuilder.Messages);
    }
}