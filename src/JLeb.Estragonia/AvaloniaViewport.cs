using System;
using Avalonia;
using Avalonia.Platform;
using Godot;
using AvControl = Avalonia.Controls.Control;

namespace JLeb.Estragonia;

public class AvaloniaViewport : SubViewport {

	private AvControl? _control;
	private GodotTopLevel? _topLevel;

	public AvControl? Control {
		get => _control;
		set {
			if (ReferenceEquals(_control, value))
				return;

			_control = value;

			if (_topLevel is not null)
				_topLevel.Content = _control;
		}
	}

	public override void _Ready() {
		base._Ready();

		if (AvaloniaLocator.Current.GetService<IPlatformGraphics>() is not GodotVkPlatformGraphics graphics) {
			throw new InvalidOperationException(
				"No Godot platform graphics found, did you forget to register your Avalonia app with UseGodot()?"
			);
		}

		_topLevel = new GodotTopLevel(new GodotTopLevelImpl(graphics));
		_topLevel.Impl.ClientSize = GetAvaloniaSize();
		_topLevel.Content = Control;
		_topLevel.Prepare();

		SizeChanged += OnSizeChanged;
	}

	public override void _Process(double delta) {
		base._Process(delta);

		_topLevel?.Impl.RenderTimer.TriggerTick(new TimeSpan((long) (Time.GetTicksUsec() * 10UL)));
	}

	private void OnSizeChanged() {
		if (_topLevel is null)
			return;

		var size = GetAvaloniaSize();
		_topLevel.Impl.ClientSize = size;
		_topLevel.Measure(size);
		_topLevel.Arrange(new Rect(size));
	}

	private Size GetAvaloniaSize() {
		var size = Size;
		return new Size(size.X, size.Y);
	}

	protected override void Dispose(bool disposing) {
		if (disposing) {
			_topLevel?.Dispose();
			_topLevel = null;
		}

		base.Dispose(disposing);
	}

}
