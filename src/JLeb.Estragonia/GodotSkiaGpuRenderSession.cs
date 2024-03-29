﻿using Avalonia.Skia;
using SkiaSharp;

namespace JLeb.Estragonia;

/// <summary>A render session that uses an underlying Skia surface.</summary>
internal sealed class GodotSkiaGpuRenderSession : ISkiaGpuRenderSession {

	public GodotSkiaSurface Surface { get; }

	public GRContext GrContext { get; }

	SKSurface ISkiaGpuRenderSession.SkSurface
		=> Surface.SkSurface;

	double ISkiaGpuRenderSession.ScaleFactor
		=> Surface.RenderScaling;

	GRSurfaceOrigin ISkiaGpuRenderSession.SurfaceOrigin
		=> GRSurfaceOrigin.TopLeft;

	public GodotSkiaGpuRenderSession(GodotSkiaSurface surface, GRContext grContext) {
		Surface = surface;
		GrContext = grContext;
	}

	public void Dispose()
		=> Surface.SkSurface.Flush();

}
