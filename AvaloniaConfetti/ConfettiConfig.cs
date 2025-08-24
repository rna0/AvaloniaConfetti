using System.Collections.Generic;
using System.Numerics;

namespace AvaloniaConfetti
{
    public class ConfettiConfig
    {
        public List<Vector2> ShootingPoints { get; } = new List<Vector2>();
        public int ConfettiPerSecond { get; set; }
        public double MinStrength { get; set; }
        public double MaxStrength { get; set; }
        public double Gravity { get; set; }
        public double SpreadFactor { get; set; }
        public double VerticalRandomness { get; set; }
        public double MinSheer { get; set; }
        public double MaxSheer { get; set; }
        public double MinSheerVelocity { get; set; }
        public double MaxSheerVelocity { get; set; }
        public double SizeMin { get; set; }
        public double SizeMax { get; set; }
        public double MinRotationVelocity { get; set; }
        public double MaxRotationVelocity { get; set; }
        public double OutOfBoundsMargin { get; set; }
    }
}
