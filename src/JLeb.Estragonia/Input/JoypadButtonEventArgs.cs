using Avalonia.Interactivity;
using Godot;

namespace JLeb.Estragonia.Input;

/// <summary>Provides information about a joypad button event.</summary>
public class JoypadButtonEventArgs : RoutedEventArgs {

	/// <summary>Gets the device where the event comes from.</summary>
	public IJoypadDevice Device { get; }

	/// <summary>Gets the button that was pressed or released.</summary>
	public JoyButton Button { get; }

	/// <summary>
	/// Gets the pressure the user puts on the button with their finger, if the controller supports it.
	/// Ranges from 0 to 1.
	/// </summary>
	public float Pressure { get; }

	public JoypadButtonEventArgs(RoutedEvent? routedEvent, object? source, IJoypadDevice device, JoyButton button, float pressure)
		: base(routedEvent, source) {
		Device = device;
		Button = button;
		Pressure = pressure;
	}

}
