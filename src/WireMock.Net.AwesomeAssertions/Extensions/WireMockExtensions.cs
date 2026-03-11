// Copyright © WireMock.Net

using WireMock.Server;

// ReSharper disable once CheckNamespace
namespace WireMock.AwesomeAssertions;

/// <summary>
/// Contains extension methods for custom assertions in unit tests.
/// </summary>
public static class WireMockExtensions
{
    /// <summary>
    /// Returns a <see cref="WireMockReceivedAssertions"/> object that can be used to assert the current <see cref="IWireMockServer"/>.
    /// </summary>
    /// <param name="instance">The WireMockServer</param>
    /// <returns><see cref="WireMockReceivedAssertions"/></returns>
    public static WireMockReceivedAssertions Should(this IWireMockServer instance)
    {
        return new WireMockReceivedAssertions(instance, AssertionChain.GetOrCreate());
    }
}