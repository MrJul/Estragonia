using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Godot;

namespace GameMenu.UI;

public sealed partial class GameLoadingViewModel : ViewModel {

	// We're loading an almost empty scene: it's nearly instantaneous.
	// For demo purposes (we want to show the loading screen), set the real loading to be 10% of the total loading,
	// and simulate the rest by waiting.
	private const double RealProgressRatio = 0.1;

	private readonly INavigator _navigator;

	[ObservableProperty]
	private bool _isLoading = true;

	[ObservableProperty]
	private double _loadingProgress;

	public GameLoadingViewModel(INavigator navigator)
		=> _navigator = navigator;

	protected override async Task LoadAsync() {
		await Task.Delay(TimeSpan.FromSeconds(0.3));

		var gameScene = await AsyncGodotResourceLoader.LoadAsync<PackedScene>(
			"res://scenes/game.tscn",
			ResourceLoader.CacheMode.Ignore,
			new SceneLoadProgress(this)
		);

		var gameNode = gameScene.Instantiate();

		await SimulateProgressAsync();

		LoadingProgress = 1.0;
		await Task.Delay(TimeSpan.FromSeconds(0.1));

		_navigator.NavigateTo(new GameViewModel { GameNode = gameNode });
		await TryCloseAsync();
		IsLoading = false;
	}

	private async Task SimulateProgressAsync() {
		var delayInMs = (double) Random.Shared.Next(2000, 3000);
		var stopwatch = Stopwatch.StartNew();

		var fakeProgress = 0.0;

		while (fakeProgress < 1.0) {
			await Task.Delay(TimeSpan.FromSeconds(0.1));
			fakeProgress = Math.Min(1.0, stopwatch.ElapsedMilliseconds / delayInMs);
			LoadingProgress = RealProgressRatio + fakeProgress * (1.0 - RealProgressRatio);
		}
	}

	private sealed class SceneLoadProgress : IProgress<double> {

		private readonly GameLoadingViewModel _owner;

		public SceneLoadProgress(GameLoadingViewModel owner)
			=> _owner = owner;

		public void Report(double value)
			=> _owner.LoadingProgress = value * RealProgressRatio;

	}

}
