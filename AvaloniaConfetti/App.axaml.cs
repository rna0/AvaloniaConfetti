using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace AvaloniaConfetti;

public class App : Application
{
    private TrayIcon? _trayIcon;
    private SettingsWindow? _settingsWindow;

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
            _trayIcon = BuildTrayIcon(desktop);
            _trayIcon.IsVisible = true;
            desktop.ShutdownRequested += (_, _) =>
            {
                _trayIcon.IsVisible = false;
                _trayIcon.Dispose();
                _trayIcon = null;
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private TrayIcon BuildTrayIcon(IClassicDesktopStyleApplicationLifetime desktop)
    {
        var exitMenuItem = new NativeMenuItem("Exit");
        exitMenuItem.Click += (_, _) => desktop.Shutdown();

        var settingsMenuItem = new NativeMenuItem("Settings");
        settingsMenuItem.Click += (_, _) => OpenSettingsWindow();

        var moveMonitorMenuItem = new NativeMenuItem("Move to next monitor");
        moveMonitorMenuItem.Click += (_, _) => MoveToNextMonitor(desktop);

        var nativeMenu = new NativeMenu();
        nativeMenu.Items.Add(settingsMenuItem);
        nativeMenu.Items.Add(moveMonitorMenuItem);
        nativeMenu.Items.Add(exitMenuItem);

        return new TrayIcon
        {
            Icon = new WindowIcon("Assets/AppIcon.ico"),
            ToolTipText = "AvaloniaConfetti",
            Menu = nativeMenu
        };
    }

    private void OpenSettingsWindow()
    {
        if (_settingsWindow is { IsVisible: true })
        {
            _settingsWindow.Activate();
            return;
        }

        _settingsWindow = new SettingsWindow();
        _settingsWindow.Closed += (_, _) => _settingsWindow = null;
        _settingsWindow.Show();
    }

    private static void MoveToNextMonitor(IClassicDesktopStyleApplicationLifetime desktop)
    {
        if (desktop.MainWindow is not { } mainWindow) return;
        var screens = mainWindow.Screens?.All;
        if (screens == null || screens.Count == 0)
        {
            var primary = mainWindow.Screens?.Primary;
            if (primary != null)
                screens = [primary];
            else
                return;
        }

        var currentScreen = screens.FirstOrDefault(s => s.Bounds.Contains(mainWindow.Position));
        int nextIndex;
        if (currentScreen == null)
        {
            nextIndex = 0;
        }
        else
        {
            var currentIndex = screens.ToList().IndexOf(currentScreen);
            nextIndex = (currentIndex + 1) % screens.Count;
        }

        var nextScreen = screens.ElementAt(nextIndex);
        mainWindow.Position = nextScreen.Bounds.Position;
        mainWindow.WindowState = WindowState.Maximized;
    }
}