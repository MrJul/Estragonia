using System;
using Avalonia;
using GameMenu.UI;

namespace GameMenu.Designer;

internal static class Program {

	public static int Main(string[] args)
		=> throw new NotSupportedException("This project isn't meant to be run: it's only for designer support.");

	// Used by designer
	public static AppBuilder BuildAvaloniaApp()
		=> AppBuilder
			.Configure<App>()
			.UseSkia();

}
