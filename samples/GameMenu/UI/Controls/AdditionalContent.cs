using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace GameMenu.UI.Controls;

public static class AdditionalContent {

	public static readonly AttachedProperty<Geometry?> GeometryProperty =
		AvaloniaProperty.RegisterAttached<Control, Geometry?>("Geometry", typeof(AdditionalContent));

	public static void SetGeometry(Control obj, Geometry? value)
		=> obj.SetValue(GeometryProperty, value);

	public static Geometry? GetGeometry(Control obj)
		=> obj.GetValue(GeometryProperty);

}
