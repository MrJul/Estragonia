using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Metadata;

namespace GameMenu.UI;

public class DirectionalFocusRow : AvaloniaObject {

	[Content]
	[SuppressMessage("ReSharper", "CollectionNeverUpdated.Global", Justification = "Updated in XAML")]
	public AvaloniaList<DirectionalFocusCell> Cells { get; } = new();

	public Control? TryGetValidDefaultControl() {
		foreach (var cell in Cells) {
			if (cell.IsRowDefault)
				return cell.TryGetValidControl();
		}

		return null;
	}

	public Control? FindNextValidControl(int startingCellIndex) {
		for (var cellIndex = startingCellIndex; cellIndex < Cells.Count; ++cellIndex) {
			if (Cells[cellIndex].TryGetValidControl() is { } control)
				return control;
		}

		return null;
	}

	public Control? FindPreviousValidControl(int startingCellIndex) {
		for (var cellIndex = startingCellIndex; cellIndex >= 0; --cellIndex) {
			if (Cells[cellIndex].TryGetValidControl() is { } control)
				return control;
		}

		return null;
	}

}
