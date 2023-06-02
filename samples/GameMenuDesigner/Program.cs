using System;
using Avalonia;
using GameMenu.UI;

internal static class Program {

	public static int Main(string[] args)
		=> throw new NotSupportedException("This project isn't meant to be run: it's only for designer support.");

	public static AppBuilder BuildAvaloniaApp()
		=> AppBuilder
			.Configure<App>()
			.UseSkia();

}
