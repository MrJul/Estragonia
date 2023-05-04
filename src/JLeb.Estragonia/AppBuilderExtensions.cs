using Avalonia;
using Avalonia.Platform;

namespace JLeb.Estragonia;

public static class AppBuilderExtensions {

	public static AppBuilder UseGodot(this AppBuilder builder)
		=> builder.AfterPlatformServicesSetup(_ => {
			var platformGraphics = new GodotVkPlatformGraphics();
			AvaloniaLocator.CurrentMutable.Bind<IPlatformGraphics>().ToConstant(platformGraphics);
		});

}
