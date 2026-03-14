using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace AvaloniaConfetti;

public partial class SettingsWindow : Window
{
    private readonly string _customEnvPath;
    private readonly string _defaultEnvPath;
    private Dictionary<string, string> _settings = new();

    public static event Action? SettingsChanged;

    // Named controls (assigned in InitializeControls)
    private Slider _particleCountSlider = null!;
    private Slider _angleSlider = null!;
    private Slider _spreadSlider = null!;
    private Slider _startVelocitySlider = null!;
    private Slider _confettiPerSecondSlider = null!;
    private Slider _decaySlider = null!;
    private Slider _gravitySlider = null!;
    private Slider _driftSlider = null!;
    private Slider _ticksSlider = null!;
    private Slider _scalarSlider = null!;
    private Slider _originXSlider = null!;
    private Slider _originYSlider = null!;

    private TextBlock _particleCountValue = null!;
    private TextBlock _angleValue = null!;
    private TextBlock _spreadValue = null!;
    private TextBlock _startVelocityValue = null!;
    private TextBlock _confettiPerSecondValue = null!;
    private TextBlock _decayValue = null!;
    private TextBlock _gravityValue = null!;
    private TextBlock _driftValue = null!;
    private TextBlock _ticksValue = null!;
    private TextBlock _scalarValue = null!;
    private TextBlock _originXValue = null!;
    private TextBlock _originYValue = null!;

    private ToggleSwitch _flatToggle = null!;
    private CheckBox _squareCheck = null!;
    private CheckBox _circleCheck = null!;
    private CheckBox _starCheck = null!;
    private TextBox _colorsInput = null!;
    private WrapPanel _colorSwatchPanel = null!;

    public SettingsWindow()
    {
        InitializeComponent();

        _customEnvPath = Path.Combine(ConfettiConfigLoader.AppBase, "custom.env");
        _defaultEnvPath = Path.Combine(ConfettiConfigLoader.AppBase, "confetti.env");

        ResolveControls();
        WireSliderValueDisplays();
        WireColorSwatchUpdates();

        LoadSettings();
        PopulateControls();

        var saveButton = this.FindControl<Button>("SaveButton");
        if (saveButton != null) saveButton.Click += SaveButton_Click;

        var resetButton = this.FindControl<Button>("ResetButton");
        if (resetButton != null) resetButton.Click += ResetButton_Click;
    }

    private void ResolveControls()
    {
        _particleCountSlider = this.FindControl<Slider>("ParticleCountSlider")!;
        _angleSlider = this.FindControl<Slider>("AngleSlider")!;
        _spreadSlider = this.FindControl<Slider>("SpreadSlider")!;
        _startVelocitySlider = this.FindControl<Slider>("StartVelocitySlider")!;
        _confettiPerSecondSlider = this.FindControl<Slider>("ConfettiPerSecondSlider")!;
        _decaySlider = this.FindControl<Slider>("DecaySlider")!;
        _gravitySlider = this.FindControl<Slider>("GravitySlider")!;
        _driftSlider = this.FindControl<Slider>("DriftSlider")!;
        _ticksSlider = this.FindControl<Slider>("TicksSlider")!;
        _scalarSlider = this.FindControl<Slider>("ScalarSlider")!;
        _originXSlider = this.FindControl<Slider>("OriginXSlider")!;
        _originYSlider = this.FindControl<Slider>("OriginYSlider")!;

        _particleCountValue = this.FindControl<TextBlock>("ParticleCountValue")!;
        _angleValue = this.FindControl<TextBlock>("AngleValue")!;
        _spreadValue = this.FindControl<TextBlock>("SpreadValue")!;
        _startVelocityValue = this.FindControl<TextBlock>("StartVelocityValue")!;
        _confettiPerSecondValue = this.FindControl<TextBlock>("ConfettiPerSecondValue")!;
        _decayValue = this.FindControl<TextBlock>("DecayValue")!;
        _gravityValue = this.FindControl<TextBlock>("GravityValue")!;
        _driftValue = this.FindControl<TextBlock>("DriftValue")!;
        _ticksValue = this.FindControl<TextBlock>("TicksValue")!;
        _scalarValue = this.FindControl<TextBlock>("ScalarValue")!;
        _originXValue = this.FindControl<TextBlock>("OriginXValue")!;
        _originYValue = this.FindControl<TextBlock>("OriginYValue")!;

        _flatToggle = this.FindControl<ToggleSwitch>("FlatToggle")!;
        _squareCheck = this.FindControl<CheckBox>("SquareCheck")!;
        _circleCheck = this.FindControl<CheckBox>("CircleCheck")!;
        _starCheck = this.FindControl<CheckBox>("StarCheck")!;
        _colorsInput = this.FindControl<TextBox>("ColorsInput")!;
        _colorSwatchPanel = this.FindControl<WrapPanel>("ColorSwatchPanel")!;
    }

    private void WireSliderValueDisplays()
    {
        // Integer sliders - show whole numbers
        WireSlider(_particleCountSlider, _particleCountValue, 0);
        WireSlider(_angleSlider, _angleValue, 0, "°");
        WireSlider(_spreadSlider, _spreadValue, 0, "°");
        WireSlider(_startVelocitySlider, _startVelocityValue, 0);
        WireSlider(_confettiPerSecondSlider, _confettiPerSecondValue, 0);
        WireSlider(_ticksSlider, _ticksValue, 0);

        // Decimal sliders
        WireSlider(_decaySlider, _decayValue, 2);
        WireSlider(_gravitySlider, _gravityValue, 1);
        WireSlider(_driftSlider, _driftValue, 1);
        WireSlider(_scalarSlider, _scalarValue, 1, "x");
        WireSlider(_originXSlider, _originXValue, 2);
        WireSlider(_originYSlider, _originYValue, 2);
    }

    private static void WireSlider(Slider slider, TextBlock display, int decimals, string suffix = "")
    {
        void Update() => display.Text = slider.Value.ToString($"F{decimals}", CultureInfo.InvariantCulture) + suffix;
        slider.PropertyChanged += (_, args) =>
        {
            if (args.Property.Name == "Value") Update();
        };
        Update();
    }

    private void WireColorSwatchUpdates()
    {
        _colorsInput.PropertyChanged += (_, args) =>
        {
            if (args.Property.Name == "Text") UpdateColorSwatches();
        };
    }

    private void UpdateColorSwatches()
    {
        _colorSwatchPanel.Children.Clear();
        var text = _colorsInput.Text ?? "";
        var hexValues = text.Split(',').Select(c => c.Trim()).Where(c => c.Length > 0);

        foreach (var hex in hexValues)
        {
            try
            {
                var color = Color.Parse(hex);
                var swatch = new Border
                {
                    Width = 28,
                    Height = 28,
                    CornerRadius = new Avalonia.CornerRadius(14),
                    Margin = new Avalonia.Thickness(3),
                    BorderThickness = new Avalonia.Thickness(2),
                    Background = new SolidColorBrush(color)
                };
                // Use a slightly darker/lighter border based on the color
                var borderColor = Color.FromArgb(60, 0, 0, 0);
                swatch.BorderBrush = new SolidColorBrush(borderColor);
                _colorSwatchPanel.Children.Add(swatch);
            }
            catch
            {
                // Show a gray swatch for invalid colors
                var swatch = new Border
                {
                    Width = 28,
                    Height = 28,
                    CornerRadius = new Avalonia.CornerRadius(14),
                    Margin = new Avalonia.Thickness(3),
                    BorderThickness = new Avalonia.Thickness(2),
                    Background = new SolidColorBrush(Color.FromRgb(128, 128, 128)),
                    BorderBrush = new SolidColorBrush(Color.FromRgb(200, 60, 60)),
                    Opacity = 0.5
                };
                _colorSwatchPanel.Children.Add(swatch);
            }
        }
    }

    private void LoadSettings()
    {
        string envPath = ConfettiConfigLoader.ResolveEnvPath();
        _settings = File.ReadAllLines(envPath)
            .Select(line => line.Trim())
            .Where(line => !string.IsNullOrWhiteSpace(line) && !line.StartsWith("#"))
            .Select(line => line.Split('=', 2))
            .Where(parts => parts.Length == 2)
            .ToDictionary(parts => parts[0].Trim(), parts => parts[1].Trim());
    }

    private void PopulateControls()
    {
        // Emission
        _particleCountSlider.Value = GetInt("PARTICLE_COUNT", 50);
        _angleSlider.Value = GetDouble("ANGLE", 90);
        _spreadSlider.Value = GetDouble("SPREAD", 360);
        _startVelocitySlider.Value = GetDouble("START_VELOCITY", 55);
        _confettiPerSecondSlider.Value = GetInt("CONFETTI_PER_SECOND", 70);

        // Physics
        _decaySlider.Value = GetDouble("DECAY", 0.94);
        _gravitySlider.Value = GetDouble("GRAVITY", 0.6);
        _driftSlider.Value = GetDouble("DRIFT", 0);

        // Lifecycle
        _ticksSlider.Value = GetInt("TICKS", 350);

        // Appearance
        _scalarSlider.Value = GetDouble("SCALAR", 1.5);
        _flatToggle.IsChecked = GetBool("FLAT", false);

        // Shapes
        var shapes = GetString("SHAPES", "square,circle").ToLower();
        _squareCheck.IsChecked = shapes.Contains("square");
        _circleCheck.IsChecked = shapes.Contains("circle");
        _starCheck.IsChecked = shapes.Contains("star");

        // Origin
        _originXSlider.Value = GetDouble("ORIGIN_X", 0.5);
        _originYSlider.Value = GetDouble("ORIGIN_Y", 0.8);

        // Colors
        _colorsInput.Text = GetString("COLORS", "#26ccff,#a25afd,#ff5e7e,#88ff5a,#fcff42,#ffa62d,#ff36ff");
        UpdateColorSwatches();
    }

    private double GetDouble(string key, double fallback)
    {
        return _settings.TryGetValue(key, out var val) &&
               double.TryParse(val, NumberStyles.Float, CultureInfo.InvariantCulture, out var d)
            ? d
            : fallback;
    }

    private int GetInt(string key, int fallback)
    {
        return _settings.TryGetValue(key, out var val) && int.TryParse(val, out var i) ? i : fallback;
    }

    private bool GetBool(string key, bool fallback)
    {
        return _settings.TryGetValue(key, out var val) && bool.TryParse(val, out var b) ? b : fallback;
    }

    private string GetString(string key, string fallback)
    {
        return _settings.TryGetValue(key, out var val) && !string.IsNullOrWhiteSpace(val) ? val : fallback;
    }

    private Dictionary<string, string> CollectSettings()
    {
        var result = new Dictionary<string, string>
        {
            // Emission
            ["PARTICLE_COUNT"] = ((int)_particleCountSlider.Value).ToString(),
            ["ANGLE"] = _angleSlider.Value.ToString(CultureInfo.InvariantCulture),
            ["SPREAD"] = _spreadSlider.Value.ToString(CultureInfo.InvariantCulture),
            ["START_VELOCITY"] = _startVelocitySlider.Value.ToString(CultureInfo.InvariantCulture),
            ["CONFETTI_PER_SECOND"] = ((int)_confettiPerSecondSlider.Value).ToString(),

            // Physics
            ["DECAY"] = _decaySlider.Value.ToString("F2", CultureInfo.InvariantCulture),
            ["GRAVITY"] = _gravitySlider.Value.ToString("F1", CultureInfo.InvariantCulture),
            ["DRIFT"] = _driftSlider.Value.ToString("F1", CultureInfo.InvariantCulture),

            // Lifecycle
            ["TICKS"] = ((int)_ticksSlider.Value).ToString(),

            // Appearance
            ["SCALAR"] = _scalarSlider.Value.ToString("F1", CultureInfo.InvariantCulture),
            ["FLAT"] = (_flatToggle.IsChecked == true).ToString(),

            // Origin
            ["ORIGIN_X"] = _originXSlider.Value.ToString("F2", CultureInfo.InvariantCulture),
            ["ORIGIN_Y"] = _originYSlider.Value.ToString("F2", CultureInfo.InvariantCulture),
        };

        // Shapes
        var shapes = new List<string>();
        if (_squareCheck.IsChecked == true) shapes.Add("square");
        if (_circleCheck.IsChecked == true) shapes.Add("circle");
        if (_starCheck.IsChecked == true) shapes.Add("star");
        result["SHAPES"] = shapes.Count > 0 ? string.Join(",", shapes) : "square,circle";

        // Colors
        result["COLORS"] = _colorsInput.Text?.Trim() ?? "#26ccff,#a25afd,#ff5e7e,#88ff5a,#fcff42,#ffa62d,#ff36ff";

        return result;
    }

    private void SaveButton_Click(object? sender, RoutedEventArgs e)
    {
        var settings = CollectSettings();

        // Validate shapes
        var shapeVal = settings["SHAPES"];
        if (string.IsNullOrWhiteSpace(shapeVal))
        {
            ShowError("Please select at least one shape.");
            return;
        }

        // Validate colors
        var colorText = settings["COLORS"];
        var colorParts = colorText.Split(',').Select(c => c.Trim()).Where(c => c.Length > 0).ToList();
        if (colorParts.Count == 0)
        {
            ShowError("Please enter at least one color.");
            return;
        }

        var invalidColors = new List<string>();
        foreach (var hex in colorParts)
        {
            try { Color.Parse(hex); }
            catch { invalidColors.Add(hex); }
        }

        if (invalidColors.Count > 0)
        {
            ShowError($"Invalid color values: {string.Join(", ", invalidColors)}");
            return;
        }

        // Write settings
        var lines = settings.Select(kvp => $"{kvp.Key}={kvp.Value}");
        File.WriteAllLines(_customEnvPath, lines);
        SettingsChanged?.Invoke();
        Close();
    }

    private async void ShowError(string message)
    {
        var errorWindow = new Window
        {
            Title = "Validation Error",
            Width = 380,
            Height = 140,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false,
            Content = new TextBlock
            {
                Text = message,
                Margin = new Avalonia.Thickness(20),
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            }
        };
        await errorWindow.ShowDialog(this);
    }

    private void ResetButton_Click(object? sender, RoutedEventArgs e)
    {
        if (!File.Exists(_defaultEnvPath)) return;

        File.Copy(_defaultEnvPath, _customEnvPath, true);
        LoadSettings();
        PopulateControls();
        SettingsChanged?.Invoke();
    }
}
