using System;
using Avalonia;
using Avalonia.Media;

namespace AvaloniaConfetti
{
    public enum ConfettiShape
    {
        Square,
        Circle,
        Star
    }

    public class Confetti
    {
        public double X;
        public double Y;
        public double Wobble;
        public double WobbleSpeed;
        public double Velocity;
        public double Angle2D;
        public double TiltAngle;
        public double TiltSin;
        public double TiltCos;
        public double WobbleX;
        public double WobbleY;
        public double Random;
        public double Decay;
        public double Drift;
        public double Gravity;
        public double OvalScalar;
        public double Scalar;
        public bool Flat;
        public int Tick;
        public int TotalTicks;
        public Color Color;
        public ConfettiShape Shape;

        private static readonly Random Rand = new();

        public bool Update()
        {
            X += Math.Cos(Angle2D) * Velocity + Drift;
            Y += Math.Sin(Angle2D) * Velocity + Gravity;
            Velocity *= Decay;

            if (Flat)
            {
                Wobble = 0;
                WobbleX = X + (10 * Scalar);
                WobbleY = Y + (10 * Scalar);
                TiltSin = 0;
                TiltCos = 0;
                Random = 1;
            }
            else
            {
                Wobble += WobbleSpeed;
                WobbleX = X + ((10 * Scalar) * Math.Cos(Wobble));
                WobbleY = Y + ((10 * Scalar) * Math.Sin(Wobble));
                TiltAngle += 0.1;
                TiltSin = Math.Sin(TiltAngle);
                TiltCos = Math.Cos(TiltAngle);
                Random = Rand.NextDouble() + 2;
            }

            Tick++;
            return Tick < TotalTicks;
        }

        public double Progress => (double)Tick / TotalTicks;

        public void Render(DrawingContext context)
        {
            var progress = Progress;
            var alpha = (byte)(255 * (1 - progress));
            if (alpha == 0) return;

            var color = Color.FromArgb(alpha, Color.R, Color.G, Color.B);
            var brush = new SolidColorBrush(color);

            var x1 = X + (Random * TiltCos);
            var y1 = Y + (Random * TiltSin);
            var x2 = WobbleX + (Random * TiltCos);
            var y2 = WobbleY + (Random * TiltSin);

            switch (Shape)
            {
                case ConfettiShape.Circle:
                    RenderCircle(context, brush, x1, y1, x2, y2);
                    break;
                case ConfettiShape.Star:
                    RenderStar(context, brush);
                    break;
                default:
                    RenderSquare(context, brush, x1, y1, x2, y2);
                    break;
            }
        }

        private void RenderSquare(DrawingContext context, IBrush brush,
            double x1, double y1, double x2, double y2)
        {
            var geometry = new StreamGeometry();
            using (var ctx = geometry.Open())
            {
                ctx.BeginFigure(new Point(Math.Floor(X), Math.Floor(Y)), true);
                ctx.LineTo(new Point(Math.Floor(WobbleX), Math.Floor(y1)));
                ctx.LineTo(new Point(Math.Floor(x2), Math.Floor(y2)));
                ctx.LineTo(new Point(Math.Floor(x1), Math.Floor(WobbleY)));
                ctx.EndFigure(true);
            }
            context.DrawGeometry(brush, null, geometry);
        }

        private void RenderCircle(DrawingContext context, IBrush brush,
            double x1, double y1, double x2, double y2)
        {
            var radiusX = Math.Abs(x2 - x1) * OvalScalar;
            var radiusY = Math.Abs(y2 - y1) * OvalScalar;
            if (radiusX < 0.5 || radiusY < 0.5) return;

            var rotation = Math.PI / 10.0 * Wobble;
            var center = new Point(X, Y);

            using (context.PushTransform(
                Matrix.CreateTranslation(-X, -Y) *
                Matrix.CreateRotation(rotation) *
                Matrix.CreateTranslation(X, Y)))
            {
                context.DrawEllipse(brush, null, center, radiusX, radiusY);
            }
        }

        private void RenderStar(DrawingContext context, IBrush brush)
        {
            var innerRadius = 4 * Scalar;
            var outerRadius = 8 * Scalar;
            var spikes = 5;
            var step = Math.PI / spikes;
            var rot = Math.PI / 2.0 * 3.0;

            var geometry = new StreamGeometry();
            using (var ctx = geometry.Open())
            {
                var first = true;
                for (var i = 0; i < spikes; i++)
                {
                    var ox = X + Math.Cos(rot) * outerRadius;
                    var oy = Y + Math.Sin(rot) * outerRadius;
                    if (first)
                    {
                        ctx.BeginFigure(new Point(ox, oy), true);
                        first = false;
                    }
                    else
                    {
                        ctx.LineTo(new Point(ox, oy));
                    }
                    rot += step;

                    var ix = X + Math.Cos(rot) * innerRadius;
                    var iy = Y + Math.Sin(rot) * innerRadius;
                    ctx.LineTo(new Point(ix, iy));
                    rot += step;
                }
                ctx.EndFigure(true);
            }
            context.DrawGeometry(brush, null, geometry);
        }
    }
}
