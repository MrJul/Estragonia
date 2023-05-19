using Avalonia.Input;

namespace JLeb.Estragonia.Input;

/// <summary>Represents a joypad (game controller) device.</summary>
public interface IJoypadDevice : IInputDevice {

	/// <summary>Gets an identifier uniquely identifying the device (-1 if the device is emulated).</summary>
	int Id { get; }

}
