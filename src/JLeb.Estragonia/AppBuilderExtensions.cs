using Avalonia;
using Avalonia.Input;
using Avalonia.Platform;

namespace JLeb.Estragonia;

/// <summary>Contains extensions methods for <see cref="AppBuilder"/> related to Godot.</summary>
public static class AppBuilderExtensions {

	public static AppBuilder UseGodot(this AppBuilder builder)
		=> builder
			.UseSkia()
			.AfterPlatformServicesSetup(
				_ => {
					var locator = AvaloniaLocator.CurrentMutable;
					locator.Bind<IKeyboardDevice>().ToConstant(new KeyboardDevice());
					locator.Bind<IMouseDevice>().ToConstant(new MouseDevice());
					locator.Bind<IPlatformGraphics>().ToConstant(new GodotVkPlatformGraphics());
				}
			);

}
