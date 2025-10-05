// Copyright © WireMock.Net

namespace WireMock.Transformers;

internal interface ITransformerContextFactory
{
    ITransformerContext Create();
}