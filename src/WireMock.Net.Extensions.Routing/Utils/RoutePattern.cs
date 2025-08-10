// Copyright © WireMock.Net

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.RegularExpressions;
using WireMock.Net.Extensions.Routing.Extensions;

namespace WireMock.Net.Extensions.Routing.Utils;

internal static class RoutePattern
{
    private static readonly Regex ArgRegex =
        new(@"{(?'name'\w+)(?::(?'type'\w+))?}", RegexOptions.Compiled);

    public static IDictionary<string, object> GetArgs(string pattern, string route) =>
        TryGetArgs(pattern, route, out var args)
            ? args
            : throw new InvalidOperationException(
                $"Url {route} does not match route pattern {pattern}");

    public static bool TryGetArgs(
        string pattern, string route, [NotNullWhen(true)] out IDictionary<string, object>? args)
    {
        var regex = new Regex(ToRegex(pattern), RegexOptions.IgnoreCase);
        var match = regex.Match(route);
        if (!match.Success)
        {
            args = null;
            return false;
        }

        var routeArgTypeMap = GetArgTypeMap(pattern);
        args = match.Groups
            .Cast<Group>()
            .Where(g => g.Index > 0)
            .ToDictionary(g => g.Name, g => routeArgTypeMap[g.Name].Parse(g.Value));
        return true;
    }

    public static string ToRegex(string pattern)
    {
        static string GetArgMatchingRegex(Match match) =>
            ArgType.GetByName(match.Groups["type"].Value).GetRegex();

        return ArgRegex
            .Replace(pattern, m => $"(?'{m.Groups["name"].Value}'{GetArgMatchingRegex(m)})")
            .ToMatchFullStringRegex();
    }

    private static IDictionary<string, ArgType> GetArgTypeMap(string pattern) =>
        ArgRegex
            .Matches(pattern)
            .ToDictionary(
                m => m.Groups["name"].Value, m => ArgType.GetByName(m.Groups["type"].Value));

    private abstract record ArgType
    {
        private ArgType(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public static ArgType String { get; } = new StringArgType();

        public static ArgType Int { get; } = new IntArgType();

        private static IReadOnlyCollection<ArgType> All { get; } =
            typeof(ArgType)
                .GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(p => p.PropertyType.IsAssignableTo(typeof(ArgType)))
                .Select(p => p.GetValue(null))
                .Cast<ArgType>()
                .ToList();

        private static IReadOnlyDictionary<string, ArgType> MapByName { get; } =
            All.ToDictionary(x => x.Name);

        public static ArgType GetByName(string name) =>
            GetByNameOrDefault(name)
            ?? throw new InvalidOperationException($"Route argument type {name} is not found");

        public static ArgType? GetByNameOrDefault(string name) =>
            !string.IsNullOrEmpty(name)
                ? MapByName.GetValueOrDefault(name)
                : String;

        public abstract object Parse(string input);

        public abstract string GetRegex();

        private sealed record StringArgType() : ArgType("string")
        {
            public override object Parse(string input) => input;

            public override string GetRegex() => @".*";
        }

        private sealed record IntArgType() : ArgType("int")
        {
            public override object Parse(string input) => int.Parse(input);

            public override string GetRegex() => @"-?\d+";
        }
    }
}
