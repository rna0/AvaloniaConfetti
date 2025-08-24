using System;
using System.Numerics;
using Avalonia.Media;
using Avalonia;

namespace AvaloniaConfetti
{
    public class Confetti(
        Vector2 position,
        Vector2 velocity,
        double sheer,
        double sheerVelocity,
        double size,
        Color color,
        double rotation,
        double rotationVelocity)
    {
        private Vector2 _velocity = velocity;
        private Vector2 Position { get; set; } = position;

        private double Sheer { get; set; } = sheer;
        private double SheerVelocity { get; set; } = sheerVelocity;
        private Color Color { get; set; } = color;
        private double Size { get; set; } = size;
        private double Rotation { get; set; } = rotation;
        private double RotationVelocity { get; set; } = rotationVelocity;

        public void Update(double gravity, double minSheer, double maxSheer)
        {
            Position += _velocity;
            _velocity.Y += (float)gravity;
            Sheer += SheerVelocity;
            if (Sheer > maxSheer || Sheer < minSheer)
                SheerVelocity = -SheerVelocity;
            Rotation += RotationVelocity;
        }

        public bool IsOutOfBounds(Rect bounds, double margin)
        {
            return Position.Y > bounds.Height || Position.X < -margin || Position.X > bounds.Width + margin;
        }

        public void Render(DrawingContext context)
        {
            var rect = new Rect(Position.X, Position.Y, Size, Size / 2.0);
            var center = rect.Center;
            var rotate = Matrix.CreateRotation(Rotation * Math.PI / 180.0, center);
            var skew = Matrix.CreateSkew(Sheer * 0.7, 0.0);
            var transform = rotate * skew;
            using (context.PushTransform(transform))
            {
                context.DrawRectangle(new SolidColorBrush(Color), null, rect);
            }
        }
    }
}
