﻿using Avalonia;
using Godot;
using JLeb.Estragonia;

namespace GameMenu.UI;

public sealed partial class AvaloniaLoader : Node {

	public override void _Ready()
		=> AppBuilder
			.Configure<App>()
			.UseGodot()
			.LogToTrace()
			.SetupWithoutStarting();

}
