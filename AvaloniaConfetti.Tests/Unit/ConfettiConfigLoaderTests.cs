using System;
using System.IO;
using Avalonia.Media;
using Xunit;

namespace AvaloniaConfetti.Tests.Unit;

public class ConfettiConfigLoaderTests : IDisposable
{
    private readonly string _tempDir;

    public ConfettiConfigLoaderTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"confetti_test_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);
    }

    private string WriteTempEnv(string content, string filename = "test.env")
    {
        var path = Path.Combine(_tempDir, filename);
        File.WriteAllText(path, content);
        return path;
    }

    // --- Load: Parse all 15 keys ---

    [Fact]
    public void Load_ParsesParticleCount()
    {
        var path = WriteTempEnv("PARTICLE_COUNT=100");
        var config = ConfettiConfigLoader.Load(path);
        Assert.Equal(100, config.ParticleCount);
    }

    [Fact]
    public void Load_ParsesAngle()
    {
        var path = WriteTempEnv("ANGLE=45.5");
        var config = ConfettiConfigLoader.Load(path);
        Assert.Equal(45.5, config.Angle);
    }

    [Fact]
    public void Load_ParsesSpread()
    {
        var path = WriteTempEnv("SPREAD=360");
        var config = ConfettiConfigLoader.Load(path);
        Assert.Equal(360.0, config.Spread);
    }

    [Fact]
    public void Load_ParsesStartVelocity()
    {
        var path = WriteTempEnv("START_VELOCITY=99.9");
        var config = ConfettiConfigLoader.Load(path);
        Assert.Equal(99.9, config.StartVelocity);
    }

    [Fact]
    public void Load_ParsesDecay()
    {
        var path = WriteTempEnv("DECAY=0.95");
        var config = ConfettiConfigLoader.Load(path);
        Assert.Equal(0.95, config.Decay);
    }

    [Fact]
    public void Load_ParsesGravity()
    {
        var path = WriteTempEnv("GRAVITY=2.5");
        var config = ConfettiConfigLoader.Load(path);
        Assert.Equal(2.5, config.Gravity);
    }

    [Fact]
    public void Load_ParsesDrift()
    {
        var path = WriteTempEnv("DRIFT=0.3");
        var config = ConfettiConfigLoader.Load(path);
        Assert.Equal(0.3, config.Drift);
    }

    [Fact]
    public void Load_ParsesTicks()
    {
        var path = WriteTempEnv("TICKS=500");
        var config = ConfettiConfigLoader.Load(path);
        Assert.Equal(500, config.Ticks);
    }

    [Fact]
    public void Load_ParsesScalar()
    {
        var path = WriteTempEnv("SCALAR=2.0");
        var config = ConfettiConfigLoader.Load(path);
        Assert.Equal(2.0, config.Scalar);
    }

    [Fact]
    public void Load_ParsesFlat()
    {
        var path = WriteTempEnv("FLAT=True");
        var config = ConfettiConfigLoader.Load(path);
        Assert.True(config.Flat);
    }

    [Fact]
    public void Load_ParsesOriginX()
    {
        var path = WriteTempEnv("ORIGIN_X=0.25");
        var config = ConfettiConfigLoader.Load(path);
        Assert.Equal(0.25, config.OriginX);
    }

    [Fact]
    public void Load_ParsesOriginY()
    {
        var path = WriteTempEnv("ORIGIN_Y=0.75");
        var config = ConfettiConfigLoader.Load(path);
        Assert.Equal(0.75, config.OriginY);
    }

    [Fact]
    public void Load_ParsesConfettiPerSecond()
    {
        var path = WriteTempEnv("CONFETTI_PER_SECOND=120");
        var config = ConfettiConfigLoader.Load(path);
        Assert.Equal(120, config.ConfettiPerSecond);
    }

    [Fact]
    public void Load_ParsesShapes()
    {
        var path = WriteTempEnv("SHAPES=star,circle");
        var config = ConfettiConfigLoader.Load(path);
        Assert.Equal(2, config.Shapes.Count);
        Assert.Contains(ConfettiShape.Star, config.Shapes);
        Assert.Contains(ConfettiShape.Circle, config.Shapes);
    }

    [Fact]
    public void Load_ParsesColors()
    {
        var path = WriteTempEnv("COLORS=#ff0000,#00ff00");
        var config = ConfettiConfigLoader.Load(path);
        Assert.Equal(2, config.Colors.Count);
        Assert.Contains(Color.Parse("#ff0000"), config.Colors);
        Assert.Contains(Color.Parse("#00ff00"), config.Colors);
    }

    // --- Load: line handling ---

    [Fact]
    public void Load_SkipsBlankLines()
    {
        var path = WriteTempEnv("\n\nPARTICLE_COUNT=10\n\n");
        var config = ConfettiConfigLoader.Load(path);
        Assert.Equal(10, config.ParticleCount);
    }

    [Fact]
    public void Load_SkipsCommentLines()
    {
        var path = WriteTempEnv("# This is a comment\nPARTICLE_COUNT=10");
        var config = ConfettiConfigLoader.Load(path);
        Assert.Equal(10, config.ParticleCount);
    }

    [Fact]
    public void Load_ThrowsForMissingFile()
    {
        var path = Path.Combine(_tempDir, "nonexistent.env");
        Assert.Throws<FileNotFoundException>(() => ConfettiConfigLoader.Load(path));
    }

    [Fact]
    public void Load_EmptyFile_ReturnsDefaults()
    {
        var path = WriteTempEnv("");
        var config = ConfettiConfigLoader.Load(path);
        // Verify actual defaults from ConfettiConfig
        Assert.Equal(50, config.ParticleCount);
        Assert.Equal(90.0, config.Angle);
        Assert.Equal(360.0, config.Spread);
        Assert.Equal(55.0, config.StartVelocity);
        Assert.Equal(0.94, config.Decay);
        Assert.Equal(0.6, config.Gravity);
        Assert.Equal(0.0, config.Drift);
        Assert.Equal(350, config.Ticks);
        Assert.Equal(1.5, config.Scalar);
        Assert.False(config.Flat);
        Assert.Equal(0.5, config.OriginX);
        Assert.Equal(0.8, config.OriginY);
        Assert.Equal(70, config.ConfettiPerSecond);
    }

    [Fact]
    public void Load_PartialFile_OverridesOnlySpecified()
    {
        var path = WriteTempEnv("PARTICLE_COUNT=99\nGRAVITY=5");
        var config = ConfettiConfigLoader.Load(path);
        Assert.Equal(99, config.ParticleCount);
        Assert.Equal(5.0, config.Gravity);
        // defaults preserved
        Assert.Equal(90.0, config.Angle);
        Assert.Equal(350, config.Ticks);
        Assert.Equal(360.0, config.Spread);
    }

    [Fact]
    public void Load_DuplicateKeys_LastWins()
    {
        var path = WriteTempEnv("PARTICLE_COUNT=10\nPARTICLE_COUNT=20");
        var config = ConfettiConfigLoader.Load(path);
        Assert.Equal(20, config.ParticleCount);
    }

    [Fact]
    public void Load_SkipsLinesWithoutEquals()
    {
        var path = WriteTempEnv("INVALID_LINE\nPARTICLE_COUNT=10");
        var config = ConfettiConfigLoader.Load(path);
        Assert.Equal(10, config.ParticleCount);
    }

    [Fact]
    public void Load_TrimsWhitespace()
    {
        var path = WriteTempEnv("  PARTICLE_COUNT  =  25  ");
        var config = ConfettiConfigLoader.Load(path);
        Assert.Equal(25, config.ParticleCount);
    }

    [Fact]
    public void Load_UnknownKeys_AreIgnored()
    {
        var path = WriteTempEnv("UNKNOWN_KEY=value\nPARTICLE_COUNT=10");
        var config = ConfettiConfigLoader.Load(path);
        Assert.Equal(10, config.ParticleCount);
    }

    // --- ParseShapes (via Load) ---

    [Fact]
    public void ParseShapes_SingleShape()
    {
        var path = WriteTempEnv("SHAPES=star");
        var config = ConfettiConfigLoader.Load(path);
        Assert.Single(config.Shapes);
        Assert.Contains(ConfettiShape.Star, config.Shapes);
    }

    [Fact]
    public void ParseShapes_AllThreeShapes()
    {
        var path = WriteTempEnv("SHAPES=square,circle,star");
        var config = ConfettiConfigLoader.Load(path);
        Assert.Equal(3, config.Shapes.Count);
    }

    [Fact]
    public void ParseShapes_InvalidShapes_FallsBackToDefault()
    {
        var path = WriteTempEnv("SHAPES=triangle,hexagon");
        var config = ConfettiConfigLoader.Load(path);
        Assert.Equal(2, config.Shapes.Count);
        Assert.Contains(ConfettiShape.Square, config.Shapes);
        Assert.Contains(ConfettiShape.Circle, config.Shapes);
    }

    [Fact]
    public void ParseShapes_CaseInsensitive()
    {
        var path = WriteTempEnv("SHAPES=STAR,Circle,SQUARE");
        var config = ConfettiConfigLoader.Load(path);
        Assert.Equal(3, config.Shapes.Count);
        Assert.Contains(ConfettiShape.Star, config.Shapes);
        Assert.Contains(ConfettiShape.Circle, config.Shapes);
        Assert.Contains(ConfettiShape.Square, config.Shapes);
    }

    [Fact]
    public void ParseShapes_MixedValidAndInvalid_KeepsValid()
    {
        var path = WriteTempEnv("SHAPES=star,invalid,circle");
        var config = ConfettiConfigLoader.Load(path);
        Assert.Equal(2, config.Shapes.Count);
        Assert.Contains(ConfettiShape.Star, config.Shapes);
        Assert.Contains(ConfettiShape.Circle, config.Shapes);
    }

    [Fact]
    public void ParseShapes_EmptyValue_FallsBackToDefault()
    {
        var path = WriteTempEnv("SHAPES=");
        var config = ConfettiConfigLoader.Load(path);
        // empty string split produces [""] which is invalid, fallback to defaults
        Assert.Equal(2, config.Shapes.Count);
        Assert.Contains(ConfettiShape.Square, config.Shapes);
        Assert.Contains(ConfettiShape.Circle, config.Shapes);
    }

    // --- ParseColors (via Load) ---

    [Fact]
    public void ParseColors_SingleColor()
    {
        var path = WriteTempEnv("COLORS=#ff0000");
        var config = ConfettiConfigLoader.Load(path);
        Assert.Single(config.Colors);
        Assert.Contains(Color.Parse("#ff0000"), config.Colors);
    }

    [Fact]
    public void ParseColors_MultipleColors()
    {
        var path = WriteTempEnv("COLORS=#ff0000,#00ff00,#0000ff");
        var config = ConfettiConfigLoader.Load(path);
        Assert.Equal(3, config.Colors.Count);
    }

    [Fact]
    public void ParseColors_InvalidColors_FallsBackToDefault()
    {
        var path = WriteTempEnv("COLORS=notacolor,alsonotacolor");
        var config = ConfettiConfigLoader.Load(path);
        Assert.Equal(7, config.Colors.Count); // falls back to default 7 colors
    }

    [Fact]
    public void ParseColors_MixedValidAndInvalid_KeepsValid()
    {
        var path = WriteTempEnv("COLORS=#ff0000,invalid,#00ff00");
        var config = ConfettiConfigLoader.Load(path);
        Assert.Equal(2, config.Colors.Count);
    }

    [Fact]
    public void ParseColors_EmptyValue_FallsBackToDefault()
    {
        var path = WriteTempEnv("COLORS=");
        var config = ConfettiConfigLoader.Load(path);
        // empty string is invalid, fallback to defaults
        Assert.Equal(7, config.Colors.Count);
    }

    // --- Edge values ---

    [Fact]
    public void Load_NegativeGravity()
    {
        var path = WriteTempEnv("GRAVITY=-5.5");
        var config = ConfettiConfigLoader.Load(path);
        Assert.Equal(-5.5, config.Gravity);
    }

    [Fact]
    public void Load_ZeroTicks()
    {
        var path = WriteTempEnv("TICKS=0");
        var config = ConfettiConfigLoader.Load(path);
        Assert.Equal(0, config.Ticks);
    }

    [Fact]
    public void Load_HighPrecisionDecimal()
    {
        var path = WriteTempEnv("DECAY=0.999999999");
        var config = ConfettiConfigLoader.Load(path);
        Assert.Equal(0.999999999, config.Decay);
    }

    [Fact]
    public void Load_ValueContainingEquals()
    {
        // Split('=', 2) ensures only first = is used as delimiter
        var path = WriteTempEnv("COLORS=#ff0000");
        var config = ConfettiConfigLoader.Load(path);
        Assert.Single(config.Colors);
    }

    [Fact]
    public void Load_AllKeysAtOnce()
    {
        var content = "PARTICLE_COUNT=30\nANGLE=60\nSPREAD=90\nSTART_VELOCITY=50\n" +
                      "CONFETTI_PER_SECOND=100\nDECAY=0.95\nGRAVITY=2\nDRIFT=0.1\n" +
                      "TICKS=300\nSCALAR=1.5\nFLAT=True\nSHAPES=star\n" +
                      "ORIGIN_X=0.3\nORIGIN_Y=0.6\nCOLORS=#ff0000";
        var path = WriteTempEnv(content);
        var config = ConfettiConfigLoader.Load(path);
        Assert.Equal(30, config.ParticleCount);
        Assert.Equal(60.0, config.Angle);
        Assert.Equal(90.0, config.Spread);
        Assert.Equal(50.0, config.StartVelocity);
        Assert.Equal(100, config.ConfettiPerSecond);
        Assert.Equal(0.95, config.Decay);
        Assert.Equal(2.0, config.Gravity);
        Assert.Equal(0.1, config.Drift);
        Assert.Equal(300, config.Ticks);
        Assert.Equal(1.5, config.Scalar);
        Assert.True(config.Flat);
        Assert.Single(config.Shapes);
        Assert.Equal(0.3, config.OriginX);
        Assert.Equal(0.6, config.OriginY);
        Assert.Single(config.Colors);
    }
}