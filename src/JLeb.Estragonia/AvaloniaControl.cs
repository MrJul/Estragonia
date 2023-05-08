using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Platform;
using Godot;
using Godot.NativeInterop;
using AvControl = Avalonia.Controls.Control;
using AvDispatcher = Avalonia.Threading.Dispatcher;
using GdControl = Godot.Control;

namespace JLeb.Estragonia;

/// <summary>Renders an Avalonia control and forwards input to it.</summary>
public class AvaloniaControl : GdControl {

	private AvControl? _control;
	private GodotTopLevel? _topLevel;

	/// <summary>Gets or sets the underlying Avalonia control that will be rendered.</summary>
	public AvControl? Control {
		get => _control;
		set {
			if (ReferenceEquals(_control, value))
				return;

			_control = value;

			if (_topLevel is not null)
				_topLevel.Content = _control;
		}
	}

	/// <summary>Gets the underlying texture where <see cref="Control"/> is rendered.</summary>
	/// <returns>A texture.</returns>
	/// <exception cref="InvalidOperationException">Thrown if the control isn't ready or has been disposed.</exception>
	public Texture2D GetTexture()
		=> _topLevel is null
			? throw new InvalidOperationException($"The {nameof(AvaloniaControl)} isn't initialized")
			: _topLevel.Impl.GetTexture();

	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret) {
		if (method == Node.MethodName._Ready && args.Count == 0) {
			_Ready();
			ret = default;
			return true;
		}

		if (method == Node.MethodName._Process && args.Count == 1) {
			_Process(VariantUtils.ConvertTo<double>(args[0]));
			ret = default;
			return true;
		}

		if (method == CanvasItem.MethodName._Draw && args.Count == 0) {
			_Draw();
			ret = default;
			return true;
		}

		if (method == MethodName._GuiInput && args.Count == 1) {
			_GuiInput(VariantUtils.ConvertTo<InputEvent>(args[0]));
			ret = default;
			return true;
		}

		return base.InvokeGodotClassMethod(method, args, out ret);
	}

	protected override bool HasGodotClassMethod(in godot_string_name method)
		=> method == Node.MethodName._Ready
			|| method == Node.MethodName._Process
			|| method == CanvasItem.MethodName._Draw
			|| base.HasGodotClassMethod(method);

	public override void _Ready() {
		if (Engine.IsEditorHint())
			return;

		var locator = AvaloniaLocator.Current;

		if (locator.GetService<IPlatformGraphics>() is not GodotVkPlatformGraphics graphics) {
			GD.PrintErr("No Godot platform graphics found, did you forget to register your Avalonia app with UseGodot()?");
			return;
		}

		var keyboardDevice = locator.GetRequiredService<IKeyboardDevice>();
		var mouseDevice = locator.GetRequiredService<IMouseDevice>();
		var clipboard = locator.GetRequiredService<IClipboard>();

		var topLevelImpl = new GodotTopLevelImpl(graphics, keyboardDevice, mouseDevice, clipboard) {
			ClientSize = Size.ToAvaloniaSize(),
			CursorChanged = OnAvaloniaCursorChanged
		};

		_topLevel = new GodotTopLevel(topLevelImpl) {
			Background = null,
			Content = Control,
			TransparencyLevelHint = WindowTransparencyLevel.Transparent
		};

		_topLevel.Prepare();
		_topLevel.Renderer.Start();

		Resized += OnResized;
		FocusEntered += OnFocusEntered;
		FocusExited += OnFocusExited;
	}

	public override void _Process(double delta)
		=> _topLevel?.Impl.RenderTimer.TriggerTick(new TimeSpan((long) (Time.GetTicksUsec() * 10UL)));

	private void OnAvaloniaCursorChanged(CursorShape cursor)
		=> MouseDefaultCursorShape = cursor;

	private void OnResized() {
		if (_topLevel is null)
			return;

		var size = Size.ToAvaloniaSize();
		_topLevel.Impl.ClientSize = size;
		_topLevel.Measure(size);
		_topLevel.Arrange(new Rect(size));
	}

	private void OnFocusEntered() {
		if (_topLevel is null)
			return;

		_topLevel.Focus();

		if (KeyboardNavigationHandler.GetNext(_topLevel, NavigationDirection.Next) is { } inputElement)
			FocusManager.Instance?.Focus(inputElement, NavigationMethod.Tab);
	}

	private void OnFocusExited()
		=> _topLevel?.Impl.OnLostFocus();

	public override void _Draw() {
		if (_topLevel is null)
			return;

		_topLevel.Renderer.Paint(new Rect(Size.ToAvaloniaSize()));
		DrawTexture(_topLevel.Impl.GetTexture(), Vector2.Zero);
	}

	public override void _GuiInput(InputEvent inputEvent) {
		if (_topLevel is null)
			return;

		if (TryHandleInput(_topLevel.Impl, inputEvent) || TryHandleAction(inputEvent))
			AcceptEvent();
	}

	private static bool TryHandleAction(InputEvent inputEvent) {
		if (!inputEvent.IsActionType())
			return false;

		if (inputEvent.IsActionPressed(GodotBuiltInActions.UIFocusNext, true, true))
			return TryFocusNext(NavigationDirection.Next);

		if (inputEvent.IsActionPressed(GodotBuiltInActions.UIFocusPrev, true, true))
			return TryFocusNext(NavigationDirection.Previous);

		return false;
	}

	private static bool TryHandleInput(GodotTopLevelImpl impl, InputEvent inputEvent)
		=> inputEvent switch {
			InputEventMouseMotion mouseMotion => impl.OnMouseMotion(mouseMotion, Time.GetTicksMsec()),
			InputEventMouseButton mouseButton => impl.OnMouseButton(mouseButton, Time.GetTicksMsec()),
			InputEventKey key => impl.OnKey(key, Time.GetTicksMsec()),
			_ => false
		};

	private static bool TryFocusNext(NavigationDirection direction) {
		if (FocusManager.Instance is not { Current: { } currentElement } focusManager)
			return false;

		var nextElement = KeyboardNavigationHandler.GetNext(currentElement, direction);
		focusManager.Focus(nextElement, NavigationMethod.Tab);
		return nextElement is not null;
	}

	protected override void Dispose(bool disposing) {
		if (disposing && _topLevel is not null) {
			_topLevel.Dispose();

			// ensure the underlying Avalonia compositor render target is disposed
			// shouldn't be needed anymore if https://github.com/AvaloniaUI/Avalonia/pull/11262/ is merged
			AvDispatcher.UIThread.RunJobs();
			_topLevel.Impl.RenderTimer.TriggerTick(new TimeSpan((long) (Time.GetTicksUsec() * 10UL)));

			_topLevel = null;

			Resized -= OnResized;
			FocusEntered -= OnFocusEntered;
			FocusExited -= OnFocusExited;
		}

		base.Dispose(disposing);
	}

}
