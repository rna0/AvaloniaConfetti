using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;

namespace AvaloniaConfetti
{
    public partial class SettingsWindow : Window
    {
        private readonly Dictionary<string, TextBox> _settingInputs = new();
        private readonly string _customEnvPath;
        private readonly string _defaultEnvPath;
        private Dictionary<string, string> _settings = new();

        public static event Action? SettingsChanged;

        private static readonly HashSet<string> IntKeys = ["PARTICLE_COUNT", "TICKS", "CONFETTI_PER_SECOND"];
        private static readonly HashSet<string> BoolKeys = ["FLAT"];
        private static readonly HashSet<string> StringKeys = ["SHAPES", "COLORS"];

        public SettingsWindow()
        {
            InitializeComponent();
            _customEnvPath = Path.Combine(ConfettiConfigLoader.AppBase, "custom.env");
            _defaultEnvPath = Path.Combine(ConfettiConfigLoader.AppBase, "confetti.env");
            LoadSettings();
            BuildSettingsControls();
            var saveButton = this.FindControl<Button>("SaveButton");
            if (saveButton != null)
                saveButton.Click += SaveButton_Click;
            var resetButton = this.FindControl<Button>("ResetButton");
            if (resetButton != null)
                resetButton.Click += ResetButton_Click;
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

        private void BuildSettingsControls()
        {
            var panel = this.FindControl<StackPanel>("SettingsPanel");
            if (panel == null) return;
            panel.Children.Clear();
            foreach (var kvp in _settings)
            {
                var displayName = kvp.Key.ToLower().Replace('_', ' ');
                var label = new TextBlock
                {
                    Text = displayName,
                    Margin = new Thickness(0, 8, 0, 0),
                    Width = 200,
                    VerticalAlignment = VerticalAlignment.Center
                };
                var input = new TextBox
                {
                    Text = kvp.Value,
                    Width = 250,
                    VerticalAlignment = VerticalAlignment.Center
                };
                _settingInputs[kvp.Key] = input;
                var row = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8,
                    VerticalAlignment = VerticalAlignment.Center
                };
                row.Children.Add(label);
                row.Children.Add(input);
                panel.Children.Add(row);
            }
        }

        private static bool IsValidValue(string key, string value)
        {
            if (IntKeys.Contains(key))
                return int.TryParse(value, out _);

            if (BoolKeys.Contains(key))
                return bool.TryParse(value, out _);

            if (StringKeys.Contains(key))
                return !string.IsNullOrWhiteSpace(value);

            return double.TryParse(value, out _);
        }

        private void SaveButton_Click(object? sender, RoutedEventArgs e)
        {
            var errors = new List<string>();
            foreach (var key in _settingInputs.Keys)
            {
                var text = _settingInputs[key].Text ?? "";
                if (!IsValidValue(key, text))
                    errors.Add(key.ToLower().Replace('_', ' '));
                else
                    _settings[key] = text;
            }

            if (errors.Count > 0)
            {
                var errorWindow = new Window
                {
                    Title = "Validation Error",
                    Width = 350,
                    Height = 150,
                    Content = new TextBlock
                    {
                        Text = $"Invalid values for: {string.Join(", ", errors)}",
                        Margin = new Thickness(16),
                        TextWrapping = Avalonia.Media.TextWrapping.Wrap
                    }
                };
                errorWindow.ShowDialog(this);
                return;
            }

            var lines = _settings.Select(kvp => $"{kvp.Key}={kvp.Value}");
            File.WriteAllLines(_customEnvPath, lines);
            SettingsChanged?.Invoke();
            Close();
        }

        private void ResetButton_Click(object? sender, RoutedEventArgs e)
        {
            if (File.Exists(_defaultEnvPath))
            {
                File.Copy(_defaultEnvPath, _customEnvPath, true);
                LoadSettings();
                BuildSettingsControls();
                SettingsChanged?.Invoke();
            }
        }
    }
}