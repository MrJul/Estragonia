using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace GameMenu.UI;

public sealed partial class DifficultyViewModel : ViewModel {

	private readonly INavigator _navigator;

	[ObservableProperty]
	private GameDifficulty _selectedDifficulty = GameDifficulty.Normal;

	public ObservableCollection<GameDifficulty> Difficulties { get; } = new(Enum.GetValues<GameDifficulty>());

	public DifficultyViewModel(INavigator navigator)
		=> _navigator = navigator;

	protected override Task LoadAsync()
		=> Task.CompletedTask;

	[RelayCommand]
	public void StartGame()
		=> _navigator.NavigateTo(new GameLoadingViewModel());

}
