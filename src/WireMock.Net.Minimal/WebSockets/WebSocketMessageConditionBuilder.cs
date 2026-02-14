// Copyright © WireMock.Net

using WireMock.Matchers;
using Stef.Validation;

namespace WireMock.WebSockets;

internal class WebSocketMessageConditionBuilder : IWebSocketMessageConditionBuilder
{
    private readonly WebSocketBuilder _parent;
    private readonly IMatcher _matcher;

    public WebSocketMessageConditionBuilder(WebSocketBuilder parent, IMatcher matcher)
    {
        _parent = Guard.NotNull(parent);
        _matcher = Guard.NotNull(matcher);
    }

    public IWebSocketBuilder ThenSendMessage(Action<IWebSocketMessageBuilder> configure)
    {
        Guard.NotNull(configure);
        var messageBuilder = new WebSocketMessageBuilder();
        configure(messageBuilder);

        return _parent.AddConditionalMessage(_matcher, messageBuilder);
    }

    public IWebSocketBuilder SendMessages(Action<IWebSocketMessagesBuilder> configure)
    {
        Guard.NotNull(configure);
        var messagesBuilder = new WebSocketMessagesBuilder();
        configure(messagesBuilder);

        return _parent.AddConditionalMessages(_matcher, messagesBuilder.Messages);
    }
}
