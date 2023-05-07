using Avalonia;
using Godot;
using JLeb.Estragonia;

namespace HelloWorld;

public partial class HelloAvalonia : AvaloniaControl {

	public override void _Ready() {
		(GetViewport() as Window)?.SetImeActive(true);

		AppBuilder.Configure<App>()
			.UseWin32() // TODO: remove this, implement all needed services
			.UseGodot()
			.SetupWithoutStarting();

		Control = new TestControl();

		base._Ready();
	}

}
