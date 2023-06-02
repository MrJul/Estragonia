using System;
using Avalonia.Skia;
using Godot;
using SkiaSharp;

namespace JLeb.Estragonia;

/// <summary>Encapsulates a Skia surface along with the Godot texture it comes from.</summary>
internal sealed class GodotSkiaSurface : ISkiaSurface {

	public SKSurface SkSurface { get; }

	public Texture2D GdTexture { get; }

	public GodotObject GdTextureOwner { get; }

	public double RenderScaling { get; }

	public bool IsDisposed { get; private set; }

	SKSurface ISkiaSurface.Surface
		=> SkSurface;

	bool ISkiaSurface.CanBlit
		=> false;

	public GodotSkiaSurface(SKSurface skSurface, Texture2D gdTexture, GodotObject gdTextureOwner, double renderScaling) {
		SkSurface = skSurface;
		GdTexture = gdTexture;
		GdTextureOwner = gdTextureOwner;
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
		GdTexture.Dispose();
		GdTextureOwner.Free();
		GdTextureOwner.Dispose();
	}

}
