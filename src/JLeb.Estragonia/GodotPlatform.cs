﻿using System;
using System.Threading;
using Avalonia;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Platform;
using Avalonia.Rendering;
using Avalonia.Rendering.Composition;
using Avalonia.Threading;
using Godot;
using Dispatcher = Avalonia.Threading.Dispatcher;

namespace JLeb.Estragonia;

/// <summary>Contains Godot to Avalonia platform initialization.</summary>
internal static class GodotPlatform {

	private static Compositor? s_compositor;
	private static ManualRenderTimer? s_renderTimer;
	private static ulong s_lastProcessFrame = UInt64.MaxValue;

	public static Compositor Compositor
		=> s_compositor ?? VerifyInitialized();

	private static Compositor VerifyInitialized()
		=> throw new InvalidOperationException($"{nameof(GodotPlatform)} hasn't been initialized");

	public static void Initialize() {
		var platformGraphics = new GodotVkPlatformGraphics();

		AvaloniaLocator.CurrentMutable
			.Bind<IClipboard>().ToConstant(new GodotClipboard())
			.Bind<ICursorFactory>().ToConstant(new GodotCursorFactory())
			.Bind<IDispatcherImpl>().ToConstant(new GodotDispatcherImpl(Thread.CurrentThread))
			.Bind<IKeyboardDevice>().ToConstant(new KeyboardDevice())
			.Bind<IMouseDevice>().ToConstant(new MouseDevice())
			.Bind<IPlatformGraphics>().ToConstant(platformGraphics)
			.Bind<IPlatformIconLoader>().ToConstant(new StubPlatformIconLoader())
			.Bind<IPlatformSettings>().ToConstant(new GodotPlatformSettings())
			.Bind<IWindowingPlatform>().ToConstant(new GodotWindowingPlatform())
			.Bind<PlatformHotkeyConfiguration>().ToConstant(new PlatformHotkeyConfiguration());

		// bind the render loop last, as it uses the dispatcher which must be registered before
		var renderTimer = new ManualRenderTimer();
		var renderLoop = new RenderLoop(renderTimer, Dispatcher.UIThread);

		AvaloniaLocator.CurrentMutable
			.Bind<IRenderTimer>().ToConstant(renderTimer)
			.Bind<IRenderLoop>().ToConstant(renderLoop);

		s_renderTimer = renderTimer;
		s_compositor = new Compositor(renderLoop, platformGraphics);
	}

	public static void TriggerRenderTick() {
		if (s_renderTimer is null)
			return;

		// if we have several AvaloniaControl, ensure we tick the timer only once each frame
		var processFrame = Engine.GetProcessFrames();
		if (processFrame == s_lastProcessFrame)
			return;

		s_lastProcessFrame = processFrame;
		s_renderTimer.TriggerTick(new TimeSpan((long) (Time.GetTicksUsec() * 10UL)));
	}

}
