using System;
using Avalonia.Controls;

namespace AvaloniaConfetti;

public partial class MainWindow : Window
{
    private readonly Action _onSettingsChanged;

    public MainWindow()
    {
        InitializeComponent();
        var confettiLayer = this.FindControl<ConfettiLayer>("ConfettiLayer");
        _onSettingsChanged = () => { confettiLayer?.ReloadConfig(); };
        SettingsWindow.SettingsChanged += _onSettingsChanged;
        Closed += (_, _) => SettingsWindow.SettingsChanged -= _onSettingsChanged;
    }
}