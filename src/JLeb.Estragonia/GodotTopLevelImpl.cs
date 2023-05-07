using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Platform;
using Avalonia.Rendering;
using Avalonia.Rendering.Composition;
using Godot;
using AvDispatcher = Avalonia.Threading.Dispatcher;
using AvKey = Avalonia.Input.Key;
using GdMouseButton = Godot.MouseButton;

namespace JLeb.Estragonia;

/// <summary>Implementation of Avalonia <see cref="ITopLevelImpl"/> that renders to a Godot texture.</summary>
internal sealed class GodotTopLevelImpl : ITopLevelImpl {

	private readonly GodotVkPlatformGraphics _platformGraphics;
	private readonly IKeyboardDevice _keyboardDevice;
	private readonly IMouseDevice _mouseDevice;
	private readonly Compositor _compositor;

	private GodotSkiaSurface? _surface;
	private WindowTransparencyLevel _transparencyLevel = WindowTransparencyLevel.Transparent;
	private Size _clientSize;
	private IInputRoot? _inputRoot;
	private bool _isDisposed;

	public ManualRenderTimer RenderTimer { get; }

	public double RenderScaling
		=> 1.0;

	public Size ClientSize {
		get => _clientSize;
		set {
			if (_clientSize.Equals(value))
				return;

			_clientSize = value;

			if (_surface is not null) {
				_surface.Dispose();
				_surface = null;
			}

			if (_isDisposed)
				return;

			_surface = CreateSurface();
			Resized?.Invoke(value, PlatformResizeReason.Unspecified);
		}
	}

	public Action<Size, PlatformResizeReason>? Resized { get;set; }

	public Action? Closed { get; set; }

	public Action<RawInputEventArgs>? Input { get; set; }

	IEnumerable<object> ITopLevelImpl.Surfaces
		=> GetOrCreateSurfaces();

	WindowTransparencyLevel ITopLevelImpl.TransparencyLevel
		=> _transparencyLevel;

	AcrylicPlatformCompensationLevels ITopLevelImpl.AcrylicCompensationLevels
		=> new(1.0, 1.0, 1.0);

	Size? ITopLevelImpl.FrameSize
		=> null;

	Action<Rect>? ITopLevelImpl.Paint { get; set; }

	Action<double>? ITopLevelImpl.ScalingChanged { get; set; }

	public Action<WindowTransparencyLevel>? TransparencyLevelChanged { get; set; }

	Action? ITopLevelImpl.LostFocus { get;set; }

	public GodotTopLevelImpl(GodotVkPlatformGraphics platformGraphics, IKeyboardDevice keyboardDevice, IMouseDevice mouseDevice) {
		_platformGraphics = platformGraphics;
		_keyboardDevice = keyboardDevice;
		_mouseDevice = mouseDevice;
		RenderTimer = new ManualRenderTimer();
		_compositor = new Compositor(new RenderLoop(RenderTimer, AvDispatcher.UIThread), platformGraphics);
	}

	private GodotSkiaSurface CreateSurface()
		=> _isDisposed
			? throw new ObjectDisposedException(nameof(GodotTopLevelImpl))
			: _platformGraphics.GetSharedContext().CreateSurface(PixelSize.FromSize(_clientSize, RenderScaling));

	private GodotSkiaSurface GetOrCreateSurface()
		=> _surface ??= CreateSurface();

	private IEnumerable<object> GetOrCreateSurfaces()
		=> new object[] { GetOrCreateSurface() };

	public Texture2D GetTexture()
		=> GetOrCreateSurface().GdTexture;

	public bool OnMouseMotion(InputEventMouseMotion inputEvent, ulong timestamp) {
		if (_inputRoot is null || Input is not { } input)
			return false;

		var tilt = inputEvent.Tilt;

		var args = new RawPointerEventArgs(
			_mouseDevice,
			timestamp,
			_inputRoot,
			RawPointerEventType.Move,
			new RawPointerPoint {
				Position = inputEvent.Position.ToAvaloniaPoint(),
				Twist = 0.0f,
				Pressure = inputEvent.Pressure,
				XTilt = tilt.X * 90.0f,
				YTilt = tilt.Y * 90.0f
			},
			GetRawInputModifiers(inputEvent)
		);

		input(args);

		return args.Handled;
	}

	public bool OnMouseButton(InputEventMouseButton inputEvent, ulong timestamp) {
		if (_inputRoot is null || Input is not { } input)
			return false;

		RawPointerEventArgs CreateButtonArgs(RawPointerEventType type)
			=> new(_mouseDevice, timestamp, _inputRoot, type, inputEvent.Position.ToAvaloniaPoint(), GetRawInputModifiers(inputEvent));

		RawMouseWheelEventArgs CreateWheelArgs(Vector delta)
			=> new(_mouseDevice, timestamp, _inputRoot, inputEvent.Position.ToAvaloniaPoint(), delta, GetRawInputModifiers(inputEvent));

		var args = (inputEvent.ButtonIndex, inputEvent.Pressed) switch {
			(GdMouseButton.Left, true) => CreateButtonArgs(RawPointerEventType.LeftButtonDown),
			(GdMouseButton.Left, false) => CreateButtonArgs(RawPointerEventType.LeftButtonUp),
			(GdMouseButton.Right, true) => CreateButtonArgs(RawPointerEventType.RightButtonDown),
			(GdMouseButton.Right, false) => CreateButtonArgs(RawPointerEventType.RightButtonUp),
			(GdMouseButton.Middle, true) => CreateButtonArgs(RawPointerEventType.MiddleButtonDown),
			(GdMouseButton.Middle, false) => CreateButtonArgs(RawPointerEventType.MiddleButtonUp),
			(GdMouseButton.Xbutton1, true) => CreateButtonArgs(RawPointerEventType.XButton1Down),
			(GdMouseButton.Xbutton1, false) => CreateButtonArgs(RawPointerEventType.XButton1Up),
			(GdMouseButton.Xbutton2, true) => CreateButtonArgs(RawPointerEventType.XButton2Down),
			(GdMouseButton.Xbutton2, false) => CreateButtonArgs(RawPointerEventType.XButton2Up),
			(GdMouseButton.WheelUp, _) => CreateWheelArgs(new Vector(0.0, inputEvent.Factor)),
			(GdMouseButton.WheelDown, _) => CreateWheelArgs(new Vector(0.0, -inputEvent.Factor)),
			(GdMouseButton.WheelLeft, _) => CreateWheelArgs(new Vector(inputEvent.Factor, 0.0)),
			(GdMouseButton.WheelRight, _) => CreateWheelArgs(new Vector(-inputEvent.Factor, 0.0)),
			_ => null
		};

		if (args is null)
			return false;

		input(args);

		return args.Handled;
	}

	public bool OnKey(InputEventKey inputEvent, ulong timestamp) {
		if (_inputRoot is null || Input is not { } input)
			return false;

		var keyCode = inputEvent.Keycode;
		var pressed = inputEvent.Pressed;

		var key = keyCode.ToAvaloniaKey();
		if (key != AvKey.None) {
			var type = pressed ? RawKeyEventType.KeyDown : RawKeyEventType.KeyUp;
			var args = new RawKeyEventArgs(_keyboardDevice, timestamp, _inputRoot, type, key, GetRawInputModifiers(inputEvent));

			input(args);

			if (args.Handled)
				return true;
		}

		if (pressed && OS.IsKeycodeUnicode((long) keyCode)) {
			var text = Char.ConvertFromUtf32((int) inputEvent.Unicode);
			var args = new RawTextInputEventArgs(_keyboardDevice, timestamp, _inputRoot, text);

			input(args);

			if (args.Handled)
				return true;
		}

		return false;
	}

	private static RawInputModifiers GetRawInputModifiers(InputEventWithModifiers inputEvent) {
		var inputModifiers = RawInputModifiers.None;

		if (inputEvent.AltPressed)
			inputModifiers |= RawInputModifiers.Alt;
		if (inputEvent.CtrlPressed)
			inputModifiers |= RawInputModifiers.Control;
		if (inputEvent.ShiftPressed)
			inputModifiers |= RawInputModifiers.Shift;
		if (inputEvent.MetaPressed)
			inputModifiers |= RawInputModifiers.Meta;

		if (inputEvent is InputEventMouse inputEventMouse) {
			var buttonMask = inputEventMouse.ButtonMask;
			if ((buttonMask & MouseButtonMask.Left) != 0)
				inputModifiers |= RawInputModifiers.LeftMouseButton;
			if ((buttonMask & MouseButtonMask.Right) != 0)
				inputModifiers |= RawInputModifiers.RightMouseButton;
			if ((buttonMask & MouseButtonMask.Middle) != 0)
				inputModifiers |= RawInputModifiers.MiddleMouseButton;
			if ((buttonMask & MouseButtonMask.MbXbutton1) != 0)
				inputModifiers |= RawInputModifiers.XButton1MouseButton;
			if ((buttonMask & MouseButtonMask.MbXbutton2) != 0)
				inputModifiers |= RawInputModifiers.XButton2MouseButton;

			if (inputEventMouse is InputEventMouseMotion { PenInverted: true })
				inputModifiers |= RawInputModifiers.PenInverted;
		}

		return inputModifiers;
	}

	IRenderer ITopLevelImpl.CreateRenderer(IRenderRoot root)
		=> new CompositingRenderer(root, _compositor, GetOrCreateSurfaces);

	void ITopLevelImpl.SetInputRoot(IInputRoot inputRoot)
		=> _inputRoot = inputRoot;

	Point ITopLevelImpl.PointToClient(PixelPoint point)
		=> point.ToPoint(RenderScaling);

	PixelPoint ITopLevelImpl.PointToScreen(Point point)
		=> PixelPoint.FromPoint(point, RenderScaling);

	void ITopLevelImpl.SetCursor(ICursorImpl? cursor) {
	}

	IPopupImpl? ITopLevelImpl.CreatePopup()
		=> null;

	void ITopLevelImpl.SetTransparencyLevelHint(WindowTransparencyLevel transparencyLevel) {
		transparencyLevel = transparencyLevel == WindowTransparencyLevel.Transparent
			? WindowTransparencyLevel.Transparent
			: WindowTransparencyLevel.None;

		if (transparencyLevel != _transparencyLevel) {
			_transparencyLevel = transparencyLevel;
			TransparencyLevelChanged?.Invoke(transparencyLevel);
		}
	}

	void ITopLevelImpl.SetFrameThemeVariant(PlatformThemeVariant themeVariant) {
	}

	object? IOptionalFeatureProvider.TryGetFeature(Type featureType)
		=> null;

	public void Dispose() {
		if (_isDisposed)
			return;

		_isDisposed = true;

		if (_surface is not null) {
			_surface.Dispose();
			_surface = null;
		}

		Closed?.Invoke();
	}

}
