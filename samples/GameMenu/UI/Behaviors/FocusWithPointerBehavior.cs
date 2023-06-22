using Avalonia;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;

namespace GameMenu.UI.Behaviors;

/// <summary>A behavior that focuses a given target when the pointer enters the associated object.</summary>
public sealed class FocusWithPointerBehavior : Behavior<InputElement> {

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
