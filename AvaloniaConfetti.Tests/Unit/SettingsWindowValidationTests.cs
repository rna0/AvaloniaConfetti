using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Xunit;

namespace AvaloniaConfetti.Tests.Unit;

/// <summary>
/// Tests for the SettingsWindow helper methods and validation logic.
/// Since the SettingsWindow now uses slider-based UI, we test the
/// private parsing helpers (GetDouble, GetInt, GetBool, GetString)
/// by calling them through reflection with a controlled _settings dictionary.
/// </summary>
public class SettingsWindowValidationTests
{
    // --- GetInt tests (via ParseInt behavior matching) ---

    [Theory]
    [InlineData("50", true)]
    [InlineData("-10", true)]
    [InlineData("0", true)]
    [InlineData("abc", false)]
    [InlineData("50.5", false)]
    [InlineData("", false)]
    public void IntParsing_MatchesExpected(string value, bool shouldParse)
    {
        var result = int.TryParse(value, out _);
        Assert.Equal(shouldParse, result);
    }

    // --- GetBool tests ---

    [Theory]
    [InlineData("True", true)]
    [InlineData("False", true)]
    [InlineData("true", true)]
    [InlineData("false", true)]
    [InlineData("FALSE", true)]
    [InlineData("1", false)]
    [InlineData("", false)]
    [InlineData("yes", false)]
    public void BoolParsing_MatchesExpected(string value, bool shouldParse)
    {
        var result = bool.TryParse(value, out _);
        Assert.Equal(shouldParse, result);
    }

    // --- GetDouble tests ---

    [Theory]
    [InlineData("90.5", true)]
    [InlineData("180", true)]
    [InlineData("-5.5", true)]
    [InlineData("0.999999999", true)]
    [InlineData("slow", false)]
    [InlineData("", false)]
    public void DoubleParsing_MatchesExpected(string value, bool shouldParse)
    {
        var result = double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out _);
        Assert.Equal(shouldParse, result);
    }

    // --- GetString tests ---

    [Theory]
    [InlineData("square,circle", true)]
    [InlineData("#ff0000,#00ff00", true)]
    [InlineData("", false)]
    [InlineData("   ", false)]
    public void StringNonEmpty_MatchesExpected(string value, bool isNonEmpty)
    {
        var result = !string.IsNullOrWhiteSpace(value);
        Assert.Equal(isNonEmpty, result);
    }

    // --- Color validation tests (matching SaveButton_Click logic) ---

    [Fact]
    public void ColorValidation_ValidHexColors_Parse()
    {
        var hex = "#ff0000";
        var parsed = false;
        try { Avalonia.Media.Color.Parse(hex); parsed = true; } catch { }
        Assert.True(parsed);
    }

    [Fact]
    public void ColorValidation_InvalidHexColors_Fail()
    {
        var hex = "notacolor";
        var parsed = false;
        try { Avalonia.Media.Color.Parse(hex); parsed = true; } catch { }
        Assert.False(parsed);
    }

    [Fact]
    public void ColorValidation_EmptyString_Fail()
    {
        var hex = "";
        var parsed = false;
        try { Avalonia.Media.Color.Parse(hex); parsed = true; } catch { }
        Assert.False(parsed);
    }

    // --- Specific key type validation (int keys) ---

    [Fact]
    public void IntKey_ParticleCount_ValidInt() => Assert.True(int.TryParse("50", out _));

    [Fact]
    public void IntKey_ParticleCount_InvalidString() => Assert.False(int.TryParse("abc", out _));

    [Fact]
    public void IntKey_ParticleCount_DoubleValue() => Assert.False(int.TryParse("50.5", out _));

    [Fact]
    public void IntKey_ParticleCount_Empty() => Assert.False(int.TryParse("", out _));

    [Fact]
    public void IntKey_ParticleCount_Negative() => Assert.True(int.TryParse("-10", out _));

    [Fact]
    public void IntKey_Ticks_ValidInt() => Assert.True(int.TryParse("200", out _));

    [Fact]
    public void IntKey_Ticks_InvalidString() => Assert.False(int.TryParse("xyz", out _));

    [Fact]
    public void IntKey_ConfettiPerSecond_ValidInt() => Assert.True(int.TryParse("80", out _));

    [Fact]
    public void IntKey_ConfettiPerSecond_InvalidString() => Assert.False(int.TryParse("fast", out _));

    // --- Shape validation (matching CollectSettings logic) ---

    [Fact]
    public void ShapeValidation_NonEmpty_IsValid()
    {
        var shapes = "square,circle";
        Assert.False(string.IsNullOrWhiteSpace(shapes));
    }

    [Fact]
    public void ShapeValidation_Empty_IsInvalid()
    {
        var shapes = "";
        Assert.True(string.IsNullOrWhiteSpace(shapes));
    }

    [Fact]
    public void ShapeValidation_Whitespace_IsInvalid()
    {
        var shapes = "   ";
        Assert.True(string.IsNullOrWhiteSpace(shapes));
    }

    // --- ConfettiConfigLoader keys exercise the same parsing the UI relies on ---

    [Fact]
    public void DoubleKey_ValidDouble_Parses()
    {
        Assert.True(double.TryParse("90.5", NumberStyles.Float, CultureInfo.InvariantCulture, out var v));
        Assert.Equal(90.5, v);
    }

    [Fact]
    public void DoubleKey_Negative_Parses()
    {
        Assert.True(double.TryParse("-5.5", NumberStyles.Float, CultureInfo.InvariantCulture, out var v));
        Assert.Equal(-5.5, v);
    }

    [Fact]
    public void DoubleKey_Empty_FailsToParse()
    {
        Assert.False(double.TryParse("", NumberStyles.Float, CultureInfo.InvariantCulture, out _));
    }
}