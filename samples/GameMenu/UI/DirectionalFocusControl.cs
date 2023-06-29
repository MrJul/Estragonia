using Avalonia;
using Avalonia.Controls;

namespace GameMenu.UI;

public sealed class DirectionalFocusControl : AvaloniaObject {

	public static readonly StyledProperty<Control?> ControlProperty =
		AvaloniaProperty.Register<DirectionalFocusControl, Control?>(nameof(Control));

	public static readonly StyledProperty<bool> IsNewRowProperty =
		AvaloniaProperty.Register<DirectionalFocusControl, bool>(nameof(IsNewRow), defaultValue: false);

	public static readonly StyledProperty<bool> IsRowDefaultProperty =
		AvaloniaProperty.Register<DirectionalFocusControl, bool>(nameof(IsRowDefault), defaultValue: false);

	public Control? Control {
		get => GetValue(ControlProperty);
		set => SetValue(ControlProperty, value);
	}

	public bool IsNewRow {
		get => GetValue(IsNewRowProperty);
		set => SetValue(IsNewRowProperty, value);
	}

	public bool IsRowDefault {
		get => GetValue(IsRowDefaultProperty);
		set => SetValue(IsRowDefaultProperty, value);
	}

	public Control? TryGetValidControl()
		=> Control is { IsEffectivelyVisible: true, IsEffectivelyEnabled: true } control ? control : null;

}
