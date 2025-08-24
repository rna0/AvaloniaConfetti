using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using System;

namespace AvaloniaConfetti;

public partial class App : Application
{
    private TrayIcon? _trayIcon;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
            desktop.MainWindow.Show();
            _trayIcon = new TrayIcon
            {
                Icon = new WindowIcon("Assets/AppIcon.ico"),
                ToolTipText = "AvaloniaConfetti"
            };
            _trayIcon.Clicked += (_, _) =>
            {
                desktop.Shutdown();
            };
            _trayIcon.IsVisible = true;
        }

        base.OnFrameworkInitializationCompleted();
    }
}