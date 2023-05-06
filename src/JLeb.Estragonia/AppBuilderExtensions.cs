using Avalonia;
using Avalonia.Platform;

namespace JLeb.Estragonia;

/// <summary>Contains extensions methods for <see cref="AppBuilder"/> related to Godot.</summary>
public static class AppBuilderExtensions {

	public static AppBuilder UseGodot(this AppBuilder builder)
		=> builder
			.UseSkia()
			.AfterPlatformServicesSetup(
				_ => {
					var platformGraphics = new GodotVkPlatformGraphics();
					AvaloniaLocator.CurrentMutable.Bind<IPlatformGraphics>().ToConstant(platformGraphics);
				}
			);

}
