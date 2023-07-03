using System;
using Avalonia;

namespace GameMenu.UI;

internal static class Designer {

	public static int Main()
		=> throw new NotSupportedException("This project isn't meant to be run: it's only for Avalonia designer support.");

	// Used by designer
	public static AppBuilder BuildAvaloniaApp()
		=> AppBuilder
			.Configure<App>()
			.UseSkia();

}
