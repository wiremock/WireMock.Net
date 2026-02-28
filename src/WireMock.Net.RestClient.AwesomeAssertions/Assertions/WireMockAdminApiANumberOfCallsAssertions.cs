// Copyright © WireMock.Net

// ReSharper disable once CheckNamespace
namespace WireMock.Client.AwesomeAssertions;

/// <summary>
/// Provides assertion methods to verify the number of calls made to a WireMock server.
/// This class is used in the context of AwesomeAssertions.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="WireMockAdminApiANumberOfCallsAssertions"/> class.
/// </remarks>
/// <param name="adminApi">The WireMock Admin API to assert against.</param>
/// <param name="callsCount">The expected number of calls to assert.</param>
/// <param name="chain">The assertion chain</param>
public class WireMockAdminApiANumberOfCallsAssertions(IWireMockAdminApi adminApi, int callsCount, AssertionChain chain)
{
    /// <summary>
    /// Returns an instance of <see cref="WireMockAdminApiAssertions"/> which can be used to assert the expected number of calls.
    /// </summary>
    /// <returns>A <see cref="WireMockAdminApiAssertions"/> instance for asserting the number of calls to the server.</returns>
    public WireMockAdminApiAssertions Calls()
    {
        return new WireMockAdminApiAssertions(adminApi, callsCount, chain);
    }
}
