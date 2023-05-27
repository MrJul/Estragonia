using JLeb.Estragonia;

namespace GameMenu.UI;

public partial class UserInterface : AvaloniaControl {

	public override void _Ready() {
		GrabFocus();

		Control = new MenuView {
			DataContext = new MenuViewModel()
		};

		base._Ready();
	}

}
