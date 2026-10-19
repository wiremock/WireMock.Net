// Copyright © WireMock.Net

namespace WireMock.Admin.Requests;

/// <summary>
/// RequestCount Model
/// </summary>
public class RequestCountModel
{
    /// <summary>
    /// The number of requests which match the criteria.
    /// </summary>
    public int Count { get; set; }
}
