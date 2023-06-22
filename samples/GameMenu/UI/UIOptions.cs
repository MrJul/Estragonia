using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Mvvm.ComponentModel;

namespace GameMenu.UI;

public sealed partial class UIOptions : ObservableObject {

	[ObservableProperty]
	private bool _vSync;

	[ObservableProperty]
	private bool _showFps;

	[ObservableProperty]
	[SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Name required for correct property generation")]
	private double _UIScale;

}
