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

	public VkImageLayout LastLayout { get; set; }

	public VkBarrierHelper BarrierHelper { get; }

	public ulong DrawCount { get; set; }

	public bool IsDisposed { get; private set; }

	public void TransitionLayoutTo(VkImageLayout newLayout) {
		if (LastLayout == newLayout)
			return;

		var sourceAccessMask = LastLayout switch {
			VkImageLayout.COLOR_ATTACHMENT_OPTIMAL => VkAccessFlags.COLOR_ATTACHMENT_READ_BIT,
			VkImageLayout.SHADER_READ_ONLY_OPTIMAL => VkAccessFlags.SHADER_READ_BIT,
			_ => VkAccessFlags.MEMORY_READ_BIT | VkAccessFlags.MEMORY_WRITE_BIT
		};

		var destinationAccessMask = newLayout switch {
			VkImageLayout.COLOR_ATTACHMENT_OPTIMAL => VkAccessFlags.COLOR_ATTACHMENT_WRITE_BIT,
			VkImageLayout.SHADER_READ_ONLY_OPTIMAL => VkAccessFlags.SHADER_WRITE_BIT,
			_ => VkAccessFlags.MEMORY_READ_BIT | VkAccessFlags.MEMORY_WRITE_BIT
		};

		BarrierHelper.TransitionImageLayout(VkImage, LastLayout, sourceAccessMask, newLayout, destinationAccessMask);
		LastLayout = newLayout;
	}

	SKSurface ISkiaSurface.Surface
		=> SkSurface;

	bool ISkiaSurface.CanBlit
		=> false;

	public GodotSkiaSurface(
		SKSurface skSurface,
		Texture2Drd gdTexture,
		VkImage vkImage,
		VkImageLayout lastLayout,
		RenderingDevice renderingDevice,
		double renderScaling,
		VkBarrierHelper barrierHelper
	) {
		SkSurface = skSurface;
		GdTexture = gdTexture;
		VkImage = vkImage;
		LastLayout = lastLayout;
		RenderingDevice = renderingDevice;
		RenderScaling = renderScaling;
		BarrierHelper = barrierHelper;
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
