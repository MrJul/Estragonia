using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;

namespace GameMenu.UI.Behaviors;

/// <summary>A behavior that focuses a given target when the associated object gets disabled while being focused.</summary>
public sealed class MoveFocusWhenDisabledBehavior : Behavior<Control> {

	public static readonly StyledProperty<IInputElement?> TargetProperty =
		AvaloniaProperty.Register<MoveFocusWhenDisabledBehavior, IInputElement?>(nameof(Target));

	private IDisposable? _subscription;

	public IInputElement? Target {
		get => GetValue(TargetProperty);
		set => SetValue(TargetProperty, value);
	}

	protected override void OnAttached() {
		base.OnAttached();

		_subscription = AssociatedObject?.GetObservable(InputElement.IsEffectivelyEnabledProperty).Subscribe(OnIsEnabledChanged);
	}

	protected override void OnDetaching() {
		_subscription?.Dispose();
		_subscription = null;

		base.OnDetaching();
	}

	private void OnIsEnabledChanged(bool isEnabled) {
		if (!isEnabled && AssociatedObject?.IsFocused == true)
			Target?.Focus();
	}

}
