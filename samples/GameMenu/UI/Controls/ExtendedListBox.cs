using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;

namespace GameMenu.UI.Controls;

public class ExtendedListBox : ListBox {

	static ExtendedListBox()
		=> KeyboardNavigation.TabNavigationProperty.OverrideDefaultValue<ExtendedListBox>(KeyboardNavigationMode.Continue);

	protected override Type StyleKeyOverride
		=> typeof(ListBox);

	protected override void OnKeyDown(KeyEventArgs e) {
		if (e.Handled)
			return;

		// Override the default ListBox behavior that changes the selection with Up/Down: we want to change the focus instead.
		switch (e.Key) {
			case Key.Up:
				if (TryMoveFocus(NavigationDirection.Up, e.KeyModifiers))
					e.Handled = true;
				break;
			case Key.Down:
				if (TryMoveFocus(NavigationDirection.Down, e.KeyModifiers))
					e.Handled = true;
				break;
			case Key.Space:
				e.Handled = UpdateSelectionFromEventSource(e.Source);
				break;
			case Key.Enter:
				// handle only if we changed the selection, otherwise let Enter bubble up so the user can accept the view
				var selectedIndex = SelectedIndex;
				e.Handled = UpdateSelectionFromEventSource(e.Source) && SelectedIndex != selectedIndex;
				break;
		}
	}

	private bool TryMoveFocus(NavigationDirection direction, KeyModifiers keyModifiers) {
		if (TopLevel.GetTopLevel(this)?.FocusManager is not { } focusManager
			|| Presenter?.Panel is not INavigableContainer navigableContainer
		) {
			return false;
		}

		var current = focusManager.GetFocusedElement() as Visual;

		while (current is not null) {
			if (current.GetVisualParent() == navigableContainer && current is IInputElement inputElement) {
				if (GetNextControl(navigableContainer, direction, inputElement, WrapSelection) is { } next) {
					next.Focus(NavigationMethod.Directional, keyModifiers);
					return true;
				}

				break;
			}

			current = current.GetVisualParent();
		}

		return false;
	}

}
