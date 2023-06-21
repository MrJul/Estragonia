using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Templates;

namespace GameMenu.UI;

public sealed class ViewLocator : IDataTemplate {

	private readonly Dictionary<Type, ViewFactory> _viewFactoryByModelType = new() {
		[typeof(MainMenuViewModel)] = new(() => new MainMenuView(), shouldCacheView: true),
		[typeof(OptionsViewModel)] = new(() => new OptionsView(), shouldCacheView: false)
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
		private readonly bool _shouldCacheView;
		private View? _cachedView;

		public View GetOrCreateView()
			=> _shouldCacheView
				? _cachedView ??= _createView()
				: _createView();

		public ViewFactory(Func<View> createView, bool shouldCacheView) {
			_createView = createView;
			_shouldCacheView = shouldCacheView;
		}

	}

}
