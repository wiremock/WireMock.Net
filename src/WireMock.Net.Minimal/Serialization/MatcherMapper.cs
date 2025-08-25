// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using System.Linq;
using AnyOfTypes;
using SimMetrics.Net;
using Stef.Validation;
using WireMock.Admin.Mappings;
using WireMock.Extensions;
using WireMock.Matchers;
using WireMock.Models;
using WireMock.Models.GraphQL;
using WireMock.Settings;
using WireMock.Util;

namespace WireMock.Serialization;

internal class MatcherMapper
{
    private readonly WireMockServerSettings _settings;

    public MatcherMapper(WireMockServerSettings settings)
    {
        _settings = Guard.NotNull(settings);
    }

    public IMatcher[]? Map(IEnumerable<MatcherModel>? matchers)
    {
        return matchers?.Select(Map).OfType<IMatcher>().ToArray();
    }

    public IMatcher? Map(MatcherModel? matcherModel)
    {
        if (matcherModel == null)
        {
            return null;
        }

        string[] parts = matcherModel.Name.Split('.');
        string matcherName = parts[0];
        string? matcherType = parts.Length > 1 ? parts[1] : null;
        var stringPatterns = ParseStringPatterns(matcherModel);
        var matchBehaviour = matcherModel.RejectOnMatch == true ? MatchBehaviour.RejectOnMatch : MatchBehaviour.AcceptOnMatch;
        var matchOperator = StringUtils.ParseMatchOperator(matcherModel.MatchOperator);
        bool ignoreCase = matcherModel.IgnoreCase == true;
        bool useRegexExtended = _settings.UseRegexExtended == true;
        bool useRegex = matcherModel.Regex == true;

        switch (matcherName)
        {
            case nameof(NotNullOrEmptyMatcher):
                return new NotNullOrEmptyMatcher(matchBehaviour);

            case "CSharpCodeMatcher":
                if (_settings.AllowCSharpCodeMatcher == true)
                {
                    return TypeLoader.LoadNewInstance<ICSharpCodeMatcher>(matchBehaviour, matchOperator, stringPatterns);
                }

                throw new NotSupportedException("It's not allowed to use the 'CSharpCodeMatcher' because WireMockServerSettings.AllowCSharpCodeMatcher is not set to 'true'.");

            case "LinqMatcher":
                throw new NotSupportedException("It's not allowed to use the 'LinqMatcher' due to CVE.");

            //case nameof(LinqMatcher):
            //    return new LinqMatcher(matchBehaviour, matchOperator, stringPatterns);

            case nameof(ExactMatcher):
                return new ExactMatcher(matchBehaviour, ignoreCase, matchOperator, stringPatterns);

            case nameof(ExactObjectMatcher):
                return CreateExactObjectMatcher(matchBehaviour, stringPatterns[0]);

            case "GraphQLMatcher":
                var patternAsString = stringPatterns[0].GetPattern();
                var schema = new AnyOf<string, StringPattern, ISchemaData>(patternAsString);
                return TypeLoader.LoadNewInstance<IGraphQLMatcher>(schema, matcherModel.CustomScalars, matchBehaviour, matchOperator);

            case "MimePartMatcher":
                return CreateMimePartMatcher(matchBehaviour, matcherModel);

            case "ProtoBufMatcher":
                return CreateProtoBufMatcher(matchBehaviour, stringPatterns.GetPatterns(), matcherModel);

            case nameof(RegexMatcher):
                return new RegexMatcher(matchBehaviour, stringPatterns, ignoreCase, useRegexExtended, matchOperator);

            case nameof(JsonMatcher):
                var valueForJsonMatcher = matcherModel.Pattern ?? matcherModel.Patterns;
                return new JsonMatcher(matchBehaviour, valueForJsonMatcher!, ignoreCase, useRegex);

            case nameof(JsonPartialMatcher):
                var valueForJsonPartialMatcher = matcherModel.Pattern ?? matcherModel.Patterns;
                return new JsonPartialMatcher(matchBehaviour, valueForJsonPartialMatcher!, ignoreCase, useRegex);

            case nameof(JsonPartialWildcardMatcher):
                var valueForJsonPartialWildcardMatcher = matcherModel.Pattern ?? matcherModel.Patterns;
                return new JsonPartialWildcardMatcher(matchBehaviour, valueForJsonPartialWildcardMatcher!, ignoreCase, useRegex);

            case nameof(JsonPathMatcher):
                return new JsonPathMatcher(matchBehaviour, matchOperator, stringPatterns);

            case nameof(JmesPathMatcher):
                return new JmesPathMatcher(matchBehaviour, matchOperator, stringPatterns);

            case nameof(XPathMatcher):
                return new XPathMatcher(matchBehaviour, matchOperator, matcherModel.XmlNamespaceMap, stringPatterns);

            case nameof(WildcardMatcher):
                return new WildcardMatcher(matchBehaviour, stringPatterns, ignoreCase, matchOperator);

            case nameof(ContentTypeMatcher):
                return new ContentTypeMatcher(matchBehaviour, stringPatterns, ignoreCase);

            case nameof(FormUrlEncodedMatcher):
                return new FormUrlEncodedMatcher(matchBehaviour, stringPatterns, ignoreCase);

            case nameof(SimMetricsMatcher):
                SimMetricType type = SimMetricType.Levenstein;
                if (!string.IsNullOrEmpty(matcherType) && !Enum.TryParse(matcherType, out type))
                {
                    throw new NotSupportedException($"Matcher '{matcherName}' with Type '{matcherType}' is not supported.");
                }

                return new SimMetricsMatcher(matchBehaviour, stringPatterns, type);

            default:
                if (_settings.CustomMatcherMappings != null && _settings.CustomMatcherMappings.ContainsKey(matcherName))
                {
                    return _settings.CustomMatcherMappings[matcherName](matcherModel);
                }

                throw new NotSupportedException($"Matcher '{matcherName}' is not supported.");
        }
    }

    public MatcherModel[]? Map(IEnumerable<IMatcher>? matchers, Action<MatcherModel>? afterMap = null)
    {
        return matchers?.Select(m => Map(m, afterMap)).OfType<MatcherModel>().ToArray();
    }

    public MatcherModel? Map(IMatcher? matcher, Action<MatcherModel>? afterMap = null)
    {
        if (matcher == null)
        {
            return null;
        }

        bool? ignoreCase = matcher is IIgnoreCaseMatcher ignoreCaseMatcher ? ignoreCaseMatcher.IgnoreCase : null;
        bool? rejectOnMatch = matcher.MatchBehaviour == MatchBehaviour.RejectOnMatch ? true : null;

        var model = new MatcherModel
        {
            RejectOnMatch = rejectOnMatch,
            IgnoreCase = ignoreCase,
            Name = matcher.Name
        };

        switch (matcher)
        {
            case JsonMatcher jsonMatcher:
                model.Regex = jsonMatcher.Regex;
                break;

            case XPathMatcher xpathMatcher:
                model.XmlNamespaceMap = xpathMatcher.XmlNamespaceMap;
                break;

            case IGraphQLMatcher graphQLMatcher:
                model.CustomScalars = graphQLMatcher.CustomScalars;
                break;
        }

        switch (matcher)
        {
            // If the matcher is a IStringMatcher, get the operator & patterns.
            case IStringMatcher stringMatcher:
                var stringPatterns = stringMatcher.GetPatterns();
                if (stringPatterns.Length == 1)
                {
                    if (stringPatterns[0].IsFirst)
                    {
                        model.Pattern = stringPatterns[0].First;
                    }
                    else
                    {
                        model.Pattern = stringPatterns[0].Second.Pattern;
                        model.PatternAsFile = stringPatterns[0].Second.PatternAsFile;
                    }
                }
                else
                {
                    model.Patterns = stringPatterns.Select(p => p.GetPattern()).Cast<object>().ToArray();
                    model.MatchOperator = stringMatcher.MatchOperator.ToString();
                }
                break;

            // If the matcher is a IObjectMatcher, get the value (can be string or object or byte[]).
            case IObjectMatcher objectMatcher:
                model.Pattern = objectMatcher.Value;
                break;

            case IMimePartMatcher mimePartMatcher:
                model.ContentDispositionMatcher = Map(mimePartMatcher.ContentDispositionMatcher);
                model.ContentMatcher = Map(mimePartMatcher.ContentMatcher);
                model.ContentTransferEncodingMatcher = Map(mimePartMatcher.ContentTransferEncodingMatcher);
                model.ContentTypeMatcher = Map(mimePartMatcher.ContentTypeMatcher);
                break;

#if PROTOBUF
            case IProtoBufMatcher protoBufMatcher:
                protoBufMatcher.ProtoDefinition().Value(id => model.Pattern = id, texts =>
                {
                    if (texts.Count == 1)
                    {
                        model.Pattern = texts[0];
                    }
                    else if (texts.Count > 1)
                    {
                        model.Patterns = texts.Cast<object>().ToArray();
                    }
                });

                model.ProtoBufMessageType = protoBufMatcher.MessageType;
                model.ContentMatcher = Map(protoBufMatcher.Matcher);
                break;
#endif
        }

        afterMap?.Invoke(model);

        return model;
    }

    private AnyOf<string, StringPattern>[] ParseStringPatterns(MatcherModel matcher)
    {
        if (matcher.Pattern is string patternAsString)
        {
            return [new AnyOf<string, StringPattern>(patternAsString)];
        }

        if (matcher.Pattern is IEnumerable<string> patternAsStringArray)
        {
            return patternAsStringArray.ToAnyOfPatterns();
        }

        if (matcher.Patterns?.OfType<string>() is { } patternsAsStringArray)
        {
            return patternsAsStringArray.ToAnyOfPatterns();
        }

        if (!string.IsNullOrEmpty(matcher.PatternAsFile))
        {
            var patternAsFile = matcher.PatternAsFile!;
            var pattern = _settings.FileSystemHandler.ReadFileAsString(patternAsFile);
            return [new AnyOf<string, StringPattern>(new StringPattern { Pattern = pattern, PatternAsFile = patternAsFile })];
        }

        return [];
    }

    private static ExactObjectMatcher CreateExactObjectMatcher(MatchBehaviour matchBehaviour, AnyOf<string, StringPattern> stringPattern)
    {
        byte[] bytePattern;
        try
        {
            bytePattern = Convert.FromBase64String(stringPattern.GetPattern());
        }
        catch
        {
            throw new ArgumentException($"Matcher 'ExactObjectMatcher' has invalid pattern. The pattern value '{stringPattern}' is not a Base64String.", nameof(stringPattern));
        }

        return new ExactObjectMatcher(matchBehaviour, bytePattern);
    }

    private IMimePartMatcher CreateMimePartMatcher(MatchBehaviour matchBehaviour, MatcherModel matcher)
    {
        var contentTypeMatcher = Map(matcher.ContentTypeMatcher) as IStringMatcher;
        var contentDispositionMatcher = Map(matcher.ContentDispositionMatcher) as IStringMatcher;
        var contentTransferEncodingMatcher = Map(matcher.ContentTransferEncodingMatcher) as IStringMatcher;
        var contentMatcher = Map(matcher.ContentMatcher);

        return TypeLoader.LoadNewInstance<IMimePartMatcher>(matchBehaviour, contentTypeMatcher, contentDispositionMatcher, contentTransferEncodingMatcher, contentMatcher);
    }

    private IProtoBufMatcher CreateProtoBufMatcher(MatchBehaviour? matchBehaviour, IReadOnlyList<string> protoDefinitions, MatcherModel matcher)
    {
        var objectMatcher = Map(matcher.ContentMatcher) as IObjectMatcher;

        return TypeLoader.LoadNewInstance<IProtoBufMatcher>(
            () => ProtoDefinitionUtils.GetIdOrTexts(_settings, protoDefinitions.ToArray()),
            matcher.ProtoBufMessageType!,
            matchBehaviour ?? MatchBehaviour.AcceptOnMatch,
            objectMatcher
        );
    }
}