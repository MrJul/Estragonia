using Avalonia.Input;
using Avalonia.Input.Raw;
using Godot;

namespace JLeb.Estragonia.Input;

/// <summary>Represents raw input event arguments related to a joypad axis.</summary>
public class RawJoypadAxisEventArgs : RawInputEventArgs {

	/// <summary>Gets the associated device.</summary>
	public new IJoypadDevice Device
		=> (IJoypadDevice) base.Device;

	/// <summary>Gets the axis.</summary>
	public JoyAxis Axis { get; }

	/// <summary>
	/// Gets the current position of the joystick on the given axis.
	/// The value ranges from -1.0 to 1.0.
	/// A value of 0 means the axis is in its resting position.
	/// </summary>
	public float AxisValue { get; }

	public RawJoypadAxisEventArgs(IJoypadDevice device, ulong timestamp, IInputRoot root, JoyAxis axis, float axisValue)
		: base(device, timestamp, root) {
		Axis = axis;
		AxisValue = axisValue;
	}

}
