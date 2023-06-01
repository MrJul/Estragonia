using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

namespace GameMenu.UI;

public class ShellViewModel : ViewModel {

	private readonly List<ViewModel> _openViewModels = new();
	private ViewModel? _currentViewModel;
	private int _framesPerSecond;

	public SceneTree SceneTree { get; }

	public ViewModel? CurrentViewModel {
		get => _currentViewModel;
		private set => SetField(ref _currentViewModel, value);
	}

	public int FramesPerSecond {
		get => _framesPerSecond;
		private set => SetField(ref _framesPerSecond, value);
	}

	public ShellViewModel(SceneTree sceneTree)
		=> SceneTree = sceneTree;

	public override async Task<bool> TryCloseAsync() {
		while (CurrentViewModel is not null) {
			if (!await TryCloseCurrentAsync())
				return false;
		}

		return true;
	}

	public async Task<bool> TryCloseCurrentAsync() {
		if (CurrentViewModel is null)
			return true;

		if (!await CurrentViewModel.TryCloseAsync())
			return false;

		_openViewModels.RemoveAt(_openViewModels.Count - 1);
		CurrentViewModel = _openViewModels.Count > 0 ? _openViewModels[^1] : null;
		return true;
	}

	public void NavigateTo(ViewModel viewModel) {
		_ = viewModel.EnsureLoadedAsync();
		_openViewModels.Add(viewModel);
		CurrentViewModel = viewModel;
	}

	protected override Task LoadAsync() {
		NavigateTo(new MainMenuViewModel(this));
		return Task.CompletedTask;
	}

	public void ProcessFrame()
		=> FramesPerSecond = (int) Engine.GetFramesPerSecond();

}
