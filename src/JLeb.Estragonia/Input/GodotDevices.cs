using System.Collections.Concurrent;
using Avalonia.Input;

namespace JLeb.Estragonia.Input;

/// <summary>Contains the various Avalonia devices created from Godot.</summary>
public static class GodotDevices {

	private static readonly ConcurrentDictionary<int, IMouseDevice> s_mouseById = new();
	private static readonly ConcurrentDictionary<int, IJoypadDevice> s_joypadById = new();

	/// <summary>The device identifier used by emulated devices.</summary>
	public const int EmulatedDeviceId = -1;

	/// <summary>Gets the keyboard device.</summary>
	/// <remarks>At this time, we don't support multiple keyboard devices since Avalonia needs a single one for focus management.</remarks>
	public static IKeyboardDevice Keyboard
		=> new KeyboardDevice();

	/// <summary>Gets a mouse device for a given Godot device identifier.</summary>
	/// <param name="deviceId">The device identifier.</param>
	/// <returns>A mouse device.</returns>
	public static IMouseDevice GetMouse(int deviceId)
		=> s_mouseById.GetOrAdd(deviceId, static id => new MouseDevice(new Pointer(id, PointerType.Mouse, id == 0)));

	/// <summary>Gets a joypad device for a given Godot device identifier.</summary>
	/// <returns>A joypad device.</returns>
	public static IJoypadDevice GetJoypad(int deviceId)
		=> s_joypadById.GetOrAdd(deviceId, static id => new JoypadDevice(id));

}
