using System.Threading.Tasks;
using Godot;

namespace GameMenu.UI;

public class OptionsViewModel : ViewModel {

	public bool IsVSyncEnabled {
		get => DisplayServer.WindowGetVsyncMode() != DisplayServer.VSyncMode.Disabled;
		set {
			if (IsVSyncEnabled == value)
				return;

			DisplayServer.WindowSetVsyncMode(value ? DisplayServer.VSyncMode.Enabled : DisplayServer.VSyncMode.Disabled);
			OnPropertyChanged();
		}
	}

	protected override Task LoadAsync()
		=> Task.CompletedTask;

}
