// Copyright © WireMock.Net

using System;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Text.RegularExpressions;
using AnyOfTypes;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Stef.Validation;
using WireMock.Constants;
using WireMock.Matchers;
using WireMock.Models;

namespace WireMock.Authentication;

/// <summary>
/// https://www.c-sharpcorner.com/article/how-to-validate-azure-ad-token-using-console-application/
/// https://stackoverflow.com/questions/38684865/validation-of-an-azure-ad-bearer-token-in-a-console-application
/// https://github.com/AzureAD/microsoft-identity-web/blob/36fb5f555638787823a89e89c67f17d6a10006ed/tools/CrossPlatformValidator/CrossPlatformValidation/CrossPlatformValidation/RequestValidator.cs#L42
/// </summary>
internal class AzureADAuthenticationMatcher : IStringMatcher
{
    private const string BearerPrefix = "Bearer ";
    private static readonly Regex ExtractTenantIdRegex = new(@"https:\/\/(?:sts\.windows\.net|login\.microsoftonline\.com)\/([a-z0-9-]{36}|[a-zA-Z0-9\.]+)/", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;
    private readonly IConfigurationManager<OpenIdConnectConfiguration> _configurationManager;
    private readonly string _tenant;
    private readonly string _audience;

    public AzureADAuthenticationMatcher(JwtSecurityTokenHandler jwtSecurityTokenHandler, IConfigurationManager<OpenIdConnectConfiguration> configurationManager, string tenant, string audience)
    {
        _jwtSecurityTokenHandler = Guard.NotNull(jwtSecurityTokenHandler);
        _configurationManager = Guard.NotNull(configurationManager);
        _audience = Guard.NotNullOrEmpty(audience);
        _tenant = Guard.NotNullOrEmpty(tenant);
    }

    public string Name => nameof(AzureADAuthenticationMatcher);

    public MatchBehaviour MatchBehaviour => MatchBehaviour.AcceptOnMatch;

    public AnyOf<string, StringPattern>[] GetPatterns()
    {
        return [];
    }

    public MatchOperator MatchOperator => MatchOperator.Or;

    public MatchResult IsMatch(string? input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return MatchResult.From(Name);
        }

        var token = Regex.Replace(input, BearerPrefix, string.Empty, RegexOptions.IgnoreCase, RegexConstants.DefaultTimeout);

        try
        {
            var config = _configurationManager.GetConfigurationAsync(default).ConfigureAwait(false).GetAwaiter().GetResult();

            var validationParameters = new TokenValidationParameters
            {
                ValidAudience = _audience,
                ValidIssuer = config.Issuer,
                IssuerValidator = (issuer, _, _) =>
                {
                    if (TryExtractTenantId(issuer, out var extractedTenant) && string.Equals(extractedTenant, _tenant, StringComparison.OrdinalIgnoreCase))
                    {
                        return issuer;
                    }

                    throw new SecurityTokenInvalidIssuerException($"tenant {extractedTenant} does not match {_tenant}.");
                },
                IssuerSigningKeys = config.SigningKeys,
                ValidateLifetime = true
            };

            // Throws an Exception as the token is invalid (expired, invalid-formatted, tenant mismatch, etc.)
            _jwtSecurityTokenHandler.ValidateToken(token, validationParameters, out _);

            return MatchResult.From(Name, MatchScores.Perfect);
        }
        catch (Exception ex)
        {
            return MatchResult.From(Name, ex);
        }
    }

    /// <inheritdoc />
    public virtual string GetCSharpCodeArguments()
    {
        throw new NotImplementedException();
    }

    // Handles: https://sts.windows.net/{tenant}/, https://login.microsoftonline.com/{tenant}/ or /v2.0
    private static bool TryExtractTenantId(string issuer, [NotNullWhen(true)] out string? tenant)
    {
        var match = ExtractTenantIdRegex.Match(issuer);

        if (match is { Success: true, Groups.Count: > 1 })
        {
            tenant = match.Groups[1].Value;
            return !string.IsNullOrEmpty(tenant);
        }

        tenant = null;
        return false;
    }
}