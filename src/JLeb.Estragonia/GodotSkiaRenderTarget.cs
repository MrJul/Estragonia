using System.Diagnostics.CodeAnalysis;
using Avalonia.Skia;
using SkiaSharp;

namespace JLeb.Estragonia;

/// <summary>A render target that uses an underlying Skia surface.</summary>
internal sealed class GodotSkiaRenderTarget : ISkiaGpuRenderTarget {

	private readonly GodotSkiaSurface _surface;
	private readonly GRContext _grContext;
	private readonly double _renderScaling;
	private readonly VkBarrierHelper _barrierHelper;

	[SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator", Justification = "Doesn't affect correctness")]
	public bool IsCorrupted
		=> _surface.IsDisposed || _grContext.IsAbandoned || _renderScaling != _surface.RenderScaling;

	public GodotSkiaRenderTarget(GodotSkiaSurface surface, GRContext grContext, VkBarrierHelper barrierHelper) {
		_renderScaling = surface.RenderScaling;
		_surface = surface;
		_grContext = grContext;
		_barrierHelper = barrierHelper;
	}

	public ISkiaGpuRenderSession BeginRenderingSession()
		=> new GodotSkiaGpuRenderSession(_surface, _grContext, _barrierHelper);

	public void Dispose() {
	}

}
