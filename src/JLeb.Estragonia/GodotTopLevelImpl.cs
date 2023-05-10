using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Input.Raw;
using Avalonia.Platform;
using Avalonia.Rendering;
using Avalonia.Rendering.Composition;
using Godot;
using AvDispatcher = Avalonia.Threading.Dispatcher;
using AvKey = Avalonia.Input.Key;
using GdCursorShape = Godot.Control.CursorShape;
using GdMouseButton = Godot.MouseButton;

namespace JLeb.Estragonia;

/// <summary>Implementation of Avalonia <see cref="ITopLevelImpl"/> that renders to a Godot texture.</summary>
internal sealed class GodotTopLevelImpl : ITopLevelImpl {

	private readonly GodotVkPlatformGraphics _platformGraphics;
	private readonly IKeyboardDevice _keyboardDevice;
	private readonly IMouseDevice _mouseDevice;
	private readonly IClipboard _clipboard;
	private readonly Compositor _compositor;

	private GodotSkiaSurface? _surface;
	private WindowTransparencyLevel _transparencyLevel = WindowTransparencyLevel.Transparent;
	private Size _clientSize;
	private IInputRoot? _inputRoot;
	private GdCursorShape _cursorShape;
	private bool _isDisposed;

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

	public Action<Rect>? Paint { get; set; }

	public Action<Size, PlatformResizeReason>? Resized { get; set; }

	public Action? Closed { get; set; }

	public Action<RawInputEventArgs>? Input { get; set; }

	public Action? LostFocus { get;set; }

	public Action<GdCursorShape>? CursorChanged { get; set; }

	IEnumerable<object> ITopLevelImpl.Surfaces
		=> GetOrCreateSurfaces();

	WindowTransparencyLevel ITopLevelImpl.TransparencyLevel
		=> _transparencyLevel;

	AcrylicPlatformCompensationLevels ITopLevelImpl.AcrylicCompensationLevels
		=> new(1.0, 1.0, 1.0);

	Size? ITopLevelImpl.FrameSize
		=> null;

	Action<double>? ITopLevelImpl.ScalingChanged { get; set; }

	public Action<WindowTransparencyLevel>? TransparencyLevelChanged { get; set; }

	public GodotTopLevelImpl(
		GodotVkPlatformGraphics platformGraphics,
		IKeyboardDevice keyboardDevice,
		IMouseDevice mouseDevice,
		IClipboard clipboard,
		Compositor compositor
	) {
		_platformGraphics = platformGraphics;
		_keyboardDevice = keyboardDevice;
		_mouseDevice = mouseDevice;
		_clipboard = clipboard;
		_compositor = compositor;
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

	public void OnDraw(Rect rect)
		=> Paint?.Invoke(rect);

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
			inputEvent.GetRawInputModifiers()
		);

		input(args);

		return args.Handled;
	}

	public bool OnMouseButton(InputEventMouseButton inputEvent, ulong timestamp) {
		if (_inputRoot is null || Input is not { } input)
			return false;

		RawPointerEventArgs CreateButtonArgs(RawPointerEventType type)
			=> new(_mouseDevice, timestamp, _inputRoot, type, inputEvent.Position.ToAvaloniaPoint(), inputEvent.GetRawInputModifiers());

		RawMouseWheelEventArgs CreateWheelArgs(Vector delta)
			=> new(_mouseDevice, timestamp, _inputRoot, inputEvent.Position.ToAvaloniaPoint(), delta, inputEvent.GetRawInputModifiers());

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
			var args = new RawKeyEventArgs(_keyboardDevice, timestamp, _inputRoot, type, key, inputEvent.GetRawInputModifiers());

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

	public void OnLostFocus()
		=> LostFocus?.Invoke();


	public bool OnMouseExited(ulong timestamp) {
		if (_inputRoot is null || Input is not { } input)
			return false;

		var args = new RawPointerEventArgs(
			_mouseDevice,
			timestamp,
			_inputRoot,
			RawPointerEventType.LeaveWindow,
			new Point(-1, -1),
			InputModifiersProvider.GetRawInputModifiers()
		);

		input(args);

		return args.Handled;
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
		var cursorShape = (cursor as GodotStandardCursorImpl)?.CursorShape ?? GdCursorShape.Arrow;
		if (_cursorShape == cursorShape)
			return;

		_cursorShape = cursorShape;
		CursorChanged?.Invoke(cursorShape);
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

	object? IOptionalFeatureProvider.TryGetFeature(Type featureType) {
		if (featureType == typeof(IClipboard))
			return _clipboard;

		return null;
	}

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
