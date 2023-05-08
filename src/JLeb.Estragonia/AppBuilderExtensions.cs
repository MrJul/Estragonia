using Avalonia;
using Avalonia.Input;
using Avalonia.Platform;

namespace JLeb.Estragonia;

/// <summary>Contains extensions methods for <see cref="AppBuilder"/> related to Godot.</summary>
public static class AppBuilderExtensions {

	public static AppBuilder UseGodot(this AppBuilder builder)
		=> builder
			.UseSkia()
			.AfterPlatformServicesSetup(_ =>
				AvaloniaLocator.CurrentMutable
					.Bind<IKeyboardDevice>().ToConstant(new KeyboardDevice())
					.Bind<IMouseDevice>().ToConstant(new MouseDevice())
					.Bind<ICursorFactory>().ToConstant(new GodotCursorFactory())
					.Bind<IPlatformGraphics>().ToConstant(new GodotVkPlatformGraphics())
			)
			.AfterSetup(_ =>
				AvaloniaLocator.CurrentMutable
					.Bind<IKeyboardNavigationHandler>().ToTransient<GodotKeyboardNavigationHandler>()
			);

}
