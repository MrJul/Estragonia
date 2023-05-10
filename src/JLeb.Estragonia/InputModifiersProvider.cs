using Avalonia.Input;
using Godot;
using GdInput = Godot.Input;
using GdKey = Godot.Key;
using GdMouseButton = Godot.MouseButton;

namespace JLeb.Estragonia;

/// <summary>Contains methods to get input modifiers.</summary>
internal static class InputModifiersProvider {

	public static RawInputModifiers GetRawInputModifiers(this InputEventWithModifiers inputEvent) {
		var modifiers = RawInputModifiers.None;

		if (inputEvent.AltPressed)
			modifiers |= RawInputModifiers.Alt;
		if (inputEvent.CtrlPressed)
			modifiers |= RawInputModifiers.Control;
		if (inputEvent.ShiftPressed)
			modifiers |= RawInputModifiers.Shift;
		if (inputEvent.MetaPressed)
			modifiers |= RawInputModifiers.Meta;

		if (inputEvent is InputEventMouse inputEventMouse) {
			var buttonMask = inputEventMouse.ButtonMask;
			if ((buttonMask & MouseButtonMask.Left) != 0)
				modifiers |= RawInputModifiers.LeftMouseButton;
			if ((buttonMask & MouseButtonMask.Right) != 0)
				modifiers |= RawInputModifiers.RightMouseButton;
			if ((buttonMask & MouseButtonMask.Middle) != 0)
				modifiers |= RawInputModifiers.MiddleMouseButton;
			if ((buttonMask & MouseButtonMask.MbXbutton1) != 0)
				modifiers |= RawInputModifiers.XButton1MouseButton;
			if ((buttonMask & MouseButtonMask.MbXbutton2) != 0)
				modifiers |= RawInputModifiers.XButton2MouseButton;

			if (inputEventMouse is InputEventMouseMotion { PenInverted: true })
				modifiers |= RawInputModifiers.PenInverted;
		}

		return modifiers;
	}

	public static RawInputModifiers GetRawInputModifiers() {
		var modifiers = RawInputModifiers.None;

		if (GdInput.IsKeyPressed(GdKey.Alt))
			modifiers |= RawInputModifiers.Alt;
		if (GdInput.IsKeyPressed(GdKey.Ctrl))
			modifiers |= RawInputModifiers.Control;
		if (GdInput.IsKeyPressed(GdKey.Shift))
			modifiers |= RawInputModifiers.Shift;
		if (GdInput.IsKeyPressed(GdKey.Meta))
			modifiers |= RawInputModifiers.Meta;
		if (GdInput.IsMouseButtonPressed(GdMouseButton.Left))
			modifiers |= RawInputModifiers.LeftMouseButton;
		if (GdInput.IsMouseButtonPressed(GdMouseButton.Right))
			modifiers |= RawInputModifiers.RightMouseButton;
		if (GdInput.IsMouseButtonPressed(GdMouseButton.Middle))
			modifiers |= RawInputModifiers.MiddleMouseButton;
		if (GdInput.IsMouseButtonPressed(GdMouseButton.Xbutton1))
			modifiers |= RawInputModifiers.XButton1MouseButton;
		if (GdInput.IsMouseButtonPressed(GdMouseButton.Xbutton2))
			modifiers |= RawInputModifiers.XButton2MouseButton;

		return modifiers;
	}

	public static KeyModifiers GetKeyModifiers(this InputEvent inputEvent)
		=> inputEvent is InputEventWithModifiers inputEventWithModifiers
			? inputEventWithModifiers.GetKeyModifiers()
			: GetKeyModifiers();

	public static KeyModifiers GetKeyModifiers(this InputEventWithModifiers inputEvent) {
		var modifiers = KeyModifiers.None;

		if (inputEvent.AltPressed)
			modifiers |= KeyModifiers.Alt;
		if (inputEvent.CtrlPressed)
			modifiers |= KeyModifiers.Control;
		if (inputEvent.ShiftPressed)
			modifiers |= KeyModifiers.Shift;
		if (inputEvent.MetaPressed)
			modifiers |= KeyModifiers.Meta;

		return modifiers;
	}

	public static KeyModifiers GetKeyModifiers() {
		var modifiers = KeyModifiers.None;

		if (GdInput.IsKeyPressed(GdKey.Alt))
			modifiers |= KeyModifiers.Alt;
		if (GdInput.IsKeyPressed(GdKey.Ctrl))
			modifiers |= KeyModifiers.Control;
		if (GdInput.IsKeyPressed(GdKey.Shift))
			modifiers |= KeyModifiers.Shift;
		if (GdInput.IsKeyPressed(GdKey.Meta))
			modifiers |= KeyModifiers.Meta;

		return modifiers;
	}

}
