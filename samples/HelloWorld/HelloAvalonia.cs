using Avalonia;
using JLeb.Estragonia;

namespace HelloWorld;

public partial class HelloAvalonia : AvaloniaControl {

	public override void _Ready() {
		GetWindow().SetImeActive(true);

		AppBuilder.Configure<App>()
			.UseGodot()
			.SetupWithoutStarting();

		Control = new TestControl();

		base._Ready();
	}

}
