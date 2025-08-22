// Copyright © WireMock.Net

using AwesomeAssertions.Primitives;
using WireMock.Server;

// ReSharper disable once CheckNamespace
namespace WireMock.AwesomeAssertions;

/// <summary>
/// Contains a number of methods to assert that the <see cref="IWireMockServer"/> is in the expected state.
/// </summary>
public class WireMockReceivedAssertions : ReferenceTypeAssertions<IWireMockServer, WireMockReceivedAssertions>
{
    private readonly AssertionChain _chain;

    /// <summary>
    /// Create a WireMockReceivedAssertions.
    /// </summary>
    /// <param name="server">The <see cref="IWireMockServer"/>.</param>
    /// <param name="chain">The assertion chain</param>
    public WireMockReceivedAssertions(IWireMockServer server, AssertionChain chain) : base(server, chain)
    {
        _chain = chain;
    }

    /// <summary>
    /// Asserts if <see cref="IWireMockServer"/> has received no calls.
    /// </summary>
    /// <returns><see cref="WireMockAssertions"/></returns>
    public WireMockAssertions HaveReceivedNoCalls()
    {
        return new WireMockAssertions(Subject, 0, _chain);
    }

    /// <summary>
    /// Asserts if <see cref="IWireMockServer"/> has received a call.
    /// </summary>
    /// <returns><see cref="WireMockAssertions"/></returns>
    public WireMockAssertions HaveReceivedACall()
    {
        return new WireMockAssertions(Subject, null, _chain);
    }

    /// <summary>
    /// Asserts if <see cref="IWireMockServer"/> has received n-calls.
    /// </summary>
    /// <param name="callsCount"></param>
    /// <returns><see cref="WireMockANumberOfCallsAssertions"/></returns>
    public WireMockANumberOfCallsAssertions HaveReceived(int callsCount)
    {
        return new WireMockANumberOfCallsAssertions(Subject, callsCount, _chain);
    }

    /// <inheritdoc />
    protected override string Identifier => "wiremockserver";
}