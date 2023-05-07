using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform;
using Godot;
using Godot.NativeInterop;
using AvControl = Avalonia.Controls.Control;
using AvDispatcher = Avalonia.Threading.Dispatcher;
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

	public Texture2D GetTexture()
		=> _topLevel is null
			? throw new InvalidOperationException($"The {nameof(AvaloniaControl)} isn't initialized")
			: _topLevel.Impl.GetTexture();

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

		if (method == MethodName._GuiInput && args.Count == 1) {
			_GuiInput(VariantUtils.ConvertTo<InputEvent>(args[0]));
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
		if (Engine.IsEditorHint())
			return;

		var locator = AvaloniaLocator.Current;

		if (locator.GetService<IPlatformGraphics>() is not GodotVkPlatformGraphics graphics) {
			GD.PrintErr("No Godot platform graphics found, did you forget to register your Avalonia app with UseGodot()?");
			return;
		}

		var keyboardDevice = locator.GetRequiredService<IKeyboardDevice>();
		var mouseDevice = locator.GetRequiredService<IMouseDevice>();

		var topLevelImpl = new GodotTopLevelImpl(graphics, keyboardDevice, mouseDevice) {
			ClientSize = Size.ToAvaloniaSize()
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

	private void OnSizeChanged() {
		if (_topLevel is null)
			return;

		var size = Size.ToAvaloniaSize();
		_topLevel.Impl.ClientSize = size;
		_topLevel.Measure(size);
		_topLevel.Arrange(new Rect(size));
	}

	public override void _Draw() {
		if (_topLevel is null)
			return;

		_topLevel.Renderer.Paint(new Rect(Size.ToAvaloniaSize()));
		DrawTexture(_topLevel.Impl.GetTexture(), Vector2.Zero);
	}

	public override void _GuiInput(InputEvent @event) {
		if (_topLevel is null)
			return;

		if (TryHandleInput(_topLevel.Impl, @event))
			GetViewport().SetInputAsHandled();
	}

	private static bool TryHandleInput(GodotTopLevelImpl impl, InputEvent inputEvent)
		=> inputEvent switch {
			InputEventMouseMotion mouseMotion => impl.OnMouseMotion(mouseMotion, Time.GetTicksMsec()),
			InputEventMouseButton mouseButton => impl.OnMouseButton(mouseButton, Time.GetTicksMsec()),
			InputEventKey key => impl.OnKey(key, Time.GetTicksMsec()),
			_ => false
		};

	protected override void Dispose(bool disposing) {
		if (disposing && _topLevel is not null) {
			_topLevel.Dispose();

			// ensure the underlying Avalonia compositor render target is disposed
			// shouldn't be needed anymore if https://github.com/AvaloniaUI/Avalonia/pull/11262/ is merged
			AvDispatcher.UIThread.RunJobs();
			_topLevel.Impl.RenderTimer.TriggerTick(new TimeSpan((long) (Time.GetTicksUsec() * 10UL)));

			_topLevel = null;
		}

		base.Dispose(disposing);
	}

}
