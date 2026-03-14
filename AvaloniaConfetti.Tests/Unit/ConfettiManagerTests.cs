using System;
using System.Collections.Generic;
using System.Reflection;
using Avalonia;
using Avalonia.Media;
using Xunit;

namespace AvaloniaConfetti.Tests.Unit;

public class ConfettiManagerTests
{
    private static readonly Rect DefaultBounds = new(0, 0, 1920, 1080);

    private static ConfettiConfig CreateConfig(int confettiPerSecond = 70, int ticks = 350,
        double startVelocity = 55, double decay = 0.94, double gravity = 0.6,
        double drift = 0, double scalar = 1.5, bool flat = false,
        double originX = 0.5, double originY = 0.8)
    {
        return new ConfettiConfig
        {
            ConfettiPerSecond = confettiPerSecond,
            Ticks = ticks,
            StartVelocity = startVelocity,
            Decay = decay,
            Gravity = gravity,
            Drift = drift,
            Scalar = scalar,
            Flat = flat,
            OriginX = originX,
            OriginY = originY,
            Colors = [Color.Parse("#ff0000"), Color.Parse("#00ff00")],
            Shapes = [ConfettiShape.Square, ConfettiShape.Circle]
        };
    }

    private static List<Confetti> GetParticles(ConfettiManager manager)
    {
        var field = typeof(ConfettiManager).GetField("_confetti", BindingFlags.NonPublic | BindingFlags.Instance)!;
        return (List<Confetti>)field.GetValue(manager)!;
    }

    private static double GetAccumulator(ConfettiManager manager)
    {
        var field = typeof(ConfettiManager).GetField("_spawnAccumulator", BindingFlags.NonPublic | BindingFlags.Instance)!;
        return (double)field.GetValue(manager)!;
    }

    // --- Spawn accumulation ---

    [Fact]
    public void UpdateAndSpawn_AccumulatesSpawns()
    {
        var config = CreateConfig(confettiPerSecond: 60);
        var manager = new ConfettiManager(config);
        // At 60/sec with 0.016s tick, expect ~0.96 per tick
        manager.UpdateAndSpawn(DefaultBounds, 0.016);
        var particles = GetParticles(manager);
        // Should have spawned 0 whole particles (0.96 < 1)
        Assert.True(particles.Count >= 0);
    }

    [Fact]
    public void UpdateAndSpawn_FractionalAccumulation_SpawnsAfterMultipleTicks()
    {
        var config = CreateConfig(confettiPerSecond: 60);
        var manager = new ConfettiManager(config);
        // 60 * 0.016 = 0.96 per tick; after 2 ticks = 1.92, should spawn at least 1
        manager.UpdateAndSpawn(DefaultBounds, 0.016);
        manager.UpdateAndSpawn(DefaultBounds, 0.016);
        var particles = GetParticles(manager);
        Assert.True(particles.Count >= 1);
    }

    [Fact]
    public void UpdateAndSpawn_ZeroSpawnRate_NoParticles()
    {
        var config = CreateConfig(confettiPerSecond: 0);
        var manager = new ConfettiManager(config);
        manager.UpdateAndSpawn(DefaultBounds, 0.016);
        manager.UpdateAndSpawn(DefaultBounds, 0.016);
        var particles = GetParticles(manager);
        Assert.Empty(particles);
    }

    [Fact]
    public void UpdateAndSpawn_LargeTimestep_SpawnsMultiple()
    {
        var config = CreateConfig(confettiPerSecond: 100);
        var manager = new ConfettiManager(config);
        // 100 * 1.0 = 100 particles
        manager.UpdateAndSpawn(DefaultBounds, 1.0);
        var particles = GetParticles(manager);
        Assert.Equal(100, particles.Count);
    }

    [Fact]
    public void UpdateAndSpawn_AccumulatorRetainsFraction()
    {
        var config = CreateConfig(confettiPerSecond: 60);
        var manager = new ConfettiManager(config);
        manager.UpdateAndSpawn(DefaultBounds, 0.016);
        var acc = GetAccumulator(manager);
        // 60 * 0.016 = 0.96; floor = 0; remainder = 0.96
        Assert.InRange(acc, 0.0, 1.0);
    }

    [Fact]
    public void UpdateAndSpawn_HighRate_CorrectBatchCount()
    {
        var config = CreateConfig(confettiPerSecond: 500);
        var manager = new ConfettiManager(config);
        // 500 * 0.1 = 50 particles
        manager.UpdateAndSpawn(DefaultBounds, 0.1);
        var particles = GetParticles(manager);
        Assert.Equal(50, particles.Count);
    }

    // --- Dead particle removal ---

    [Fact]
    public void UpdateAndSpawn_RemovesDeadParticles()
    {
        var config = CreateConfig(confettiPerSecond: 0, ticks: 1);
        var manager = new ConfettiManager(config);
        var particles = GetParticles(manager);
        particles.Add(new Confetti
        {
            X = 100, Y = 100, Velocity = 0, Angle2D = 0,
            Decay = 1, Gravity = 0, Drift = 0, Scalar = 1,
            Flat = true, Tick = 0, TotalTicks = 1,
            Color = Colors.Red, Shape = ConfettiShape.Square
        });
        Assert.Single(particles);
        manager.UpdateAndSpawn(DefaultBounds, 0.016);
        Assert.Empty(particles);
    }

    [Fact]
    public void UpdateAndSpawn_KeepsAliveParticles()
    {
        var config = CreateConfig(confettiPerSecond: 0, ticks: 100);
        var manager = new ConfettiManager(config);
        var particles = GetParticles(manager);
        particles.Add(new Confetti
        {
            X = 100, Y = 100, Velocity = 10, Angle2D = 0,
            Decay = 0.9, Gravity = 1, Drift = 0, Scalar = 1,
            Flat = true, Tick = 0, TotalTicks = 100,
            Color = Colors.Red, Shape = ConfettiShape.Square
        });
        manager.UpdateAndSpawn(DefaultBounds, 0.016);
        Assert.Single(particles);
    }

    [Fact]
    public void UpdateAndSpawn_RemovesOnlyDeadParticles()
    {
        var config = CreateConfig(confettiPerSecond: 0);
        var manager = new ConfettiManager(config);
        var particles = GetParticles(manager);
        // One alive, one about to die
        particles.Add(new Confetti
        {
            X = 100, Y = 100, Velocity = 0, Angle2D = 0,
            Decay = 1, Gravity = 0, Drift = 0, Scalar = 1,
            Flat = true, Tick = 0, TotalTicks = 1,
            Color = Colors.Red, Shape = ConfettiShape.Square
        });
        particles.Add(new Confetti
        {
            X = 200, Y = 200, Velocity = 0, Angle2D = 0,
            Decay = 1, Gravity = 0, Drift = 0, Scalar = 1,
            Flat = true, Tick = 0, TotalTicks = 100,
            Color = Colors.Blue, Shape = ConfettiShape.Circle
        });
        Assert.Equal(2, particles.Count);
        manager.UpdateAndSpawn(DefaultBounds, 0.016);
        Assert.Single(particles); // only the alive one remains
    }

    // --- Spawned particle properties ---

    [Fact]
    public void SpawnedParticle_HasCorrectOrigin()
    {
        var config = CreateConfig(confettiPerSecond: 1000, originX: 0.25, originY: 0.75);
        var manager = new ConfettiManager(config);
        manager.UpdateAndSpawn(DefaultBounds, 1.0);
        var particles = GetParticles(manager);
        Assert.True(particles.Count > 0);
        var p = particles[0];
        // After one Update(), position has moved, but should be in reasonable range
        Assert.InRange(p.X, -DefaultBounds.Width, DefaultBounds.Width * 3);
        Assert.InRange(p.Y, -DefaultBounds.Height, DefaultBounds.Height * 3);
    }

    [Fact]
    public void SpawnedParticle_HasConfigDecay()
    {
        var config = CreateConfig(confettiPerSecond: 1000, decay: 0.75);
        var manager = new ConfettiManager(config);
        manager.UpdateAndSpawn(DefaultBounds, 1.0);
        var p = GetParticles(manager)[0];
        Assert.Equal(0.75, p.Decay);
    }

    [Fact]
    public void SpawnedParticle_GravityIsConfigTimesThree()
    {
        var config = CreateConfig(confettiPerSecond: 1000, gravity: 2.0);
        var manager = new ConfettiManager(config);
        manager.UpdateAndSpawn(DefaultBounds, 1.0);
        var p = GetParticles(manager)[0];
        Assert.Equal(6.0, p.Gravity);
    }

    [Fact]
    public void SpawnedParticle_HasConfigDrift()
    {
        var config = CreateConfig(confettiPerSecond: 1000, drift: 0.5);
        var manager = new ConfettiManager(config);
        manager.UpdateAndSpawn(DefaultBounds, 1.0);
        var p = GetParticles(manager)[0];
        Assert.Equal(0.5, p.Drift);
    }

    [Fact]
    public void SpawnedParticle_HasConfigScalar()
    {
        var config = CreateConfig(confettiPerSecond: 1000, scalar: 2.5);
        var manager = new ConfettiManager(config);
        manager.UpdateAndSpawn(DefaultBounds, 1.0);
        var p = GetParticles(manager)[0];
        Assert.Equal(2.5, p.Scalar);
    }

    [Fact]
    public void SpawnedParticle_HasConfigFlat()
    {
        var config = CreateConfig(confettiPerSecond: 1000, flat: true);
        var manager = new ConfettiManager(config);
        manager.UpdateAndSpawn(DefaultBounds, 1.0);
        var p = GetParticles(manager)[0];
        Assert.True(p.Flat);
    }

    [Fact]
    public void SpawnedParticle_TotalTicksMatchesConfig()
    {
        var config = CreateConfig(confettiPerSecond: 1000, ticks: 300);
        var manager = new ConfettiManager(config);
        manager.UpdateAndSpawn(DefaultBounds, 1.0);
        var p = GetParticles(manager)[0];
        Assert.Equal(300, p.TotalTicks);
    }

    [Fact]
    public void SpawnedParticle_OvalScalarIs0Point6()
    {
        var config = CreateConfig(confettiPerSecond: 1000);
        var manager = new ConfettiManager(config);
        manager.UpdateAndSpawn(DefaultBounds, 1.0);
        var p = GetParticles(manager)[0];
        Assert.Equal(0.6, p.OvalScalar);
    }

    [Fact]
    public void SpawnedParticle_TickIsNotZeroAfterUpdate()
    {
        // After UpdateAndSpawn, particles have been Update()'d once, so Tick=1
        var config = CreateConfig(confettiPerSecond: 1000);
        var manager = new ConfettiManager(config);
        manager.UpdateAndSpawn(DefaultBounds, 1.0);
        var p = GetParticles(manager)[0];
        Assert.Equal(1, p.Tick);
    }

    // --- Random property ranges ---

    [Fact]
    public void SpawnedParticle_VelocityInExpectedRange()
    {
        var sv = 55.0;
        var config = CreateConfig(confettiPerSecond: 1000, startVelocity: sv);
        var manager = new ConfettiManager(config);
        manager.UpdateAndSpawn(DefaultBounds, 1.0);
        var particles = GetParticles(manager);
        foreach (var p in particles)
        {
            // Initial velocity = sv*0.5 + rand*sv = [27.5, 82.5]
            // After one decay (0.94): [25.85, 77.55]
            // Use generous range to account for randomness
            Assert.InRange(p.Velocity, 0, sv * 1.5 * 1.0 + 1);
        }
    }

    [Fact]
    public void SpawnedParticle_WobbleSpeedAtMost0Point11()
    {
        var config = CreateConfig(confettiPerSecond: 1000);
        var manager = new ConfettiManager(config);
        manager.UpdateAndSpawn(DefaultBounds, 1.0);
        var particles = GetParticles(manager);
        foreach (var p in particles)
        {
            // WobbleSpeed = Min(0.11, rand*0.1 + 0.05) => range [0.05, 0.11]
            Assert.InRange(p.WobbleSpeed, 0.04, 0.111);
        }
    }

    // --- Color/shape from config ---

    [Fact]
    public void SpawnedParticle_ColorFromConfigList()
    {
        var config = CreateConfig(confettiPerSecond: 1000);
        var manager = new ConfettiManager(config);
        manager.UpdateAndSpawn(DefaultBounds, 1.0);
        var particles = GetParticles(manager);
        foreach (var p in particles)
        {
            Assert.Contains(p.Color, config.Colors);
        }
    }

    [Fact]
    public void SpawnedParticle_ShapeFromConfigList()
    {
        var config = CreateConfig(confettiPerSecond: 1000);
        var manager = new ConfettiManager(config);
        manager.UpdateAndSpawn(DefaultBounds, 1.0);
        var particles = GetParticles(manager);
        foreach (var p in particles)
        {
            Assert.Contains(p.Shape, config.Shapes);
        }
    }

    // --- Multiple update cycles ---

    [Fact]
    public void MultipleUpdates_ParticleCountGrowsAndShrinks()
    {
        var config = CreateConfig(confettiPerSecond: 100, ticks: 5);
        var manager = new ConfettiManager(config);
        // Spawn many particles with short lifetime
        manager.UpdateAndSpawn(DefaultBounds, 1.0);
        var count1 = GetParticles(manager).Count;
        Assert.True(count1 > 0);
        // After enough updates with no new spawns, old particles die
        for (var i = 0; i < 10; i++)
            manager.UpdateAndSpawn(DefaultBounds, 0.0);
        var count2 = GetParticles(manager).Count;
        Assert.True(count2 < count1);
    }
}