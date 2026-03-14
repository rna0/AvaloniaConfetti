using Avalonia.Media;
using Xunit;

namespace AvaloniaConfetti.Tests.Unit;

public class ConfettiConfigTests
{
    [Fact]
    public void Default_ParticleCount_Is50()
    {
        var config = new ConfettiConfig();
        Assert.Equal(50, config.ParticleCount);
    }

    [Fact]
    public void Default_Angle_Is90()
    {
        var config = new ConfettiConfig();
        Assert.Equal(90.0, config.Angle);
    }

    [Fact]
    public void Default_Spread_Is360()
    {
        var config = new ConfettiConfig();
        Assert.Equal(360.0, config.Spread);
    }

    [Fact]
    public void Default_StartVelocity_Is55()
    {
        var config = new ConfettiConfig();
        Assert.Equal(55.0, config.StartVelocity);
    }

    [Fact]
    public void Default_Decay_Is0Point94()
    {
        var config = new ConfettiConfig();
        Assert.Equal(0.94, config.Decay);
    }

    [Fact]
    public void Default_Gravity_Is0Point6()
    {
        var config = new ConfettiConfig();
        Assert.Equal(0.6, config.Gravity);
    }

    [Fact]
    public void Default_Drift_Is0()
    {
        var config = new ConfettiConfig();
        Assert.Equal(0.0, config.Drift);
    }

    [Fact]
    public void Default_Ticks_Is350()
    {
        var config = new ConfettiConfig();
        Assert.Equal(350, config.Ticks);
    }

    [Fact]
    public void Default_Scalar_Is1Point5()
    {
        var config = new ConfettiConfig();
        Assert.Equal(1.5, config.Scalar);
    }

    [Fact]
    public void Default_Flat_IsFalse()
    {
        var config = new ConfettiConfig();
        Assert.False(config.Flat);
    }

    [Fact]
    public void Default_Shapes_ContainsSquareAndCircle()
    {
        var config = new ConfettiConfig();
        Assert.Equal(2, config.Shapes.Count);
        Assert.Contains(ConfettiShape.Square, config.Shapes);
        Assert.Contains(ConfettiShape.Circle, config.Shapes);
    }

    [Fact]
    public void Default_OriginX_Is0Point5()
    {
        var config = new ConfettiConfig();
        Assert.Equal(0.5, config.OriginX);
    }

    [Fact]
    public void Default_OriginY_Is0Point8()
    {
        var config = new ConfettiConfig();
        Assert.Equal(0.8, config.OriginY);
    }

    [Fact]
    public void Default_ConfettiPerSecond_Is70()
    {
        var config = new ConfettiConfig();
        Assert.Equal(70, config.ConfettiPerSecond);
    }

    [Fact]
    public void Default_Colors_Has7Colors()
    {
        var config = new ConfettiConfig();
        Assert.Equal(7, config.Colors.Count);
    }

    [Fact]
    public void Default_Colors_ContainsExpectedValues()
    {
        var config = new ConfettiConfig();
        Assert.Contains(Color.Parse("#26ccff"), config.Colors);
        Assert.Contains(Color.Parse("#a25afd"), config.Colors);
        Assert.Contains(Color.Parse("#ff5e7e"), config.Colors);
        Assert.Contains(Color.Parse("#88ff5a"), config.Colors);
        Assert.Contains(Color.Parse("#fcff42"), config.Colors);
        Assert.Contains(Color.Parse("#ffa62d"), config.Colors);
        Assert.Contains(Color.Parse("#ff36ff"), config.Colors);
    }

    [Fact]
    public void ParticleCount_IsMutable()
    {
        var config = new ConfettiConfig { ParticleCount = 100 };
        Assert.Equal(100, config.ParticleCount);
    }

    [Fact]
    public void Angle_IsMutable()
    {
        var config = new ConfettiConfig { Angle = 180 };
        Assert.Equal(180.0, config.Angle);
    }

    [Fact]
    public void Shapes_IsMutable()
    {
        var config = new ConfettiConfig { Shapes = [ConfettiShape.Star] };
        Assert.Single(config.Shapes);
        Assert.Contains(ConfettiShape.Star, config.Shapes);
    }

    [Fact]
    public void Colors_IsMutable()
    {
        var config = new ConfettiConfig { Colors = [Color.Parse("#ffffff")] };
        Assert.Single(config.Colors);
    }

    [Fact]
    public void AllProperties_CanBeSetAtOnce()
    {
        var config = new ConfettiConfig
        {
            ParticleCount = 10,
            Angle = 45,
            Spread = 90,
            StartVelocity = 20,
            Decay = 0.5,
            Gravity = 2,
            Drift = 1,
            Ticks = 100,
            Scalar = 2,
            Flat = true,
            OriginX = 0.1,
            OriginY = 0.9,
            ConfettiPerSecond = 200
        };
        Assert.Equal(10, config.ParticleCount);
        Assert.Equal(45.0, config.Angle);
        Assert.Equal(90.0, config.Spread);
        Assert.Equal(20.0, config.StartVelocity);
        Assert.Equal(0.5, config.Decay);
        Assert.Equal(2.0, config.Gravity);
        Assert.Equal(1.0, config.Drift);
        Assert.Equal(100, config.Ticks);
        Assert.Equal(2.0, config.Scalar);
        Assert.True(config.Flat);
        Assert.Equal(0.1, config.OriginX);
        Assert.Equal(0.9, config.OriginY);
        Assert.Equal(200, config.ConfettiPerSecond);
    }
}