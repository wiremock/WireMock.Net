// Copyright © WireMock.Net

using WireMock.Validators;

// ReSharper disable once CheckNamespace
namespace WireMock.Admin.Mappings;

/// <summary>
/// RequestModelBuilder
/// </summary>
public partial class RequestModelBuilder
{
    /// <summary>
    /// UsingConnect: add HTTP Method matching on `CONNECT`.
    /// </summary>
    /// <returns>The <see cref="RequestModelBuilder"/>.</returns>
    public RequestModelBuilder UsingConnect() => WithMethods("CONNECT");

    /// <summary>
    /// UsingDelete: add HTTP Method matching on `DELETE`.
    /// </summary>
    /// <returns>The <see cref="RequestModelBuilder"/>.</returns>
    public RequestModelBuilder UsingDelete() => WithMethods("DELETE");

    /// <summary>
    /// UsingGet: add HTTP Method matching on `GET`.
    /// </summary>
    /// <returns>The <see cref="RequestModelBuilder"/>.</returns>
    public RequestModelBuilder UsingGet() => WithMethods("GET");

    /// <summary>
    /// UsingHead: Add HTTP Method matching on `HEAD`.
    /// </summary>
    /// <returns>The <see cref="RequestModelBuilder"/>.</returns>
    public RequestModelBuilder UsingHead() => WithMethods("HEAD");

    /// <summary>
    /// UsingPost: add HTTP Method matching on `POST`.
    /// </summary>
    /// <returns>The <see cref="RequestModelBuilder"/>.</returns>
    public RequestModelBuilder UsingPost() => WithMethods("POST");

    /// <summary>
    /// UsingPatch: add HTTP Method matching on `PATCH`.
    /// </summary>
    /// <returns>The <see cref="RequestModelBuilder"/>.</returns>
    public RequestModelBuilder UsingPatch() => WithMethods("PATCH");

    /// <summary>
    /// UsingPut: add HTTP Method matching on `OPTIONS`.
    /// </summary>
    /// <returns>The <see cref="RequestModelBuilder"/>.</returns>
    public RequestModelBuilder UsingOptions() => WithMethods("OPTIONS");

    /// <summary>
    /// UsingPut: add HTTP Method matching on `PUT`.
    /// </summary>
    /// <returns>The <see cref="RequestModelBuilder"/>.</returns>
    public RequestModelBuilder UsingPut() => WithMethods("PUT");

    /// <summary>
    /// UsingTrace: add HTTP Method matching on `TRACE`.
    /// </summary>
    /// <returns>The <see cref="RequestModelBuilder"/>.</returns>
    public RequestModelBuilder UsingTrace() => WithMethods("TRACE");

    /// <summary>
    /// UsingAnyMethod: add HTTP Method matching on any method.
    /// </summary>
    /// <returns>The <see cref="RequestModelBuilder"/>.</returns>
    public RequestModelBuilder UsingAnyMethod() => this;

    /// <summary>
    /// Set the ClientIP.
    /// </summary>
    public RequestModelBuilder WithClientIP(string value) => WithClientIP(() => value);

    /// <summary>
    /// Set the ClientIP.
    /// </summary>
    public RequestModelBuilder WithClientIP(ClientIPModel value) => WithClientIP(() => value);

    /// <summary>
    /// Set the ClientIP.
    /// </summary>
    public RequestModelBuilder WithClientIP(Action<ClientIPModelBuilder> action)
    {
        return WithClientIP(() =>
        {
            var builder = new ClientIPModelBuilder();
            action(builder);
            return builder.Build();
        });
    }

    /// <summary>
    /// Set the Path. Must start with a forward slash (/).
    /// </summary>
    public RequestModelBuilder WithPath(string value)
    {
        PathValidator.ValidateAndThrow(value);

        return WithPath(() => value);
    }

    /// <summary>
    /// Set the Path.
    /// </summary>
    public RequestModelBuilder WithPath(PathModel value) => WithPath(() => value);

    /// <summary>
    /// Set the Path.
    /// </summary>
    public RequestModelBuilder WithPath(Action<PathModelBuilder> action)
    {
        return WithPath(() =>
        {
            var builder = new PathModelBuilder();
            action(builder);
            return builder.Build();
        });
    }

    /// <summary>
    /// Set the Url.
    /// </summary>
    public RequestModelBuilder WithUrl(string value) => WithUrl(() => value);

    /// <summary>
    /// Set the Url.
    /// </summary>
    public RequestModelBuilder WithUrl(UrlModel value) => WithUrl(() => value);

    /// <summary>
    /// Set the Url.
    /// </summary>
    public RequestModelBuilder WithUrl(Action<UrlModelBuilder> action)
    {
        return WithUrl(() =>
        {
            var builder = new UrlModelBuilder();
            action(builder);
            return builder.Build();
        });
    }

    /// <summary>
    /// WithHeader: matching based on name, pattern and matchBehaviour.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="pattern">The pattern.</param>
    /// <param name="rejectOnMatch">The match behaviour. Default value is <c>false</c>.</param>
    /// <returns>The <see cref="RequestModelBuilder"/>.</returns>
    public RequestModelBuilder WithHeader(string name, string pattern, bool rejectOnMatch = false)
    {
        return WithHeaders(headersBuilder => headersBuilder
            .Add(headerBuilder => headerBuilder
                .WithName(name)
                .WithMatchers(matchersBuilder => matchersBuilder
                    .Add(matcherBuilder => matcherBuilder
                        .WithName("WildcardMatcher")
                        .WithPattern(pattern)
                        .WithRejectOnMatch(rejectOnMatch)
                    )
                )
            )
        );
    }
}