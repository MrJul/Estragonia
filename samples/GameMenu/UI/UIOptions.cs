namespace GameMenu.UI;

public sealed class UIOptions : NotificationObject {

	private bool _isVSyncEnabled = true;
	private double _uiScale = 1.0;

	public bool IsVSyncEnabled {
		get => _isVSyncEnabled;
		set => SetField(ref _isVSyncEnabled, value);
	}

	public double UIScale {
		get => _uiScale;
		set => SetField(ref _uiScale, value);
	}

}
