using Avalonia.Platform;
using Godot;

namespace JLeb.Estragonia;

/// <summary>Implementation of <see cref="IPlatformSettings"/> for Godot.</summary>
internal sealed class GodotPlatformSettings : DefaultPlatformSettings {

	public override PlatformColorValues GetColorValues()
		=> new() {
			ThemeVariant = PlatformThemeVariant.Dark,
			ContrastPreference = ColorContrastPreference.NoPreference,
			AccentColor1 = DisplayServer.GetAccentColor().ToAvaloniaColor()
		};

}
