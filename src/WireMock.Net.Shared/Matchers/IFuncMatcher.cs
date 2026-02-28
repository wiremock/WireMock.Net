// Copyright © WireMock.Net

namespace WireMock.Matchers;

/// <summary>
/// IFuncMatcher
/// </summary>
/// <inheritdoc cref="IMatcher"/>
public interface IFuncMatcher : IMatcher
{
    /// <summary>
    /// Determines whether the specified function is match.
    /// </summary>
    /// <param name="value">The value to check for a match.</param>
    /// <returns>MatchResult</returns>
    MatchResult IsMatch(object? value);
}