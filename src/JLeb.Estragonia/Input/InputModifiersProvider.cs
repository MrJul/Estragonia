using Avalonia.Input;
using Godot;
using GdInput = Godot.Input;
using GdKey = Godot.Key;
using GdMouseButton = Godot.MouseButton;

namespace JLeb.Estragonia.Input;

/// <summary>Contains methods to get input modifiers.</summary>
internal static class InputModifiersProvider {

	public static RawInputModifiers GetRawInputModifiers(this InputEvent inputEvent) {
		var modifiers = RawInputModifiers.None;

		if (inputEvent is InputEventWithModifiers inputEventWithModifiers) {
			if (inputEventWithModifiers.AltPressed)
				modifiers |= RawInputModifiers.Alt;
			if (inputEventWithModifiers.CtrlPressed)
				modifiers |= RawInputModifiers.Control;
			if (inputEventWithModifiers.ShiftPressed)
				modifiers |= RawInputModifiers.Shift;
			if (inputEventWithModifiers.MetaPressed)
				modifiers |= RawInputModifiers.Meta;

			if (inputEventWithModifiers is InputEventMouse inputEventMouse) {
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
			else {
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
			}
		}
		else {
			modifiers = GetRawInputModifiers();

			if (inputEvent is InputEventScreenDrag { PenInverted: true })
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
