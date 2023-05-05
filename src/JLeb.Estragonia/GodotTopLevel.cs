using Avalonia.Controls.Embedding;

namespace JLeb.Estragonia;

public sealed class GodotTopLevel : EmbeddableControlRoot {

	internal GodotTopLevelImpl Impl { get; }

	internal GodotTopLevel(GodotTopLevelImpl impl)
		: base(impl)
		=> Impl = impl;

}
