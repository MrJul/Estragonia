using Avalonia;
using Avalonia.Input;

namespace JLeb.Estragonia;

/// <summary>Contains extensions methods for <see cref="AppBuilder"/> related to Godot.</summary>
public static class AppBuilderExtensions {

	public static AppBuilder UseGodot(this AppBuilder builder)
		=> builder
			.UseSkia()
			.UseWindowingSubsystem(GodotPlatform.Initialize)
			.AfterSetup(_ =>
				AvaloniaLocator.CurrentMutable
					.Bind<IKeyboardNavigationHandler>().ToTransient<GodotKeyboardNavigationHandler>()
			);

}
