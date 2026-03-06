// Copyright © WireMock.Net

using AwesomeAssertions;

namespace WireMock.Net.Aspire.Tests;

public class WireMockServerArgumentsTests
{
    [Fact]
    public void DefaultValues_ShouldBeSetCorrectly()
    {
        // Arrange & Act
        var args = new WireMockServerArguments();

        // Assert
        args.HttpPorts.Should().BeEmpty();
        args.AdminUsername.Should().BeNull();
        args.AdminPassword.Should().BeNull();
        args.ReadStaticMappings.Should().BeFalse();
        args.WatchStaticMappings.Should().BeFalse();
        args.MappingsPath.Should().BeNull();
    }

    [Fact]
    public void HasBasicAuthentication_ShouldReturnTrue_WhenUsernameAndPasswordAreProvided()
    {
        // Arrange
        var args = new WireMockServerArguments
        {
            AdminUsername = "admin",
            AdminPassword = "password"
        };

        // Act & Assert
        args.HasBasicAuthentication.Should().BeTrue();
    }

    [Fact]
    public void HasBasicAuthentication_ShouldReturnFalse_WhenEitherUsernameOrPasswordIsNotProvided()
    {
        // Arrange
        var argsWithUsernameOnly = new WireMockServerArguments { AdminUsername = "admin" };
        var argsWithPasswordOnly = new WireMockServerArguments { AdminPassword = "password" };

        // Act & Assert
        argsWithUsernameOnly.HasBasicAuthentication.Should().BeFalse();
        argsWithPasswordOnly.HasBasicAuthentication.Should().BeFalse();
    }

    [Fact]
    public void GetArgs_WhenReadStaticMappingsIsTrue_ShouldContainReadStaticMappingsTrue()
    {
        // Arrange
        var args = new WireMockServerArguments
        {
            ReadStaticMappings = true
        };

        // Act
        var commandLineArgs = args.GetArgs();

        // Assert
        commandLineArgs.Should().ContainInOrder("--ReadStaticMappings", "true");
    }

    [Fact]
    public void GetArgs_WhenReadStaticMappingsIsFalse_ShouldNotContainReadStaticMappingsTrue()
    {
        // Arrange
        var args = new WireMockServerArguments
        {
            ReadStaticMappings = false
        };

        // Act
        var commandLineArgs = args.GetArgs();

        // Assert
        commandLineArgs.Should().NotContain("--ReadStaticMappings", "true");
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void GetArgs_WhenWithWatchStaticMappingsIsTrue_ShouldContainWatchStaticMappingsTrue(bool readStaticMappings)
    {
        // Arrange
        var args = new WireMockServerArguments
        {
            WatchStaticMappings = true,
            ReadStaticMappings = readStaticMappings
        };

        // Act
        var commandLineArgs = args.GetArgs();

        // Assert
        commandLineArgs.Should().ContainInOrder("--ReadStaticMappings", "true", "--WatchStaticMappings", "true", "--WatchStaticMappingsInSubdirectories", "true");
    }

    [Fact]
    public void GetArgs_WhenWithWatchStaticMappingsIsFalse_ShouldNotContainWatchStaticMappingsTrue()
    {
        // Arrange
        var args = new WireMockServerArguments
        {
            WatchStaticMappings = false
        };

        // Act
        var commandLineArgs = args.GetArgs();

        // Assert
        commandLineArgs.Should().NotContain("--WatchStaticMappings", "true").And.NotContain("--WatchStaticMappingsInSubdirectories", "true");
    }

    [Fact]
    public void GetArgs_ShouldIncludeAuthenticationDetails_WhenAuthenticationIsRequired()
    {
        // Arrange
        var args = new WireMockServerArguments
        {
            AdminUsername = "admin",
            AdminPassword = "password"
        };

        // Act
        var commandLineArgs = args.GetArgs();

        // Assert
        commandLineArgs.Should().Contain("--AdminUserName", "admin");
        commandLineArgs.Should().Contain("--AdminPassword", "password");
    }
}