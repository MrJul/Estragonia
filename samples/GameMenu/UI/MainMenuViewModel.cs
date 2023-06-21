using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace GameMenu.UI;

public sealed class MainMenuViewModel : ViewModel {

	private readonly INavigator _navigator;
	private readonly UIOptions _uiOptions;

	public ObservableCollection<MainMenuItem> Items { get; } = new();

	public MainMenuViewModel(INavigator navigator, UIOptions uiOptions) {
		_navigator = navigator;
		_uiOptions = uiOptions;

		Items.Add(new MainMenuItem("New Game", StartNewGameAsync));
		Items.Add(new MainMenuItem("Continue", ContinueGameAsync));
		Items.Add(new MainMenuItem("Options", OpenOptionsAsync));
		Items.Add(new MainMenuItem("Exit", ExitAsync));
	}

	protected override Task LoadAsync()
		=> Task.CompletedTask;

	private Task ContinueGameAsync()
		=> Task.CompletedTask;

	private Task StartNewGameAsync()
		=> Task.CompletedTask;

	private Task OpenOptionsAsync() {
		_navigator.NavigateTo(new OptionsViewModel(_uiOptions));
		return Task.CompletedTask;
	}

	private Task ExitAsync() {
		_navigator.Quit();
		return Task.CompletedTask;
	}

}
