using Avalonia.Skia;
using Godot;
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

		// Clear the texture on first draw. This is already done by Avalonia, but Godot doesn't know that.
		// We need it to avoid texture corruption on first draw on AMD GPUs. It will result in a few transparent frames after resizing.
		// TODO: find a better solution.
		if (Surface.DrawCount == 0)
			Surface.RenderingDevice.TextureClear(Surface.GdTexture.TextureRdRid, new Color(0u), 0, 1, 0, 1);

		// Godot leaves the image in SHADER_READ_ONLY_OPTIMAL but Skia expects it in COLOR_ATTACHMENT_OPTIMAL
		Surface.TransitionLayoutTo(VkImageLayout.COLOR_ATTACHMENT_OPTIMAL);
	}

	public void Dispose() {
		Surface.SkSurface.Flush(true);

		// Switch back to SHADER_READ_ONLY_OPTIMAL for Godot
		Surface.TransitionLayoutTo(VkImageLayout.SHADER_READ_ONLY_OPTIMAL);

		Surface.DrawCount++;
	}

}
