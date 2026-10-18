// Copyright © WireMock.Net

using Moq;
using Scriban.Runtime;
using WireMock.Handlers;
using WireMock.Settings;
using WireMock.Transformers.Scriban;
using WireMock.Types;

namespace WireMock.Net.Tests.Transformers.Scriban
{
    public class ScribanContextFactoryTests
    {
        private readonly Mock<IFileSystemHandler> _fileSystemHandlerMock = new Mock<IFileSystemHandler>();

        [Theory]
        [InlineData(TransformerType.Scriban)]
        [InlineData(TransformerType.ScribanDotLiquid)]
        public void Create_With_Scriban_TransformerType_Creates_ITransformerContext(TransformerType transformerType)
        {
            // Arrange
            var settings = new WireMockServerSettings
            {
                FileSystemHandler = _fileSystemHandlerMock.Object
            };
            var sut = new ScribanContextFactory(settings, transformerType);

            // Act
            var result = sut.Create();

            // Assert
            result.Should().NotBeNull();
            result.FileSystemHandler.Should().BeSameAs(_fileSystemHandlerMock.Object);
        }

        [Fact]
        public void Create_With_Invalid_TransformerType_Throws_Exception()
        {
            // Arrange
            var settings = new WireMockServerSettings
            {
                FileSystemHandler = _fileSystemHandlerMock.Object
            };

            // Act
            Action action = () => new ScribanContextFactory(settings, TransformerType.Handlebars);

            // Assert
            action.Should().Throw<Exception>();
        }

        [Theory]
        [InlineData(TransformerType.Scriban)]
        [InlineData(TransformerType.ScribanDotLiquid)]
        public void Create_WithScribanRegistrationCallback_InvokesCallbackOnceWithScriptObjectAndFileSystemHandler(TransformerType transformerType)
        {
            // Arrange
            var calls = new List<(object ScriptObject, IFileSystemHandler FileSystemHandler)>();
            var settings = new WireMockServerSettings
            {
                FileSystemHandler = _fileSystemHandlerMock.Object,
                ScribanRegistrationCallback = (scriptObject, fileSystemHandler) => calls.Add((scriptObject, fileSystemHandler))
            };
            var sut = new ScribanContextFactory(settings, transformerType);

            // Act
            var result = sut.Create();

            // Assert
            result.Should().NotBeNull();
            calls.Should().ContainSingle();
            calls[0].ScriptObject.Should().BeOfType<ScriptObject>();
            calls[0].FileSystemHandler.Should().BeSameAs(_fileSystemHandlerMock.Object);
        }

        [Fact]
        public void Create_CalledTwice_InvokesScribanRegistrationCallbackWithNewScriptObjectEachTime()
        {
            // Arrange
            var scriptObjects = new List<object>();
            var settings = new WireMockServerSettings
            {
                FileSystemHandler = _fileSystemHandlerMock.Object,
                ScribanRegistrationCallback = (scriptObject, _) => scriptObjects.Add(scriptObject)
            };
            var sut = new ScribanContextFactory(settings, TransformerType.Scriban);

            // Act
            sut.Create();
            sut.Create();

            // Assert
            scriptObjects.Should().HaveCount(2);
            scriptObjects[0].Should().NotBeSameAs(scriptObjects[1]);
        }
    }
}