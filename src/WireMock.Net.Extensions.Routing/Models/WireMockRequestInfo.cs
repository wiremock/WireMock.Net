namespace WireMock.Net.Extensions.Routing.Models;

/// <summary>
/// Represents request information for WireMock.Net routing, including the request message and route arguments.
/// </summary>
public class WireMockRequestInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WireMockRequestInfo"/> class.
    /// </summary>
    /// <param name="request">The incoming request message.</param>
    public WireMockRequestInfo(IRequestMessage request)
    {
        Request = request;
    }

    /// <summary>
    /// Gets the incoming request message.
    /// </summary>
    public IRequestMessage Request { get; }

    /// <summary>
    /// Gets or initializes the route arguments extracted from the request path.
    /// </summary>
    public IDictionary<string, object> RouteArgs { get; init; } =
        new Dictionary<string, object>();
}
