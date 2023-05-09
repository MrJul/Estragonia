using System.IO;
using Avalonia.Platform;

namespace JLeb.Estragonia;

/// <summary>A fake window icon that can't be displayed but can still be saved.</summary>
internal sealed class StubWindowIconImpl : IWindowIconImpl {

	private readonly MemoryStream _stream;

	public StubWindowIconImpl(MemoryStream stream)
		=> _stream = stream;

	public void Save(Stream outputStream) {
		_stream.Position = 0L;
		_stream.CopyTo(outputStream);
	}

}
