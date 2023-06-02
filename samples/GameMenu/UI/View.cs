using System;
using Avalonia.Controls;
using Avalonia.Input;

namespace GameMenu.UI;

#pragma warning disable CS0618

public abstract class View : UserControl {

	protected override void OnLoaded() {
		base.OnLoaded();

		GetFirstFocusableChild()?.Focus();
	}

	private IInputElement? GetFirstFocusableChild()
		=> VisualChildren is [IInputElement firstChild, ..]
			? firstChild.Focusable ? firstChild : KeyboardNavigationHandler.GetNext(firstChild, NavigationDirection.Next)
			: null;

	private void FocusDirectional(Func<IInputElement, IInputElement?> findNext) {
		if (TopLevel.GetTopLevel(this)?.FocusManager is not { } focusManager)
			return;

		var currentElement = focusManager.GetFocusedElement();
		var nextElement = currentElement is null ? GetFirstFocusableChild() : findNext(currentElement);
		nextElement?.Focus();
	}

	protected override void OnKeyDown(KeyEventArgs e) {
		base.OnKeyDown(e);

		if (e.Handled || e.KeyModifiers != KeyModifiers.None)
			return;

		switch (e.Key) {
			case Key.Up:
				FocusDirectional(current => KeyboardNavigationHandler.GetNext(current, NavigationDirection.Previous));
				e.Handled = true;
				break;
			case Key.Down:
				FocusDirectional(current => KeyboardNavigationHandler.GetNext(current, NavigationDirection.Next));
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
