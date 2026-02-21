// Copyright © WireMock.Net

using System;
using WireMock.Matchers.Request;

namespace WireMock.Logging;

/// <summary>
/// ILogEntry
/// </summary>
public interface ILogEntry
{
    /// <summary>
    /// Gets the unique identifier.
    /// </summary>
    Guid Guid { get; }

    /// <summary>
    /// Gets the mapping unique identifier.
    /// </summary>
    Guid? MappingGuid { get; }

    /// <summary>
    /// Gets the mapping unique title.
    /// </summary>
    string? MappingTitle { get; }

    /// <summary>
    /// Gets the partial mapping unique identifier.
    /// </summary>
    Guid? PartialMappingGuid { get; }

    /// <summary>
    /// Gets the partial mapping unique title.
    /// </summary>
    string? PartialMappingTitle { get; }

    /// <summary>
    /// Gets the partial match result.
    /// </summary>
    IRequestMatchResult? PartialMatchResult { get; }

    /// <summary>
    /// Gets the request match result.
    /// </summary>
    IRequestMatchResult? RequestMatchResult { get; }

    /// <summary>
    /// Gets the request message.
    /// </summary>
    IRequestMessage? RequestMessage { get; }

    /// <summary>
    /// Gets the response message.
    /// </summary>
    IResponseMessage? ResponseMessage { get; }
}