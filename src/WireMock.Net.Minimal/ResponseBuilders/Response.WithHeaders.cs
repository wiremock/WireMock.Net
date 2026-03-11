// Copyright © WireMock.Net

using System.Linq;
using Stef.Validation;
using WireMock.Types;

namespace WireMock.ResponseBuilders;

public partial class Response
{
    /// <inheritdoc />
    public IResponseBuilder WithHeader(string name, params string[] values)
    {
        Guard.NotNull(name);

        ResponseMessage.AddHeader(name, values);
        return this;
    }

    /// <inheritdoc />
    public IResponseBuilder WithHeaders(IDictionary<string, string> headers)
    {
        Guard.NotNull(headers);

        ResponseMessage.Headers = headers.ToDictionary(header => header.Key, header => new WireMockList<string>(header.Value));
        return this;
    }

    /// <inheritdoc />
    public IResponseBuilder WithHeaders(IDictionary<string, string[]> headers)
    {
        Guard.NotNull(headers);

        ResponseMessage.Headers = headers.ToDictionary(header => header.Key, header => new WireMockList<string>(header.Value));
        return this;
    }

    /// <inheritdoc />
    public IResponseBuilder WithHeaders(IDictionary<string, WireMockList<string>> headers)
    {
        Guard.NotNull(headers);

        ResponseMessage.Headers = headers;
        return this;
    }

    /// <inheritdoc />
    public IResponseBuilder WithTrailingHeader(string name, params string[] values)
    {
#if !TRAILINGHEADERS
        throw new System.NotSupportedException("The WithTrailingHeader method can not be used for .NETStandard1.3 or .NET Framework 4.6.1 or lower.");
#else

        Guard.NotNull(name);

        ResponseMessage.AddTrailingHeader(name, values);
        return this;
#endif
    }

    /// <inheritdoc />
    public IResponseBuilder WithTrailingHeaders(IDictionary<string, string> headers)
    {
#if !TRAILINGHEADERS
        throw new System.NotSupportedException("The WithTrailingHeaders method can not be used for .NETStandard1.3 or .NET Framework 4.6.1 or lower.");
#else

        Guard.NotNull(headers);

        ResponseMessage.TrailingHeaders = headers.ToDictionary(header => header.Key, header => new WireMockList<string>(header.Value));
        return this;
#endif
    }

    /// <inheritdoc />
    public IResponseBuilder WithTrailingHeaders(IDictionary<string, string[]> headers)
    {
#if !TRAILINGHEADERS
        throw new System.NotSupportedException("The WithTrailingHeaders method can not be used for .NETStandard1.3 or .NET Framework 4.6.1 or lower.");
#else

        Guard.NotNull(headers);

        ResponseMessage.TrailingHeaders = headers.ToDictionary(header => header.Key, header => new WireMockList<string>(header.Value));
        return this;
#endif
    }

    /// <inheritdoc />
    public IResponseBuilder WithTrailingHeaders(IDictionary<string, WireMockList<string>> headers)
    {
#if !TRAILINGHEADERS
        throw new System.NotSupportedException("The WithBodyAsProtoBuf method can not be used for .NETStandard1.3 or .NET Framework 4.6.1 or lower.");
#else

        Guard.NotNull(headers);

        ResponseMessage.TrailingHeaders = headers;
        return this;
#endif
    }
}