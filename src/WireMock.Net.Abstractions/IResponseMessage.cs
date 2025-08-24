// Copyright © WireMock.Net

using System.Collections.Generic;
using WireMock.ResponseBuilders;
using WireMock.Types;
using WireMock.Util;

namespace WireMock;

/// <summary>
/// IResponseMessage
/// </summary>
public interface IResponseMessage
{
    /// <summary>
    /// The Body.
    /// </summary>
    IBodyData? BodyData { get; set; }

    /// <summary>
    /// Gets the body destination (Null, SameAsSource, String or Bytes).
    /// </summary>
    string? BodyDestination { get; set; }

    /// <summary>
    /// Gets or sets the body.
    /// </summary>
    string? BodyOriginal { get; }

    /// <summary>
    /// Gets the Fault percentage.
    /// </summary>
    double? FaultPercentage { get; set; }

    /// <summary>
    /// The FaultType.
    /// </summary>
    FaultType FaultType { get; set; }

    /// <summary>
    /// Gets the headers.
    /// </summary>
    IDictionary<string, WireMockList<string>>? Headers { get; set; }

    /// <summary>
    /// Gets the trailing headers.
    /// </summary>
    IDictionary<string, WireMockList<string>>? TrailingHeaders { get; set; }

    /// <summary>
    /// Gets or sets the status code.
    /// </summary>
    object? StatusCode { get; set; }

    /// <summary>
    /// Adds the header.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    void AddHeader(string name, string value);

    /// <summary>
    /// Adds the trailing header.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="values">The values.</param>
    void AddHeader(string name, params string[] values);

    /// <summary>
    /// Adds the trailing header.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    void AddTrailingHeader(string name, string value);

    /// <summary>
    /// Adds the header.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="values">The values.</param>
    void AddTrailingHeader(string name, params string[] values);
}