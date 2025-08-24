using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;

namespace AvaloniaConfetti
{
    public class ConfettiLayer : Control
    {
        private ConfettiManager _manager;
        private ConfettiConfig _config;
        private DispatcherTimer? _timer;
        private const int WindowLoadDelayMs = 200;
        private const int TimerIntervalMs = 16;

        public ConfettiLayer()
        {
            var customEnvPath = Path.Combine(AppContext.BaseDirectory, "custom.env");
            var defaultEnvPath = Path.Combine(AppContext.BaseDirectory, "confetti.env");
            var envPath = File.Exists(customEnvPath) ? customEnvPath : defaultEnvPath;
            _config = ConfettiConfigLoader.Load(envPath);
            _manager = new ConfettiManager(_config);
            AttachedToVisualTree += (_, _) => OnAttached();
            DetachedFromVisualTree += (_, _) => OnDetached();
        }

        private void StartAnimation()
        {
            if (_timer != null) return;
            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(TimerIntervalMs) };
            _timer.Tick += (_, _) =>
            {
                var secondsPerTick = _timer.Interval.TotalSeconds;
                _manager.UpdateAndSpawn(new Rect(0, 0, Bounds.Width, Bounds.Height), secondsPerTick);
                InvalidateVisual();
            };
            _timer.Start();
        }

        private void OnAttached()
        {
            InvalidateMeasure();
            InvalidateVisual();

            LayoutUpdated += LayoutHandler;
        }

        private void LayoutHandler(object? sender, EventArgs e)
        {
            if (!(Bounds.Width > 0) || !(Bounds.Height > 0)) return;
            _ = HandleLayoutAsync();
        }

        private async Task HandleLayoutAsync()
        {
            try
            {
                await Task.Delay(WindowLoadDelayMs);
                StartAnimation();
                LayoutUpdated -= LayoutHandler;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in HandleLayoutAsync: {ex}");
            }
        }

        private void OnDetached()
        {
            _timer?.Stop();
            _timer = null;
        }

        public void ReloadConfig()
        {
            var customEnvPath = Path.Combine(AppContext.BaseDirectory, "custom.env");
            var defaultEnvPath = Path.Combine(AppContext.BaseDirectory, "confetti.env");
            var envPath = File.Exists(customEnvPath) ? customEnvPath : defaultEnvPath;
            _config = ConfettiConfigLoader.Load(envPath);
            _manager = new ConfettiManager(_config);
        }

        public override void Render(DrawingContext context)
        {
            _manager.RenderAll(context);
        }
    }
}