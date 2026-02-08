// Copyright © WireMock.Net

using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Stef.Validation;
using WireMock.Models;
using WireMock.Settings;
using WireMock.Types;

namespace WireMock.ResponseBuilders;

/// <summary>
/// Builder for constructing WebSocket responses with method chaining support.
/// </summary>
public class WebSocketResponseBuilder : IWebSocketResponseBuilder
{
    private readonly WebSocketResponse _response = new();
    private readonly IResponseBuilder? _responseBuilder;

    /// <summary>
    /// Initialize a new WebSocketResponseBuilder with an associated response builder for chaining.
    /// </summary>
    /// <param name="responseBuilder">The parent response builder for fluent chaining (optional)</param>
    public WebSocketResponseBuilder(IResponseBuilder? responseBuilder = null)
    {
        _responseBuilder = responseBuilder;
    }

    /// <inheritdoc />
    public IResponseBuilder WithMessage(string message, int delayMs = 0)
    {
        Guard.NotNullOrEmpty(message);
        if (delayMs < 0) throw new ArgumentException("Delay must be non-negative", nameof(delayMs));

        var wsMessage = new WebSocketMessage
        {
            BodyAsString = message,
            DelayMs = delayMs
        };

        _response.AddMessage(wsMessage);
        return _responseBuilder ?? this;
    }

    /// <inheritdoc />
    public IResponseBuilder WithJsonMessage(object message, int delayMs = 0)
    {
        Guard.NotNull(message);
        if (delayMs < 0) throw new ArgumentException("Delay must be non-negative", nameof(delayMs));

        var json = JsonConvert.SerializeObject(message);
        var wsMessage = new WebSocketMessage
        {
            BodyAsString = json,
            DelayMs = delayMs
        };

        _response.AddMessage(wsMessage);
        return _responseBuilder ?? this;
    }

    /// <inheritdoc />
    public IResponseBuilder WithBinaryMessage(byte[] message, int delayMs = 0)
    {
        Guard.NotNull(message);
        if (delayMs < 0) throw new ArgumentException("Delay must be non-negative", nameof(delayMs));

        var wsMessage = new WebSocketMessage
        {
            BodyAsBytes = message,
            DelayMs = delayMs
        };

        _response.AddMessage(wsMessage);
        return _responseBuilder ?? this;
    }

    /// <inheritdoc />
    public IResponseBuilder WithTransformer(TransformerType transformerType = TransformerType.Handlebars)
    {
        _response.UseTransformer = true;
        _response.TransformerType = transformerType;
        return _responseBuilder ?? this;
    }

    /// <inheritdoc />
    public IResponseBuilder WithClose(int code, string? message = null)
    {
        if (code < 1000 || code > 4999)
        {
            throw new ArgumentOutOfRangeException(nameof(code), "WebSocket close code must be between 1000 and 4999");
        }

        _response.CloseCode = code;
        _response.CloseMessage = message;
        return _responseBuilder ?? this;
    }

    /// <inheritdoc />
    public IResponseBuilder WithSubprotocol(string subprotocol)
    {
        Guard.NotNullOrEmpty(subprotocol);

        _response.Subprotocol = subprotocol;
        return _responseBuilder ?? this;
    }

    /// <inheritdoc />
    public IResponseBuilder WithAutoClose(int delayMs = 0)
    {
        if (delayMs < 0) throw new ArgumentException("Delay must be non-negative", nameof(delayMs));

        _response.AutoCloseDelayMs = delayMs;
        return _responseBuilder ?? this;
    }

    /// <inheritdoc />
    public IWebSocketResponse? Build()
    {
        return _response;
    }

    /// <inheritdoc />
    public Task<(IResponseMessage Message, IMapping? Mapping)> ProvideResponseAsync(IMapping mapping, IRequestMessage requestMessage, WireMockServerSettings settings)
    {
        // WebSocket responses are not directly provided through this mechanism
        // This is handled by the Response builder when processing WebSocket requests
        throw new NotImplementedException("WebSocket responses are handled by the Response builder");
    }
}
