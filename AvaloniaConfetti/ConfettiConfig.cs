using System.Collections.Generic;
using Avalonia.Media;

namespace AvaloniaConfetti
{
    public class ConfettiConfig
    {
        // Emission
        public int ParticleCount { get; set; } = 50;
        public double Angle { get; set; } = 90;
        public double Spread { get; set; } = 360;
        public double StartVelocity { get; set; } = 55;

        // Physics
        public double Decay { get; set; } = 0.94;
        public double Gravity { get; set; } = 0.6;
        public double Drift { get; set; } = 0;

        // Lifecycle
        public int Ticks { get; set; } = 350;

        // Appearance
        public double Scalar { get; set; } = 1.5;
        public bool Flat { get; set; } = false;
        public List<ConfettiShape> Shapes { get; set; } = [ConfettiShape.Square, ConfettiShape.Circle];

        // Origin (0-1 normalized coordinates)
        public double OriginX { get; set; } = 0.5;
        public double OriginY { get; set; } = 0.8;

        // Continuous mode
        public int ConfettiPerSecond { get; set; } = 70;

        // Colors (canvas-confetti defaults)
        public List<Color> Colors { get; set; } =
        [
            Color.Parse("#26ccff"),
            Color.Parse("#a25afd"),
            Color.Parse("#ff5e7e"),
            Color.Parse("#88ff5a"),
            Color.Parse("#fcff42"),
            Color.Parse("#ffa62d"),
            Color.Parse("#ff36ff")
        ];
    }
}