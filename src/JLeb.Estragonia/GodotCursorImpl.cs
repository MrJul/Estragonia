using Avalonia.Platform;
using GdCursorShape = Godot.Control.CursorShape;

namespace JLeb.Estragonia;

/// <summary>A standard cursor, represented by a <see cref="GdCursorShape"/> enum value.</summary>
internal sealed class GodotStandardCursorImpl : ICursorImpl {

	public GdCursorShape CursorShape { get; }

	public GodotStandardCursorImpl(GdCursorShape cursorShape)
		=> CursorShape = cursorShape;

	public override string ToString()
		=> CursorShape.ToString();

	public void Dispose() {
	}

}
