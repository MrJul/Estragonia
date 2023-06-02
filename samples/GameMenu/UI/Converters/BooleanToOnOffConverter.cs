using Avalonia.Data.Converters;

namespace GameMenu.UI.Converters;

public sealed class BooleanToOnOffConverter : FuncValueConverter<bool, string> {

	public BooleanToOnOffConverter()
		: base(value => value ? "On" : "Off") {
	}

}
