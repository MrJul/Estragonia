using System;

namespace GameMenu.UI;

public class MenuItemViewModel : ViewModel {

	private string _display = String.Empty;

	public string Display {
		get => _display;
		set => SetField(ref _display, value);
	}

}
