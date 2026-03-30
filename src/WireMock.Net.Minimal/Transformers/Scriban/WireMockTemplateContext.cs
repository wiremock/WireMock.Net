// Copyright © WireMock.Net

using Scriban;
using Scriban.Runtime;
using WireMock.Types;

namespace WireMock.Transformers.Scriban;

internal class WireMockTemplateContext : TemplateContext
{
    protected override IObjectAccessor? GetMemberAccessorImpl(object target)
    {
        return target.GetType().GetGenericTypeDefinition() == typeof(WireMockList<>) ?
            new WireMockListAccessor() :
            base.GetMemberAccessorImpl(target);
    }
}