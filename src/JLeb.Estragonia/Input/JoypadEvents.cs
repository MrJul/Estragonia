using Avalonia.Input;
using Avalonia.Interactivity;

namespace JLeb.Estragonia.Input;

/// <summary>Contains Avalonia events related to joypad (game controller).</summary>
public static class JoypadEvents {

	public static readonly RoutedEvent<JoypadButtonEventArgs> JoypadButtonDownEvent =
		RoutedEvent.Register<InputElement, JoypadButtonEventArgs>(
			"JoypadButtonDown",
			RoutingStrategies.Tunnel | RoutingStrategies.Bubble
		);

	public static readonly RoutedEvent<JoypadButtonEventArgs> JoypadButtonUpEvent =
		RoutedEvent.Register<InputElement, JoypadButtonEventArgs>(
			"JoypadButtonUp",
			RoutingStrategies.Tunnel | RoutingStrategies.Bubble
		);

	public static readonly RoutedEvent<JoypadAxisEventArgs> JoypadAxisMovedEvent =
		RoutedEvent.Register<InputElement, JoypadAxisEventArgs>(
			"JoypadAxisMoved",
			RoutingStrategies.Tunnel | RoutingStrategies.Bubble
		);

}
