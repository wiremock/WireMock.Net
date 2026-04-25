// Copyright © WireMock.Net

using HandlebarsDotNet;
using WireMock.Transformers;

namespace WireMock.Transformers.Handlebars;

internal interface IHandlebarsContext : ITransformerContext
{
    IHandlebars Handlebars { get; }
}