using WireMock.Matchers;

namespace WireMock.Owin;

internal interface IAdminPaths
{
    string Files { get; }
    string Health { get; }
    string Mappings { get; }
    string MappingsCode { get; }
    string MappingsWireMockOrg { get; }
    RegexMatcher MappingsGuidPathMatcher { get; }
    RegexMatcher MappingsGuidEnablePathMatcher { get; }
    RegexMatcher MappingsGuidDisablePathMatcher { get; }
    RegexMatcher MappingsCodeGuidPathMatcher { get; }
    RegexMatcher RequestsGuidPathMatcher { get; }
    RegexMatcher ScenariosNameMatcher { get; }
    RegexMatcher ScenariosNameWithStateMatcher { get; }
    RegexMatcher ScenariosNameWithResetMatcher { get; }
    RegexMatcher FilesFilenamePathMatcher { get; }
    RegexMatcher ProtoDefinitionsIdPathMatcher { get; }
    string Requests { get; }
    string Settings { get; }
    string Scenarios { get; }
    string OpenApi { get; }
    string ProtoDefinitions { get; }
    bool Includes(string? path);
}