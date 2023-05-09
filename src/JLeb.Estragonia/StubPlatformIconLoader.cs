using System.IO;
using Avalonia.Platform;

namespace JLeb.Estragonia;

/// <summary>An implementation of <see cref="IPlatformIconLoader"/> that provides fake icons (never displayed).</summary>
internal sealed class StubPlatformIconLoader : IPlatformIconLoader {

	public IWindowIconImpl LoadIcon(string fileName) {
		using var stream = File.OpenRead(fileName);
		return LoadIcon(stream);
	}

	public IWindowIconImpl LoadIcon(Stream stream) {
		var memoryStream = new MemoryStream(stream.CanSeek ? (int) stream.Length : 0);
		stream.CopyTo(memoryStream);
		memoryStream.Position = 0L;
		return new StubWindowIconImpl(memoryStream);
	}

	public IWindowIconImpl LoadIcon(IBitmapImpl bitmap) {
		var memoryStream = new MemoryStream();
		bitmap.Save(memoryStream);
		memoryStream.Position = 0L;
		return new StubWindowIconImpl(memoryStream);
	}

}
