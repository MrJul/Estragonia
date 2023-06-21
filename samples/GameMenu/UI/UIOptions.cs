namespace GameMenu.UI;

public sealed class UIOptions : NotificationObject {

	private bool _vSync = true;
	private bool _showFps = true;
	private double _uiScale = 1.0;

	public bool VSync {
		get => _vSync;
		set => SetField(ref _vSync, value);
	}

	public bool ShowFps {
		get => _showFps;
		set => SetField(ref _showFps, value);
	}

	public double UIScale {
		get => _uiScale;
		set => SetField(ref _uiScale, value);
	}

}
