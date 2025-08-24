// Copyright © WireMock.Net

using System;
using Stef.Validation;
using WireMock.Matchers;

namespace WireMock.Settings;

/// <summary>
/// Represents settings for saving a proxy mapping with a non-nullable value and a specific match behaviour.
/// </summary>
/// <typeparam name="T">The non-nullable type of the value associated with the proxy save mapping setting.</typeparam>
public class ProxySaveMappingSetting<T> where T : notnull
{
    /// <summary>
    /// Gets the match behaviour for the proxy save mapping setting.
    /// </summary>
    /// <value>The match behaviour which determines how matches are evaluated.</value>
    public MatchBehaviour MatchBehaviour { get; }

    /// <summary>
    /// Gets the non-nullable value associated with the proxy save mapping setting.
    /// </summary>
    /// <value>The value of type <typeparamref name="T"/>.</value>
    public T Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProxySaveMappingSetting{T}"/> class with specified non-nullable value and match behaviour.
    /// </summary>
    /// <param name="value">The non-nullable value of type <typeparamref name="T"/>.</param>
    /// <param name="matchBehaviour">The match behaviour (optional, default is <c>MatchBehaviour.AcceptOnMatch</c>.</param>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="value"/> is null.</exception>
    public ProxySaveMappingSetting(T value, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        Value = Guard.NotNull(value);
        MatchBehaviour = matchBehaviour;
    }

    /// <summary>
    /// Converts a non-nullable value of type <typeparamref name="T"/> to a <see cref="ProxySaveMappingSetting{T}"/> implicitly.
    /// </summary>
    /// <param name="value">The non-nullable value to convert.</param>
    public static implicit operator ProxySaveMappingSetting<T>(T value) => new(value);

    /// <summary>
    /// Converts a <see cref="ProxySaveMappingSetting{T}"/> to its underlying non-nullable value of type <typeparamref name="T"/> implicitly.
    /// </summary>
    /// <param name="instance">The <see cref="ProxySaveMappingSetting{T}"/> to convert.</param>
    public static implicit operator T(ProxySaveMappingSetting<T> instance) => instance.Value;
}
