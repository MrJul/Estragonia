using Avalonia.Controls;
using Avalonia.Media;

namespace HelloWorld;

public partial class HelloWorldView : UserControl {

	public HelloWorldView() {
		_ = new SolidColorBrush(); // force the color brush animator to be registered
		InitializeComponent();
	}

}

