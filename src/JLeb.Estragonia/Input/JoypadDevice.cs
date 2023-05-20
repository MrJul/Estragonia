using Avalonia.Input.Raw;

namespace JLeb.Estragonia.Input;

/// <summary>Implementation of <see cref="IJoypadDevice"/>.</summary>
internal sealed class JoypadDevice : IJoypadDevice {

	public int Id { get; }

	public JoypadDevice(int id)
		=> Id = id;

	public void ProcessRawEvent(RawInputEventArgs ev) {
		if (ev.Handled)
			return;

		switch (ev) {
			case RawJoypadButtonEventArgs buttonArgs:
				ProcessButtonEvent(buttonArgs);
				break;
			case RawJoypadAxisEventArgs axisArgs:
				ProcessAxisEvent(axisArgs);
				break;
		}
	}

	private void ProcessButtonEvent(RawJoypadButtonEventArgs rawArgs) {
		var routedEvent = rawArgs.Type switch {
			RawJoypadButtonEventType.ButtonDown => JoypadEvents.JoypadButtonDownEvent,
			RawJoypadButtonEventType.ButtonUp => JoypadEvents.JoypadButtonUpEvent,
			_ => null
		};

		if (routedEvent is null)
			return;

		var element = rawArgs.Root.FocusManager?.GetFocusedElement() ?? rawArgs.Root;
		var args = new JoypadButtonEventArgs(routedEvent, element, this, rawArgs.Button, rawArgs.Pressure);
		element.RaiseEvent(args);
		rawArgs.Handled = args.Handled;
	}

	private void ProcessAxisEvent(RawJoypadAxisEventArgs rawArgs) {
		var element = rawArgs.Root.FocusManager?.GetFocusedElement() ?? rawArgs.Root;
		var args = new JoypadAxisEventArgs(JoypadEvents.JoypadAxisMovedEvent, element, this, rawArgs.Axis, rawArgs.AxisValue);
		element.RaiseEvent(args);
		rawArgs.Handled = args.Handled;
	}

}
