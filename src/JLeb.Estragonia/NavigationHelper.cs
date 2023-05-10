using Avalonia.Controls;
using Avalonia.Input;

namespace JLeb.Estragonia;

/// <summary>Contains methods to help with focus navigation.</summary>
internal static class NavigationHelper {

	public static IInputElement? GetNextFocusableControl(
		this INavigableContainer container,
		NavigationDirection direction,
		IInputElement? from,
		bool wrapSelection
	)
		=> ItemsControlEx.GetNextControl(container, direction, from, wrapSelection);

	private sealed class ItemsControlEx : ItemsControl {

		// Expose the protected static method as public
		// TODO: open an Avalonia issue to see if this can be made public/moved outside ItemsControl
		public new static IInputElement? GetNextControl(
			INavigableContainer container,
			NavigationDirection direction,
			IInputElement? from,
			bool wrap
		)
			=> ItemsControl.GetNextControl(container, direction, from, wrap);

	}

}
