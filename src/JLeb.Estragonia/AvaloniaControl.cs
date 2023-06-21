using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Platform;
using Godot;
using Godot.NativeInterop;
using JLeb.Estragonia.Input;
using AvControl = Avalonia.Controls.Control;
using AvDispatcher = Avalonia.Threading.Dispatcher;
using GdControl = Godot.Control;
using GdInput = Godot.Input;
using GdKey = Godot.Key;

namespace JLeb.Estragonia;

/// <summary>Renders an Avalonia control and forwards input to it.</summary>
public class AvaloniaControl : GdControl {

	private AvControl? _control;
	private double _renderScaling = 1.0;
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

	/// <summary>Gets or sets the render scaling for the Avalonia control. Defaults to 1.0.</summary>
	[SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator", Justification = "Doesn't affect correctness")]
	public double RenderScaling {
		get => _renderScaling;
		set {
			var newValue = Math.Max(1.0, value);
			if (_renderScaling == newValue)
				return;

			_renderScaling = newValue;
			OnResized();
			QueueRedraw();
		}
	}

	/// <summary>
	/// Gets or sets whether some Godot UI actions will be automatically mapped to an <see cref="InputElement.KeyDownEvent"/> event.
	/// The mapped actions are ui_left, ui_right, ui_up, ui_down, ui_accept and ui_cancel.
	/// Defaults to true.
	/// </summary>
	public bool AutoConvertUIActionToKeyDown { get; set; } = true;

	/// <summary>Gets the underlying Avalonia top-level element.</summary>
	/// <returns>The Avalonia top-level element.</returns>
	/// <exception cref="InvalidOperationException">Thrown if the control isn't ready or has been disposed.</exception>
	public GodotTopLevel GetTopLevel()
		=> _topLevel ?? throw new InvalidOperationException($"The {nameof(AvaloniaControl)} isn't initialized");

	/// <summary>Gets the underlying Godot texture where <see cref="Control"/> is rendered.</summary>
	/// <returns>A texture.</returns>
	/// <exception cref="InvalidOperationException">Thrown if the control isn't ready or has been disposed.</exception>
	public Texture2D GetTexture()
		=> GetTopLevel().Impl.GetTexture();

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
			|| method == MethodName._GuiInput
			|| base.HasGodotClassMethod(method);

	public override void _Ready() {
		if (Engine.IsEditorHint())
			return;

		// Skia outputs a premultiplied alpha image, ensure we got the correct blend mode if the user didn't specify any
		Material ??= new CanvasItemMaterial {
			BlendMode = CanvasItemMaterial.BlendModeEnum.PremultAlpha,
			LightMode = CanvasItemMaterial.LightModeEnum.Unshaded
		};

		var locator = AvaloniaLocator.Current;

		if (locator.GetService<IPlatformGraphics>() is not GodotVkPlatformGraphics graphics) {
			GD.PrintErr("No Godot platform graphics found, did you forget to register your Avalonia app with UseGodot()?");
			return;
		}

		var topLevelImpl = new GodotTopLevelImpl(graphics, locator.GetRequiredService<IClipboard>(), GodotPlatform.Compositor) {
			CursorChanged = OnAvaloniaCursorChanged
		};

		topLevelImpl.SetRenderSize(GetFrameSize(), RenderScaling);

		_topLevel = new GodotTopLevel(topLevelImpl) {
			Background = null,
			Content = Control,
			TransparencyLevelHint = new[] { WindowTransparencyLevel.Transparent, WindowTransparencyLevel.None }
		};

		_topLevel.Prepare();
		_topLevel.StartRendering();

		Resized += OnResized;
		FocusEntered += OnFocusEntered;
		FocusExited += OnFocusExited;
		MouseExited += OnMouseExited;

		if (HasFocus())
			OnFocusEntered();
	}

	public override void _Process(double delta)
		=> GodotPlatform.TriggerRenderTick();

	private PixelSize GetFrameSize()
		=> PixelSize.FromSize(Size.ToAvaloniaSize(), 1.0);

	private void OnAvaloniaCursorChanged(CursorShape cursor)
		=> MouseDefaultCursorShape = cursor;

	private void OnResized()
		=> _topLevel?.Impl.SetRenderSize(GetFrameSize(), RenderScaling);

	private void OnFocusEntered() {
		if (_topLevel is null)
			return;

		_topLevel.Focus();

		if (KeyboardNavigationHandler.GetNext(_topLevel, NavigationDirection.Next) is not { } inputElement)
			return;

		NavigationMethod navigationMethod;

		if (GdInput.IsActionPressed(GodotBuiltInActions.UIFocusNext) || GdInput.IsActionPressed(GodotBuiltInActions.UIFocusPrev))
			navigationMethod = NavigationMethod.Tab;
		else if (GdInput.GetMouseButtonMask() != 0)
			navigationMethod = NavigationMethod.Pointer;
		else
			navigationMethod = NavigationMethod.Unspecified;

		inputElement.Focus(navigationMethod);
	}

	private void OnFocusExited()
		=> _topLevel?.Impl.OnLostFocus();

	public override void _Draw() {
		if (_topLevel is null)
			return;

		_topLevel.Impl.OnDraw(new Rect(Size.ToAvaloniaSize()));
		DrawTexture(_topLevel.Impl.GetTexture(), Vector2.Zero);
	}

	public override void _GuiInput(InputEvent @event) {
		if (_topLevel is null)
			return;

		if (TryHandleInput(_topLevel.Impl, @event) || TryHandleAction(@event))
			AcceptEvent();
	}

	private bool TryHandleAction(InputEvent inputEvent) {
		if (!inputEvent.IsActionType())
			return false;

		if (inputEvent.IsActionPressed(GodotBuiltInActions.UIFocusNext, true, true))
			return TryMoveFocus(NavigationDirection.Next, inputEvent);

		if (inputEvent.IsActionPressed(GodotBuiltInActions.UIFocusPrev, true, true))
			return TryMoveFocus(NavigationDirection.Previous, inputEvent);

		if (AutoConvertUIActionToKeyDown) {

			if (inputEvent.IsActionPressed(GodotBuiltInActions.UILeft, true, true))
				return SimulateKeyDownFromAction(inputEvent, GdKey.Left);

			if (inputEvent.IsActionPressed(GodotBuiltInActions.UIRight, true, true))
				return SimulateKeyDownFromAction(inputEvent, GdKey.Right);

			if (inputEvent.IsActionPressed(GodotBuiltInActions.UIUp, true, true))
				return SimulateKeyDownFromAction(inputEvent, GdKey.Up);

			if (inputEvent.IsActionPressed(GodotBuiltInActions.UIDown, true, true))
				return SimulateKeyDownFromAction(inputEvent, GdKey.Down);

			if (inputEvent.IsActionPressed(GodotBuiltInActions.UIAccept, true, true))
				return SimulateKeyDownFromAction(inputEvent, GdKey.Enter);

			if (inputEvent.IsActionPressed(GodotBuiltInActions.UICancel, true, true))
				return SimulateKeyDownFromAction(inputEvent, GdKey.Escape);

		}

		return false;
	}

	private bool SimulateKeyDownFromAction(InputEvent inputEvent, GdKey key) {
		// if the action already matches the key we're going to simulate, abort: it already got through TryHandleInput and wasn't handled
		if (inputEvent is InputEventKey inputEventKey && inputEventKey.Keycode == key)
			return false;

		if (_topLevel?.FocusManager?.GetFocusedElement() is not { } currentElement)
			return false;

		var args = new KeyEventArgs {
			RoutedEvent = InputElement.KeyDownEvent,
			Key = key.ToAvaloniaKey(),
			KeyModifiers = inputEvent.GetKeyModifiers()
		};
		currentElement.RaiseEvent(args);
		return args.Handled;
	}

	private static bool TryHandleInput(GodotTopLevelImpl impl, InputEvent inputEvent)
		=> inputEvent switch {
			InputEventMouseMotion mouseMotion => impl.OnMouseMotion(mouseMotion, Time.GetTicksMsec()),
			InputEventMouseButton mouseButton => impl.OnMouseButton(mouseButton, Time.GetTicksMsec()),
			InputEventScreenTouch screenTouch => impl.OnScreenTouch(screenTouch, Time.GetTicksMsec()),
			InputEventScreenDrag screenDrag => impl.OnScreenDrag(screenDrag, Time.GetTicksMsec()),
			InputEventKey key => impl.OnKey(key, Time.GetTicksMsec()),
			InputEventJoypadButton joypadButton => impl.OnJoypadButton(joypadButton, Time.GetTicksMsec()),
			InputEventJoypadMotion joypadMotion => impl.OnJoypadMotion(joypadMotion, Time.GetTicksMsec()),
			_ => false
		};

	private bool TryMoveFocus(NavigationDirection direction, InputEvent inputEvent) {
		if (_topLevel?.FocusManager is { } focusManager
			&& KeyboardNavigationHandler.GetNext(focusManager.GetFocusedElement() ?? _topLevel, direction) is { } nextElement
		) {
			nextElement.Focus(NavigationMethod.Tab, inputEvent.GetKeyModifiers());
			return true;
		}

		return false;
	}

	private void OnMouseExited()
		=> _topLevel?.Impl.OnMouseExited(Time.GetTicksMsec());

	protected override void Dispose(bool disposing) {
		if (disposing && _topLevel is not null) {

			Resized -= OnResized;
			FocusEntered -= OnFocusEntered;
			FocusExited -= OnFocusExited;
			MouseExited -= OnMouseExited;

			_topLevel.Dispose();
			_topLevel = null;
		}

		base.Dispose(disposing);
	}

}
