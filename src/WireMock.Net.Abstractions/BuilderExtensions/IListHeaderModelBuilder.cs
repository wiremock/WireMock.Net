// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using System.Text;

namespace WireMock.Admin.Mappings;

public partial class IListHeaderModelBuilder
{
    public IListHeaderModelBuilder WithHeader(string name, Action<IListMatcherModelBuilder> action, bool rejectOnMatch = false)
    {
        return Add(headerBuilder => headerBuilder
            .WithName(name)
            .WithRejectOnMatch(rejectOnMatch)
            .WithMatchers(matchersBuilder => action(matchersBuilder))
        );
    }
}
