// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using System.Text;

namespace WireMock.Admin.Mappings;

public partial class IListCookieModelBuilder
{
    public IListCookieModelBuilder WithCookie(string name, Action<IListMatcherModelBuilder> action, bool rejectOnMatch = false)
    {
        return Add(cookieBuilder => cookieBuilder
            .WithName(name)
            .WithRejectOnMatch(rejectOnMatch)
            .WithMatchers(matchersBuilder => action(matchersBuilder))
        );
    }
}
