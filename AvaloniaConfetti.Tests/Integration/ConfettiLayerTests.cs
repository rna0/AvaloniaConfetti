using System;
using System.IO;
using Avalonia;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Themes.Fluent;
using Xunit;

[assembly: AvaloniaTestApplication(typeof(AvaloniaConfetti.Tests.Integration.TestAppBuilder))]

namespace AvaloniaConfetti.Tests.Integration;

public class TestAppBuilder
{
    public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<TestApp>()
        .UseHeadless(new AvaloniaHeadlessPlatformOptions());
}

public class TestApp : Application
{
    public override void Initialize()
    {
        Styles.Add(new FluentTheme());
    }
}

/// <summary>
/// Ensures a confetti.env file exists at AppBase so ConfettiLayer can instantiate.
/// </summary>
public class ConfettiLayerTests : IDisposable
{
    private readonly string _envPath;
    private readonly bool _envExisted;

    public ConfettiLayerTests()
    {
        // ConfettiLayer constructor calls ConfettiConfigLoader.Load(ResolveEnvPath())
        // which requires confetti.env at AppBase. Ensure it exists for tests.
        _envPath = Path.Combine(ConfettiConfigLoader.AppBase, "confetti.env");
        _envExisted = File.Exists(_envPath);
        if (!_envExisted)
        {
            // Write a minimal valid config so ConfettiLayer can load
            Directory.CreateDirectory(ConfettiConfigLoader.AppBase);
            File.WriteAllText(_envPath, "PARTICLE_COUNT=50\nTICKS=200\n");
        }
    }

    public void Dispose()
    {
        // Clean up only if we created the file
        if (!_envExisted && File.Exists(_envPath))
            File.Delete(_envPath);
    }

    [AvaloniaFact]
    public void ConfettiLayer_CanBeInstantiated()
    {
        var layer = new ConfettiLayer();
        Assert.NotNull(layer);
    }

    [AvaloniaFact]
    public void ConfettiLayer_ReloadConfig_DoesNotThrow()
    {
        var layer = new ConfettiLayer();
        var exception = Record.Exception(() => layer.ReloadConfig());
        Assert.Null(exception);
    }

    [AvaloniaFact]
    public void ConfettiLayer_HasManagerAfterConstruction()
    {
        var layer = new ConfettiLayer();
        var field = typeof(ConfettiLayer).GetField("_manager",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
        var manager = field.GetValue(layer);
        Assert.NotNull(manager);
    }
}