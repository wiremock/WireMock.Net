// Copyright © WireMock.Net

using System.Text.RegularExpressions;
using Stef.Validation;

namespace WireMock.RegularExpressions;

/// <summary>
/// Extension to the <see cref="Regex"/> object, adding support for GUID tokens for matching on.
/// </summary>
[Serializable]
internal class RegexExtended : Regex
{
    /// <inheritdoc cref="Regex"/>
    public RegexExtended(string pattern) : this(pattern, RegexOptions.None)
    {
    }

    /// <inheritdoc cref="Regex"/>
    public RegexExtended(string pattern, RegexOptions options) :
        this(pattern, options, InfiniteMatchTimeout)
    {
    }

    /// <inheritdoc cref="Regex"/>
    public RegexExtended(string pattern, RegexOptions options, TimeSpan matchTimeout) :
        base(ReplaceGuidPattern(pattern), options, matchTimeout)
    {
    }

#if !NET8_0_OR_GREATER
    /// <inheritdoc cref="Regex"/>
    protected RegexExtended(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) :
        base(info, context)
    {
    }
#endif

    // Dictionary of various Guid tokens with a corresponding regular expression pattern to use instead.
    private static readonly Dictionary<string, string> GuidTokenPatterns = new()
    {
        // Lower case format `B` Guid pattern
        { @"\guidb", @"(\{[a-z0-9]{8}-([a-z0-9]{4}-){3}[a-z0-9]{12}\})" },

        // Upper case format `B` Guid pattern
        { @"\GUIDB", @"(\{[A-Z0-9]{8}-([A-Z0-9]{4}-){3}[A-Z0-9]{12}\})" },

        // Lower case format `D` Guid pattern
        { @"\guidd", "([a-z0-9]{8}-([a-z0-9]{4}-){3}[a-z0-9]{12})" },

        // Upper case format `D` Guid pattern
        { @"\GUIDD", "([A-Z0-9]{8}-([A-Z0-9]{4}-){3}[A-Z0-9]{12})" },

        // Lower case format `N` Guid pattern
        { @"\guidn", "([a-z0-9]{32})" },

        // Upper case format `N` Guid pattern
        { @"\GUIDN", "([A-Z0-9]{32})" },

        // Lower case format `P` Guid pattern
        { @"\guidp", @"(\([a-z0-9]{8}-([a-z0-9]{4}-){3}[a-z0-9]{12}\))" },

        // Upper case format `P` Guid pattern
        { @"\GUIDP", @"(\([A-Z0-9]{8}-([A-Z0-9]{4}-){3}[A-Z0-9]{12}\))" },

        // Lower case format `X` Guid pattern
        { @"\guidx", @"(\{0x[a-f0-9]{8},0x[a-f0-9]{4},0x[a-f0-9]{4},\{(0x[a-f0-9]{2},){7}(0x[a-f0-9]{2})\}\})" },

        // Upper case format `X` Guid pattern
        { @"\GUIDX", @"(\{0x[A-F0-9]{8},0x[A-F0-9]{4},0x[A-F0-9]{4},\{(0x[A-F0-9]{2},){7}(0x[A-F0-9]{2})\}\})" },
    };

    /// <summary>
    /// Replaces all instances of valid GUID tokens with the correct regular expression to match.
    /// </summary>
    /// <param name="pattern">Pattern to replace token for.</param>
    private static string ReplaceGuidPattern(string pattern)
    {
        Guard.NotNull(pattern);

        foreach (var tokenPattern in GuidTokenPatterns)
        {
            pattern = pattern.Replace(tokenPattern.Key, tokenPattern.Value);
        }

        return pattern;
    }
}