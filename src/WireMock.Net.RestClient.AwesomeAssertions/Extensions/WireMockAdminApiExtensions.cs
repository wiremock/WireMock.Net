// Copyright © WireMock.Net

// ReSharper disable once CheckNamespace
namespace WireMock.Client.AwesomeAssertions;

/// <summary>
/// Contains extension methods for custom assertions in unit tests.
/// </summary>
public static class WireMockAdminApiExtensions
{
    /// <summary>
    /// Returns a <see cref="WireMockAdminApiReceivedAssertions"/> object that can be used to assert the current <see cref="IWireMockAdminApi"/>.
    /// </summary>
    /// <param name="instance">The WireMock Admin API client.</param>
    /// <returns><see cref="WireMockAdminApiReceivedAssertions"/></returns>
    public static WireMockAdminApiReceivedAssertions Should(this IWireMockAdminApi instance)
    {
        return new WireMockAdminApiReceivedAssertions(instance, AssertionChain.GetOrCreate());
    }
}
