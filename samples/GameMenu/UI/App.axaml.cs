using Avalonia;
using Avalonia.Markup.Xaml;

namespace GameMenu.UI;

public class App : Application {

	public override void Initialize()
		=> AvaloniaXamlLoader.Load(this);

}
