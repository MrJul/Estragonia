using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using GameMenu.UI.Controls;

namespace GameMenu.UI;

public abstract class View : UserControl {

	private Control? _lastFocusedChild;

	public static readonly StyledProperty<DirectionalFocusStructure?> DirectionalFocusStructureProperty =
		AvaloniaProperty.Register<View, DirectionalFocusStructure?>(nameof(DirectionalFocusStructure));

	public DirectionalFocusStructure? DirectionalFocusStructure {
		get => GetValue(DirectionalFocusStructureProperty);
		set => SetValue(DirectionalFocusStructureProperty, value);
	}

	protected override void OnLoaded(RoutedEventArgs e) {
		base.OnLoaded(e);

		var focusableChild = _lastFocusedChild ?? TryGetFirstFocusableChild();
		focusableChild?.Focus();
	}

	protected override void OnGotFocus(GotFocusEventArgs e) {
		_lastFocusedChild = e.Source as Control;
		base.OnGotFocus(e);
	}

	private Control? TryGetFirstFocusableChild()
		=> AdjustFocusTarget(DirectionalFocusStructure?.GetDown(null), forward: true);

	private void FocusDirectional(Func<Control?, Control?> findNext) {
		if (TopLevel.GetTopLevel(this)?.FocusManager is not { } focusManager)
			return;

		var current = focusManager.GetFocusedElement() as Control;

		// on a list item, use the list instead
		if (current is ILogical logical && logical.GetLogicalParent() is ItemsControl itemsControl)
			current = itemsControl;

		var next = current is null ? TryGetFirstFocusableChild() : findNext(current);
		next?.Focus();
	}

	private static Control? AdjustFocusTarget(Control? control, bool forward) {
		// on a list, use a list item instead
		if (control is ExtendedListBox listBox) {
			var containers = listBox.GetRealizedContainers();
			return forward ? containers.FirstOrDefault() : containers.LastOrDefault();
		}

		return control;
	}

	protected override void OnKeyDown(KeyEventArgs e) {
		base.OnKeyDown(e);

		if (e.Handled || e.KeyModifiers != KeyModifiers.None)
			return;

		switch (e.Key) {
			case Key.Up:
				FocusDirectional(current => AdjustFocusTarget(DirectionalFocusStructure?.GetUp(current), forward: false));
				e.Handled = true;
				break;

			case Key.Down:
				FocusDirectional(current => AdjustFocusTarget(DirectionalFocusStructure?.GetDown(current), forward: true));
				e.Handled = true;
				break;

			case Key.Left:
				FocusDirectional(current => AdjustFocusTarget(DirectionalFocusStructure?.GetLeft(current), forward: false));
				e.Handled = true;
				break;

			case Key.Right:
				FocusDirectional(current => AdjustFocusTarget(DirectionalFocusStructure?.GetRight(current), forward: true));
				e.Handled = true;
				break;
		}
	}

}
