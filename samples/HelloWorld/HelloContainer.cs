using Avalonia;
using JLeb.Estragonia;

namespace HelloWorld;

public partial class HelloContainer : AvaloniaContainer {

	public override void _Ready() {
		AppBuilder.Configure<App>()
			.UseWin32() // TODO: remove this, implement all needed services
			.UseSkia()
			.UseGodot()
			.With(new Win32PlatformOptions() {

			})
			.SetupWithoutStarting();

		Control = new TestControl();

		base._Ready();
	}

	public override void _Notification(int what)
		=> base._Notification(what);

}
