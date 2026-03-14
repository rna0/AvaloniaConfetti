using System;
using Avalonia.Media;
using Xunit;

namespace AvaloniaConfetti.Tests.Unit;

public class ConfettiTests
{
    private static Confetti CreateFlat(double velocity = 10, double angle2D = 0,
        double drift = 0, double gravity = 1, double decay = 1, double scalar = 1,
        int totalTicks = 100)
    {
        return new Confetti
        {
            X = 100, Y = 100,
            Velocity = velocity, Angle2D = angle2D,
            Drift = drift, Gravity = gravity, Decay = decay,
            Scalar = scalar, Flat = true,
            Tick = 0, TotalTicks = totalTicks,
            Color = Colors.Red, Shape = ConfettiShape.Square,
            Wobble = 5, WobbleSpeed = 0.1, TiltAngle = 1,
            TiltSin = 0.5, TiltCos = 0.5, Random = 2.5
        };
    }

    private static Confetti CreateNonFlat(int totalTicks = 100)
    {
        return new Confetti
        {
            X = 100, Y = 100,
            Velocity = 10, Angle2D = 0,
            Drift = 0, Gravity = 1, Decay = 0.9,
            Scalar = 1, Flat = false,
            Tick = 0, TotalTicks = totalTicks,
            Wobble = 0, WobbleSpeed = 0.08,
            TiltAngle = 0.5, TiltSin = 0, TiltCos = 0,
            Random = 2.5, Color = Colors.Red,
            Shape = ConfettiShape.Circle
        };
    }

    // --- Flat mode (deterministic) ---

    [Fact]
    public void Update_Flat_PositionUpdatedByVelocityAndAngle()
    {
        var c = CreateFlat(velocity: 10, angle2D: 0, drift: 0, gravity: 0);
        var startX = c.X;
        c.Update();
        // cos(0) = 1, so X should increase by velocity
        Assert.Equal(startX + 10, c.X, 5);
    }

    [Fact]
    public void Update_Flat_GravityAffectsY()
    {
        var c = CreateFlat(velocity: 0, gravity: 5);
        var startY = c.Y;
        c.Update();
        Assert.Equal(startY + 5, c.Y, 5);
    }

    [Fact]
    public void Update_Flat_DriftAffectsX()
    {
        var c = CreateFlat(velocity: 0, drift: 3);
        var startX = c.X;
        c.Update();
        Assert.Equal(startX + 3, c.X, 5);
    }

    [Fact]
    public void Update_Flat_VelocityDecays()
    {
        var c = CreateFlat(velocity: 10, decay: 0.5);
        c.Update();
        Assert.Equal(5.0, c.Velocity, 5);
    }

    [Fact]
    public void Update_Flat_WobbleIsZero()
    {
        var c = CreateFlat();
        c.Update();
        Assert.Equal(0, c.Wobble);
    }

    [Fact]
    public void Update_Flat_TiltSinCosAreZero()
    {
        var c = CreateFlat();
        c.Update();
        Assert.Equal(0, c.TiltSin);
        Assert.Equal(0, c.TiltCos);
    }

    [Fact]
    public void Update_Flat_RandomIsOne()
    {
        var c = CreateFlat();
        c.Update();
        Assert.Equal(1.0, c.Random);
    }

    [Fact]
    public void Update_Flat_WobbleXY_SetToPositionPlusScalar10()
    {
        var c = CreateFlat(velocity: 0, gravity: 0, scalar: 2);
        c.Update();
        Assert.Equal(c.X + 20, c.WobbleX, 5); // 10 * scalar = 20
        Assert.Equal(c.Y + 20, c.WobbleY, 5);
    }

    // --- Non-Flat mode ---

    [Fact]
    public void Update_NonFlat_WobbleIncrements()
    {
        var c = CreateNonFlat();
        var startWobble = c.Wobble;
        var wobbleSpeed = c.WobbleSpeed;
        c.Update();
        Assert.Equal(startWobble + wobbleSpeed, c.Wobble, 10);
    }

    [Fact]
    public void Update_NonFlat_TiltAngleIncrements()
    {
        var c = CreateNonFlat();
        var startTilt = c.TiltAngle;
        c.Update();
        Assert.Equal(startTilt + 0.1, c.TiltAngle, 10);
    }

    [Fact]
    public void Update_NonFlat_RandomInRange2To3()
    {
        var c = CreateNonFlat();
        c.Update();
        Assert.InRange(c.Random, 2.0, 3.0);
    }

    // --- Tick lifecycle ---

    [Fact]
    public void Update_ReturnsTrueWhileAlive()
    {
        var c = CreateFlat(totalTicks: 5);
        Assert.True(c.Update()); // tick 1 of 5
        Assert.True(c.Update()); // tick 2 of 5
        Assert.True(c.Update()); // tick 3 of 5
        Assert.True(c.Update()); // tick 4 of 5
    }

    [Fact]
    public void Update_ReturnsFalseWhenDead()
    {
        var c = CreateFlat(totalTicks: 2);
        Assert.True(c.Update());  // tick 1 of 2
        Assert.False(c.Update()); // tick 2 of 2 -> dead
    }

    [Fact]
    public void Update_TotalTicks1_DiesImmediately()
    {
        var c = CreateFlat(totalTicks: 1);
        Assert.False(c.Update()); // tick 1 of 1 -> dead
    }

    [Fact]
    public void Progress_ReflectsTickRatio()
    {
        var c = CreateFlat(totalTicks: 10);
        Assert.Equal(0.0, c.Progress);
        c.Update();
        Assert.Equal(0.1, c.Progress, 5);
    }

    // --- Edge cases ---

    [Fact]
    public void Update_ZeroVelocity_PositionOnlyChangedByDriftAndGravity()
    {
        var c = CreateFlat(velocity: 0, drift: 1, gravity: 2);
        var startX = c.X;
        var startY = c.Y;
        c.Update();
        Assert.Equal(startX + 1, c.X, 5);
        Assert.Equal(startY + 2, c.Y, 5);
    }

    [Fact]
    public void Update_ZeroDecay_VelocityBecomesZero()
    {
        var c = CreateFlat(velocity: 10, decay: 0);
        c.Update();
        Assert.Equal(0, c.Velocity);
    }
}
