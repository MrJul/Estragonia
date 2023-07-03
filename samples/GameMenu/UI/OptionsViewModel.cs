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
	private bool _fullscreen;

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
		_fullscreen = uiOptions.Fullscreen;
		_showFps = uiOptions.ShowFps;
		_UIScale = uiOptions.UIScale;
	}

	protected override void OnPropertyChanged(PropertyChangedEventArgs e) {
		base.OnPropertyChanged(e);

		if (e.PropertyName is nameof(VSync) or nameof(Fullscreen) or nameof(ShowFps) or nameof(UIScale))
			CanApply = true;
	}

	protected override Task LoadAsync()
		=> Task.CompletedTask;

	[RelayCommand(CanExecute = nameof(CanApply))]
	public void Apply() {
		_uiOptions.VSync = VSync;
		_uiOptions.Fullscreen = Fullscreen;
		_uiOptions.ShowFps = ShowFps;
		_uiOptions.UIScale = UIScale;
		CanApply = false;
	}

}
