// Copyright © WireMock.Net

using WireMock.Matchers;
using WireMock.Owin;
using WireMock.Server;

namespace WireMock.Net.Tests
{
    public class WireMockServerAuthenticationTests
    {
        [Fact]
        public void WireMockServer_Authentication_SetBasicAuthentication()
        {
            // Assign
            var server = WireMockServer.Start();

            // Act
            server.SetBasicAuthentication("x", "y");

            // Assert
            var options = server.GetPrivateFieldValue<IWireMockMiddlewareOptions>("_options");
            options.AuthenticationMatcher.Name.Should().Be("BasicAuthenticationMatcher");
            options.AuthenticationMatcher.MatchBehaviour.Should().Be(MatchBehaviour.AcceptOnMatch);
            options.AuthenticationMatcher.GetPatterns().Should().ContainSingle("^(?i)BASIC eDp5$");

            server.Stop();
        }

        [Fact]
        public void WireMockServer_Authentication_SetSetAzureADAuthentication()
        {
            // Assign
            var server = WireMockServer.Start();

            // Act
            server.SetAzureADAuthentication("x", "y");

            // Assert
            var options = server.GetPrivateFieldValue<IWireMockMiddlewareOptions>("_options");
            options.AuthenticationMatcher.Name.Should().Be("AzureADAuthenticationMatcher");
            options.AuthenticationMatcher.MatchBehaviour.Should().Be(MatchBehaviour.AcceptOnMatch);

            server.Stop();
        }

        [Fact]
        public void WireMockServer_Authentication_RemoveAuthentication()
        {
            // Assign
            var server = WireMockServer.Start();
            server.SetBasicAuthentication("x", "y");

            // Act
            server.RemoveAuthentication();

            // Assert
            var options = server.GetPrivateFieldValue<IWireMockMiddlewareOptions>("_options");
            options.AuthenticationMatcher.Should().BeNull();

            server.Stop();
        }
    }
}

