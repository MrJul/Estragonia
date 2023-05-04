using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace HelloWorld;

public partial class TestControl : UserControl {

	public TestControl() {
		_ = new SolidColorBrush(); // force animator to be registered; TODO: investigate and remove
		InitializeComponent();
	}

	protected override Size MeasureCore(Size availableSize)
		=> base.MeasureCore(availableSize);

	protected override Size MeasureOverride(Size availableSize)
		=> base.MeasureOverride(availableSize);

}
