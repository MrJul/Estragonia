using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Platform;
using Avalonia.Rendering;
using Avalonia.Rendering.Composition;
using Avalonia.Threading;

namespace JLeb.Estragonia;

/// <summary>Implementation of Avalonia <see cref="ITopLevelImpl"/> for a Godot viewport.</summary>
internal sealed class GodotTopLevelImpl : ITopLevelImpl {

	private readonly GodotVkPlatformGraphics _platformGraphics;
	private readonly Compositor _compositor;

	private Size _clientSize;
	private GodotSkiaSurface? _surface;
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
				_surface.IsValid = false;
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

	public GodotSkiaSurface Surface
		=> _surface ??= CreateSurface();

	public IEnumerable<object> Surfaces
		=> new object[] { Surface };

	WindowTransparencyLevel ITopLevelImpl.TransparencyLevel
		=> WindowTransparencyLevel.Transparent;

	AcrylicPlatformCompensationLevels ITopLevelImpl.AcrylicCompensationLevels
		=> new(1.0, 1.0, 1.0);

	Size? ITopLevelImpl.FrameSize
		=> null;

	Action<RawInputEventArgs>? ITopLevelImpl.Input { get; set; }

	Action<Rect>? ITopLevelImpl.Paint { get; set; }

	Action<double>? ITopLevelImpl.ScalingChanged { get; set; }

	Action<WindowTransparencyLevel>? ITopLevelImpl.TransparencyLevelChanged { get;set; }

	Action? ITopLevelImpl.LostFocus { get;set; }

	public GodotTopLevelImpl(GodotVkPlatformGraphics platformGraphics) {
		_platformGraphics = platformGraphics;
		RenderTimer = new();
		_compositor = new Compositor(new RenderLoop(RenderTimer, Dispatcher.UIThread), platformGraphics);
	}

	private GodotSkiaSurface CreateSurface()
		=> _isDisposed
			? throw new ObjectDisposedException(nameof(GodotTopLevelImpl))
			: _platformGraphics.GetSharedContext().CreateSurface(PixelSize.FromSize(_clientSize, RenderScaling));

	IRenderer ITopLevelImpl.CreateRenderer(IRenderRoot root)
		=> new CompositingRenderer(root, _compositor, () => Surfaces);

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
