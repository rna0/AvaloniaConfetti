using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Media;

namespace AvaloniaConfetti
{
    public class ConfettiManager(ConfettiConfig config)
    {
        private readonly List<Confetti> _confetti = [];
        private readonly Random _rand = new();
        private double _spawnAccumulator;

        public void UpdateAndSpawn(Rect bounds, double secondsPerTick)
        {
            _spawnAccumulator += config.ConfettiPerSecond * secondsPerTick;
            var toSpawn = (int)_spawnAccumulator;
            _spawnAccumulator -= toSpawn;

            for (var i = 0; i < toSpawn; i++)
                SpawnBatch(bounds);

            UpdateAll();
        }

        private void SpawnBatch(Rect bounds)
        {
            var startX = bounds.Width * config.OriginX;
            var startY = bounds.Height * config.OriginY;
            var radAngle = config.Angle * (Math.PI / 180.0);
            var radSpread = config.Spread * (Math.PI / 180.0);

            var colorIndex = _rand.Next(config.Colors.Count);
            var shapeIndex = _rand.Next(config.Shapes.Count);

            var fetti = new Confetti
            {
                X = startX,
                Y = startY,
                Wobble = _rand.NextDouble() * 10,
                WobbleSpeed = Math.Min(0.11, _rand.NextDouble() * 0.1 + 0.05),
                Velocity = (config.StartVelocity * 0.5) + (_rand.NextDouble() * config.StartVelocity),
                Angle2D = -radAngle + ((0.5 * radSpread) - (_rand.NextDouble() * radSpread)),
                TiltAngle = (_rand.NextDouble() * (0.75 - 0.25) + 0.25) * Math.PI,
                Color = config.Colors[colorIndex],
                Shape = config.Shapes[shapeIndex],
                Tick = 0,
                TotalTicks = config.Ticks,
                Decay = config.Decay,
                Drift = config.Drift,
                Random = _rand.NextDouble() + 2,
                TiltSin = 0,
                TiltCos = 0,
                WobbleX = 0,
                WobbleY = 0,
                Gravity = config.Gravity * 3,
                OvalScalar = 0.6,
                Scalar = config.Scalar,
                Flat = config.Flat
            };

            _confetti.Add(fetti);
        }

        private void UpdateAll()
        {
            for (var i = _confetti.Count - 1; i >= 0; i--)
            {
                if (!_confetti[i].Update())
                    _confetti.RemoveAt(i);
            }
        }

        public void RenderAll(DrawingContext context)
        {
            foreach (var c in _confetti)
                c.Render(context);
        }
    }
}