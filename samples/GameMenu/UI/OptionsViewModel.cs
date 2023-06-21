using System.Threading.Tasks;

namespace GameMenu.UI;

public sealed class OptionsViewModel : ViewModel {

	private readonly UIOptions _uiOptions;
	private bool _vSync;
	private bool _showFps;
	private double _uiScale;

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

	public OptionsViewModel(UIOptions uiOptions) {
		_uiOptions = uiOptions;
		_vSync = uiOptions.VSync;
		_showFps = uiOptions.ShowFps;
		_uiScale = uiOptions.UIScale;
	}

	protected override Task LoadAsync()
		=> Task.CompletedTask;

	public Task AcceptAsync() {
		_uiOptions.VSync = VSync;
		_uiOptions.ShowFps = ShowFps;
		_uiOptions.UIScale = UIScale;
		return Task.CompletedTask;
	}

}
