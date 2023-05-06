using Avalonia.Skia;
using SkiaSharp;

namespace JLeb.Estragonia;

/// <summary>A render target that uses an underlying Skia surface.</summary>
internal sealed class GodotSkiaRenderTarget : ISkiaGpuRenderTarget {

	private readonly GodotSkiaSurface _surface;
	private readonly GRContext _grContext;

	public GodotSkiaRenderTarget(GodotSkiaSurface surface, GRContext grContext) {
		_surface = surface;
		_grContext = grContext;
	}

	public ISkiaGpuRenderSession BeginRenderingSession()
		=> new GodotSkiaGpuRenderSession(_surface, _grContext);

	public bool IsCorrupted
		=> _surface.IsDisposed || _grContext.IsAbandoned;

	public void Dispose() {
	}

}
