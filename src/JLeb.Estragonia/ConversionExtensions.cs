using Avalonia;
using Godot;

namespace JLeb.Estragonia;

/// <summary>Contains extensions methods to convert between Godot and Avalonia types.</summary>
public static class ConversionExtensions {

	public static Size ToAvaloniaSize(this Vector2 source)
		=> new(source.X, source.Y);

	public static Point ToAvaloniaPoint(this Vector2 source)
		=> new(source.X, source.Y);

}
