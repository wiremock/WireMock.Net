// Copyright © WireMock.Net

namespace WireMock.Net.Extensions.Routing.Models;

/// <summary>
/// Represents request information with a strongly-typed deserialized body for WireMock.Net routing.
/// </summary>
/// <typeparam name="TBody">The type of the deserialized request body.</typeparam>
public sealed class WireMockRequestInfo<TBody> : WireMockRequestInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WireMockRequestInfo{TBody}"/> class.
    /// </summary>
    /// <param name="request">The incoming request message.</param>
    public WireMockRequestInfo(IRequestMessage request)
        : base(request)
    {
    }

    /// <summary>
    /// Gets or initializes the deserialized request body.
    /// </summary>
    public TBody? Body { get; init; }
}
