using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Platform;
using Avalonia.Rendering;
using Avalonia.Rendering.Composition;
using Dispatcher = Avalonia.Threading.Dispatcher;

namespace JLeb.Estragonia;

/// <summary>Implementation of Avalonia <see cref="ITopLevelImpl"/> for a Godot viewport.</summary>
internal sealed class GodotTopLevelImpl : ITopLevelImpl {

	private readonly Compositor _compositor;

	private GodotSkiaSurface? _surface;
	private WindowTransparencyLevel _transparencyLevel = WindowTransparencyLevel.Transparent;
	private Size _clientSize;
	private IInputRoot? _inputRoot;
	private bool _isDisposed;

	public GodotVkPlatformGraphics PlatformGraphics { get; }

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
				_surface.IsValid = false;
				_surface = null;
			}

			if (_isDisposed)
				return;

			_surface = CreateSurface();
			Resized?.Invoke(value, PlatformResizeReason.Unspecified);
		}
	}

	public GodotSkiaSurface Surface
		=> _surface ??= CreateSurface();

	public Action<Size, PlatformResizeReason>? Resized { get;set; }

	public Action? Closed { get; set; }

	IEnumerable<object> ITopLevelImpl.Surfaces
		=> GetSurfaces();

	WindowTransparencyLevel ITopLevelImpl.TransparencyLevel
		=> _transparencyLevel;

	AcrylicPlatformCompensationLevels ITopLevelImpl.AcrylicCompensationLevels
		=> new(1.0, 1.0, 1.0);

	Size? ITopLevelImpl.FrameSize
		=> null;

	Action<RawInputEventArgs>? ITopLevelImpl.Input { get; set; }

	Action<Rect>? ITopLevelImpl.Paint { get; set; }

	Action<double>? ITopLevelImpl.ScalingChanged { get; set; }

	public Action<WindowTransparencyLevel>? TransparencyLevelChanged { get; set; }

	Action? ITopLevelImpl.LostFocus { get;set; }

	public GodotTopLevelImpl(GodotVkPlatformGraphics platformGraphics) {
		PlatformGraphics = platformGraphics;
		RenderTimer = new ManualRenderTimer();
		_compositor = new Compositor(new RenderLoop(RenderTimer, Dispatcher.UIThread), platformGraphics);
	}

	private GodotSkiaSurface CreateSurface()
		=> _isDisposed
			? throw new ObjectDisposedException(nameof(GodotTopLevelImpl))
			: PlatformGraphics.GetSharedContext().CreateSurface(PixelSize.FromSize(_clientSize, RenderScaling));

	private IEnumerable<object> GetSurfaces()
		=> new object[] { Surface };

	IRenderer ITopLevelImpl.CreateRenderer(IRenderRoot root)
		=> new CompositingRenderer(root, _compositor, GetSurfaces);

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
			_surface.IsValid = false;
			_surface = null;
		}

		Closed?.Invoke();
	}

}
