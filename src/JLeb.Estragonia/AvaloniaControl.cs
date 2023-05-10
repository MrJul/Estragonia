using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Platform;
using Avalonia.VisualTree;
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
			|| method == MethodName._GuiInput
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
		var compositor = GodotPlatform.Compositor;

		var topLevelImpl = new GodotTopLevelImpl(graphics, keyboardDevice, mouseDevice, clipboard, compositor) {
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
		MouseExited += OnMouseExited;
	}

	public override void _Process(double delta)
		=> GodotPlatform.TriggerRenderTick();

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

		_topLevel.Impl.OnDraw(new Rect(Size.ToAvaloniaSize()));
		DrawTexture(_topLevel.Impl.GetTexture(), Vector2.Zero);
	}

	public override void _GuiInput(InputEvent @event) {
		if (_topLevel is null)
			return;

		if (TryHandleInput(_topLevel.Impl, @event) || TryHandleAction(@event))
			AcceptEvent();
	}

	private static bool TryHandleAction(InputEvent inputEvent) {
		if (!inputEvent.IsActionType())
			return false;

		if (inputEvent.IsActionPressed(GodotBuiltInActions.UIFocusNext, true, true))
			return TryFocusTab(NavigationDirection.Next, inputEvent);

		if (inputEvent.IsActionPressed(GodotBuiltInActions.UIFocusPrev, true, true))
			return TryFocusTab(NavigationDirection.Previous, inputEvent);

		if (inputEvent.IsActionPressed(GodotBuiltInActions.UILeft, true, true))
			return TryFocusDirectional(inputEvent, NavigationDirection.Left);

		if (inputEvent.IsActionPressed(GodotBuiltInActions.UIRight, true, true))
			return TryFocusDirectional(inputEvent, NavigationDirection.Right);

		if (inputEvent.IsActionPressed(GodotBuiltInActions.UIUp, true, true))
			return TryFocusDirectional(inputEvent, NavigationDirection.Up);

		if (inputEvent.IsActionPressed(GodotBuiltInActions.UIDown, true, true))
			return TryFocusDirectional(inputEvent, NavigationDirection.Down);

		return false;
	}

	private static bool TryHandleInput(GodotTopLevelImpl impl, InputEvent inputEvent)
		=> inputEvent switch {
			InputEventMouseMotion mouseMotion => impl.OnMouseMotion(mouseMotion, Time.GetTicksMsec()),
			InputEventMouseButton mouseButton => impl.OnMouseButton(mouseButton, Time.GetTicksMsec()),
			InputEventKey key => impl.OnKey(key, Time.GetTicksMsec()),
			_ => false
		};

	private static bool TryFocusTab(NavigationDirection direction, InputEvent inputEvent) {
		if (FocusManager.Instance is not { Current: { } currentElement } focusManager)
			return false;

		var nextElement = KeyboardNavigationHandler.GetNext(currentElement, direction);
		focusManager.Focus(nextElement, NavigationMethod.Tab, inputEvent.GetKeyModifiers());
		return nextElement is not null;
	}

	private static bool TryFocusDirectional(InputEvent inputEvent, NavigationDirection direction) {
		if (FocusManager.Instance is not { Current: Visual currentElement } focusManager)
			return false;

		IInputElement? nextElement;

		if (currentElement.FindAncestorOfType<ICustomKeyboardNavigation>(includeSelf: true) is { } customKeyboardNavigation)
			(_, nextElement) = customKeyboardNavigation.GetNext((IInputElement) currentElement, direction);
		else if (currentElement.GetVisualParent() is INavigableContainer navigableContainer) {
			var wrapSelection = currentElement is SelectingItemsControl { WrapSelection: true };
			nextElement = navigableContainer.GetNextFocusableControl(direction, (IInputElement) currentElement, wrapSelection);
		}
		else
			return false;

		if (nextElement is not null)
			focusManager.Focus(nextElement, NavigationMethod.Directional, inputEvent.GetKeyModifiers());

		return true;
	}

	private void OnMouseExited()
		=> _topLevel?.Impl.OnMouseExited(Time.GetTicksMsec());

	protected override void Dispose(bool disposing) {
		if (disposing && _topLevel is not null) {

			// Currently leaks the ServerCompositionTarget, see https://github.com/AvaloniaUI/Avalonia/pull/11262/
			_topLevel.Dispose();

			Resized -= OnResized;
			FocusEntered -= OnFocusEntered;
			FocusExited -= OnFocusExited;
			MouseExited -= OnMouseExited;

			_topLevel = null;
		}

		base.Dispose(disposing);
	}

}
