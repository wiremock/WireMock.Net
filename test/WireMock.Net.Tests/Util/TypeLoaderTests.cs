// Copyright © WireMock.Net

using System.IO;
using AnyOfTypes;
using FluentAssertions;
using WireMock.Matchers;
using WireMock.Models;
using WireMock.Util;
using Xunit;

namespace WireMock.Net.Tests.Util;

public class TypeLoaderTests
{
    public interface IDummyInterfaceNoImplementation
    {
    }

    public interface IDummyInterfaceWithImplementation
    {
    }

    public class DummyClass : IDummyInterfaceWithImplementation
    {
    }

    public interface IDummyInterfaceWithImplementationUsedForStaticTest
    {
    }

    public class DummyClass1UsedForStaticTest : IDummyInterfaceWithImplementationUsedForStaticTest
    {
        public DummyClass1UsedForStaticTest(Counter counter)
        {
            counter.AddOne();
        }
    }

    public class DummyClass2UsedForStaticTest : IDummyInterfaceWithImplementationUsedForStaticTest
    {
        public DummyClass2UsedForStaticTest(Counter counter)
        {
            counter.AddOne();
        }
    }

    public class Counter
    {
        public int Value { get; private set; }

        public void AddOne()
        {
            Value++;
        }
    }

    [Fact]
    public void TryLoadNewInstance()
    {
        var current = Directory.GetCurrentDirectory();
        try
        {
            Directory.SetCurrentDirectory(Path.GetTempPath());

            // Act
            AnyOf<string, StringPattern> pattern = "x";
            var result = TypeLoader.TryLoadNewInstance<ICSharpCodeMatcher>(out var instance, MatchBehaviour.AcceptOnMatch, MatchOperator.Or, pattern);

            // Assert
            result.Should().BeTrue();
            instance.Should().BeOfType<CSharpCodeMatcher>();
        }
        finally
        {
            Directory.SetCurrentDirectory(current);
        }
    }

    [Fact]
    public void TryLoadNewInstanceByFullName()
    {
        // Act
        var result = TypeLoader.TryLoadNewInstanceByFullName<IDummyInterfaceWithImplementation>(out var instance, typeof(DummyClass).FullName!);

        // Assert
        result.Should().BeTrue();
        instance.Should().BeOfType<DummyClass>();
    }

    [Fact]
    public void TryLoadStaticInstance_ShouldOnlyCreateInstanceOnce()
    {
        // Arrange
        var counter = new Counter();

        // Act
        var result = TypeLoader.TryLoadStaticInstance<IDummyInterfaceWithImplementationUsedForStaticTest>(out var staticInstance, counter);
        TypeLoader.TryLoadStaticInstance(out staticInstance, counter);

        // Assert
        result.Should().BeTrue();
        staticInstance.Should().BeOfType<DummyClass1UsedForStaticTest>();
        counter.Value.Should().Be(1);
    }

    [Fact]
    public void TryLoadStaticInstanceByFullName_ShouldOnlyCreateInstanceOnce()
    {
        // Arrange
        var counter = new Counter();
        var fullName = typeof(DummyClass2UsedForStaticTest).FullName!;

        // Act
        var result = TypeLoader.TryLoadStaticInstanceByFullName<IDummyInterfaceWithImplementationUsedForStaticTest>(out var staticInstance, fullName, counter);
        TypeLoader.TryLoadStaticInstanceByFullName(out staticInstance, fullName, counter);

        // Assert
        result.Should().BeTrue();
        staticInstance.Should().BeOfType<DummyClass2UsedForStaticTest>();
        counter.Value.Should().Be(1);
    }

    [Fact]
    public void TryLoadNewInstance_ButNoImplementationFoundForInterface_ReturnsFalse()
    {
        // Act
        var result = TypeLoader.TryLoadNewInstance<IDummyInterfaceNoImplementation>(out _);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void TryLoadNewInstanceByFullName_ButNoImplementationFoundForInterface_ReturnsFalse()
    {
        // Act
        var result = TypeLoader.TryLoadNewInstanceByFullName<IDummyInterfaceWithImplementation>(out _, "xyz");

        // Assert
        result.Should().BeFalse();
    }
}