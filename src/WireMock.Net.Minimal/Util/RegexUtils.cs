// Copyright © WireMock.Net

using System.Text.RegularExpressions;
using WireMock.Constants;
using WireMock.RegularExpressions;

namespace WireMock.Util;

internal static class RegexUtils
{
    public static Dictionary<string, string> GetNamedGroups(Regex regex, string input)
    {
        var namedGroupsDictionary = new Dictionary<string, string>();

        GroupCollection groups = regex.Match(input).Groups;
        foreach (string groupName in regex.GetGroupNames())
        {
            if (groups[groupName].Captures.Count > 0)
            {
                namedGroupsDictionary.Add(groupName, groups[groupName].Value);
            }
        }

        return namedGroupsDictionary;
    }

    public static (bool IsValid, bool Result) MatchRegex(string? pattern, string input, bool useRegexExtended = true)
    {
        if (string.IsNullOrEmpty(pattern))
        {
            return (false, false);
        }

        try
        {
            if (useRegexExtended)
            {
                var regexExtended = new RegexExtended(pattern!, RegexOptions.None, RegexConstants.DefaultTimeout);
                return (true, regexExtended.IsMatch(input));
            }

            var regex = new Regex(pattern, RegexOptions.None, RegexConstants.DefaultTimeout);
            return (true, regex.IsMatch(input));
        }
        catch
        {
            return (false, false);
        }
    }
}