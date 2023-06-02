using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace GameMenu.UI;

public static class DirectionalFocus {

	public static readonly AttachedProperty<IInputElement?> FocusLeftProperty =
		AvaloniaProperty.RegisterAttached<Control, IInputElement?>("FocusLeft", typeof(Gestures));

	public static readonly AttachedProperty<IInputElement?> FocusRightProperty =
		AvaloniaProperty.RegisterAttached<Control, IInputElement?>("FocusRight", typeof(Gestures));

	public static void SetFocusLeft(Control obj, IInputElement? value)
		=> obj.SetValue(FocusLeftProperty, value);

	public static IInputElement? GetFocusLeft(Control obj)
		=> obj.GetValue(FocusLeftProperty);

	public static void SetFocusRight(Control obj, IInputElement? value)
		=> obj.SetValue(FocusRightProperty, value);

	public static IInputElement? GetFocusRight(Control obj)
		=> obj.GetValue(FocusRightProperty);

}
