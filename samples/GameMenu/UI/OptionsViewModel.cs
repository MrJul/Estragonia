using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace GameMenu.UI;

public sealed partial class OptionsViewModel : ViewModel {

	private readonly UIOptions _uiOptions;

	[ObservableProperty]
	private bool _vSync;

	[ObservableProperty]
	private bool _showFps;

	[ObservableProperty]
	[SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Name required for correct property generation")]
	private double _UIScale;

	private bool _canApply;

	public bool CanApply {
		get => _canApply;
		private set {
			if (SetProperty(ref _canApply, value))
				ApplyCommand.NotifyCanExecuteChanged();
		}
	}

	public OptionsViewModel(UIOptions uiOptions) {
		_uiOptions = uiOptions;
		_vSync = uiOptions.VSync;
		_showFps = uiOptions.ShowFps;
		_UIScale = uiOptions.UIScale;
	}

	protected override void OnPropertyChanged(PropertyChangedEventArgs e) {
		base.OnPropertyChanged(e);

		if (e.PropertyName != nameof(CanApply))
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
