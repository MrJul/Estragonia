using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Godot;

namespace HelloWorld;

public partial class TestControl : UserControl {

	public TestControl() {
		_ = new SolidColorBrush(); // force animator to be registered; TODO: investigate and remove
		InitializeComponent();
	}

	protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e) {
		base.OnAttachedToLogicalTree(e);

		this.GetLogicalParent<TopLevel>()?.AttachDevTools();
	}

	private void Button_OnClick(object? sender, RoutedEventArgs e)
		=> GD.Print("Clicked!");

}
