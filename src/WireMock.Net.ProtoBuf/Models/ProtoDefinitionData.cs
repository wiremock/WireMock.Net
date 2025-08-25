// Copyright © WireMock.Net

using System.Collections.Generic;
using System.Linq;
using Stef.Validation;

namespace WireMock.Models;

/// <summary>
/// A placeholder class for Proto Definitions.
/// </summary>
public class ProtoDefinitionData
{
    private readonly IDictionary<string, string> _filenameMappedToProtoDefinition;

    internal ProtoDefinitionData(IDictionary<string, string> filenameMappedToProtoDefinition)
    {
        _filenameMappedToProtoDefinition = filenameMappedToProtoDefinition;
    }

    /// <summary>
    /// Get all the ProtoDefinitions.
    /// Note: the main ProtoDefinition will be the first one in the list.
    /// </summary>
    /// <param name="mainProtoFilename">The main ProtoDefinition filename.</param>
    public IReadOnlyList<string> ToList(string mainProtoFilename)
    {
        Guard.NotNullOrEmpty(mainProtoFilename);

        if (!_filenameMappedToProtoDefinition.TryGetValue(mainProtoFilename, out var mainProtoDefinition))
        {
            throw new KeyNotFoundException($"The ProtoDefinition with filename '{mainProtoFilename}' was not found.");
        }

        var list = new List<string> { mainProtoDefinition };
        list.AddRange(_filenameMappedToProtoDefinition.Where(kvp => kvp.Key != mainProtoFilename).Select(kvp => kvp.Value));
        return list;
    }
}