using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;

namespace GameMenu.UI;

public sealed partial class OptionsViewModel : ViewModel {

	private readonly UIOptions _uiOptions;
	private bool _vSync;
	private bool _showFps;
	private double _uiScale;
	private bool _canApply;

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

	public bool CanApply {
		get => _canApply;
		set {
			if (SetField(ref _canApply, value))
				ApplyCommand.NotifyCanExecuteChanged();
		}
	}

	public OptionsViewModel(UIOptions uiOptions) {
		_uiOptions = uiOptions;
		_vSync = uiOptions.VSync;
		_showFps = uiOptions.ShowFps;
		_uiScale = uiOptions.UIScale;
	}

	protected override void OnPropertyChanged(string? propertyName = null) {
		base.OnPropertyChanged(propertyName);

		if (propertyName != nameof(CanApply))
			CanApply = true;
	}

	protected override Task LoadAsync()
		=> Task.CompletedTask;

	[RelayCommand(CanExecute = nameof(CanApply))]
	public void Apply() {
		_uiOptions.VSync = VSync;
		_uiOptions.ShowFps = ShowFps;
		_uiOptions.UIScale = UIScale;
		CanApply = false;
	}

}
