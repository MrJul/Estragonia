using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

namespace GameMenu.UI;

public class MainViewModel : ViewModel {

	private readonly List<ViewModel> _openViewModels = new();

	private int _framesPerSecond;

	public SceneTree SceneTree { get; }

	public UIOptions UIOptions { get; }

	public ViewModel? CurrentViewModel
		=> _openViewModels.Count > 0 ? _openViewModels[^1] : null;

	public int FramesPerSecond {
		get => _framesPerSecond;
		private set => SetField(ref _framesPerSecond, value);
	}

	public MainViewModel(SceneTree sceneTree, UIOptions uiOptions) {
		UIOptions = uiOptions;
		SceneTree = sceneTree;
	}

	protected override async Task<bool> TryCloseCoreAsync() {
		while (CurrentViewModel is not null) {
			if (!await TryCloseCurrentAsync())
				return false;
		}

		return true;
	}

	public async Task<bool> TryCloseCurrentAsync()
		=> CurrentViewModel is { } viewModel && await viewModel.TryCloseAsync();

	public void NavigateTo(ViewModel viewModel) {
		_ = viewModel.EnsureLoadedAsync();

		_openViewModels.Add(viewModel);
		viewModel.Closed += OnViewModelClosed;
		OnPropertyChanged(nameof(CurrentViewModel));
		return;

		void OnViewModelClosed(object? sender, EventArgs e) {
			viewModel.Closed -= OnViewModelClosed;

			var isCurrent = CurrentViewModel == viewModel;
			_openViewModels.Remove(viewModel);

			if (isCurrent)
				OnPropertyChanged(nameof(CurrentViewModel));
		}
	}

	protected override Task LoadAsync() {
		NavigateTo(new MainMenuViewModel(this, UIOptions));
		return Task.CompletedTask;
	}

	public void ProcessFrame()
		=> FramesPerSecond = (int) Engine.GetFramesPerSecond();

}
