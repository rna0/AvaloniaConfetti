using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Linq;

namespace AvaloniaConfetti;

public class App : Application
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
            var exitMenuItem = new NativeMenuItem("Exit");
            exitMenuItem.Click += (_, _) => desktop.Shutdown();
            var settingsMenuItem = new NativeMenuItem("Settings");
            settingsMenuItem.Click += (_, _) =>
            {
                var settingsWindow = new SettingsWindow();
                settingsWindow.Show();
            };
            var moveMonitorMenuItem = new NativeMenuItem("Move to next monitor");
            moveMonitorMenuItem.Click += (_, _) =>
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
            };
            var nativeMenu = new NativeMenu();
            nativeMenu.Items.Add(settingsMenuItem);
            nativeMenu.Items.Add(moveMonitorMenuItem);
            nativeMenu.Items.Add(exitMenuItem);
            _trayIcon = new TrayIcon
            {
                Icon = new WindowIcon("Assets/AppIcon.ico"),
                ToolTipText = "AvaloniaConfetti",
                Menu = nativeMenu
            };
            _trayIcon.IsVisible = true;
        }

        base.OnFrameworkInitializationCompleted();
    }
}