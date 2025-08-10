// Copyright © WireMock.Net

using System;
using System.Collections.Generic;

namespace WireMock.Matchers;

/// <summary>
/// GraphQLMatcher
/// </summary>
/// <inheritdoc cref="IStringMatcher"/>
public interface IGraphQLMatcher : IStringMatcher
{
    /// <summary>
    /// An optional dictionary defining the custom Scalar and the type.
    /// </summary>
    public IDictionary<string, Type>? CustomScalars { get; }
}