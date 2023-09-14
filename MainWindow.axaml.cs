using System;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Media.Transformation;
using Avalonia.Remote.Protocol.Input;
using Avalonia.Styling;
using Avalonia.Threading;

namespace AvaloniaConfetti;

public partial class MainWindow : Window
{
    private Random random = new Random();
    private DispatcherTimer timer = new DispatcherTimer();
    private const int NumConfettiPieces = 50;

    public MainWindow()
    {
        InitializeComponent();

        // Initialize the timer to continuously add confetti
        timer.Tick += AddConfetti;
        timer.Interval = TimeSpan.FromMilliseconds(500);
        timer.Start();
    }

    private void AddConfetti(object sender, EventArgs e)
    {
        for (int i = 0; i < NumConfettiPieces; i++)
        {
            var confetti = new Ellipse
            {
                Width = 20,
                Height = 20,
                Fill = new SolidColorBrush(Color.FromRgb((byte)random.Next(256), (byte)random.Next(256),
                    (byte)random.Next(256))),
                Opacity = 0.8,
                Styles =
                {
                    new Style(x => x.OfType<Ellipse>())
                    {
                        Setters =
                        {
                            new Setter(TranslateTransform.YProperty, 10)
                        }
                    }
                },
                Transitions = new Transitions()
                {
                    new DoubleTransition()
                    {
                        Property = TranslateTransform.YProperty,
                        Duration = TimeSpan.FromSeconds(5)
                    }
                },
                RenderTransform = new TranslateTransform
                {
                    X = random.NextDouble() * Width,
                    Y = 200
                }
            };

            MainCanvas.Children.Add(confetti);
        }
    }
}