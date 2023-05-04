using Avalonia;
using JLeb.Estragonia;

namespace HelloWorld;

public partial class HelloViewport : AvaloniaViewport {

	public override void _Ready() {
		AppBuilder.Configure<App>()
			.UseWin32() // TODO: remove this, implement all needed services
			.UseSkia()
			.UseGodot()
			.SetupWithoutStarting();

		Control = new TestControl();

		base._Ready();
	}

}
