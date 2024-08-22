using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Threading;

namespace ConfettiApp;

internal class MainWindow : Window
{
    private readonly Random _random = new();
    private readonly List<Ellipse> _confetti = [];
    private readonly Subject<Unit> _updateSubject = new();

    public MainWindow()
    {
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        WindowState = WindowState.Maximized;
        CanResize = false;
        SystemDecorations = SystemDecorations.None;
        ShowInTaskbar = false;
        Topmost = true;
        TransparencyLevelHint = [WindowTransparencyLevel.Transparent];
        Background = Brushes.Transparent;
        ExtendClientAreaToDecorationsHint = true;
        ExtendClientAreaTitleBarHeightHint = -1;

        Icon = new WindowIcon("Assets/AppIcon.ico");

        var canvas = new Canvas();
        Content = canvas;

        StartConfetti(canvas);
    }

    private void StartConfetti(Canvas canvas)
    {
        Observable.Interval(TimeSpan.FromMilliseconds(100)).Subscribe(_ =>
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                var ellipse = CreateConfetti();
                _confetti.Add(ellipse);
                canvas.Children.Add(ellipse);
            });
        });

        _updateSubject.AsObservable().Sample(TimeSpan.FromMilliseconds(16.6)).Subscribe(_ =>
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                foreach (var confetti in _confetti.ToList())
                {
                    Canvas.SetTop(confetti, Canvas.GetTop(confetti) + _random.Next(1, 5));
                    if (Canvas.GetTop(confetti) > ClientSize.Height)
                    {
                        _confetti.Remove(confetti);
                        (Content as Canvas)?.Children.Remove(confetti);
                    }
                }
            });
        });

        Observable.Interval(TimeSpan.FromMilliseconds(16.6)).Subscribe(_ => _updateSubject.OnNext(Unit.Default));
    }

    private Ellipse CreateConfetti()
    {
        return new Ellipse
        {
            Width = _random.Next(5, 15),
            Height = _random.Next(5, 15),
            Fill = new SolidColorBrush(Color.FromRgb((byte)_random.Next(256),
                (byte)_random.Next(256), (byte)_random.Next(256))),
            [Canvas.LeftProperty] = _random.NextDouble() * ClientSize.Width, [Canvas.TopProperty] = 0.0,
        };
    }
}