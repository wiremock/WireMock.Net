// Copyright © WireMock.Net

using System;
using System.Threading.Tasks;
using Stef.Validation;
using WireMock.Settings;
using WireMock.Types;

namespace WireMock.WebSockets;

internal class WebSocketBuilder : IWebSocketBuilder
{
    /// <inheritdoc />
    public string? AcceptProtocol { get; private set; }

    /// <inheritdoc />
    public bool IsEcho { get; private set; }

    /// <inheritdoc />
    public bool IsBroadcast { get; private set; }

    /// <inheritdoc />
    public Func<WebSocketMessage, IWebSocketContext, Task>? MessageHandler { get; private set; }

    /// <inheritdoc />
    public WebSocketMessageSequence? MessageSequence { get; private set; }

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

    public IWebSocketBuilder WithText(string text)
    {
        Guard.NotNull(text);
        return WithMessageHandler(async (message, context) =>
        {
            await context.SendAsync(text);
        });
    }

    public IWebSocketBuilder WithBytes(byte[] bytes)
    {
        Guard.NotNull(bytes);
        return WithMessageHandler(async (message, context) =>
        {
            await context.SendAsync(bytes);
        });
    }

    public IWebSocketBuilder WithJson(object data)
    {
        Guard.NotNull(data);
        return WithMessageHandler(async (message, context) =>
        {
            await context.SendAsJsonAsync(data);
        });
    }

    public IWebSocketBuilder WithMessageHandler(Func<WebSocketMessage, IWebSocketContext, Task> handler)
    {
        MessageHandler = Guard.NotNull(handler);
        IsEcho = false; // Disable echo if custom handler is set
        return this;
    }

    public IWebSocketBuilder WithMessageSequence(Action<IWebSocketMessageSequenceBuilder> configure)
    {
        var sequenceBuilder = new WebSocketMessageSequenceBuilder();
        configure(sequenceBuilder);
        MessageSequence = sequenceBuilder.Build();
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
}