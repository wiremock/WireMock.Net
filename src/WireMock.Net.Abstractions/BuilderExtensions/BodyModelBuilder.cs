// Copyright © WireMock.Net

// ReSharper disable once CheckNamespace
namespace WireMock.Admin.Mappings;

/// <summary>
/// BodyModelBuilder
/// </summary>
public partial class BodyModelBuilder
{
    public BodyModelBuilder WithNotNullOrEmptyMatcher(bool rejectOnMatch = false)
    {
        return WithMatcher(mb => mb
            .WithName("NotNullOrEmptyMatcher")
            .WithRejectOnMatch(rejectOnMatch)
        );
    }

    public BodyModelBuilder WithCSharpCodeMatcher(string pattern, bool rejectOnMatch = false)
    {
        return WithMatcher("CSharpCodeMatcher", pattern, rejectOnMatch);
    }

    public BodyModelBuilder WithLinqMatcher(string pattern, bool rejectOnMatch = false)
    {
        return WithMatcher("LinqMatcher", pattern, rejectOnMatch);
    }

    public BodyModelBuilder WithExactMatcher(string pattern, bool rejectOnMatch = false)
    {
        return WithMatcher("ExactMatcher", pattern, rejectOnMatch);
    }

    public BodyModelBuilder WithExactObjectMatcher(object value, bool rejectOnMatch = false)
    {
        return WithMatcher("ExactObjectMatcher", value, rejectOnMatch);
    }

    public BodyModelBuilder WithGraphQLMatcher(string pattern, IDictionary<string, Type>? customScalars = null, bool rejectOnMatch = false)
    {
        return WithMatcher(mb => mb
            .WithName("GraphQLMatcher")
            .WithCustomScalars(customScalars)
            .WithPattern(pattern)
            .WithRejectOnMatch(rejectOnMatch)
        );
    }

    public BodyModelBuilder WithProtoBufMatcher(string pattern, bool rejectOnMatch = false)
    {
        return WithMatcher(mb => mb
            .WithName("ProtoBufMatcher")
            .WithPattern(pattern)
            .WithRejectOnMatch(rejectOnMatch)
        );
    }

    public BodyModelBuilder WithRegexMatcher(string pattern, bool ignoreCase = false, bool rejectOnMatch = false)
    {
        return WithMatcher(mb => mb
            .WithName("RegexMatcher")
            .WithPattern(pattern)
            .WithIgnoreCase(ignoreCase)
            .WithRejectOnMatch(rejectOnMatch)
        );
    }

    public BodyModelBuilder WithJsonMatcher(string pattern, bool ignoreCase = false, bool useRegex = false, bool rejectOnMatch = false)
    {
        return WithMatcher(mb => mb
            .WithName("JsonMatcher")
            .WithPattern(pattern)
            .WithIgnoreCase(ignoreCase)
            .WithRegex(useRegex)
            .WithRejectOnMatch(rejectOnMatch)
        );
    }

    public BodyModelBuilder WithJsonPartialMatcher(string pattern, bool ignoreCase = false, bool useRegex = false, bool rejectOnMatch = false)
    {
        return WithMatcher(mb => mb
            .WithName("JsonPartialMatcher")
            .WithPattern(pattern)
            .WithIgnoreCase(ignoreCase)
            .WithRegex(useRegex)
            .WithRejectOnMatch(rejectOnMatch)
        );
    }

    public BodyModelBuilder WithJsonPathMatcher(string pattern, bool rejectOnMatch = false)
    {
        return WithMatcher("JsonPathMatcher", pattern, rejectOnMatch);
    }

    public BodyModelBuilder WithJmesPathMatcher(string pattern, bool rejectOnMatch = false)
    {
        return WithMatcher("JmesPathMatcher", pattern, rejectOnMatch);
    }

    public BodyModelBuilder WithXPathMatcher(string pattern, XmlNamespace[]? xmlNamespaceMap = null, bool rejectOnMatch = false)
    {
        return WithMatcher(mb => mb
            .WithName("PathMatcher")
            .WithPattern(pattern)
            .WithXmlNamespaceMap(xmlNamespaceMap)
            .WithRejectOnMatch(rejectOnMatch)
        );
    }

    public BodyModelBuilder WithWildcardMatcher(string pattern, bool ignoreCase = false, bool rejectOnMatch = false)
    {
        return WithMatcher("WildcardMatcher", pattern, rejectOnMatch, ignoreCase);
    }

    public BodyModelBuilder WithSimMetricsMatcher(string pattern, bool ignoreCase = false, bool rejectOnMatch = false)
    {
        return WithMatcher("SimMetricsMatcher", pattern, rejectOnMatch, ignoreCase);
    }

    private BodyModelBuilder WithMatcher(string name, object pattern, bool rejectOnMatch, bool ignoreCase = false)
    {
        return WithMatcher(mb => mb
            .WithName(name)
            .WithPattern(pattern)
            .WithRejectOnMatch(rejectOnMatch)
            .WithIgnoreCase(ignoreCase)
        );
    }
}