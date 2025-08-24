using Avalonia.Controls;

namespace AvaloniaConfetti;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        var confettiLayer = this.FindControl<ConfettiLayer>("ConfettiLayer");
        SettingsWindow.SettingsChanged += () => { confettiLayer?.ReloadConfig(); };
    }
}