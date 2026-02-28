// Copyright © WireMock.Net

namespace WireMock.Util;

internal interface IGuidUtils
{
    Guid NewGuid();
}

internal class GuidUtils : IGuidUtils
{
    public Guid NewGuid()
    {
        return Guid.NewGuid();
    }
}