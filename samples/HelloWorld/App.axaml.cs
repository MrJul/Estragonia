using Avalonia;
using Avalonia.Markup.Xaml;

namespace HelloWorld;

public class App : Application {

	public override void Initialize()
		=> AvaloniaXamlLoader.Load(this);

}
