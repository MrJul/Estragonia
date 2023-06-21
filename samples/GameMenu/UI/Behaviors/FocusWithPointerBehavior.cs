using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;

namespace GameMenu.UI.Behaviors;

public class FocusWithPointerBehavior : Behavior<Control> {

	public static readonly StyledProperty<IInputElement?> TargetProperty =
		AvaloniaProperty.Register<FocusWithPointerBehavior, IInputElement?>(nameof(Target));

	public IInputElement? Target {
		get => GetValue(TargetProperty);
		set => SetValue(TargetProperty, value);
	}

	protected override void OnAttached() {
		base.OnAttached();

		if (AssociatedObject is { } button)
			button.PointerEntered += OnPointerEntered;
	}

	protected override void OnDetaching() {
		if (AssociatedObject is { } button)
			button.PointerEntered -= OnPointerEntered;

		base.OnDetaching();
	}

	private void OnPointerEntered(object? sender, PointerEventArgs e)
		=> (Target ?? AssociatedObject)?.Focus();

}
