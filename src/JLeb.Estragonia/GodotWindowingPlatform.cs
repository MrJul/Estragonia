using System;
using Avalonia.Platform;

namespace JLeb.Estragonia;

internal sealed class GodotWindowingPlatform : IWindowingPlatform {

	public IWindowImpl CreateWindow()
		=> throw new NotImplementedException("Sub windows aren't implemented yet");

	public IWindowImpl CreateEmbeddableWindow()
		=> throw new NotImplementedException("Sub windows aren't implemented yet");

	public ITrayIconImpl? CreateTrayIcon()
		=> null;

}
