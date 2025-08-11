// Copyright © WireMock.Net

#if !(NET452 || NET461 || NETCOREAPP3_1)
using System;
using System.Threading.Tasks;
using Moq;
using VerifyTests;
using VerifyXunit;
using WireMock.Handlers;
using WireMock.Logging;
using WireMock.Matchers;
using WireMock.Net.Tests.VerifyExtensions;
using WireMock.Owin;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Serialization;
using WireMock.Settings;
using WireMock.Types;
using WireMock.Util;
using Xunit;

namespace WireMock.Net.Tests;

[UsesVerify]
public class MappingBuilderTests
{
    private static readonly VerifySettings VerifySettings = new();
    static MappingBuilderTests()
    {
        VerifySettings.Init();
    }

    private const string MappingGuid = "41372914-1838-4c67-916b-b9aacdd096ce";
    private static readonly DateTime UtcNow = new(2023, 1, 14, 15, 16, 17);

    private readonly Mock<IFileSystemHandler> _fileSystemHandlerMock;
    private readonly int _numMappings;

    private readonly MappingBuilder _sut;

    public MappingBuilderTests()
    {
        _fileSystemHandlerMock = new Mock<IFileSystemHandler>();

        var guidUtilsMock = new Mock<IGuidUtils>();
        var startGuid = 1000;
        guidUtilsMock.Setup(g => g.NewGuid()).Returns(() => new Guid($"98fae52e-76df-47d9-876f-2ee32e93{startGuid++}"));

        var dateTimeUtilsMock = new Mock<IDateTimeUtils>();
        dateTimeUtilsMock.SetupGet(d => d.UtcNow).Returns(UtcNow);

        var settings = new WireMockServerSettings
        {
            FileSystemHandler = _fileSystemHandlerMock.Object,
            Logger = Mock.Of<IWireMockLogger>()
        };
        var options = new WireMockMiddlewareOptions();
        var matcherMapper = new MatcherMapper(settings);
        var mappingConverter = new MappingConverter(matcherMapper);
        var mappingToFileSaver = new MappingToFileSaver(settings, mappingConverter);

        _sut = new MappingBuilder(
            settings,
            options,
            mappingConverter,
            mappingToFileSaver,
            guidUtilsMock.Object,
            dateTimeUtilsMock.Object
        );

        _sut.Given(Request.Create()
            .WithPath("/foo")
            .WithParam("test", new ExactMatcher("abc"))
            .UsingGet()
        )
        .WithGuid(MappingGuid)
        .RespondWith(Response.Create()
            .WithBody(@"{ msg: ""Hello world!""}")
        );

        _sut.Given(Request.Create()
            .WithPath("/users/post1")
            .UsingPost()
            .WithBodyAsJson(new
            {
                Request = "Hello?"
            })
        ).RespondWith(Response.Create());

        _sut.Given(Request.Create()
            .WithPath("/users/post2")
            .UsingPost()
            .WithBody(new JsonMatcher(new
            {
                city = "Amsterdam",
                country = "The Netherlands"
            }))
        ).RespondWith(Response.Create());

        _sut.Given(Request.Create()
            .UsingPost()
            .WithPath("/form-urlencoded")
            .WithHeader("Content-Type", "application/x-www-form-urlencoded")
            .WithBody(new FormUrlEncodedMatcher(["name=John Doe", "email=johndoe@example.com"]))
        ).RespondWith(Response.Create());

        _sut.Given(Request.Create()
            .WithPath("/regex")
            .WithParam("foo", new RegexMatcher(".*"))
            .UsingGet()
        ).RespondWith(Response.Create());

        _sut.Given(Request.Create()
            .WithPath("/todo/items")
            .UsingGet())
        .InScenario("To do list")
        .WillSetStateTo("TodoList State Started", 2)
        .RespondWith(Response.Create()
            .WithBody("Buy milk"));

        _numMappings = _sut.GetMappings().Length;
    }

    [Fact]
    public Task GetMappings()
    {
        // Act
        var mappings = _sut.GetMappings();

        // Verify
        return Verifier.Verify(mappings, VerifySettings).DontScrubGuids();
    }

    [Fact]
    public Task ToJson()
    {
        // Act
        var json = _sut.ToJson();

        // Verify
        return Verifier.VerifyJson(json, VerifySettings).DontScrubGuids();
    }

    [Fact]
    public Task ToCSharpCode_Server()
    {
        // Act
        var code = _sut.ToCSharpCode(MappingConverterType.Server);

        // Verify
        return Verifier.Verify(code, VerifySettings).DontScrubGuids();
    }

    [Fact]
    public Task ToCSharpCode_Builder()
    {
        // Act
        var code = _sut.ToCSharpCode(MappingConverterType.Builder);

        // Verify
        return Verifier.Verify(code, VerifySettings).DontScrubGuids();
    }

    [Fact]
    public void SaveMappingsToFile_FolderExists_IsFalse()
    {
        // Arrange
        var path = "path";

        // Act
        _sut.SaveMappingsToFile(path);

        // Verify
        _fileSystemHandlerMock.Verify(fs => fs.GetMappingFolder(), Times.Never);
        _fileSystemHandlerMock.Verify(fs => fs.FolderExists(path), Times.Once);
        _fileSystemHandlerMock.Verify(fs => fs.CreateFolder(path), Times.Once);
        _fileSystemHandlerMock.Verify(fs => fs.WriteMappingFile(path, It.IsAny<string>()), Times.Once);
        _fileSystemHandlerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void SaveMappingsToFile_FolderExists_IsTrue()
    {
        // Arrange
        var path = "path";
        _fileSystemHandlerMock.Setup(fs => fs.FolderExists(It.IsAny<string>())).Returns(true);

        // Act
        _sut.SaveMappingsToFile(path);

        // Verify
        _fileSystemHandlerMock.Verify(fs => fs.GetMappingFolder(), Times.Never);
        _fileSystemHandlerMock.Verify(fs => fs.FolderExists(path), Times.Once);
        _fileSystemHandlerMock.Verify(fs => fs.WriteMappingFile(path, It.IsAny<string>()), Times.Once);
        _fileSystemHandlerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void SaveMappingsToFolder_FolderIsNull()
    {
        // Arrange
        var mappingFolder = "mapping-folder";
        _fileSystemHandlerMock.Setup(fs => fs.GetMappingFolder()).Returns(mappingFolder);
        _fileSystemHandlerMock.Setup(fs => fs.FolderExists(It.IsAny<string>())).Returns(true);

        // Act
        _sut.SaveMappingsToFolder(null);

        // Verify
        _fileSystemHandlerMock.Verify(fs => fs.GetMappingFolder(), Times.Exactly(_numMappings));
        _fileSystemHandlerMock.Verify(fs => fs.FolderExists(mappingFolder), Times.Exactly(_numMappings));
        _fileSystemHandlerMock.Verify(fs => fs.WriteMappingFile(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(_numMappings));
        _fileSystemHandlerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void SaveMappingsToFolder_FolderExists_IsTrue()
    {
        // Arrange
        var path = "path";
        _fileSystemHandlerMock.Setup(fs => fs.FolderExists(It.IsAny<string>())).Returns(true);

        // Act
        _sut.SaveMappingsToFolder(path);

        // Verify
        _fileSystemHandlerMock.Verify(fs => fs.GetMappingFolder(), Times.Never);
        _fileSystemHandlerMock.Verify(fs => fs.FolderExists(path), Times.Exactly(_numMappings));
        _fileSystemHandlerMock.Verify(fs => fs.WriteMappingFile(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(_numMappings));
        _fileSystemHandlerMock.VerifyNoOtherCalls();
    }
}
#endif