using JLeb.Estragonia;

namespace GameMenu.UI;

public partial class UserInterface : AvaloniaControl {

	private ShellViewModel _shellViewModel = null!;

	public override void _Ready() {
		GrabFocus();

		_shellViewModel = new ShellViewModel(GetTree());
		_ = _shellViewModel.EnsureLoadedAsync();

		Control = new ShellView {
			DataContext = _shellViewModel
		};

		base._Ready();
	}

	public override void _Process(double delta) {
		_shellViewModel.ProcessFrame();

		base._Process(delta);
	}

}
