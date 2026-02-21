// Copyright © WireMock.Net

using System.Diagnostics.CodeAnalysis;
using AwesomeAssertions;
using WireMock.Owin;
using WireMock.Types;

namespace WireMock.Net.Tests.Owin;

[ExcludeFromCodeCoverage]
public class HostUrlOptionsTests
{
    [Fact]
    public void GetDetails_WithHostingSchemeHttpAndPort_ShouldReturnCorrectDetails()
    {
        // Arrange
        var options = new HostUrlOptions
        {
            HostingScheme = HostingScheme.Http,
            Port = 8080
        };

        // Act
        var details = options.GetDetails();

        // Assert
        details.Should().HaveCount(1);
        var detail = details.Single();
        detail.Should().Match<HostUrlDetails>(d =>
            d.Scheme == "http" &&
            d.Host == "*" &&
            d.Port == 8080 &&
            d.IsHttps == false
        );
    }

    [Fact]
    public void GetDetails_WithHostingSchemeHttpsAndPort_ShouldReturnCorrectDetails()
    {
        // Arrange
        var options = new HostUrlOptions
        {
            HostingScheme = HostingScheme.Https,
            Port = 8081
        };

        // Act
        var details = options.GetDetails();

        // Assert
        details.Should().HaveCount(1);
        var detail = details.Single();
        detail.Should().Match<HostUrlDetails>(d =>
            d.Scheme == "https" &&
            d.Host == "*" &&
            d.Port == 8081 &&
            d.IsHttps == true
        );
    }
}