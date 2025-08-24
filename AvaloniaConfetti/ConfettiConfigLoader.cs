using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace AvaloniaConfetti
{
    public static class ConfettiConfigLoader
    {
        public static ConfettiConfig Load(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"Config file not found: {path}");

            var config = new ConfettiConfig();
            var lines = File.ReadAllLines(path);
            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith('#')) continue;
                var parts = trimmed.Split('=', 2);
                if (parts.Length != 2) continue;
                var key = parts[0].Trim();
                var value = parts[1].Trim();
                switch (key)
                {
                    case "CONFETTI_PER_SECOND":
                        config.ConfettiPerSecond = int.Parse(value);
                        break;
                    case "CONFETTI_MIN_STRENGTH":
                        config.MinStrength = double.Parse(value);
                        break;
                    case "CONFETTI_MAX_STRENGTH":
                        config.MaxStrength = double.Parse(value);
                        break;
                    case "CONFETTI_GRAVITY":
                        config.Gravity = double.Parse(value);
                        break;
                    case "CONFETTI_SPREAD_FACTOR":
                        config.SpreadFactor = double.Parse(value);
                        break;
                    case "CONFETTI_VERTICAL_RANDOMNESS":
                        config.VerticalRandomness = double.Parse(value);
                        break;
                    case "CONFETTI_MIN_SHEER":
                        config.MinSheer = double.Parse(value);
                        break;
                    case "CONFETTI_MAX_SHEER":
                        config.MaxSheer = double.Parse(value);
                        break;
                    case "CONFETTI_MIN_SHEER_VELOCITY":
                        config.MinSheerVelocity = double.Parse(value);
                        break;
                    case "CONFETTI_MAX_SHEER_VELOCITY":
                        config.MaxSheerVelocity = double.Parse(value);
                        break;
                    case "CONFETTI_SIZE_MIN":
                        config.SizeMin = double.Parse(value);
                        break;
                    case "CONFETTI_SIZE_MAX":
                        config.SizeMax = double.Parse(value);
                        break;
                    case "CONFETTI_MIN_ROTATION_VELOCITY":
                        config.MinRotationVelocity = double.Parse(value);
                        break;
                    case "CONFETTI_MAX_ROTATION_VELOCITY":
                        config.MaxRotationVelocity = double.Parse(value);
                        break;
                    case "CONFETTI_OUT_OF_BOUNDS_MARGIN":
                        config.OutOfBoundsMargin = double.Parse(value);
                        break;
                    case "CONFETTI_SHOOT_POINTS":
                        config.ShootingPoints.Clear();
                        var points = value.Split(';');
                        foreach (var pt in points)
                        {
                            var xy = pt.Split(',');
                            // Parse as percentage (0-100)
                            if (xy.Length == 2 && float.TryParse(xy[0], out var x) && float.TryParse(xy[1], out var y))
                            {
                                // Clamp between 0 and 100
                                x = Math.Clamp(x, 0f, 100f);
                                y = Math.Clamp(y, 0f, 100f);
                                config.ShootingPoints.Add(new Vector2(x, y));
                            }
                        }
                        break;
                }
            }
            return config;
        }
    }
}
