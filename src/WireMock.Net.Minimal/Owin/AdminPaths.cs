// Copyright © WireMock.Net

using System.Text.RegularExpressions;
using WireMock.Matchers;

namespace WireMock.Owin;

internal sealed class AdminPaths(string? adminPath) : IAdminPaths
{
    private const string DefaultAdminPathPrefix = "/__admin";

    private readonly string _prefix = adminPath ?? DefaultAdminPathPrefix;

    public string Files => $"{_prefix}/files";

    public string Health => $"{_prefix}/health";

    public string Mappings => $"{_prefix}/mappings";

    public string MappingsCode => $"{_prefix}/mappings/code";

    public string MappingsWireMockOrg => $"{_prefix}/mappings/wiremock.org";

    public string Requests => $"{_prefix}/requests";

    public string Settings => $"{_prefix}/settings";

    public string Scenarios => $"{_prefix}/scenarios";

    public string OpenApi => $"{_prefix}/openapi";

    public string ProtoDefinitions => $"{_prefix}/protodefinitions";

    private string PrefixRegexEscaped => Regex.Escape(_prefix);

    public RegexMatcher MappingsGuidPathMatcher => new($@"^{PrefixRegexEscaped}\/mappings\/([0-9A-Fa-f]{{8}}[-][0-9A-Fa-f]{{4}}[-][0-9A-Fa-f]{{4}}[-][0-9A-Fa-f]{{4}}[-][0-9A-Fa-f]{{12}})$");

    public RegexMatcher MappingsGuidEnablePathMatcher => new($@"^{PrefixRegexEscaped}\/mappings\/([0-9A-Fa-f]{{8}}[-][0-9A-Fa-f]{{4}}[-][0-9A-Fa-f]{{4}}[-][0-9A-Fa-f]{{4}}[-][0-9A-Fa-f]{{12}})\/enable$");

    public RegexMatcher MappingsGuidDisablePathMatcher => new($@"^{PrefixRegexEscaped}\/mappings\/([0-9A-Fa-f]{{8}}[-][0-9A-Fa-f]{{4}}[-][0-9A-Fa-f]{{4}}[-][0-9A-Fa-f]{{4}}[-][0-9A-Fa-f]{{12}})\/disable$");

    public RegexMatcher MappingsCodeGuidPathMatcher => new($@"^{PrefixRegexEscaped}\/mappings\/code\/([0-9A-Fa-f]{{8}}[-][0-9A-Fa-f]{{4}}[-][0-9A-Fa-f]{{4}}[-][0-9A-Fa-f]{{4}}[-][0-9A-Fa-f]{{12}})$");

    public RegexMatcher RequestsGuidPathMatcher => new($@"^{PrefixRegexEscaped}\/requests\/([0-9A-Fa-f]{{8}}[-][0-9A-Fa-f]{{4}}[-][0-9A-Fa-f]{{4}}[-][0-9A-Fa-f]{{4}}[-][0-9A-Fa-f]{{12}})$");

    public RegexMatcher ScenariosNameMatcher => new($@"^{PrefixRegexEscaped}\/scenarios\/.+$");

    public RegexMatcher ScenariosNameWithStateMatcher => new($@"^{PrefixRegexEscaped}\/scenarios\/.+\/state$");

    public RegexMatcher ScenariosNameWithResetMatcher => new($@"^{PrefixRegexEscaped}\/scenarios\/.+\/reset$");

    public RegexMatcher FilesFilenamePathMatcher => new($@"^{PrefixRegexEscaped}\/files\/.+$");

    public RegexMatcher ProtoDefinitionsIdPathMatcher => new($@"^{PrefixRegexEscaped}\/protodefinitions\/.+$");

    public bool Includes(string? path) => path?.StartsWith($"{_prefix}/") ?? false;

    public override string ToString() => _prefix;
}