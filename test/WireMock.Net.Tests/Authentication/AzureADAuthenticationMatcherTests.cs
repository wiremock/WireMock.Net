// Copyright © WireMock.Net

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Moq;
using WireMock.Authentication;

namespace WireMock.Net.Tests.Authentication;

public class AzureADAuthenticationMatcherTests
{
    public enum AzureADTokenVersion
    {
        V1,
        V2
    }

    private const string Tenant = "test-tenant-id";
    private const string Audience = "test-audience";
    private static readonly Dictionary<AzureADTokenVersion, string> IssuerUrlTemplates = new()
    {
        { AzureADTokenVersion.V1, "https://sts.windows.net/{0}/" },
        { AzureADTokenVersion.V2, "https://login.microsoftonline.com/{0}/v2.0" }
    };
    private readonly Mock<IConfigurationManager<OpenIdConnectConfiguration>> _openIdConnectConfigurationManagerMock = new();

    private readonly AzureADAuthenticationMatcher _sut;

    public AzureADAuthenticationMatcherTests()
    {
        var jwtSecurityTokenHandler = new MockJwtSecurityTokenHandler();
        _openIdConnectConfigurationManagerMock.Setup(c => c.GetConfigurationAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new OpenIdConnectConfiguration());
        
        _sut = new(jwtSecurityTokenHandler, _openIdConnectConfigurationManagerMock.Object, Tenant, Audience);
    }

    [Fact]
    public void AzureADAuthenticationMatcher_Name_ShouldReturnCorrectName()
    {
        // Act
        var name = _sut.Name;

        // Assert
        Assert.Equal("AzureADAuthenticationMatcher", name);
    }

    [Fact]
    public void AzureADAuthenticationMatcher_GetPatterns_ShouldReturnEmptyPatterns()
    {
        // Act
        var patterns = _sut.GetPatterns();

        // Assert
        Assert.NotNull(patterns);
        Assert.Empty(patterns);
    }

    [Fact]
    public void AzureADAuthenticationMatcher_IsMatch_ShouldReturnMismatch_WhenTokenIsInvalid()
    {
        // Arrange
        var sut = new AzureADAuthenticationMatcher(new JwtSecurityTokenHandler(), _openIdConnectConfigurationManagerMock.Object, Tenant, Audience);
        var invalidToken = "invalid-token";

        // Act
        var result = sut.IsMatch($"Bearer {invalidToken}");

        // Assert
        Assert.Equal(0.0, result.Score);
        Assert.NotNull(result.Exception);
    }

    [Fact]
    public void AzureADAuthenticationMatcher_IsMatch_ShouldReturnMismatch_WhenTokenIsNullOrEmpty()
    {
        // Act
        var result = _sut.IsMatch(null);

        // Assert
        Assert.Equal(0.0, result.Score);
        Assert.Null(result.Exception);
    }

    [Theory]
    [InlineData(AzureADTokenVersion.V1)]
    [InlineData(AzureADTokenVersion.V2)]
    public void AzureADAuthenticationMatcher_IsMatch_ShouldReturnPerfect_WhenTokenIsValid(AzureADTokenVersion version)
    {
        // Arrange
        var token = GenerateValidToken(Tenant, Audience, version);

        // Act
        var result = _sut.IsMatch($"Bearer {token}");

        // Assert
        Assert.Equal(1.0, result.Score);
        Assert.Null(result.Exception);
    }

    [Theory]
    [InlineData(AzureADTokenVersion.V1)]
    [InlineData(AzureADTokenVersion.V2)]
    public void AzureADAuthenticationMatcher_IsMatch_ShouldReturnMismatch_WhenTenantMismatch(AzureADTokenVersion version)
    {
        // Arrange
        var sut = new AzureADAuthenticationMatcher(new JwtSecurityTokenHandler(), _openIdConnectConfigurationManagerMock.Object, Tenant, Audience);
        var token = GenerateValidToken("different-tenant", Audience, version);

        // Act
        var result = sut.IsMatch($"Bearer {token}");

        // Assert
        Assert.Equal(0.0, result.Score);
        Assert.NotNull(result.Exception);
    }

    private static string GenerateValidToken(string tenant, string audience, AzureADTokenVersion version)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes($"test-signing-key-{Guid.NewGuid()}"));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, "test-user"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("tid", tenant)
        };

        var issuer = string.Format(IssuerUrlTemplates[version], tenant);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}