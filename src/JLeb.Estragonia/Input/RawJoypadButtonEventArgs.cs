using Avalonia.Input;
using Avalonia.Input.Raw;
using Godot;

namespace JLeb.Estragonia.Input;

/// <summary>Represents raw input event arguments related to a joypad button.</summary>
public class RawJoypadButtonEventArgs : RawInputEventArgs {

	/// <summary>Gets the associated device.</summary>
	public new IJoypadDevice Device
		=> (IJoypadDevice) base.Device;

	/// <summary>Gets whether the button is pressed or released.</summary>
	public RawJoypadButtonEventType Type { get; }

	/// <summary>Gets the button that was pressed or released.</summary>
	public JoyButton Button { get; }

	/// <summary>
	/// Gets the pressure the user puts on the button with their finger, if the controller supports it.
	/// Ranges from 0 to 1.
	/// </summary>
	public float Pressure { get; }

	public RawJoypadButtonEventArgs(
		IJoypadDevice device,
		ulong timestamp,
		IInputRoot root,
		RawJoypadButtonEventType type,
		JoyButton button,
		float pressure
	) : base(device, timestamp, root) {
		Button = button;
		Pressure = pressure;
		Type = type;
	}

}
