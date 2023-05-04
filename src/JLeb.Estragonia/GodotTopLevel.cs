using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Styling;

namespace JLeb.Estragonia;

public sealed class GodotTopLevel : TopLevel, IStyleable, IFocusScope, IDisposable {

	internal GodotTopLevelImpl Impl { get; }

	Type IStyleable.StyleKey
		=> typeof(GodotTopLevel);

	internal GodotTopLevel(GodotTopLevelImpl impl)
		: base(impl)
		=> Impl = impl;

	public void Prepare() {
		EnsureInitialized();
		ApplyTemplate();
		LayoutManager.ExecuteInitialLayoutPass();
	}

	private void EnsureInitialized() {
		if (IsInitialized)
			return;

		BeginInit();
		EndInit();
	}

	protected override Size MeasureOverride(Size availableSize) {
		var clientSize = PlatformImpl?.ClientSize ?? default;
		base.MeasureOverride(clientSize);
		return clientSize;
	}

	public void Dispose()
		=> Impl.Dispose();

}
