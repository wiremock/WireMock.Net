// Copyright © WireMock.Net

namespace WireMock.ResponseBuilders;

/// <summary>
/// The ResponseBuilder interface.
/// </summary>
public interface IResponseBuilder : IProxyResponseBuilder
{
    /// <summary>
    /// The link back to the mapping.
    /// </summary>
    IMapping Mapping { get; }

    /// <summary>
    /// Gets the response message.
    /// </summary>
    IResponseMessage ResponseMessage { get; }
}