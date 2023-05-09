using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Media;

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

	private async void Button_OnClick(object? sender, RoutedEventArgs e) {
		Debug.WriteLine($"Clicked on thread {System.Environment.CurrentManagedThreadId}");
		Debug.WriteLine($"Sync context is {SynchronizationContext.Current?.GetType()}");

		await Task.Run(() => {
			Debug.WriteLine($"Running some async code on thread {System.Environment.CurrentManagedThreadId}");
			Thread.Sleep(200);
		});

		Debug.WriteLine($"Back on thread {System.Environment.CurrentManagedThreadId}");
		Debug.WriteLine($"Sync context is {SynchronizationContext.Current?.GetType()}");
	}

}
