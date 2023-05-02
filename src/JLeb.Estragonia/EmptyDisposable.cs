using System;

namespace JLeb.Estragonia;

/// <summary>A reusable <see cref="IDisposable"/> implementation that does nothing when disposed.</summary>
internal sealed class EmptyDisposable : IDisposable {

	public static EmptyDisposable Instance { get; } = new();

	private EmptyDisposable() {
	}

	public void Dispose() {
	}

}