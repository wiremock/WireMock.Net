// Copyright © WireMock.Net

using WireMock.Models;
using WireMock.Settings;

namespace WireMock.Util;

/// <summary>
/// Some helper methods for Proto Definitions.
/// </summary>
public static class ProtoDefinitionUtils
{
    internal static IdOrTexts GetIdOrTexts(WireMockServerSettings settings, params string[] protoDefinitionOrId)
    {
        switch (protoDefinitionOrId.Length)
        {
            case 1:
                var idOrText = protoDefinitionOrId[0];
                if (settings.ProtoDefinitions?.TryGetValue(idOrText, out var protoDefinitions) == true)
                {
                    return new(idOrText, protoDefinitions);
                }

                return new(null, protoDefinitionOrId);

            default:
                return new(null, protoDefinitionOrId);
        }
    }
}