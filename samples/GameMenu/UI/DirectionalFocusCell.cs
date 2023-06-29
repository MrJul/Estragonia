using Avalonia;
using Avalonia.Controls;

namespace GameMenu.UI;

public sealed class DirectionalFocusCell : AvaloniaObject {

	public static readonly StyledProperty<Control?> ControlProperty =
		AvaloniaProperty.Register<DirectionalFocusCell, Control?>(nameof(Control));

	public static readonly StyledProperty<bool> IsRowDefaultProperty =
		AvaloniaProperty.Register<DirectionalFocusCell, bool>(nameof(IsRowDefault));

	public Control? Control {
		get => GetValue(ControlProperty);
		set => SetValue(ControlProperty, value);
	}

	public bool IsRowDefault {
		get => GetValue(IsRowDefaultProperty);
		set => SetValue(IsRowDefaultProperty, value);
	}

	public Control? TryGetValidControl()
		=> Control is { IsEffectivelyVisible: true, IsEffectivelyEnabled: true } control ? control : null;

}
