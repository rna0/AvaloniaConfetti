using System;
using System.Collections.Generic;
using System.Numerics;
using Avalonia;
using Avalonia.Media;

namespace AvaloniaConfetti
{
    public class ConfettiManager(ConfettiConfig config)
    {
        private readonly List<Confetti> _confetti = [];
        private readonly Random _rand = new();
        private double _confettiAccumulator;

        public void UpdateAndSpawn(Rect bounds, double secondsPerTick)
        {
            _confettiAccumulator += config.ConfettiPerSecond * secondsPerTick;
            int toSpawn = (int)_confettiAccumulator;
            _confettiAccumulator -= toSpawn;
            for (int i = 0; i < toSpawn; i++)
                _confetti.Add(CreateConfetti(bounds));
            UpdateAll(bounds);
        }

        private Confetti CreateConfetti(Rect bounds)
        {
            var percentPoint = config.ShootingPoints[_rand.Next(config.ShootingPoints.Count)];
            float shootX = (float)(bounds.Width * (percentPoint.X / 100f));
            float shootY = (float)(bounds.Height * (percentPoint.Y / 100f));
            var shootPoint = new Vector2(shootX, shootY);
            double centerX = bounds.Width / 2.0;
            double topY = 0.0;
            double spread = (_rand.NextDouble() - 0.5) * bounds.Width * config.SpreadFactor;
            double targetX = centerX + spread;
            double targetY = topY + (_rand.NextDouble() * bounds.Height * config.VerticalRandomness);
            var targetPoint = new Vector2((float)targetX, (float)targetY);
            var toTarget = targetPoint - shootPoint;
            toTarget = Vector2.Normalize(toTarget);
            double speed = _rand.NextDouble() * (config.MaxStrength - config.MinStrength) + config.MinStrength;
            Vector2 velocity = toTarget * (float)speed;
            double sheer = _rand.NextDouble() * (config.MaxSheer - config.MinSheer) + config.MinSheer;
            double sheerVel = _rand.NextDouble() * (config.MaxSheerVelocity - config.MinSheerVelocity) + config.MinSheerVelocity;
            double size = _rand.NextDouble() * (config.SizeMax - config.SizeMin) + config.SizeMin;
            double rot = _rand.NextDouble() * 360.0;
            double rotVel = _rand.NextDouble() * (config.MaxRotationVelocity - config.MinRotationVelocity) + config.MinRotationVelocity;
            Color color = Color.FromRgb((byte)_rand.Next(80, 255), (byte)_rand.Next(80, 255), (byte)_rand.Next(80, 255));
            return new Confetti(shootPoint, velocity, sheer, sheerVel, size, color, rot, rotVel);
        }

        private void UpdateAll(Rect bounds)
        {
            for (var i = _confetti.Count - 1; i >= 0; i--)
            {
                var c = _confetti[i];
                c.Update(config.Gravity, config.MinSheer, config.MaxSheer);
                if (c.IsOutOfBounds(bounds, config.OutOfBoundsMargin))
                    _confetti.RemoveAt(i);
                else
                    _confetti[i] = c;
            }
        }

        public void RenderAll(DrawingContext context)
        {
            foreach (var c in _confetti)
                c.Render(context);
        }
    }
}
