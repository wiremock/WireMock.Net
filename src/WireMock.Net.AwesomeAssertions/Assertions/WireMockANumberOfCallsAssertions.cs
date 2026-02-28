// Copyright © WireMock.Net

using WireMock.Server;

// ReSharper disable once CheckNamespace
namespace WireMock.AwesomeAssertions;

/// <summary>
/// Provides assertion methods to verify the number of calls made to a WireMock server.
/// This class is used in the context of AwesomeAssertions.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="WireMockANumberOfCallsAssertions"/> class.
/// </remarks>
/// <param name="server">The WireMock server to assert against.</param>
/// <param name="callsCount">The expected number of calls to assert.</param>
/// <param name="chain">The assertion chain</param>
public class WireMockANumberOfCallsAssertions(IWireMockServer server, int callsCount, AssertionChain chain)
{
    /// <summary>
    /// Returns an instance of <see cref="WireMockAssertions"/> which can be used to assert the expected number of calls.
    /// </summary>
    /// <returns>A <see cref="WireMockAssertions"/> instance for asserting the number of calls to the server.</returns>
    public WireMockAssertions Calls()
    {
        return new WireMockAssertions(server, callsCount, chain);
    }
}