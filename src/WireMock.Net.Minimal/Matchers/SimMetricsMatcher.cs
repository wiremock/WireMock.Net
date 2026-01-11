// Copyright © WireMock.Net

using System.Linq;
using AnyOfTypes;
using SimMetrics.Net;
using SimMetrics.Net.API;
using SimMetrics.Net.Metric;
using Stef.Validation;
using WireMock.Extensions;
using WireMock.Models;
using WireMock.Util;

namespace WireMock.Matchers;

/// <summary>
/// SimMetricsMatcher
/// </summary>
/// <seealso cref="IStringMatcher" />
public class SimMetricsMatcher : IStringMatcher
{
    private readonly AnyOf<string, StringPattern>[] _patterns;
    private readonly SimMetricType _simMetricType;

    /// <inheritdoc />
    public MatchBehaviour MatchBehaviour { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SimMetricsMatcher"/> class.
    /// </summary>
    /// <param name="pattern">The pattern.</param>
    /// <param name="simMetricType">The SimMetric Type</param>
    public SimMetricsMatcher(AnyOf<string, StringPattern> pattern, SimMetricType simMetricType = SimMetricType.Levenstein) : this(new[] { pattern }, simMetricType)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SimMetricsMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="pattern">The pattern.</param>
    /// <param name="simMetricType">The SimMetric Type</param>
    public SimMetricsMatcher(MatchBehaviour matchBehaviour, AnyOf<string, StringPattern> pattern, SimMetricType simMetricType = SimMetricType.Levenstein) : this(matchBehaviour, new[] { pattern }, simMetricType)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SimMetricsMatcher"/> class.
    /// </summary>
    /// <param name="patterns">The patterns.</param>
    /// <param name="simMetricType">The SimMetric Type</param>
    public SimMetricsMatcher(string[] patterns, SimMetricType simMetricType = SimMetricType.Levenstein) : this(MatchBehaviour.AcceptOnMatch, patterns.ToAnyOfPatterns(), simMetricType)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SimMetricsMatcher"/> class.
    /// </summary>
    /// <param name="patterns">The patterns.</param>
    /// <param name="simMetricType">The SimMetric Type</param>
    public SimMetricsMatcher(AnyOf<string, StringPattern>[] patterns, SimMetricType simMetricType = SimMetricType.Levenstein) : this(MatchBehaviour.AcceptOnMatch, patterns, simMetricType)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SimMetricsMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="patterns">The patterns.</param>
    /// <param name="simMetricType">The SimMetric Type</param>
    /// <param name="matchOperator">The <see cref="Matchers.MatchOperator"/> to use. (default = "Or")</param>
    public SimMetricsMatcher(
        MatchBehaviour matchBehaviour,
        AnyOf<string, StringPattern>[] patterns,
        SimMetricType simMetricType = SimMetricType.Levenstein,
        MatchOperator matchOperator = MatchOperator.Average)
    {
        _patterns = Guard.NotNull(patterns);
        _simMetricType = simMetricType;
        MatchBehaviour = matchBehaviour;
        MatchOperator = matchOperator;
    }

    /// <inheritdoc />
    public MatchResult IsMatch(string? input)
    {
        IStringMetric stringMetricType = GetStringMetricType();

        var score = MatchScores.ToScore(_patterns.Select(p => stringMetricType.GetSimilarity(p.GetPattern(), input)).ToArray(), MatchOperator);
        return MatchResult.From(Name, MatchBehaviourHelper.Convert(MatchBehaviour, score));
    }

    /// <inheritdoc />
    public virtual string GetCSharpCodeArguments()
    {
        return $"new {Name}" +
               $"(" +
               $"{MatchBehaviour.GetFullyQualifiedEnumValue()}, " +
               $"{MappingConverterUtils.ToCSharpCodeArguments(_patterns)}, " +
               $"{_simMetricType.GetFullyQualifiedEnumValue()}, " +
               $"{MatchOperator.GetFullyQualifiedEnumValue()}" +
               $")";
    }

    private IStringMetric GetStringMetricType()
    {
        return _simMetricType switch
        {
            SimMetricType.BlockDistance => new BlockDistance(),
            SimMetricType.ChapmanLengthDeviation => new ChapmanLengthDeviation(),
            SimMetricType.CosineSimilarity => new CosineSimilarity(),
            SimMetricType.DiceSimilarity => new DiceSimilarity(),
            SimMetricType.EuclideanDistance => new EuclideanDistance(),
            SimMetricType.JaccardSimilarity => new JaccardSimilarity(),
            SimMetricType.Jaro => new Jaro(),
            SimMetricType.JaroWinkler => new JaroWinkler(),
            SimMetricType.MatchingCoefficient => new MatchingCoefficient(),
            SimMetricType.MongeElkan => new MongeElkan(),
            SimMetricType.NeedlemanWunch => new NeedlemanWunch(),
            SimMetricType.OverlapCoefficient => new OverlapCoefficient(),
            SimMetricType.QGramsDistance => new QGramsDistance(),
            SimMetricType.SmithWaterman => new SmithWaterman(),
            SimMetricType.SmithWatermanGotoh => new SmithWatermanGotoh(),
            SimMetricType.SmithWatermanGotohWindowedAffine => new SmithWatermanGotohWindowedAffine(),
            SimMetricType.ChapmanMeanLength => new ChapmanMeanLength(),
            _ => new Levenstein()
        };
    }

    /// <inheritdoc />
    public AnyOf<string, StringPattern>[] GetPatterns()
    {
        return _patterns;
    }

    /// <inheritdoc />
    public MatchOperator MatchOperator { get; } = MatchOperator.Average;

    /// <inheritdoc />
    public string Name => $"SimMetricsMatcher.{_simMetricType}";
}