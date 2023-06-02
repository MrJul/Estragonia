using System.Threading.Tasks;

namespace GameMenu.UI;

public class OptionsViewModel : ViewModel {

	private readonly UIOptions _uiOptions;
	private bool _isVSyncEnabled;
	private double _uiScale;

	public bool IsVSyncEnabled {
		get => _isVSyncEnabled;
		set => SetField(ref _isVSyncEnabled, value);
	}

	public double UIScale {
		get => _uiScale;
		set => SetField(ref _uiScale, value);
	}

	public OptionsViewModel(UIOptions uiOptions) {
		_uiOptions = uiOptions;
		_isVSyncEnabled = uiOptions.IsVSyncEnabled;
		_uiScale = uiOptions.UIScale;
	}

	protected override Task LoadAsync()
		=> Task.CompletedTask;

	public Task AcceptAsync() {
		_uiOptions.IsVSyncEnabled = IsVSyncEnabled;
		_uiOptions.UIScale = UIScale;
		return Task.CompletedTask;
	}

}
