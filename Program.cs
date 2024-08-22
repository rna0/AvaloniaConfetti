using Avalonia;

namespace ConfettiApp;

internal static class Program
{
    private static void Main(string[] args) => BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

    private static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>().UsePlatformDetect().LogToTrace();
}