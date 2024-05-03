using System;
using Avalonia.Skia;
using Godot;
using SkiaSharp;
using static JLeb.Estragonia.VkInterop;

namespace JLeb.Estragonia;

/// <summary>Encapsulates a Skia surface along with the Godot texture it comes from.</summary>
internal sealed class GodotSkiaSurface : ISkiaSurface {

	public SKSurface SkSurface { get; }

	public Texture2Drd GdTexture { get; }

	public VkImage VkImage { get; }

	public RenderingDevice RenderingDevice { get; }

	public double RenderScaling { get; set; }

	public bool IsDisposed { get; private set; }

	SKSurface ISkiaSurface.Surface
		=> SkSurface;

	bool ISkiaSurface.CanBlit
		=> false;

	public GodotSkiaSurface(
		SKSurface skSurface,
		Texture2Drd gdTexture,
		VkImage vkImage,
		RenderingDevice renderingDevice,
		double renderScaling
	) {
		SkSurface = skSurface;
		GdTexture = gdTexture;
		VkImage = vkImage;
		RenderingDevice = renderingDevice;
		RenderScaling = renderScaling;
		IsDisposed = false;
	}

	void ISkiaSurface.Blit(SKCanvas canvas)
		=> throw new NotSupportedException();

	public void Dispose() {
		if (IsDisposed)
			return;

		IsDisposed = true;
		SkSurface.Dispose();
		RenderingDevice.FreeRid(GdTexture.TextureRdRid);
		GdTexture.Dispose();
	}

}
