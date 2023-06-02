using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace GameMenu.UI;

public class MainMenuViewModel : ViewModel {

	private readonly MainViewModel _mainViewModel;
	private readonly UIOptions _uiOptions;

	public ObservableCollection<MainMenuItem> Items { get; } = new();

	public MainMenuViewModel(MainViewModel mainViewModel, UIOptions uiOptions) {
		_mainViewModel = mainViewModel;
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
		_mainViewModel.NavigateTo(new OptionsViewModel(_uiOptions));
		return Task.CompletedTask;
	}

	private Task ExitAsync() {
		_mainViewModel.SceneTree.Quit();
		return Task.CompletedTask;
	}

}
