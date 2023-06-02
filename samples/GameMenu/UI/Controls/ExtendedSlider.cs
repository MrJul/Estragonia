using System;
using Avalonia.Controls;
using Avalonia.Input;

namespace GameMenu.UI.Controls;

/// <summary>A <see cref="Slider"/> that doesn't handle the Up and Down keys so navigation can occur naturally.</summary>
public class ExtendedSlider : Slider {

	protected override Type StyleKeyOverride
		=> typeof(Slider);

	protected override void OnKeyDown(KeyEventArgs e) {
		if (e is not { KeyModifiers: KeyModifiers.None, Key: Key.Up or Key.Down })
			base.OnKeyDown(e);
	}

}
