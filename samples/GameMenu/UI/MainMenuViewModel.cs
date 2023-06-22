using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;

namespace GameMenu.UI;

public sealed partial class MainMenuViewModel : ViewModel {

	private readonly INavigator _navigator;
	private readonly UIOptions _uiOptions;

	public MainMenuViewModel(INavigator navigator, UIOptions uiOptions) {
		_navigator = navigator;
		_uiOptions = uiOptions;
	}

	protected override Task LoadAsync()
		=> Task.CompletedTask;

	[RelayCommand]
	public void StartNewGame() {
	}

	[RelayCommand]
	public void LoadExistingGame() {
	}

	[RelayCommand]
	public void OpenOptions()
		=> _navigator.NavigateTo(new OptionsViewModel(_uiOptions));

	[RelayCommand]
	public void Exit()
		=> _navigator.Quit();

}
