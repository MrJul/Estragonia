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

	protected override void OnLoaded() {
		base.OnLoaded();

		if (KeyboardNavigation.GetTabNavigation(this) == KeyboardNavigationMode.Once)
			KeyboardNavigation.SetTabNavigation(this, KeyboardNavigationMode.Continue);
	}

	protected override void OnKeyDown(KeyEventArgs e) {
		if (e.Handled)
			return;

		// Override the default ListBox behavior that changes the selection with Up/Down: we want to change the focus instead.
		if (e.Key.ToNavigationDirection() is { } direction and (NavigationDirection.Up or NavigationDirection.Down)) {
			if (TopLevel.GetTopLevel(this)?.FocusManager is not { } focusManager
				|| Presenter?.Panel is not INavigableContainer navigableContainer
			) {
				return;
			}

			var current = focusManager.GetFocusedElement() as Visual;

			while (current is not null) {
				if (current.GetVisualParent() == navigableContainer && current is IInputElement inputElement) {
					var next = GetNextControl(navigableContainer, direction, inputElement, WrapSelection);

					if (next is not null) {
						next.Focus(NavigationMethod.Directional, e.KeyModifiers);
						e.Handled = true;
						return;
					}

					break;
				}

				current = current.GetVisualParent();
			}
		}

		base.OnKeyDown(e);
	}

}
