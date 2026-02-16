// Copyright © WireMock.Net

namespace WireMock.Admin.Mappings;

public partial class ArrayMatcherModelBuilder
{
    public ArrayMatcherModelBuilder WithExactMatcher(object pattern, bool rejectOnMatch = false, bool ignoreCase = false)
    {
        return WithMatcher("ExactMatcher", pattern, rejectOnMatch, ignoreCase);
    }

    public ArrayMatcherModelBuilder WithWildcardMatcher(object pattern, bool rejectOnMatch = false, bool ignoreCase = false)
    {
        return WithMatcher("WildcardMatcher", pattern, rejectOnMatch, ignoreCase);
    }

    public ArrayMatcherModelBuilder WithRegexMatcher(object pattern, bool rejectOnMatch = false, bool ignoreCase = false)
    {
        return WithMatcher("RegexMatcher", pattern, rejectOnMatch, ignoreCase);
    }

    public ArrayMatcherModelBuilder WithNotNullOrEmptyMatcher(bool rejectOnMatch = false)
    {
        return Add(mb => mb
            .WithName("NotNullOrEmptyMatcher")
            .WithRejectOnMatch(rejectOnMatch)
        );
    }

    private ArrayMatcherModelBuilder WithMatcher(string name, object pattern, bool rejectOnMatch, bool ignoreCase = false)
    {
        return Add(mb => mb
            .WithName(name)
            .WithPattern(pattern)
            .WithRejectOnMatch(rejectOnMatch)
            .WithIgnoreCase(ignoreCase)
        );
    }
}
