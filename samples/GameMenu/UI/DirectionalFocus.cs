using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace GameMenu.UI;

public static class DirectionalFocus {

	public static readonly AttachedProperty<IInputElement?> FocusUpProperty =
		AvaloniaProperty.RegisterAttached<Control, IInputElement?>("FocusUp", typeof(DirectionalFocus));

	public static readonly AttachedProperty<IInputElement?> FocusDownProperty =
		AvaloniaProperty.RegisterAttached<Control, IInputElement?>("FocusDown", typeof(DirectionalFocus));

	public static readonly AttachedProperty<IInputElement?> FocusLeftProperty =
		AvaloniaProperty.RegisterAttached<Control, IInputElement?>("FocusLeft", typeof(DirectionalFocus));

	public static readonly AttachedProperty<IInputElement?> FocusRightProperty =
		AvaloniaProperty.RegisterAttached<Control, IInputElement?>("FocusRight", typeof(DirectionalFocus));

	public static void SetFocusUp(Control obj, IInputElement? value)
		=> obj.SetValue(FocusUpProperty, value);

	public static IInputElement? GetFocusUp(Control obj)
		=> obj.GetValue(FocusUpProperty);

	public static void SetFocusDown(Control obj, IInputElement? value)
		=> obj.SetValue(FocusDownProperty, value);

	public static IInputElement? GetFocusDown(Control obj)
		=> obj.GetValue(FocusDownProperty);

	public static void SetFocusLeft(Control obj, IInputElement? value)
		=> obj.SetValue(FocusLeftProperty, value);

	public static IInputElement? GetFocusLeft(Control obj)
		=> obj.GetValue(FocusLeftProperty);

	public static void SetFocusRight(Control obj, IInputElement? value)
		=> obj.SetValue(FocusRightProperty, value);

	public static IInputElement? GetFocusRight(Control obj)
		=> obj.GetValue(FocusRightProperty);

}
