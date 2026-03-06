// Copyright © WireMock.Net

using JetBrains.Annotations;
using Stef.Validation;
using WireMock.Matchers.Request;
using WireMock.RequestBuilders;

namespace WireMock.Server;

public partial class WireMockServer
{
    /// <summary>
    /// Given
    /// </summary>
    /// <param name="requestMatcher">The request matcher.</param>
    /// <param name="saveToFile">Optional boolean to indicate if this mapping should be saved as static mapping file.</param>
    /// <returns>The <see cref="IRespondWithAProvider"/>.</returns>
    [PublicAPI]
    public IRespondWithAProvider Given(IRequestMatcher requestMatcher, bool saveToFile = false)
    {
        return _mappingBuilder.Given(requestMatcher, saveToFile);
    }

    /// <summary>
    /// WhenRequest
    /// </summary>
    /// <param name="action">The action to use the fluent <see cref="IRequestBuilder"/>.</param>
    /// <param name="saveToFile">Optional boolean to indicate if this mapping should be saved as static mapping file.</param>
    /// <returns>The <see cref="IRespondWithAProvider"/>.</returns>
    [PublicAPI]
    public IRespondWithAProvider WhenRequest(Action<IRequestBuilder> action, bool saveToFile = false)
    {
        Guard.NotNull(action);

        var requestBuilder = Request.Create();

        action(requestBuilder);

        return Given(requestBuilder, saveToFile);
    }
}