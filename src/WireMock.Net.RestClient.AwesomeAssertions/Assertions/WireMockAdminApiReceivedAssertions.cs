// Copyright © WireMock.Net

using AwesomeAssertions.Primitives;

// ReSharper disable once CheckNamespace
namespace WireMock.Client.AwesomeAssertions;

/// <summary>
/// Contains a number of methods to assert that the <see cref="IWireMockAdminApi"/> is in the expected state.
/// </summary>
/// <remarks>
/// Create a WireMockReceivedAssertions.
/// </remarks>
/// <param name="adminApi">The <see cref="IWireMockAdminApi"/>.</param>
/// <param name="chain">The assertion chain</param>
public class WireMockAdminApiReceivedAssertions(IWireMockAdminApi adminApi, AssertionChain chain) :
    ReferenceTypeAssertions<IWireMockAdminApi, WireMockAdminApiReceivedAssertions>(adminApi, chain)
{

    /// <summary>
    /// Asserts if <see cref="IWireMockAdminApi"/> has received no calls.
    /// </summary>
    /// <returns><see cref="WireMockAdminApiAssertions"/></returns>
    public WireMockAdminApiAssertions HaveReceivedNoCalls()
    {
        return new WireMockAdminApiAssertions(Subject, 0, CurrentAssertionChain);
    }

    /// <summary>
    /// Asserts if <see cref="IWireMockAdminApi"/> has received a call.
    /// </summary>
    /// <returns><see cref="WireMockAdminApiAssertions"/></returns>
    public WireMockAdminApiAssertions HaveReceivedACall()
    {
        return new WireMockAdminApiAssertions(Subject, null, CurrentAssertionChain);
    }

    /// <summary>
    /// Asserts if <see cref="IWireMockAdminApi"/> has received n-calls.
    /// </summary>
    /// <param name="callsCount"></param>
    /// <returns><see cref="WireMockAdminApiANumberOfCallsAssertions"/></returns>
    public WireMockAdminApiANumberOfCallsAssertions HaveReceived(int callsCount)
    {
        return new WireMockAdminApiANumberOfCallsAssertions(Subject, callsCount, CurrentAssertionChain);
    }

    /// <inheritdoc />
    protected override string Identifier => "wiremockadminapi";
}
