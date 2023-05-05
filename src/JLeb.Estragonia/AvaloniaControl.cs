using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using Godot;
using Godot.NativeInterop;
using AvControl = Avalonia.Controls.Control;
using GdControl = Godot.Control;

namespace JLeb.Estragonia;

public class AvaloniaControl : GdControl {

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

	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret) {
		if (method == Node.MethodName._Ready && args.Count == 0) {
			_Ready();
			ret = default;
			return true;
		}

		if (method == Node.MethodName._Process && args.Count == 1) {
			_Process(VariantUtils.ConvertTo<double>(args[0]));
			ret = default;
			return true;
		}

		if (method == CanvasItem.MethodName._Draw && args.Count == 0) {
			_Draw();
			ret = default;
			return true;
		}

		return base.InvokeGodotClassMethod(method, args, out ret);
	}

	protected override bool HasGodotClassMethod(in godot_string_name method)
		=> method == Node.MethodName._Ready
			|| method == Node.MethodName._Process
			|| method == CanvasItem.MethodName._Draw
			|| base.HasGodotClassMethod(method);

	public override void _Ready() {
		base._Ready();

		if (Engine.IsEditorHint())
			return;

		if (AvaloniaLocator.Current.GetService<IPlatformGraphics>() is not GodotVkPlatformGraphics graphics) {
			GD.PrintErr("No Godot platform graphics found, did you forget to register your Avalonia app with UseGodot()?");
			return;
		}

		var topLevelImpl = new GodotTopLevelImpl(graphics) {
			ClientSize = GetAvaloniaSize()
		};

		_topLevel = new GodotTopLevel(topLevelImpl) {
			Background = null,
			Content = Control,
			TransparencyLevelHint = WindowTransparencyLevel.Transparent
		};

		_topLevel.Prepare();
		_topLevel.Renderer.Start();

		Resized += OnSizeChanged;
	}

	public override void _Process(double delta)
		=> _topLevel?.Impl.RenderTimer.TriggerTick(new TimeSpan((long) (Time.GetTicksUsec() * 10UL)));

	private Size GetAvaloniaSize() {
		var size = Size;
		return new Size(size.X, size.Y);
	}

	private void OnSizeChanged() {
		if (_topLevel is null)
			return;

		var size = GetAvaloniaSize();
		_topLevel.Impl.ClientSize = size;
		_topLevel.Measure(size);
		_topLevel.Arrange(new Rect(size));
	}

	public override void _Draw() {
		if (_topLevel is null)
			return;

		_topLevel.Renderer.Paint(new Rect(GetAvaloniaSize()));
		DrawTexture(_topLevel.Impl.Surface.GdTexture, Vector2.Zero);
	}

	protected override void Dispose(bool disposing) {
		if (disposing) {
			_topLevel?.Dispose();
			_topLevel = null;
		}

		base.Dispose(disposing);
	}

}
