using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Metadata;

namespace GameMenu.UI;

/// <summary>
/// Defines a directional focus grid that contains all focusable controls in the view, ordered by rows and cells.
/// </summary>
/// <remarks>
/// Avalonia doesn't have proper directional navigation yet (https://github.com/AvaloniaUI/Avalonia/issues/7607)
/// Let's add a simple one here based on explicitly set controls only.
/// </remarks>
public class DirectionalFocusGrid : AvaloniaObject {

	[Content]
	[SuppressMessage("ReSharper", "CollectionNeverUpdated.Global", Justification = "Updated in XAML")]
	public AvaloniaList<DirectionalFocusRow> Rows { get; } = new();

	/// <summary>
	/// Gets the control to the bottom of the anchor:
	///  - If the anchor is null, returns the first control in the grid.
	///  - If the next row has a default control, returns it.
	///  - Otherwise, returns the first control in the next row.
	/// </summary>
	public Control? GetDown(Control? anchor) {
		if (FindAnchor(anchor) is not var (rowIndex, _))
			rowIndex = -1;

		return FindNextValidControl(rowIndex + 1);
	}

	/// <summary>
	/// Gets the control to the top of the anchor:
	///  - If the anchor is null, returns null.
	///  - If the previous row has a default control, returns it.
	///  - Otherwise returns the last control in the previous row.
	/// </summary>
	public Control? GetUp(Control? anchor) {
		if (FindAnchor(anchor) is not var (rowIndex, _))
			rowIndex = Rows.Count;

		return FindPreviousValidControl(rowIndex - 1);
	}

	/// <summary>
	/// Gets the control to the right of the anchor:
	///  - If the anchor is null, returns the first control in the grid.
	///  - If there's a control to the right in the same row as the anchor, returns it.
	/// </summary>
	public Control? GetRight(Control? anchor)
		=> FindAnchor(anchor) is var (rowIndex, cellIndex)
			? Rows[rowIndex].FindNextValidControl(cellIndex + 1)
			: FindNextValidControl(0);

	/// <summary>
	/// Gets the control to the left of the anchor:
	///  - If the anchor is null, returns null.
	///  - If there's a control to the left in the same row as the anchor, returns it.
	/// </summary>
	public Control? GetLeft(Control? anchor)
		=> FindAnchor(anchor) is var (rowIndex, cellIndex)
			? Rows[rowIndex].FindPreviousValidControl(cellIndex - 1)
			: FindPreviousValidControl(Rows.Count - 1);

	private (int rowIndex, int cellIndex)? FindAnchor(Control? anchor) {
		if (anchor is null)
			return null;

		for (var rowIndex = 0; rowIndex < Rows.Count; ++rowIndex) {
			var row = Rows[rowIndex];
			for (var cellIndex = 0; cellIndex < row.Cells.Count; ++cellIndex) {
				if (row.Cells[cellIndex].Control == anchor)
					return (rowIndex, cellIndex);
			}
		}

		return null;
	}

	private Control? FindNextValidControl(int startingRowIndex) {
		for (var rowIndex = startingRowIndex; rowIndex < Rows.Count; ++rowIndex) {
			var row = Rows[rowIndex];
			if ((row.TryGetValidDefaultControl() ?? row.FindNextValidControl(0)) is { } control)
				return control;
		}

		return null;
	}

	private Control? FindPreviousValidControl(int startingRowIndex) {
		for (var rowIndex = startingRowIndex; rowIndex >= 0; --rowIndex) {
			var row = Rows[rowIndex];
			if ((row.TryGetValidDefaultControl() ?? row.FindPreviousValidControl(row.Cells.Count - 1)) is { } control)
				return control;
		}

		return null;
	}

}
