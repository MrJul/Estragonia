using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Animation;

namespace GameMenu.UI.Controls;

/// <summary>A slide transition that goes backwards when coming back to the main menu.</summary>
public sealed class AutoReversePageSlide : PageSlide {

	public override Task Start(Visual? from, Visual? to, bool forward, CancellationToken cancellationToken) {
		if (to?.DataContext is MainMenuViewModel)
			forward = !forward;

		return base.Start(from, to, forward, cancellationToken);
	}

}
