using System;
using Avalonia.Skia;
using Godot;
using SkiaSharp;

namespace JLeb.Estragonia;

/// <summary>Encapsulates a Skia surface along with the Godot texture it comes from.</summary>
internal sealed class GodotSkiaSurface : ISkiaSurface {

	public SKSurface SkSurface { get; }

	public Texture2D GdTexture { get; }

	public bool IsValid { get; set; }

	SKSurface ISkiaSurface.Surface
		=> SkSurface;

	bool ISkiaSurface.CanBlit
		=> false;

	public GodotSkiaSurface(SKSurface skSurface, Texture2D gdTexture) {
		SkSurface = skSurface;
		GdTexture = gdTexture;
		IsValid = true;
	}

	void ISkiaSurface.Blit(SKCanvas canvas)
		=> throw new NotSupportedException();

	public void Dispose() {
		SkSurface.Dispose();
		GdTexture.Dispose();
	}

}