// Copyright © WireMock.Net and mock4net by Alexandre Victoor

// This source file is based on mock4net by Alexandre Victoor which is licensed under the Apache 2.0 License.
// For more details see 'mock4net/LICENSE.txt' and 'mock4net/readme.md' in this project root.
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using Stef.Validation;
using WireMock.Matchers;
using WireMock.Matchers.Request;

namespace WireMock.RequestBuilders;

/// <summary>
/// The Request Builder
/// </summary>
public partial class Request : RequestMessageCompositeMatcher, IRequestBuilder
{
    private readonly IList<IRequestMatcher> _requestMatchers;

    /// <inheritdoc />
    public IMapping Mapping { get; set; } = null!;

    /// <summary>
    /// Creates this instance.
    /// </summary>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    public static IRequestBuilder Create()
    {
        return new Request([]);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Request"/> class.
    /// </summary>
    /// <param name="requestMatchers">The request matchers.</param>
    private Request(IList<IRequestMatcher> requestMatchers) : base(requestMatchers)
    {
        _requestMatchers = Guard.NotNull(requestMatchers);
    }

    /// <summary>
    /// Gets the request message matchers.
    /// </summary>
    /// <typeparam name="T">Type of IRequestMatcher</typeparam>
    /// <returns>A List{T}</returns>
    public IList<T> GetRequestMessageMatchers<T>() where T : IRequestMatcher
    {
        return new ReadOnlyCollection<T>(_requestMatchers.OfType<T>().ToList());
    }

    /// <summary>
    /// Gets the request message matcher.
    /// </summary>
    /// <typeparam name="T">Type of IRequestMatcher</typeparam>
    /// <returns>A RequestMatcher</returns>
    public T? GetRequestMessageMatcher<T>() where T : IRequestMatcher
    {
        return _requestMatchers.OfType<T>().FirstOrDefault();
    }

    /// <summary>
    /// Gets the request message matcher.
    /// </summary>
    /// <typeparam name="T">Type of IRequestMatcher</typeparam>
    /// <returns>A RequestMatcher</returns>
    public T? GetRequestMessageMatcher<T>(Func<T, bool> func) where T : IRequestMatcher
    {
        return _requestMatchers.OfType<T>().FirstOrDefault(func);
    }

    /// <inheritdoc />
    public IRequestBuilder Add<T>(T requestMatcher) where T : IRequestMatcher
    {
        foreach (var existing in _requestMatchers.OfType<T>().ToArray())
        {
            _requestMatchers.Remove(existing);
        }

        _requestMatchers.Add(requestMatcher);
        return this;
    }

    internal bool TryGetProtoBufMatcher([NotNullWhen(true)] out IProtoBufMatcher? protoBufMatcher)
    {
        protoBufMatcher = GetRequestMessageMatcher<RequestMessageProtoBufMatcher>()?.Matcher;
        if (protoBufMatcher != null)
        {
            return true;
        }

        var bodyMatcher = GetRequestMessageMatcher<RequestMessageBodyMatcher>();
        protoBufMatcher = bodyMatcher?.Matchers?.OfType<IProtoBufMatcher>().FirstOrDefault();
        return protoBufMatcher != null;
    }
}