using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace ConfettiApp;

public class App : Application
{
    private TrayIcon? _trayIcon;

    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var windowIcon = new WindowIcon("Assets/AppIcon.ico");
            desktop.MainWindow = new MainWindow { Icon = windowIcon };

            _trayIcon = new TrayIcon
            {
                Icon = windowIcon,
                ToolTipText = "Confetti App",
                IsVisible = true
            };
            _trayIcon.Clicked += TrayIcon_Clicked;
            desktop.Exit += OnExit;
            desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void TrayIcon_Clicked(object? sender, EventArgs e)
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Shutdown();
        }
    }

    private void OnExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
    {
        _trayIcon?.Dispose();
    }
}