using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Avalonia.Media;

namespace AvaloniaConfetti
{
    public static class ConfettiConfigLoader
    {
        public static string AppBase { get; } =
            Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName)
            ?? AppContext.BaseDirectory;

        public static string ResolveEnvPath()
        {
            var customEnvPath = Path.Combine(AppBase, "custom.env");
            var defaultEnvPath = Path.Combine(AppBase, "confetti.env");
            return File.Exists(customEnvPath) ? customEnvPath : defaultEnvPath;
        }

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
                    case "PARTICLE_COUNT":
                        config.ParticleCount = int.Parse(value);
                        break;
                    case "ANGLE":
                        config.Angle = double.Parse(value);
                        break;
                    case "SPREAD":
                        config.Spread = double.Parse(value);
                        break;
                    case "START_VELOCITY":
                        config.StartVelocity = double.Parse(value);
                        break;
                    case "DECAY":
                        config.Decay = double.Parse(value);
                        break;
                    case "GRAVITY":
                        config.Gravity = double.Parse(value);
                        break;
                    case "DRIFT":
                        config.Drift = double.Parse(value);
                        break;
                    case "TICKS":
                        config.Ticks = int.Parse(value);
                        break;
                    case "SCALAR":
                        config.Scalar = double.Parse(value);
                        break;
                    case "FLAT":
                        config.Flat = bool.Parse(value);
                        break;
                    case "ORIGIN_X":
                        config.OriginX = double.Parse(value);
                        break;
                    case "ORIGIN_Y":
                        config.OriginY = double.Parse(value);
                        break;
                    case "CONFETTI_PER_SECOND":
                        config.ConfettiPerSecond = int.Parse(value);
                        break;
                    case "SHAPES":
                        config.Shapes = ParseShapes(value);
                        break;
                    case "COLORS":
                        config.Colors = ParseColors(value);
                        break;
                }
            }

            return config;
        }

        private static List<ConfettiShape> ParseShapes(string value)
        {
            var shapes = new List<ConfettiShape>();
            foreach (var s in value.Split(',').Select(s => s.Trim().ToLower()))
            {
                switch (s)
                {
                    case "square":
                        shapes.Add(ConfettiShape.Square);
                        break;
                    case "circle":
                        shapes.Add(ConfettiShape.Circle);
                        break;
                    case "star":
                        shapes.Add(ConfettiShape.Star);
                        break;
                }
            }
            return shapes.Count > 0 ? shapes : [ConfettiShape.Square, ConfettiShape.Circle];
        }

        private static List<Color> ParseColors(string value)
        {
            var colors = new List<Color>();
            foreach (var c in value.Split(',').Select(c => c.Trim()))
            {
                try
                {
                    colors.Add(Color.Parse(c));
                }
                catch
                {
                    // skip invalid colors
                }
            }
            return colors.Count > 0 ? colors : new ConfettiConfig().Colors;
        }
    }
}