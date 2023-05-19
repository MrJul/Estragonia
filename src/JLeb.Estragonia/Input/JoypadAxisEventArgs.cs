using Avalonia.Interactivity;
using Godot;

namespace JLeb.Estragonia.Input;

/// <summary>Provides information about a joypad axis event.</summary>
public class JoypadAxisEventArgs : RoutedEventArgs {

	/// <summary>Gets the device where the event comes from.</summary>
	public IJoypadDevice Device { get; }

	/// <summary>Gets the axis.</summary>
	public JoyAxis Axis { get; }

	/// <summary>
	/// Gets the current position of the joystick on the given axis.
	/// The value ranges from -1.0 to 1.0.
	/// A value of 0 means the axis is in its resting position.
	/// </summary>
	public float AxisValue { get; }

	public JoypadAxisEventArgs(RoutedEvent? routedEvent, object? source, IJoypadDevice device, JoyAxis axis, float axisValue)
		: base(routedEvent, source) {
		Device = device;
		Axis = axis;
		AxisValue = axisValue;
	}

}
