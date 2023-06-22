using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Input;

namespace GameMenu.UI.Controls;

/// <summary>A slide transition that goes backwards when coming back to the main menu.</summary>
public sealed class AutoReversePageSlide : PageSlide {

	public override async Task Start(Visual? from, Visual? to, bool forward, CancellationToken cancellationToken) {
		var fromElement = from as InputElement;

		if (fromElement is not null)
			fromElement.IsHitTestVisible = false;

		if (to?.DataContext is MainMenuViewModel)
			forward = !forward;

		try {
			await base.Start(from, to, forward, cancellationToken);
		}
		finally {
			if (fromElement is not null)
				fromElement.IsHitTestVisible = true;
		}
	}

}
