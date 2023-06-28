using System;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;

namespace GameMenu.UI;

#pragma warning disable CS0618

public abstract class View : UserControl {

	private IInputElement? _lastFocusedChild;

	protected override void OnLoaded(RoutedEventArgs e) {
		base.OnLoaded(e);

		var focusableChild = _lastFocusedChild ?? TryGetFirstFocusableChild();
		focusableChild?.Focus();
	}

	protected override void OnGotFocus(GotFocusEventArgs e) {
		_lastFocusedChild = e.Source as IInputElement;
		base.OnGotFocus(e);
	}

	private IInputElement? TryGetFirstFocusableChild()
		=> VisualChildren is [IInputElement firstChild, ..]
			? firstChild.Focusable ? firstChild : KeyboardNavigationHandler.GetNext(firstChild, NavigationDirection.Next)
			: null;

	private void FocusDirectional(Func<IInputElement, IInputElement?> findNext) {
		if (TopLevel.GetTopLevel(this)?.FocusManager is not { } focusManager)
			return;

		var currentElement = focusManager.GetFocusedElement();

		// on a list item, use the list instead
		if (currentElement is ILogical logical && logical.GetLogicalParent() is ItemsControl itemsControl)
			currentElement = itemsControl;

		var nextElement = currentElement is null ? TryGetFirstFocusableChild() : findNext(currentElement);
		if (nextElement is SelectingItemsControl selectingItemsControl)
			selectingItemsControl.ContainerFromIndex(selectingItemsControl.SelectedIndex)?.Focus();

		nextElement?.Focus();
	}

	protected override void OnKeyDown(KeyEventArgs e) {
		base.OnKeyDown(e);

		if (e.Handled || e.KeyModifiers != KeyModifiers.None)
			return;

		switch (e.Key) {
			case Key.Up:
				FocusDirectional(
					current => current is Control control && DirectionalFocus.GetFocusUp(control) is { } next
						? next
						: KeyboardNavigationHandler.GetNext(current, NavigationDirection.Previous)
				);
				e.Handled = true;
				break;
			case Key.Down:
				FocusDirectional(
					current => current is Control control && DirectionalFocus.GetFocusDown(control) is { } next
						? next
						: KeyboardNavigationHandler.GetNext(current, NavigationDirection.Next)
				);
				e.Handled = true;
				break;
			case Key.Left:
				FocusDirectional(current => current is Control control ? DirectionalFocus.GetFocusLeft(control) : null);
				e.Handled = true;
				break;
			case Key.Right:
				FocusDirectional(current => current is Control control ? DirectionalFocus.GetFocusRight(control) : null);
				e.Handled = true;
				break;
		}
	}

}
