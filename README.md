<p align="center">
  <img src="AvaloniaConfetti/Assets/AppIcon.ico" alt="AvaloniaConfetti" width="128" />
</p>

<h1 align="center">AvaloniaConfetti</h1>

<p align="center">
  A lightweight, cross-platform desktop confetti overlay built with <a href="https://avaloniaui.net/">Avalonia UI</a> and .NET 8.
</p>

<p align="center">
  <a href="https://github.com/rna0/AvaloniaConfetti/releases"><img src="https://img.shields.io/github/v/release/rna0/AvaloniaConfetti?style=flat-square" alt="Release" /></a>
  <a href="https://github.com/rna0/AvaloniaConfetti/actions"><img src="https://img.shields.io/github/actions/workflow/status/rna0/AvaloniaConfetti/release.yml?style=flat-square" alt="Build" /></a>
  <a href="https://github.com/rna0/AvaloniaConfetti/blob/master/LICENSE"><img src="https://img.shields.io/github/license/rna0/AvaloniaConfetti?style=flat-square" alt="License" /></a>
</p>

---

AvaloniaConfetti renders a real-time particle system on a transparent, always-on-top fullscreen window. Confetti shoots from configurable points, arcs toward a target, and falls under gravity with rotation and sheer effects. It lives in the system tray and stays out of your way.

## Features

- **Real-time particle physics** -- gravity, velocity, rotation, and skew animation per particle
- **Fully configurable** -- spawn rate, strength, spread, gravity, size, colors, shoot points, and more
- **System tray integration** -- settings, monitor switching, and exit from the tray icon
- **Multi-monitor support** -- move the overlay to any connected display
- **Hot-reload settings** -- changes apply instantly without restarting
- **Transparent overlay** -- click-through fullscreen window that sits on top of everything
- **Cross-platform** -- runs on Windows, Linux, and macOS via Avalonia

## Installation

Download the latest release for your platform from the [Releases](https://github.com/rna0/AvaloniaConfetti/releases) page.

| Platform | Archive |
|----------|---------|
| Windows (x64) | `AvaloniaConfetti-win-x64.zip` |
| Linux (x64) | `AvaloniaConfetti-linux-x64.zip` |
| macOS (x64) | `AvaloniaConfetti-osx-x64.zip` |
| macOS (Apple Silicon) | `AvaloniaConfetti-osx-arm64.zip` |

Extract and run the executable. No installer required.

## Build from Source

```bash
git clone https://github.com/rna0/AvaloniaConfetti.git
cd AvaloniaConfetti
dotnet build -c Release
dotnet run --project AvaloniaConfetti
```

Requires [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0).

## Configuration

Settings are stored in a simple key-value `.env` file. Edit via the Settings window (right-click the tray icon) or directly in `custom.env` alongside the executable.

| Parameter | Default | Description |
|-----------|---------|-------------|
| `CONFETTI_PER_SECOND` | `150` | Particles spawned per second |
| `CONFETTI_MIN_STRENGTH` | `25.0` | Minimum launch velocity |
| `CONFETTI_MAX_STRENGTH` | `40.0` | Maximum launch velocity |
| `CONFETTI_SHOOT_POINTS` | `0,100;100,100` | Spawn positions (x,y percentages; semicolon-separated) |
| `CONFETTI_TARGET_POINT` | `50,0` | Aim target (x,y percentage) |
| `CONFETTI_GRAVITY` | `0.5` | Downward acceleration |
| `CONFETTI_SPREAD_FACTOR` | `0.1` | Horizontal trajectory randomization |
| `CONFETTI_VERTICAL_RANDOMNESS` | `0.02` | Vertical trajectory randomization |
| `CONFETTI_SIZE_MIN` / `SIZE_MAX` | `10.0` / `20.0` | Particle width range |
| `CONFETTI_MIN_ROTATION_VELOCITY` / `MAX` | `-0.5` / `0.5` | Spin speed range |
| `CONFETTI_MIN_SHEER` / `MAX_SHEER` | `-0.1` / `0.1` | Skew bounds |
| `CONFETTI_OUT_OF_BOUNDS_MARGIN` | `32.0` | Extra pixels before particle cleanup |

## Tech Stack

- [.NET 8](https://dotnet.microsoft.com/) -- runtime and SDK
- [Avalonia UI 11](https://avaloniaui.net/) -- cross-platform XAML UI framework
- Custom 2D particle engine with per-frame physics updates at ~60 fps

## Support

If you find this project useful, consider buying me a coffee.

<a href="https://buymeacoffee.com/rna0">
  <img src="https://img.buymeacoffee.com/button-api/?text=Buy me a coffee&emoji=&slug=rna0&button_colour=FFDD00&font_colour=000000&font_family=Cookie&outline_colour=000000&coffee_colour=ffffff" alt="Buy Me A Coffee" />
</a>

## License

This project is open source. See the [LICENSE](LICENSE) file for details.