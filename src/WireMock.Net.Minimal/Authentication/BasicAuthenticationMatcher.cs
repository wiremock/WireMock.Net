// Copyright © WireMock.Net

using System.Text;
using WireMock.Matchers;

namespace WireMock.Authentication;

internal class BasicAuthenticationMatcher(string username, string password)
    : RegexMatcher(BuildPattern(username, password))
{
    public override string Name => nameof(BasicAuthenticationMatcher);

    private static string BuildPattern(string username, string password)
    {
        return "^(?i)BASIC " + Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password)) + "$";
    }
}