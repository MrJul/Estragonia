using Avalonia.Skia;
using SkiaSharp;
using static JLeb.Estragonia.VkInterop;

namespace JLeb.Estragonia;

/// <summary>A render session that uses an underlying Skia surface.</summary>
internal sealed class GodotSkiaGpuRenderSession : ISkiaGpuRenderSession {

	public GodotSkiaSurface Surface { get; }

	public GRContext GrContext { get; }

	public VkBarrierHelper BarrierHelper { get; }

	SKSurface ISkiaGpuRenderSession.SkSurface
		=> Surface.SkSurface;

	double ISkiaGpuRenderSession.ScaleFactor
		=> Surface.RenderScaling;

	GRSurfaceOrigin ISkiaGpuRenderSession.SurfaceOrigin
		=> GRSurfaceOrigin.TopLeft;

	public GodotSkiaGpuRenderSession(GodotSkiaSurface surface, GRContext grContext, VkBarrierHelper barrierHelper) {
		Surface = surface;
		GrContext = grContext;
		BarrierHelper = barrierHelper;
	}

	public void Dispose() {
		// Godot leaves the image in SHADER_READ_ONLY_OPTIMAL but Skia expects it in COLOR_ATTACHMENT_OPTIMAL
		BarrierHelper.TransitionImageLayout(
			Surface.VkImage,
			VkImageLayout.SHADER_READ_ONLY_OPTIMAL,
			VkAccessFlags.SHADER_READ_BIT,
			VkImageLayout.COLOR_ATTACHMENT_OPTIMAL,
			VkAccessFlags.COLOR_ATTACHMENT_WRITE_BIT);

		// Render Skia
		Surface.SkSurface.Flush();

		// Switch back to SHADER_READ_ONLY_OPTIMAL for Godot
		BarrierHelper.TransitionImageLayout(
			Surface.VkImage,
			VkImageLayout.COLOR_ATTACHMENT_OPTIMAL,
			VkAccessFlags.COLOR_ATTACHMENT_WRITE_BIT,
			VkImageLayout.SHADER_READ_ONLY_OPTIMAL,
			VkAccessFlags.SHADER_READ_BIT);
	}

}
