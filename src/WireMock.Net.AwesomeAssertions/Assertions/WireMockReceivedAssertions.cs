// Copyright © WireMock.Net

using AwesomeAssertions.Primitives;
using WireMock.Server;

// ReSharper disable once CheckNamespace
namespace WireMock.AwesomeAssertions;

/// <summary>
/// Contains a number of methods to assert that the <see cref="IWireMockServer"/> is in the expected state.
/// </summary>
/// <remarks>
/// Create a WireMockReceivedAssertions.
/// </remarks>
/// <param name="server">The <see cref="IWireMockServer"/>.</param>
/// <param name="chain">The assertion chain</param>
public class WireMockReceivedAssertions(IWireMockServer server, AssertionChain chain) : ReferenceTypeAssertions<IWireMockServer, WireMockReceivedAssertions>(server, chain)
{

    /// <summary>
    /// Asserts if <see cref="IWireMockServer"/> has received no calls.
    /// </summary>
    /// <returns><see cref="WireMockAssertions"/></returns>
    public WireMockAssertions HaveReceivedNoCalls()
    {
        return new WireMockAssertions(Subject, 0, CurrentAssertionChain);
    }

    /// <summary>
    /// Asserts if <see cref="IWireMockServer"/> has received a call.
    /// </summary>
    /// <returns><see cref="WireMockAssertions"/></returns>
    public WireMockAssertions HaveReceivedACall()
    {
        return new WireMockAssertions(Subject, null, CurrentAssertionChain);
    }

    /// <summary>
    /// Asserts if <see cref="IWireMockServer"/> has received n-calls.
    /// </summary>
    /// <param name="callsCount"></param>
    /// <returns><see cref="WireMockANumberOfCallsAssertions"/></returns>
    public WireMockANumberOfCallsAssertions HaveReceived(int callsCount)
    {
        return new WireMockANumberOfCallsAssertions(Subject, callsCount, CurrentAssertionChain);
    }

    /// <inheritdoc />
    protected override string Identifier => "wiremockserver";
}