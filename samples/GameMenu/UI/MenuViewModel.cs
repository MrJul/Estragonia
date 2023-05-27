using System.Collections.ObjectModel;

namespace GameMenu.UI;

public class MenuViewModel : ViewModel {

	public ObservableCollection<MenuItemViewModel> Items { get; } = new() {
		new MenuItemViewModel { Display = "New Game" },
		new MenuItemViewModel { Display = "Continue" },
		new MenuItemViewModel { Display = "Options" },
		new MenuItemViewModel { Display = "Exit" }
	};

}
