// Copyright © WireMock.Net

using Moq;
using Scriban.Runtime;
using WireMock.Handlers;
using WireMock.Settings;

namespace WireMock.Net.Tests.Settings;

public class WireMockServerSettingsExtensionsTests
{
    private readonly IFileSystemHandler _fileSystemHandler = Mock.Of<IFileSystemHandler>();

    [Fact]
    public void WithScribanRegistration_SetsCallbackWhichPassesTypedScriptObjectAndFileSystemHandler()
    {
        // Arrange
        ScriptObject? receivedScriptObject = null;
        IFileSystemHandler? receivedFileSystemHandler = null;
        var settings = new WireMockServerSettings();
        var scriptObject = new ScriptObject();

        // Act
        var result = settings.WithScribanRegistration((so, fs) =>
        {
            receivedScriptObject = so;
            receivedFileSystemHandler = fs;
        });
        settings.ScribanRegistrationCallback!(scriptObject, _fileSystemHandler);

        // Assert
        result.Should().BeSameAs(settings);
        receivedScriptObject.Should().BeSameAs(scriptObject);
        receivedFileSystemHandler.Should().BeSameAs(_fileSystemHandler);
    }

    [Fact]
    public void WithScribanRegistration_WithExistingCallback_InvokesBothInRegistrationOrder()
    {
        // Arrange
        var calls = new List<string>();
        var settings = new WireMockServerSettings
        {
            ScribanRegistrationCallback = (_, _) => calls.Add("existing")
        };

        // Act
        settings
            .WithScribanRegistration((_, _) => calls.Add("first"))
            .WithScribanRegistration((_, _) => calls.Add("second"));
        settings.ScribanRegistrationCallback!(new ScriptObject(), _fileSystemHandler);

        // Assert
        calls.Should().Equal("existing", "first", "second");
    }

    [Fact]
    public void WithScribanRegistration_WithNullSettings_ThrowsArgumentNullException()
    {
        // Arrange
        WireMockServerSettings settings = null!;

        // Act
        Action act = () => settings.WithScribanRegistration((_, _) => { });

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void WithScribanRegistration_WithNullCallback_ThrowsArgumentNullException()
    {
        // Arrange
        var settings = new WireMockServerSettings();

        // Act
        Action act = () => settings.WithScribanRegistration(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }
}
