// Copyright © WireMock.Net

using System.Net.WebSockets;
using Stef.Validation;

namespace WireMock.WebSockets;

internal class WebSocketMessageBuilder : IWebSocketMessageBuilder
{
    /// <inheritdoc />
    public string? MessageText { get; private set; }

    /// <inheritdoc />
    public byte[]? MessageBytes { get; private set; }

    /// <inheritdoc />
    public TimeSpan? Delay { get; private set; }

    /// <inheritdoc />
    public WebSocketMessageType Type { get; private set; }

    /// <inheritdoc />
    public bool ShouldClose { get; private set; }

    /// <inheritdoc />
    public IWebSocketMessageBuilder WithEcho()
    {
        Type = WebSocketMessageType.Close;
        return this;
    }

    /// <inheritdoc />
    public IWebSocketMessageBuilder WithText(string text)
    {
        MessageText = Guard.NotNull(text);
        Type = WebSocketMessageType.Text;
        return this;
    }

    /// <inheritdoc />
    public IWebSocketMessageBuilder WithBinary(byte[] bytes)
    {
        MessageBytes = Guard.NotNull(bytes);
        Type = WebSocketMessageType.Binary;
        return this;
    }

    /// <inheritdoc />
    public IWebSocketMessageBuilder WithDelay(TimeSpan delay)
    {
        Delay = delay;
        return this;
    }

    /// <inheritdoc />
    public IWebSocketMessageBuilder WithDelay(int delayInMilliseconds)
    {
        Guard.Condition(delayInMilliseconds, d => d >= 0);
        return WithDelay(TimeSpan.FromMilliseconds(delayInMilliseconds));
    }

    /// <inheritdoc />
    public IWebSocketMessageBuilder Close()
    {
        ShouldClose = true;
        return this;
    }

    /// <inheritdoc />
    public IWebSocketMessageBuilder AndClose() => Close();
}