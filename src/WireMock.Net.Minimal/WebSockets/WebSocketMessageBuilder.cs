// Copyright © WireMock.Net

using Stef.Validation;

namespace WireMock.WebSockets;

internal class WebSocketMessageBuilder : IWebSocketMessageBuilder
{
    public string? MessageText { get; private set; }

    public byte[]? MessageBytes { get; private set; }

    public object? MessageData { get; private set; }

    public TimeSpan? Delay { get; private set; }

    public MessageType Type { get; private set; }

    public bool ShouldClose { get; private set; }

    public IWebSocketMessageBuilder WithText(string text)
    {
        MessageText = Guard.NotNull(text);
        Type = MessageType.Text;
        return this;
    }

    public IWebSocketMessageBuilder WithBytes(byte[] bytes)
    {
        MessageBytes = Guard.NotNull(bytes);
        Type = MessageType.Bytes;
        return this;
    }

    public IWebSocketMessageBuilder WithJson(object data)
    {
        MessageData = Guard.NotNull(data);
        Type = MessageType.Json;
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
        Delay = TimeSpan.FromMilliseconds(delayInMilliseconds);
        return this;
    }

    public IWebSocketMessageBuilder AndClose()
    {
        ShouldClose = true;
        return this;
    }

    public IWebSocketMessageBuilder Close()
    {
        ShouldClose = true;
        return this;
    }

    internal enum MessageType
    {
        Text,
        Bytes,
        Json
    }
}
