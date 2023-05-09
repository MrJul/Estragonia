using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Platform;

namespace JLeb.Estragonia;

/// <summary>Godot Vulkan-based <see cref="IPlatformGraphics"/> implementation.</summary>
[SuppressMessage(
	"Design",
	"CA1001:Types that own disposable fields should be disposable",
	Justification = "This type has equivalent to a static lifetime"
)]
internal sealed class GodotVkPlatformGraphics : IPlatformGraphics {

	private GodotVkSkiaGpu? _context;

	bool IPlatformGraphics.UsesSharedContext
		=> true;

	public GodotVkSkiaGpu GetSharedContext() {
		if (_context is null || _context.IsLost) {
			_context?.Dispose();
			_context = null;
			_context = new GodotVkSkiaGpu();
		}

		return _context;
	}

	IPlatformGraphicsContext IPlatformGraphics.CreateContext()
		=> throw new NotSupportedException();

	IPlatformGraphicsContext IPlatformGraphics.GetSharedContext()
		=> GetSharedContext();

}
