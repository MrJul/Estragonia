using System;
using System.Threading;
using Avalonia;
using Avalonia.Controls.Platform;
using Avalonia.Dialogs;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Platform;
using Avalonia.Rendering;
using Avalonia.Threading;
using Godot;
using JLeb.Estragonia.Input;
using AvCompositor = Avalonia.Rendering.Composition.Compositor;

namespace JLeb.Estragonia;

/// <summary>Contains Godot to Avalonia platform initialization.</summary>
internal static class GodotPlatform {

	private static AvCompositor? s_compositor;
	private static ManualRenderTimer? s_renderTimer;
	private static ulong s_lastProcessFrame = UInt64.MaxValue;

	public static AvCompositor Compositor
		=> s_compositor ?? throw new InvalidOperationException($"{nameof(GodotPlatform)} hasn't been initialized");

	public static void Initialize() {
		AvaloniaSynchronizationContext.AutoInstall = false; // Godot has its own sync context, don't replace it

		var platformGraphics = new GodotVkPlatformGraphics();
		var renderTimer = new ManualRenderTimer();

		AvaloniaLocator.CurrentMutable
			.Bind<IClipboard>().ToConstant(new GodotClipboard())
			.Bind<ICursorFactory>().ToConstant(new GodotCursorFactory())
			.Bind<IDispatcherImpl>().ToConstant(new GodotDispatcherImpl(Thread.CurrentThread))
			.Bind<IKeyboardDevice>().ToConstant(GodotDevices.Keyboard)
			.Bind<IPlatformGraphics>().ToConstant(platformGraphics)
			.Bind<IPlatformIconLoader>().ToConstant(new StubPlatformIconLoader())
			.Bind<IPlatformSettings>().ToConstant(new GodotPlatformSettings())
			.Bind<IRenderTimer>().ToConstant(renderTimer)
			.Bind<IWindowingPlatform>().ToConstant(new GodotWindowingPlatform())
			.Bind<IStorageProviderFactory>().ToConstant(new GodotStorageProviderFactory())
			.Bind<PlatformHotkeyConfiguration>().ToConstant(CreatePlatformHotKeyConfiguration())
			.Bind<ManagedFileDialogOptions>().ToConstant(new ManagedFileDialogOptions { AllowDirectorySelection = true });

		s_renderTimer = renderTimer;
		s_compositor = new AvCompositor(platformGraphics);
	}

	private static PlatformHotkeyConfiguration CreatePlatformHotKeyConfiguration()
		=> OperatingSystem.IsMacOS()
			? new PlatformHotkeyConfiguration(commandModifiers: KeyModifiers.Meta, wholeWordTextActionModifiers: KeyModifiers.Alt)
			: new PlatformHotkeyConfiguration(commandModifiers: KeyModifiers.Control);

	public static void TriggerRenderTick() {
		if (s_renderTimer is null)
			return;

		// if we have several AvaloniaControls, ensure we tick the timer only once each frame
		var processFrame = Engine.GetProcessFrames();
		if (processFrame == s_lastProcessFrame)
			return;

		s_lastProcessFrame = processFrame;
		s_renderTimer.TriggerTick(new TimeSpan((long) (Time.GetTicksUsec() * 10UL)));
	}

}
