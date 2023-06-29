using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Metadata;

namespace GameMenu.UI;

/// <summary>Defines a directional focus structure that contains all focusable controls in the view, grouped by rows.</summary>
/// <remarks>
/// Avalonia doesn't have proper directional navigation yet (https://github.com/AvaloniaUI/Avalonia/issues/7607)
/// Let's add a simple one here based on explicitly set controls only.
/// </remarks>
public class DirectionalFocusStructure : AvaloniaObject {

	private BuiltStructure? _structure;

	private BuiltStructure Structure
		=> _structure ??= BuildStructure();

	[Content]
	[SuppressMessage("ReSharper", "CollectionNeverUpdated.Global", Justification = "Updated in XAML")]
	public AvaloniaList<DirectionalFocusControl> Controls { get; } = new();

	private BuiltStructure BuildStructure() {
		var rows = new List<Row>();
		var currentRowControls = new List<Control>();
		Control? currentRowDefaultControl = null;

		foreach (var focusControl in Controls) {
			if (focusControl.Control is not { } control)
				continue;

			if (focusControl.IsNewRow)
				AddCurrentRow();

			currentRowControls.Add(control);

			if (focusControl.IsRowDefault)
				currentRowDefaultControl ??= control;
		}

		AddCurrentRow();

		return new BuiltStructure(rows.ToArray());

		void AddCurrentRow() {
			if (currentRowControls.Count == 0)
				return;

			rows.Add(new Row(currentRowControls.ToArray(), currentRowDefaultControl));
			currentRowControls.Clear();
			currentRowDefaultControl = null;
		}
	}

	/// <summary>
	/// Gets the control to the bottom of the anchor:
	///  - If the anchor is null, returns the first control in the grid.
	///  - If the next row has a default control, returns it.
	///  - Otherwise, returns the first control in the next row.
	/// </summary>
	public Control? GetDown(Control? anchor)
		=> Structure.GetDown(anchor);

	/// <summary>
	/// Gets the control to the top of the anchor:
	///  - If the anchor is null, returns null.
	///  - If the previous row has a default control, returns it.
	///  - Otherwise returns the last control in the previous row.
	/// </summary>
	public Control? GetUp(Control? anchor)
		=> Structure.GetUp(anchor);

	/// <summary>
	/// Gets the control to the right of the anchor:
	///  - If the anchor is null, returns the first control in the grid.
	///  - If there's a control to the right in the same row as the anchor, returns it.
	/// </summary>
	public Control? GetRight(Control? anchor)
		=> Structure.GetRight(anchor);

	/// <summary>
	/// Gets the control to the left of the anchor:
	///  - If the anchor is null, returns null.
	///  - If there's a control to the left in the same row as the anchor, returns it.
	/// </summary>
	public Control? GetLeft(Control? anchor)
		=> Structure.GetLeft(anchor);

	private readonly struct BuiltStructure {

		private readonly Row[] _rows;

		public BuiltStructure(Row[] rows)
			=> _rows = rows;

		public Control? GetDown(Control? anchor) {
			if (FindControl(anchor) is not var (rowIndex, _))
				rowIndex = -1;

			return FindNextValidControl(rowIndex + 1);
		}

		public Control? GetUp(Control? anchor) {
			if (FindControl(anchor) is not var (rowIndex, _))
				rowIndex = _rows.Length;

			return FindPreviousValidControl(rowIndex - 1);
		}

		public Control? GetRight(Control? anchor)
			=> FindControl(anchor) is var (rowIndex, controlIndex)
				? _rows[rowIndex].FindNextValidControl(controlIndex + 1)
				: FindNextValidControl(0);

		public Control? GetLeft(Control? anchor)
			=> FindControl(anchor) is var (rowIndex, controlIndex)
				? _rows[rowIndex].FindPreviousValidControl(controlIndex - 1)
				: FindPreviousValidControl(_rows.Length - 1);

		private (int rowIndex, int controlIndex)? FindControl(Control? control) {
			if (control is null)
				return null;

			for (var rowIndex = 0; rowIndex < _rows.Length; ++rowIndex) {
				var row = _rows[rowIndex];
				var controlIndex = row.IndexOf(control);
				if (controlIndex >= 0)
					return (rowIndex, controlIndex);
			}

			return null;
		}

		private Control? FindNextValidControl(int startRowIndex) {
			for (var rowIndex = startRowIndex; rowIndex < _rows.Length; ++rowIndex) {
				var row = _rows[rowIndex];
				if ((row.TryGetValidDefaultControl() ?? row.FindNextValidControl(0)) is { } control)
					return control;
			}

			return null;
		}

		private Control? FindPreviousValidControl(int startRowIndex) {
			for (var rowIndex = startRowIndex; rowIndex >= 0; --rowIndex) {
				var row = _rows[rowIndex];
				if ((row.TryGetValidDefaultControl() ?? row.FindPreviousValidControl(row.Count - 1)) is { } control)
					return control;
			}

			return null;
		}

	}

	private readonly struct Row {

		private readonly Control[] _controls;
		private readonly Control? _defaultControl;

		public int Count
			=> _controls.Length;

		public Row(Control[] controls, Control? defaultControl) {
			_controls = controls;
			_defaultControl = defaultControl;
		}

		public int IndexOf(Control control)
			=> Array.IndexOf(_controls, control);

		public Control? TryGetValidDefaultControl()
			=> IsValidControl(_defaultControl) ? _defaultControl : null;

		public Control? FindNextValidControl(int startIndex) {
			for (var index = startIndex; index < _controls.Length; ++index) {
				if (IsValidControl(_controls[index]))
					return _controls[index];
			}

			return null;
		}

		public Control? FindPreviousValidControl(int startIndex) {
			for (var index = startIndex; index >= 0; --index) {
				if (IsValidControl(_controls[index]))
					return _controls[index];
			}

			return null;
		}

		private static bool IsValidControl(Control? control)
			=> control is { IsEffectivelyVisible: true, IsEffectivelyEnabled: true };

	}

}
