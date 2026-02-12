// Copyright © WireMock.Net

using FluentAssertions;
using WireMock.Matchers;

namespace WireMock.Net.Tests.Matchers;

public class FuncMatcherTests
{
    [Fact]
    public void FuncMatcher_For_String_IsMatch_Should_Return_Perfect_When_Function_Returns_True()
    {
        // Arrange
        Func<string?, bool> func = s => s == "test";
        var matcher = new FuncMatcher(func);

        // Act
        var result = matcher.IsMatch("test");

        // Assert
        result.IsPerfect().Should().BeTrue();
    }

    [Fact]
    public void FuncMatcher_For_String_IsMatch_Should_Return_Mismatch_When_Function_Returns_False()
    {
        // Arrange
        Func<string?, bool> func = s => s == "test";
        var matcher = new FuncMatcher(func);

        // Act
        var result = matcher.IsMatch("other");

        // Assert
        result.IsPerfect().Should().BeFalse();
    }

    [Fact]
    public void FuncMatcher_For_String_IsMatch_Should_Handle_Null_String()
    {
        // Arrange
        Func<string?, bool> func = s => s == null;
        var matcher = new FuncMatcher(func);

        // Act - passing null as object, not as string
        var result = matcher.IsMatch((object?)null);

        // Assert - null object doesn't match, returns mismatch
        result.IsPerfect().Should().BeFalse();
    }

    [Fact]
    public void FuncMatcher_For_String_IsMatch_With_ByteArray_Input_Should_Return_Mismatch()
    {
        // Arrange
        Func<string?, bool> func = s => s == "test";
        var matcher = new FuncMatcher(func);

        // Act
        var result = matcher.IsMatch(new byte[] { 1, 2, 3 });

        // Assert
        result.IsPerfect().Should().BeFalse();
    }

    [Fact]
    public void FuncMatcher_For_String_IsMatch_With_Null_Object_Should_Return_Mismatch()
    {
        // Arrange
        Func<string?, bool> func = s => s == "test";
        var matcher = new FuncMatcher(func);

        // Act
        var result = matcher.IsMatch((object?)null);

        // Assert
        result.IsPerfect().Should().BeFalse();
    }

    [Fact]
    public void FuncMatcher_For_String_IsMatch_Should_Handle_Exception()
    {
        // Arrange
        Func<string?, bool> func = s => throw new InvalidOperationException("Test exception");
        var matcher = new FuncMatcher(func);

        // Act
        var result = matcher.IsMatch("test");

        // Assert
        result.IsPerfect().Should().BeFalse();
        result.Exception.Should().NotBeNull();
        result.Exception.Should().BeOfType<InvalidOperationException>();
        result.Exception!.Message.Should().Be("Test exception");
    }

    [Fact]
    public void FuncMatcher_For_Bytes_IsMatch_Should_Return_Perfect_When_Function_Returns_True()
    {
        // Arrange
        Func<byte[]?, bool> func = b => b != null && b.Length == 3;
        var matcher = new FuncMatcher(func);

        // Act
        var result = matcher.IsMatch(new byte[] { 1, 2, 3 });

        // Assert
        result.IsPerfect().Should().BeTrue();
    }

    [Fact]
    public void FuncMatcher_For_Bytes_IsMatch_Should_Return_Mismatch_When_Function_Returns_False()
    {
        // Arrange
        Func<byte[]?, bool> func = b => b != null && b.Length == 3;
        var matcher = new FuncMatcher(func);

        // Act
        var result = matcher.IsMatch(new byte[] { 1, 2, 3, 4, 5 });

        // Assert
        result.IsPerfect().Should().BeFalse();
    }

    [Fact]
    public void FuncMatcher_For_Bytes_IsMatch_Should_Handle_Null_ByteArray()
    {
        // Arrange
        Func<byte[]?, bool> func = b => b == null;
        var matcher = new FuncMatcher(func);

        // Act - passing null as object, not as byte[]
        var result = matcher.IsMatch((object?)null);

        // Assert - null object doesn't match, returns mismatch
        result.IsPerfect().Should().BeFalse();
    }

    [Fact]
    public void FuncMatcher_For_Bytes_IsMatch_With_String_Input_Should_Return_Mismatch()
    {
        // Arrange
        Func<byte[]?, bool> func = b => b != null && b.Length > 0;
        var matcher = new FuncMatcher(func);

        // Act
        var result = matcher.IsMatch("test");

        // Assert
        result.IsPerfect().Should().BeFalse();
    }

    [Fact]
    public void FuncMatcher_For_Bytes_IsMatch_Should_Handle_Exception()
    {
        // Arrange
        Func<byte[]?, bool> func = b => throw new InvalidOperationException("Bytes exception");
        var matcher = new FuncMatcher(func);

        // Act
        var result = matcher.IsMatch(new byte[] { 1, 2, 3 });

        // Assert
        result.IsPerfect().Should().BeFalse();
        result.Exception.Should().NotBeNull();
        result.Exception.Should().BeOfType<InvalidOperationException>();
        result.Exception!.Message.Should().Be("Bytes exception");
    }

    [Fact]
    public void FuncMatcher_For_String_With_Contains_Logic_Should_Work()
    {
        // Arrange
        Func<string?, bool> func = s => s?.Contains("foo") == true;
        var matcher = new FuncMatcher(func);

        // Act
        var result1 = matcher.IsMatch("foo");
        var result2 = matcher.IsMatch("foobar");
        var result3 = matcher.IsMatch("bar");

        // Assert
        result1.IsPerfect().Should().BeTrue();
        result2.IsPerfect().Should().BeTrue();
        result3.IsPerfect().Should().BeFalse();
    }

    [Fact]
    public void FuncMatcher_For_Bytes_With_Length_Logic_Should_Work()
    {
        // Arrange
        Func<byte[]?, bool> func = b => b != null && b.Length > 2;
        var matcher = new FuncMatcher(func);

        // Act
        var result1 = matcher.IsMatch(new byte[] { 1 });
        var result2 = matcher.IsMatch(new byte[] { 1, 2 });
        var result3 = matcher.IsMatch(new byte[] { 1, 2, 3 });

        // Assert
        result1.IsPerfect().Should().BeFalse();
        result2.IsPerfect().Should().BeFalse();
        result3.IsPerfect().Should().BeTrue();
    }

    [Fact]
    public void FuncMatcher_Name_Should_Return_FuncMatcher()
    {
        // Arrange
        Func<string?, bool> func = s => s == "test";
        var matcher = new FuncMatcher(func);

        // Act & Assert
        matcher.Name.Should().Be("FuncMatcher");
    }

    [Fact]
    public void FuncMatcher_MatchBehaviour_Should_Return_AcceptOnMatch_By_Default()
    {
        // Arrange
        Func<string?, bool> func = s => s == "test";
        var matcher = new FuncMatcher(func);

        // Act & Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.AcceptOnMatch);
    }

    [Fact]
    public void FuncMatcher_MatchBehaviour_Should_Return_Custom_Value_For_String()
    {
        // Arrange
        Func<string?, bool> func = s => s == "test";
        var matcher = new FuncMatcher(func, MatchBehaviour.RejectOnMatch);

        // Act & Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.RejectOnMatch);
    }

    [Fact]
    public void FuncMatcher_MatchBehaviour_Should_Return_Custom_Value_For_Bytes()
    {
        // Arrange
        Func<byte[]?, bool> func = b => b != null;
        var matcher = new FuncMatcher(func, MatchBehaviour.RejectOnMatch);

        // Act & Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.RejectOnMatch);
    }

    [Fact]
    public void FuncMatcher_GetCSharpCodeArguments_For_String_Should_Return_Correct_Code()
    {
        // Arrange
        Func<string?, bool> func = s => s == "test";
        var matcher = new FuncMatcher(func);

        // Act
        var code = matcher.GetCSharpCodeArguments();

        // Assert
        code.Should().Be("new FuncMatcher(/* Func<string?, bool> function */, WireMock.Matchers.MatchBehaviour.AcceptOnMatch)");
    }

    [Fact]
    public void FuncMatcher_GetCSharpCodeArguments_For_Bytes_Should_Return_Correct_Code()
    {
        // Arrange
        Func<byte[]?, bool> func = b => b != null;
        var matcher = new FuncMatcher(func);

        // Act
        var code = matcher.GetCSharpCodeArguments();

        // Assert
        code.Should().Be("new FuncMatcher(/* Func<byte[]?, bool> function */, WireMock.Matchers.MatchBehaviour.AcceptOnMatch)");
    }

    [Fact]
    public void FuncMatcher_With_RejectOnMatch_For_String_Should_Invert_Result_When_True()
    {
        // Arrange
        Func<string?, bool> func = s => s == "test";
        var matcher = new FuncMatcher(func, MatchBehaviour.RejectOnMatch);

        // Act
        var result = matcher.IsMatch("test");

        // Assert
        result.IsPerfect().Should().BeFalse();
    }

    [Fact]
    public void FuncMatcher_With_RejectOnMatch_For_String_Should_Invert_Result_When_False()
    {
        // Arrange
        Func<string?, bool> func = s => s == "test";
        var matcher = new FuncMatcher(func, MatchBehaviour.RejectOnMatch);

        // Act
        var result = matcher.IsMatch("other");

        // Assert
        result.IsPerfect().Should().BeTrue();
    }

    [Fact]
    public void FuncMatcher_With_RejectOnMatch_For_Bytes_Should_Invert_Result_When_True()
    {
        // Arrange
        Func<byte[]?, bool> func = b => b != null && b.Length > 0;
        var matcher = new FuncMatcher(func, MatchBehaviour.RejectOnMatch);

        // Act
        var result = matcher.IsMatch(new byte[] { 1, 2, 3 });

        // Assert
        result.IsPerfect().Should().BeFalse();
    }

    [Fact]
    public void FuncMatcher_With_RejectOnMatch_For_Bytes_Should_Invert_Result_When_False()
    {
        // Arrange
        Func<byte[]?, bool> func = b => b != null && b.Length > 0;
        var matcher = new FuncMatcher(func, MatchBehaviour.RejectOnMatch);

        // Act
        var result = matcher.IsMatch(new byte[0]);

        // Assert
        result.IsPerfect().Should().BeTrue();
    }

    [Fact]
    public void FuncMatcher_For_String_IsMatch_With_Integer_Input_Should_Return_Mismatch()
    {
        // Arrange
        Func<string?, bool> func = s => s == "test";
        var matcher = new FuncMatcher(func);

        // Act
        var result = matcher.IsMatch(42);

        // Assert
        result.IsPerfect().Should().BeFalse();
    }

    [Fact]
    public void FuncMatcher_For_Bytes_IsMatch_With_Integer_Input_Should_Return_Mismatch()
    {
        // Arrange
        Func<byte[]?, bool> func = b => b != null;
        var matcher = new FuncMatcher(func);

        // Act
        var result = matcher.IsMatch(42);

        // Assert
        result.IsPerfect().Should().BeFalse();
    }

    [Fact]
    public void FuncMatcher_For_String_With_Empty_String_Should_Work()
    {
        // Arrange
        Func<string?, bool> func = s => string.IsNullOrEmpty(s);
        var matcher = new FuncMatcher(func);

        // Act
        var result1 = matcher.IsMatch("");
        var result2 = matcher.IsMatch("test");

        // Assert
        result1.IsPerfect().Should().BeTrue();
        result2.IsPerfect().Should().BeFalse();
    }

    [Fact]
    public void FuncMatcher_For_Bytes_With_Empty_Array_Should_Work()
    {
        // Arrange
        Func<byte[]?, bool> func = b => b != null && b.Length == 0;
        var matcher = new FuncMatcher(func);

        // Act
        var result1 = matcher.IsMatch(new byte[0]);
        var result2 = matcher.IsMatch(new byte[] { 1 });

        // Assert
        result1.IsPerfect().Should().BeTrue();
        result2.IsPerfect().Should().BeFalse();
    }

    [Fact]
    public void FuncMatcher_For_String_With_Complex_Logic_Should_Work()
    {
        // Arrange
        Func<string?, bool> func = s => s != null && s.Length > 3 && s.StartsWith("t") && s.EndsWith("t");
        var matcher = new FuncMatcher(func);

        // Act
        var result1 = matcher.IsMatch("test");
        var result2 = matcher.IsMatch("tart");
        var result3 = matcher.IsMatch("tes");
        var result4 = matcher.IsMatch("best");

        // Assert
        result1.IsPerfect().Should().BeTrue();
        result2.IsPerfect().Should().BeTrue();
        result3.IsPerfect().Should().BeFalse();
        result4.IsPerfect().Should().BeFalse();
    }

    [Fact]
    public void FuncMatcher_For_Bytes_With_Specific_Byte_Check_Should_Work()
    {
        // Arrange
        Func<byte[]?, bool> func = b => b != null && b.Length > 0 && b[0] == 0xFF;
        var matcher = new FuncMatcher(func);

        // Act
        var result1 = matcher.IsMatch(new byte[] { 0xFF, 0x00 });
        var result2 = matcher.IsMatch(new byte[] { 0x00, 0xFF });

        // Assert
        result1.IsPerfect().Should().BeTrue();
        result2.IsPerfect().Should().BeFalse();
    }
}
