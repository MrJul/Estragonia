using System.Threading.Tasks;

namespace GameMenu.UI;

public sealed class MainMenuViewModel : ViewModel {

	private readonly INavigator _navigator;
	private readonly UIOptions _uiOptions;

	public MainMenuViewModel(INavigator navigator, UIOptions uiOptions) {
		_navigator = navigator;
		_uiOptions = uiOptions;
	}

	protected override Task LoadAsync()
		=> Task.CompletedTask;

	public Task StartNewGameAsync()
		=> Task.CompletedTask;

	public Task LoadExistingGameAsync()
		=> Task.CompletedTask;

	public Task OpenOptionsAsync() {
		_navigator.NavigateTo(new OptionsViewModel(_uiOptions));
		return Task.CompletedTask;
	}

	public Task ExitAsync() {
		_navigator.Quit();
		return Task.CompletedTask;
	}

}
