// Copyright © WireMock.Net

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using ProtoBufJsonConverter;
using Stef.Validation;
using WireMock.Extensions;

namespace WireMock.Util;

/// <summary>
/// This resolver is used to resolve the extra ProtoDefinition files.
/// 
/// It assumes that:
/// - The first commented line of each ProtoDefinition file is the filepath which is used in the import of the other ProtoDefinition file(s).
/// </summary>
internal class WireMockProtoFileResolver : IProtoFileResolver
{
    private readonly Dictionary<string, string> _files = [];

    public WireMockProtoFileResolver(IReadOnlyCollection<string> protoDefinitions)
    {
        if (Guard.NotNullOrEmpty(protoDefinitions).Count() <= 1)
        {
            return;
        }

        foreach (var extraProtoDefinition in protoDefinitions)
        {
            var firstNonEmptyLine = extraProtoDefinition.Split('\r', '\n').FirstOrDefault(l => !string.IsNullOrEmpty(l));
            if (firstNonEmptyLine != null)
            {
                if (TryGetValidPath(firstNonEmptyLine.TrimStart('/', ' '), out var validPath))
                {
                    _files.Add(validPath, extraProtoDefinition);
                }
                else
                {
                    _files.Add(extraProtoDefinition.GetDeterministicHashCodeAsString(), extraProtoDefinition);
                }
            }
        }
    }

    public bool Exists(string path)
    {
        return _files.ContainsKey(path);
    }

    public TextReader OpenText(string path)
    {
        if (_files.TryGetValue(path, out var extraProtoDefinition))
        {
            return new StringReader(extraProtoDefinition);
        }

        throw new FileNotFoundException($"The ProtoDefinition '{path}' was not found.");
    }

    private static bool TryGetValidPath(string path, [NotNullWhen(true)] out string? validPath)
    {
        if (!path.Any(c => Path.GetInvalidPathChars().Contains(c)))
        {
            validPath = path;
            return true;
        }

        validPath = null;
        return false;
    }
}