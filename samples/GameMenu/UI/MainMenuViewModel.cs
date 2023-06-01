using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace GameMenu.UI;

public class MainMenuViewModel : ViewModel {

	private readonly ShellViewModel _shell;

	public ObservableCollection<MainMenuItem> Items { get; } = new();

	public MainMenuViewModel(ShellViewModel shell) {
		_shell = shell;

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
		_shell.NavigateTo(new OptionsViewModel());
		return Task.CompletedTask;
	}

	private Task ExitAsync() {
		_shell.SceneTree.Quit();
		return Task.CompletedTask;
	}

}
