using Avalonia;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Platform;

namespace JLeb.Estragonia;

/// <summary>Contains extensions methods for <see cref="AppBuilder"/> related to Godot.</summary>
public static class AppBuilderExtensions {

	public static AppBuilder UseGodot(this AppBuilder builder)
		=> builder
			.UseSkia()
			.AfterPlatformServicesSetup(_ =>
				AvaloniaLocator.CurrentMutable
					.Bind<IClipboard>().ToConstant(new GodotClipboard())
					.Bind<ICursorFactory>().ToConstant(new GodotCursorFactory())
					.Bind<IKeyboardDevice>().ToConstant(new KeyboardDevice())
					.Bind<IMouseDevice>().ToConstant(new MouseDevice())
					.Bind<IPlatformIconLoader>().ToConstant(new StubPlatformIconLoader())
					.Bind<IPlatformGraphics>().ToConstant(new GodotVkPlatformGraphics())
					.Bind<IPlatformSettings>().ToConstant(new GodotPlatformSettings())
			)
			.AfterSetup(_ =>
				AvaloniaLocator.CurrentMutable
					.Bind<IKeyboardNavigationHandler>().ToTransient<GodotKeyboardNavigationHandler>()
			);

}
