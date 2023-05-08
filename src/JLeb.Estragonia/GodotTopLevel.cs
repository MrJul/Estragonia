using Avalonia.Controls;
using Avalonia.Controls.Embedding;
using Avalonia.Input;

namespace JLeb.Estragonia;

/// <summary>
/// A <see cref="TopLevel"/> used with Godot.
/// This is implicitly created by <see cref="AvaloniaControl"/>.
/// </summary>
public sealed class GodotTopLevel : EmbeddableControlRoot {

	internal GodotTopLevelImpl Impl { get; }

	static GodotTopLevel()
		// TopLevel has Cycle navigation mode but we want the focus to be able to leave Avalonia to return back to godot: use Continue
		=> KeyboardNavigation.TabNavigationProperty.OverrideDefaultValue<GodotTopLevel>(KeyboardNavigationMode.Continue);

	internal GodotTopLevel(GodotTopLevelImpl impl)
		: base(impl)
		=> Impl = impl;

}
