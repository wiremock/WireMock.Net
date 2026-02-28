// Copyright © WireMock.Net

using Newtonsoft.Json.Linq;
using Stef.Validation;

namespace WireMock.Matchers.Request;

/// <summary>
/// The request body matcher.
/// </summary>
public class RequestMessageBodyMatcher<T> : IRequestMatcher
{
    private const string _name = nameof(RequestMessageBodyMatcher<T>);

    /// <summary>
    /// The body data function for type T
    /// </summary>
    public Func<T?, bool>? Func { get; }

    /// <summary>
    /// The <see cref="MatchOperator"/>
    /// </summary>
    public MatchOperator MatchOperator { get; } = MatchOperator.Or;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessageBodyMatcher"/> class.
    /// </summary>
    /// <param name="func">The function.</param>
    public RequestMessageBodyMatcher(Func<T?, bool> func)
    {
        Func = Guard.NotNull(func);
    }

    /// <inheritdoc />
    public double GetMatchingScore(IRequestMessage requestMessage, IRequestMatchResult requestMatchResult)
    {
        var (score, exception) = CalculateMatchScore(requestMessage).Expand();
        return requestMatchResult.AddScore(GetType(), score, exception);
    }

    private MatchResult CalculateMatchScore(IRequestMessage requestMessage)
    {
        if (Func != null)
        {
            if (requestMessage.BodyData?.BodyAsJson is JObject jsonObject)
            {
                try
                {
                    var bodyAsT = jsonObject.ToObject<T>();
                    return MatchResult.From(_name, MatchScores.ToScore(Func(bodyAsT)));
                }
                catch (Exception ex)
                {
                    return MatchResult.From(_name, ex);
                }
            }
        }

        return MatchResult.From(_name);
    }
}