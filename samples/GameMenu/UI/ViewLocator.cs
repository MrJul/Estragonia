using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Templates;

namespace GameMenu.UI;

public sealed class ViewLocator : IDataTemplate {

	private readonly Dictionary<Type, ViewFactory> _viewFactoryByModelType = new() {
		[typeof(MainMenuViewModel)] = new(() => new MainMenuView(), cached: true),
		[typeof(DifficultyViewModel)] = new(() => new DifficultyView()),
		[typeof(OptionsViewModel)] = new(() => new OptionsView())
	};

	public bool Match(object? data)
		=> data is ViewModel;

	public Control? Build(object? param) {
		var viewModelType = param?.GetType();

		return viewModelType is not null && _viewFactoryByModelType.TryGetValue(viewModelType, out var viewFactory)
			? viewFactory.GetOrCreateView()
			: null;
	}

	private sealed class ViewFactory {

		private readonly Func<View> _createView;
		private readonly bool _cached;
		private View? _cachedView;

		public View GetOrCreateView()
			=> _cached
				? _cachedView ??= _createView()
				: _createView();

		public ViewFactory(Func<View> createView, bool cached = false) {
			_createView = createView;
			_cached = cached;
		}

	}

}
