using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Avalonia.Threading;
using Godot;
using GdDispatcher = Godot.Dispatcher;
using SysTimer = System.Threading.Timer;

namespace JLeb.Estragonia;

/// <summary>An implementation of <see cref="IDispatcherImpl"/> that uses the underlying Godot dispatcher.</summary>
[SuppressMessage(
	"Design",
	"CA1001:Types that own disposable fields should be disposable",
	Justification = "This type has equivalent to a static lifetime"
)]
internal sealed class GodotDispatcherImpl : IDispatcherImpl {

	private readonly Thread _mainThread;
	private readonly SysTimer _timer;
	private readonly SendOrPostCallback _invokeSignaled; // cached delegate
	private readonly SendOrPostCallback _invokeTimer;  // cached delegate

	public long Now
		=> (long) Time.GetTicksMsec();

	public bool CurrentThreadIsLoopThread
		=> _mainThread == Thread.CurrentThread;

	public event Action? Signaled;

	public event Action? Timer;

	public GodotDispatcherImpl(Thread mainThread) {
		_mainThread = mainThread;
		_invokeSignaled = InvokeSignaled;
		_invokeTimer = InvokeTimer;
		_timer = new(OnTimerTick, this, Timeout.Infinite, Timeout.Infinite);
	}

	public void UpdateTimer(long? dueTimeInMs)
		=> _timer.Change(dueTimeInMs.GetValueOrDefault(Timeout.Infinite), Timeout.Infinite);

	private void OnTimerTick(object? state)
		=> GdDispatcher.SynchronizationContext.Post(_invokeTimer, null);

	public void Signal()
		=> GdDispatcher.SynchronizationContext.Post(_invokeSignaled, this);

	private void InvokeSignaled(object? state)
		=> Signaled?.Invoke();

	private void InvokeTimer(object? state)
		=> Timer?.Invoke();

}
