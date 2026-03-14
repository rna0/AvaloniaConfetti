using System;
using System.IO;
using Avalonia.Media;
using Xunit;

namespace AvaloniaConfetti.Tests.Integration;

public class ConfigLoaderFileTests : IDisposable
{
    private readonly string _tempDir;

    public ConfigLoaderFileTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"confetti_integ_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);
    }

    [Fact]
    public void ResolveEnvPath_ReturnsAPath()
    {
        // ResolveEnvPath uses AppBase which is the process directory
        var path = ConfettiConfigLoader.ResolveEnvPath();
        Assert.False(string.IsNullOrEmpty(path));
        Assert.EndsWith(".env", path);
    }

    [Fact]
    public void RoundTrip_WriteAndLoad()
    {
        var envContent =
            "PARTICLE_COUNT=77\n" +
            "ANGLE=45\n" +
            "SPREAD=120\n" +
            "START_VELOCITY=20\n" +
            "DECAY=0.85\n" +
            "GRAVITY=1.5\n" +
            "DRIFT=0.3\n" +
            "TICKS=150\n" +
            "SCALAR=0.8\n" +
            "FLAT=True\n" +
            "ORIGIN_X=0.1\n" +
            "ORIGIN_Y=0.9\n" +
            "CONFETTI_PER_SECOND=200\n" +
            "SHAPES=star\n" +
            "COLORS=#aabbcc\n";
        var path = Path.Combine(_tempDir, "roundtrip.env");
        File.WriteAllText(path, envContent);
        var config = ConfettiConfigLoader.Load(path);

        Assert.Equal(77, config.ParticleCount);
        Assert.Equal(45.0, config.Angle);
        Assert.Equal(120.0, config.Spread);
        Assert.Equal(20.0, config.StartVelocity);
        Assert.Equal(0.85, config.Decay);
        Assert.Equal(1.5, config.Gravity);
        Assert.Equal(0.3, config.Drift);
        Assert.Equal(150, config.Ticks);
        Assert.Equal(0.8, config.Scalar);
        Assert.True(config.Flat);
        Assert.Equal(0.1, config.OriginX);
        Assert.Equal(0.9, config.OriginY);
        Assert.Equal(200, config.ConfettiPerSecond);
        Assert.Single(config.Shapes);
        Assert.Contains(ConfettiShape.Star, config.Shapes);
        Assert.Single(config.Colors);
        Assert.Contains(Color.Parse("#aabbcc"), config.Colors);
    }

    [Fact]
    public void HotReload_ModifyFile_NewValuesLoaded()
    {
        var path = Path.Combine(_tempDir, "reload.env");
        File.WriteAllText(path, "PARTICLE_COUNT=10\nGRAVITY=1");
        var config1 = ConfettiConfigLoader.Load(path);
        Assert.Equal(10, config1.ParticleCount);
        Assert.Equal(1.0, config1.Gravity);

        // Modify the file
        File.WriteAllText(path, "PARTICLE_COUNT=99\nGRAVITY=5.5");
        var config2 = ConfettiConfigLoader.Load(path);
        Assert.Equal(99, config2.ParticleCount);
        Assert.Equal(5.5, config2.Gravity);
    }

    [Fact]
    public void Load_AllKeysFromConfettiEnv()
    {
        var envContent =
            "PARTICLE_COUNT=30\n" +
            "ANGLE=60\n" +
            "SPREAD=90\n" +
            "START_VELOCITY=50\n" +
            "CONFETTI_PER_SECOND=100\n" +
            "DECAY=0.95\n" +
            "GRAVITY=2\n" +
            "DRIFT=0.1\n" +
            "TICKS=300\n" +
            "SCALAR=1.5\n" +
            "FLAT=False\n" +
            "SHAPES=square,circle,star\n" +
            "ORIGIN_X=0.3\n" +
            "ORIGIN_Y=0.6\n" +
            "COLORS=#ff0000,#00ff00,#0000ff\n";
        var path = Path.Combine(_tempDir, "full.env");
        File.WriteAllText(path, envContent);
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
        Assert.False(config.Flat);
        Assert.Equal(3, config.Shapes.Count);
        Assert.Equal(0.3, config.OriginX);
        Assert.Equal(0.6, config.OriginY);
        Assert.Equal(3, config.Colors.Count);
    }

    [Fact]
    public void Load_FileWithCommentsAndBlankLines()
    {
        var envContent =
            "# Header comment\n" +
            "\n" +
            "PARTICLE_COUNT=42\n" +
            "\n" +
            "# Another comment\n" +
            "GRAVITY=3.14\n" +
            "\n";
        var path = Path.Combine(_tempDir, "comments.env");
        File.WriteAllText(path, envContent);
        var config = ConfettiConfigLoader.Load(path);

        Assert.Equal(42, config.ParticleCount);
        Assert.Equal(3.14, config.Gravity);
        // Unset values should be defaults
        Assert.Equal(90.0, config.Angle);
    }
}