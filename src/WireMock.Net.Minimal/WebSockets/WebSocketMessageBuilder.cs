// Copyright © WireMock.Net

using System.Net.WebSockets;
using Stef.Validation;

namespace WireMock.WebSockets;

internal class WebSocketMessageBuilder : IWebSocketMessageBuilder
{
    public string? MessageText { get; private set; }

    public byte[]? MessageBytes { get; private set; }

    public object? MessageData { get; private set; }

    public TimeSpan? Delay { get; private set; }

    public WebSocketMessageType Type { get; private set; }

    public bool ShouldClose { get; private set; }

    public IWebSocketMessageBuilder WithText(string text)
    {
        MessageText = Guard.NotNull(text);
        Type = WebSocketMessageType.Text;
        return this;
    }

    public IWebSocketMessageBuilder WithBinary(byte[] bytes)
    {
        MessageBytes = Guard.NotNull(bytes);
        Type = WebSocketMessageType.Binary;
        return this;
    }

    public IWebSocketMessageBuilder WithDelay(TimeSpan delay)
    {
        Delay = delay;
        return this;
    }

    public IWebSocketMessageBuilder WithDelay(int delayInMilliseconds)
    {
        Guard.Condition(delayInMilliseconds, d => d >= 0, nameof(delayInMilliseconds));
        return WithDelay(TimeSpan.FromMilliseconds(delayInMilliseconds));
    }

    public IWebSocketMessageBuilder Close()
    {
        ShouldClose = true;
        return this;
    }

    public IWebSocketMessageBuilder AndClose() => Close();
}