using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Templates;

namespace GameMenu.UI;

public sealed class ViewLocator : IDataTemplate {

	private readonly Dictionary<Type, Func<UserControl>> _viewFactoryByModelType = new() {
		[typeof(MainMenuViewModel)] = () => new MainMenuView(),
		[typeof(OptionsViewModel)] = () => new OptionsView()
	};

	public bool Match(object? data)
		=> data is ViewModel;

	public Control? Build(object? param) {
		var viewModelType = param?.GetType();

		return viewModelType is not null && _viewFactoryByModelType.TryGetValue(viewModelType, out var factory)
			? factory()
			: null;
	}

}
