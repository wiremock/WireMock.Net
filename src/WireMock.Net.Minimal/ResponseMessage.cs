// Copyright © WireMock.Net and mock4net by Alexandre Victoor

// This source file is based on mock4net by Alexandre Victoor which is licensed under the Apache 2.0 License.
// For more details see 'mock4net/LICENSE.txt' and 'mock4net/readme.md' in this project root.
using Stef.Validation;
using WireMock.ResponseBuilders;
using WireMock.Types;
using WireMock.Util;

namespace WireMock;

/// <summary>
/// The ResponseMessage.
/// </summary>
public class ResponseMessage : IResponseMessage
{
    /// <inheritdoc />
    public IDictionary<string, WireMockList<string>>? Headers { get; set; } = new Dictionary<string, WireMockList<string>>();

    /// <inheritdoc />
    public IDictionary<string, WireMockList<string>>? TrailingHeaders { get; set; } = new Dictionary<string, WireMockList<string>>();

    /// <inheritdoc cref="IResponseMessage.StatusCode" />
    public object? StatusCode { get; set; }

    /// <inheritdoc cref="IResponseMessage.BodyOriginal" />
    public string? BodyOriginal { get; set; }

    /// <inheritdoc cref="IResponseMessage.BodyDestination" />
    public string? BodyDestination { get; set; }

    /// <inheritdoc cref="IResponseMessage.BodyData" />
    public IBodyData? BodyData { get; set; }

    /// <inheritdoc cref="IResponseMessage.FaultType" />
    public FaultType FaultType { get; set; }

    /// <inheritdoc cref="IResponseMessage.FaultPercentage" />
    public double? FaultPercentage { get; set; }

    /// <inheritdoc />
    public DateTime DateTime { get; set; }

    /// <inheritdoc />
    public void AddHeader(string name, string value)
    {
        Headers ??= new Dictionary<string, WireMockList<string>>();
        Headers.Add(name, value);
    }

    /// <inheritdoc />
    public void AddHeader(string name, params string[] values)
    {
        Guard.NotNullOrEmpty(values);

        Headers ??= new Dictionary<string, WireMockList<string>>();
        var newHeaderValues = Headers.TryGetValue(name, out var existingValues)
            ? values.Union(existingValues).ToArray()
            : values;

        Headers[name] = newHeaderValues;
    }

    /// <inheritdoc />
    public void AddTrailingHeader(string name, string value)
    {
        TrailingHeaders ??= new Dictionary<string, WireMockList<string>>();
        TrailingHeaders.Add(name, value);
    }

    /// <inheritdoc />
    public void AddTrailingHeader(string name, params string[] values)
    {
        Guard.NotNullOrEmpty(values);

        TrailingHeaders ??= new Dictionary<string, WireMockList<string>>();
        var newHeaderValues = TrailingHeaders.TryGetValue(name, out var existingValues)
            ? values.Union(existingValues).ToArray()
            : values;

        TrailingHeaders[name] = newHeaderValues;
    }
}