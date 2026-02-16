// Copyright © WireMock.Net

using System;

namespace WireMock.Admin.Mappings;

public partial class IListParamModelBuilder
{
    public IListParamModelBuilder WithParam(string name, Action<ArrayMatcherModelBuilder> action, bool rejectOnMatch = false)
    {
        return Add(paramBuilder => paramBuilder
            .WithName(name)
            .WithRejectOnMatch(rejectOnMatch)
            .WithMatchers(matchersBuilder => action(matchersBuilder))
        );
    }
}