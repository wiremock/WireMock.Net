// Copyright © WireMock.Net

using System.Net.WebSockets;
using Stef.Validation;
using WireMock.Matchers;
using WireMock.Settings;
using WireMock.Types;

namespace WireMock.WebSockets;

internal class WebSocketBuilder : IWebSocketBuilder
{
    private readonly List<(IMatcher matcher, List<WebSocketMessageBuilder> messages)> _conditionalMessages = [];

    /// <inheritdoc />
    public string? AcceptProtocol { get; private set; }

    /// <inheritdoc />
    public bool IsEcho { get; private set; }

    /// <inheritdoc />
    public bool IsBroadcast { get; private set; }

    /// <inheritdoc />
    public Func<WebSocketMessage, IWebSocketContext, Task>? MessageHandler { get; private set; }

    /// <inheritdoc />
    public ProxyAndRecordSettings? ProxySettings { get; private set; }

    /// <inheritdoc />
    public TimeSpan? CloseTimeout { get; private set; }

    /// <inheritdoc />
    public int? MaxMessageSize { get; private set; }

    /// <inheritdoc />
    public int? ReceiveBufferSize { get; private set; }

    /// <inheritdoc />
    public TimeSpan? KeepAliveIntervalSeconds { get; private set; }

    /// <inheritdoc />
    public bool UseTransformer { get; private set; }

    /// <inheritdoc />
    public TransformerType TransformerType { get; private set; }

    /// <inheritdoc />
    public bool UseTransformerForBodyAsFile { get; private set; }

    /// <inheritdoc />
    public ReplaceNodeOptions TransformerReplaceNodeOptions { get; private set; }

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

            await SendMessageAsync(context, messageBuilder);
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

                await SendMessageAsync(context, messageBuilder);
            }
        });
    }

    public IWebSocketMessageConditionBuilder WhenMessage(string condition)
    {
        Guard.NotNull(condition);
        // Use RegexMatcher for substring matching - escape special chars and wrap with wildcards
        // Convert the string to a wildcard pattern that matches if it contains the condition
        var pattern = $"*{condition}*";
        var matcher = new WildcardMatcher(MatchBehaviour.AcceptOnMatch, pattern);
        return new WebSocketMessageConditionBuilder(this, matcher);
    }

    public IWebSocketMessageConditionBuilder WhenMessage(byte[] condition)
    {
        Guard.NotNull(condition);
        // Use ExactObjectMatcher for byte matching
        var matcher = new ExactObjectMatcher(MatchBehaviour.AcceptOnMatch, condition);
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

    public IWebSocketBuilder WithBroadcast()
    {
        IsBroadcast = true;
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

    public IWebSocketBuilder WithTransformer(
        TransformerType transformerType = TransformerType.Handlebars,
        bool useTransformerForBodyAsFile = false,
        ReplaceNodeOptions transformerReplaceNodeOptions = ReplaceNodeOptions.EvaluateAndTryToConvert)
    {
        UseTransformer = true;
        TransformerType = transformerType;
        UseTransformerForBodyAsFile = useTransformerForBodyAsFile;
        TransformerReplaceNodeOptions = transformerReplaceNodeOptions;
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
                if (await MatchMessageAsync(message, matcher)   )
                {
                    // Execute the corresponding messages
                    foreach (var messageBuilder in messages)
                    {
                        if (messageBuilder.Delay.HasValue)
                        {
                            await Task.Delay(messageBuilder.Delay.Value);
                        }

                        await SendMessageAsync(context, messageBuilder);

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

    private static async Task<bool> MatchMessageAsync(WebSocketMessage message, IMatcher matcher)
    {
        if (message.MessageType == WebSocketMessageType.Text && matcher is IStringMatcher stringMatcher)
        {
            var result = stringMatcher.IsMatch(message.Text);
            return result.IsPerfect();
        }

        if (message.MessageType == WebSocketMessageType.Binary && matcher is IBytesMatcher bytesMatcher && message.Bytes != null)
        {
            var result = await bytesMatcher.IsMatchAsync(message.Bytes);
            return result.IsPerfect();
        }

        return false;
    }

    private static async Task SendMessageAsync(IWebSocketContext context, WebSocketMessageBuilder messageBuilder)
    {
        switch (messageBuilder.Type)
        {
            case WebSocketMessageBuilder.MessageType.Text:
                await context.SendAsync(messageBuilder.MessageText!);
                break;
            case WebSocketMessageBuilder.MessageType.Bytes:
                await context.SendAsync(messageBuilder.MessageBytes!);
                break;
            case WebSocketMessageBuilder.MessageType.Json:
                await context.SendAsJsonAsync(messageBuilder.MessageData!);
                break;
        }
    }
}