// Copyright © WireMock.Net

using System.Linq;
using FluentAssertions;
using Moq;
using NFluent;
using WireMock.Authentication;
using WireMock.Constants;
using WireMock.Logging;
using WireMock.Owin;
using WireMock.Server;
using WireMock.Settings;
using Xunit;

namespace WireMock.Net.Tests;

public class WireMockServerSettingsTests
{
    private readonly Mock<IWireMockLogger> _loggerMock;

    public WireMockServerSettingsTests()
    {
        _loggerMock = new Mock<IWireMockLogger>();
        _loggerMock.Setup(l => l.Info(It.IsAny<string>(), It.IsAny<object[]>()));
    }

    [Fact]
    public void WireMockServer_WireMockServerSettings_StartAdminInterfaceTrue_BasicAuthenticationIsSet()
    {
        // Assign and Act
        var server = WireMockServer.Start(new WireMockServerSettings
        {
            StartAdminInterface = true,
            AdminUsername = "u",
            AdminPassword = "p"
        });

        // Assert
        var options = server.GetPrivateFieldValue<IWireMockMiddlewareOptions>("_options");
        options.AuthenticationMatcher.Should().NotBeNull().And.BeOfType<BasicAuthenticationMatcher>();
    }

    [Fact]
    public void WireMockServer_WireMockServerSettings_StartAdminInterfaceTrue_AzureADAuthenticationIsSet()
    {
        // Assign and Act
        var server = WireMockServer.Start(new WireMockServerSettings
        {
            StartAdminInterface = true,
            AdminAzureADTenant = "t",
            AdminAzureADAudience = "a"
        });

        // Assert
        var options = server.GetPrivateFieldValue<IWireMockMiddlewareOptions>("_options");
        options.AuthenticationMatcher.Should().NotBeNull().And.BeOfType<AzureADAuthenticationMatcher>();
    }

    [Fact]
    public void WireMockServer_WireMockServerSettings_StartAdminInterfaceFalse_BasicAuthenticationIsNotSet()
    {
        // Assign and Act
        var server = WireMockServer.Start(new WireMockServerSettings
        {
            StartAdminInterface = false,
            AdminUsername = "u",
            AdminPassword = "p"
        });

        // Assert
        var options = server.GetPrivateFieldValue<IWireMockMiddlewareOptions>("_options");
        Check.That(options.AuthenticationMatcher).IsNull();
    }

    [Fact]
    public void WireMockServer_WireMockServerSettings_PriorityFromAllAdminMappingsIsLow_When_StartAdminInterface_IsTrue()
    {
        // Assign and Act
        var server = WireMockServer.Start(new WireMockServerSettings
        {
            StartAdminInterface = true
        });

        // Assert
        server.Mappings.Should().NotBeNull();
        server.Mappings.Should().HaveCount(Constants.NumAdminMappings);
        server.Mappings.All(m => m.Priority == WireMockConstants.AdminPriority).Should().BeTrue();
    }

    [Fact]
    public void WireMockServer_WireMockServerSettings_ProxyAndRecordSettings_ProxyPriority_IsMinus2000000_When_StartAdminInterface_IsTrue()
    {
        // Assign and Act
        var server = WireMockServer.Start(new WireMockServerSettings
        {
            StartAdminInterface = true,
            ProxyAndRecordSettings = new ProxyAndRecordSettings
            {
                Url = "www.google.com"
            }
        });

        // Assert
        server.Mappings.Should().NotBeNull();
        server.Mappings.Should().HaveCount(Constants.NumAdminMappings + 1);

        server.Mappings.Count(m => m.Priority == WireMockConstants.AdminPriority).Should().Be(Constants.NumAdminMappings);
        server.Mappings.Count(m => m.Priority == WireMockConstants.ProxyPriority).Should().Be(1);
    }

    [Fact]
    public void WireMockServer_WireMockServerSettings_ProxyAndRecordSettings_ProxyPriority_Is0_When_StartAdminInterface_IsFalse()
    {
        // Assign and Act
        var server = WireMockServer.Start(new WireMockServerSettings
        {
            ProxyAndRecordSettings = new ProxyAndRecordSettings
            {
                Url = "www.google.com"
            }
        });

        // Assert
        var mappings = server.Mappings.ToArray();
        Check.That(mappings.Count()).IsEqualTo(1);
        Check.That(mappings[0].Priority).IsEqualTo(0);
    }

    [Fact]
    public void WireMockServer_WireMockServerSettings_AllowPartialMapping()
    {
        // Assign and Act
        var server = WireMockServer.Start(new WireMockServerSettings
        {
            Logger = _loggerMock.Object,
            AllowPartialMapping = true
        });

        // Assert
        var options = server.GetPrivateFieldValue<IWireMockMiddlewareOptions>("_options");
        Check.That(options.AllowPartialMapping).Equals(true);

        // Verify
        _loggerMock.Verify(l => l.Info(It.IsAny<string>(), It.IsAny<bool>()));
    }

    [Fact]
    public void WireMockServer_WireMockServerSettings_AllowBodyForAllHttpMethods()
    {
        // Assign and Act
        var server = WireMockServer.Start(new WireMockServerSettings
        {
            Logger = _loggerMock.Object,
            AllowBodyForAllHttpMethods = true
        });

        // Assert
        var options = server.GetPrivateFieldValue<IWireMockMiddlewareOptions>("_options");
        Check.That(options.AllowBodyForAllHttpMethods).Equals(true);

        // Verify
        _loggerMock.Verify(l => l.Info(It.Is<string>(s => s.Contains("AllowBodyForAllHttpMethods") && s.Contains("True"))));
    }

    [Fact]
    public void WireMockServer_WireMockServerSettings_AllowOnlyDefinedHttpStatusCodeInResponse()
    {
        // Assign and Act
        var server = WireMockServer.Start(new WireMockServerSettings
        {
            Logger = _loggerMock.Object,
            AllowOnlyDefinedHttpStatusCodeInResponse = true
        });

        // Assert
        var options = server.GetPrivateFieldValue<IWireMockMiddlewareOptions>("_options");
        Check.That(options.AllowOnlyDefinedHttpStatusCodeInResponse).Equals(true);

        // Verify
        _loggerMock.Verify(l => l.Info(It.Is<string>(s => s.Contains("AllowOnlyDefinedHttpStatusCodeInResponse") && s.Contains("True"))));
    }

    [Fact]
    public void WireMockServer_WireMockServerSettings_RequestLogExpirationDuration()
    {
        // Assign and Act
        var server = WireMockServer.Start(new WireMockServerSettings
        {
            Logger = _loggerMock.Object,
            RequestLogExpirationDuration = 1
        });

        // Assert
        var options = server.GetPrivateFieldValue<IWireMockMiddlewareOptions>("_options");
        Check.That(options.RequestLogExpirationDuration).IsEqualTo(1);
    }

#if NET6_0_OR_GREATER
    [Fact]
    public void WireMockServer_WireMockServerSettings_ActivityTracingOptions_AreMappedToMiddlewareOptions()
    {
        // Assign and Act
        var server = WireMockServer.Start(new WireMockServerSettings
        {
            ActivityTracingOptions = new ActivityTracingOptions
            {
                ExcludeAdminRequests = false,
                RecordRequestBody = true,
                RecordResponseBody = true,
                RecordMatchDetails = false
            }
        });

        // Assert
        var options = server.GetPrivateFieldValue<IWireMockMiddlewareOptions>("_options");
        options.ActivityTracingOptions.Should().NotBeNull();
        options.ActivityTracingOptions!.ExcludeAdminRequests.Should().BeFalse();
        options.ActivityTracingOptions.RecordRequestBody.Should().BeTrue();
        options.ActivityTracingOptions.RecordResponseBody.Should().BeTrue();
        options.ActivityTracingOptions.RecordMatchDetails.Should().BeFalse();
    }

    [Fact]
    public void WireMockServer_WireMockServerSettings_Without_ActivityTracingOptions_ShouldNotSetMiddlewareOptions()
    {
        // Assign and Act
        var server = WireMockServer.Start(new WireMockServerSettings());

        // Assert
        var options = server.GetPrivateFieldValue<IWireMockMiddlewareOptions>("_options");
        options.ActivityTracingOptions.Should().BeNull();
    }
#endif
}