using System.ComponentModel;
using Godot;
using JLeb.Estragonia;

namespace GameMenu.UI;

public partial class UserInterface : AvaloniaControl {

	private UIOptions _uiOptions = null!;
	private MainViewModel _mainViewModel = null!;

	public override void _Ready() {
		GetWindow().MinSize = new Vector2I(480, 480);

		GrabFocus();

		_uiOptions = new UIOptions {
			UIScale = RenderScaling,
			IsVSyncEnabled = DisplayServer.WindowGetVsyncMode() != DisplayServer.VSyncMode.Disabled
		};
		_uiOptions.PropertyChanged += OnUIOptionsPropertyChanged;

		_mainViewModel = new MainViewModel(GetTree(), _uiOptions);
		_ = _mainViewModel.EnsureLoadedAsync();

		Control = new MainView {
			DataContext = _mainViewModel
		};

		base._Ready();
	}

	private void OnUIOptionsPropertyChanged(object? sender, PropertyChangedEventArgs e) {
		switch (e.PropertyName)
		{
			case nameof(UIOptions.IsVSyncEnabled):
				var vsyncMode = _uiOptions.IsVSyncEnabled ? DisplayServer.VSyncMode.Enabled : DisplayServer.VSyncMode.Disabled;
				DisplayServer.WindowSetVsyncMode(vsyncMode);
				break;
			case nameof(UIOptions.UIScale):
				RenderScaling = _uiOptions.UIScale;
				break;
		}
	}

	public override void _Process(double delta) {
		_mainViewModel.ProcessFrame();

		base._Process(delta);
	}

}
