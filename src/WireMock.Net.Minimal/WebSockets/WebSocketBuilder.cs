// Copyright © WireMock.Net

using System.Net.WebSockets;
using Stef.Validation;
using WireMock.Matchers;
using WireMock.ResponseBuilders;
using WireMock.Settings;
using WireMock.Transformers;

namespace WireMock.WebSockets;

internal class WebSocketBuilder(Response response) : IWebSocketBuilder
{
    private readonly List<(IMatcher matcher, List<WebSocketMessageBuilder> messages)> _conditionalMessages = [];

    public string? AcceptProtocol { get; private set; }

    public bool IsEcho { get; private set; }

    public Func<WebSocketMessage, IWebSocketContext, Task>? MessageHandler { get; private set; }

    public ProxyAndRecordSettings? ProxySettings { get; private set; }

    public TimeSpan? CloseTimeout { get; private set; }

    public int? MaxMessageSize { get; private set; }

    public int? ReceiveBufferSize { get; private set; }

    public TimeSpan? KeepAliveIntervalSeconds { get; private set; }

    public IWebSocketBuilder WithAcceptProtocol(string protocol)
    {
        AcceptProtocol = Guard.NotNull(protocol);
        return this;
    }

    public IWebSocketBuilder WithEcho()
    {
        IsEcho = true;
        return this;
    }

    public IWebSocketBuilder SendMessage(Action<IWebSocketMessageBuilder> configure)
    {
        Guard.NotNull(configure);
        var messageBuilder = new WebSocketMessageBuilder();
        configure(messageBuilder);

        return WithMessageHandler(async (message, context) =>
        {
            if (messageBuilder.Delay.HasValue)
            {
                await Task.Delay(messageBuilder.Delay.Value);
            }

            await SendMessageAsync(context, messageBuilder, message);
        });
    }

    public IWebSocketBuilder SendMessages(Action<IWebSocketMessagesBuilder> configure)
    {
        Guard.NotNull(configure);
        var messagesBuilder = new WebSocketMessagesBuilder();
        configure(messagesBuilder);

        return WithMessageHandler(async (message, context) =>
        {
            foreach (var messageBuilder in messagesBuilder.Messages)
            {
                if (messageBuilder.Delay.HasValue)
                {
                    await Task.Delay(messageBuilder.Delay.Value);
                }

                await SendMessageAsync(context, messageBuilder, message);
            }
        });
    }

    public IWebSocketMessageConditionBuilder WhenMessage(string wildcardPattern)
    {
        Guard.NotNull(wildcardPattern);
        var matcher = new WildcardMatcher(MatchBehaviour.AcceptOnMatch, wildcardPattern);
        return new WebSocketMessageConditionBuilder(this, matcher);
    }

    public IWebSocketMessageConditionBuilder WhenMessage(byte[] exactPattern)
    {
        Guard.NotNull(exactPattern);
        var matcher = new ExactObjectMatcher(MatchBehaviour.AcceptOnMatch, exactPattern);
        return new WebSocketMessageConditionBuilder(this, matcher);
    }

    public IWebSocketMessageConditionBuilder WhenMessage(IMatcher matcher)
    {
        Guard.NotNull(matcher);
        return new WebSocketMessageConditionBuilder(this, matcher);
    }

    public IWebSocketBuilder WithMessageHandler(Func<WebSocketMessage, IWebSocketContext, Task> handler)
    {
        MessageHandler = Guard.NotNull(handler);
        IsEcho = false;
        return this;
    }

    public IWebSocketBuilder WithProxy(ProxyAndRecordSettings settings)
    {
        ProxySettings = Guard.NotNull(settings);
        IsEcho = false;
        return this;
    }

    public IWebSocketBuilder WithCloseTimeout(TimeSpan timeout)
    {
        CloseTimeout = timeout;
        return this;
    }

    public IWebSocketBuilder WithMaxMessageSize(int sizeInBytes)
    {
        MaxMessageSize = Guard.Condition(sizeInBytes, s => s > 0);
        return this;
    }

    public IWebSocketBuilder WithReceiveBufferSize(int sizeInBytes)
    {
        ReceiveBufferSize = Guard.Condition(sizeInBytes, s => s > 0);
        return this;
    }

    public IWebSocketBuilder WithKeepAliveInterval(TimeSpan interval)
    {
        KeepAliveIntervalSeconds = interval;
        return this;
    }

    internal IWebSocketBuilder AddConditionalMessage(IMatcher matcher, WebSocketMessageBuilder messageBuilder)
    {
        _conditionalMessages.Add((matcher, new List<WebSocketMessageBuilder> { messageBuilder }));
        SetupConditionalHandler();
        return this;
    }

    internal IWebSocketBuilder AddConditionalMessages(IMatcher matcher, List<WebSocketMessageBuilder> messages)
    {
        _conditionalMessages.Add((matcher, messages));
        SetupConditionalHandler();
        return this;
    }

    private void SetupConditionalHandler()
    {
        if (_conditionalMessages.Count == 0)
        {
            return;
        }

        WithMessageHandler(async (message, context) =>
        {
            // Check each condition in order
            foreach (var (matcher, messages) in _conditionalMessages)
            {
                // Try to match the message
                if (await MatchMessageAsync(message, matcher))
                {
                    // Execute the corresponding messages
                    foreach (var messageBuilder in messages)
                    {
                        if (messageBuilder.Delay.HasValue)
                        {
                            await Task.Delay(messageBuilder.Delay.Value);
                        }

                        await SendMessageAsync(context, messageBuilder, message);

                        // If this message should close the connection, do it after sending
                        if (messageBuilder.ShouldClose)
                        {
                            try
                            {
                                await Task.Delay(100); // Small delay to ensure message is sent
                                await context.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by handler");
                            }
                            catch
                            {
                                // Ignore errors during close
                            }
                        }
                    }
                    return; // Stop after first match
                }
            }
        });
    }

    private async Task SendMessageAsync(IWebSocketContext context, WebSocketMessageBuilder messageBuilder, WebSocketMessage incomingMessage)
    {
        switch (messageBuilder.Type)
        {
            case WebSocketMessageType.Text:
                var text = messageBuilder.MessageText!;
                if (response.UseTransformer)
                {
                    text = ApplyTransformer(context, incomingMessage, text);
                }
                await context.SendAsync(text);
                break;

            case WebSocketMessageType.Binary:
                await context.SendAsync(messageBuilder.MessageBytes!);
                break;
        }
    }

    private string ApplyTransformer(IWebSocketContext context, WebSocketMessage incomingMessage, string text)
    {
        try
        {
            if (incomingMessage == null)
            {
                // No incoming message, can't apply transformer
                return text;
            }

            var transformer = TransformerFactory.Create(response.TransformerType, context.Mapping.Settings);

            var model = new WebSocketTransformModel
            {
                Mapping = context.Mapping,
                Request = context.RequestMessage,
                Message = incomingMessage,
                Data = context.Mapping.Data
            };

            return transformer.Transform(text, model);
        }
        catch
        {
            // If transformation fails, return original text
            return text;
        }
    }

    private static async Task<bool> MatchMessageAsync(WebSocketMessage message, IMatcher matcher)
    {
        if (message.MessageType == WebSocketMessageType.Text)
        {
            if (matcher is IStringMatcher stringMatcher)
            {
                var result = stringMatcher.IsMatch(message.Text);
                return result.IsPerfect();
            }

            if (matcher is IFuncMatcher funcMatcher)
            {
                var result = funcMatcher.IsMatch(message.Text);
                return result.IsPerfect();
            }
        }

        if (message.MessageType == WebSocketMessageType.Binary)
        {
            if (matcher is IBytesMatcher bytesMatcher)
            {
                var result = await bytesMatcher.IsMatchAsync(message.Bytes);
                return result.IsPerfect();
            }

            if (matcher is IFuncMatcher funcMatcher)
            {
                var result = funcMatcher.IsMatch(message.Bytes);
                return result.IsPerfect();
            }            
        }

        return false;
    }
}