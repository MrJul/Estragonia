using Avalonia.Controls;
using Avalonia.Controls.Embedding;

namespace JLeb.Estragonia;

/// <summary>
/// A <see cref="TopLevel"/> used with Godot.
/// This is implicitly created by <see cref="AvaloniaControl"/>.
/// </summary>
public sealed class GodotTopLevel : EmbeddableControlRoot {

	internal GodotTopLevelImpl Impl { get; }

	internal GodotTopLevel(GodotTopLevelImpl impl)
		: base(impl)
		=> Impl = impl;

}
