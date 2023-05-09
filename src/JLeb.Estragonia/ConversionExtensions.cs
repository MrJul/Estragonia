using Avalonia;
using Avalonia.Input;
using Godot;
using Godot.Collections;
using AvColor = Avalonia.Media.Color;
using AvKey = Avalonia.Input.Key;
using GdCursorShape = Godot.Control.CursorShape;
using GdColor = Godot.Color;
using GdKey = Godot.Key;

namespace JLeb.Estragonia;

/// <summary>Contains extensions methods to convert between Godot and Avalonia types.</summary>
public static class ConversionExtensions {

	// Reference: https://github.com/godotengine/godot/blob/master/platform/windows/key_mapping_windows.cpp
	private static readonly Dictionary<GdKey, AvKey> s_keyMap = new() {
		[GdKey.Backspace] = AvKey.Back,
		[GdKey.Tab] = AvKey.Tab, // Godot maps Tab and CrSel to the same key
		[GdKey.Clear] = AvKey.Clear, // Godot maps both Clear and OEM Clear to the same key
		[GdKey.Enter] = AvKey.Return,
		[GdKey.Shift] = AvKey.LeftShift, // Godot maps Left Shift and Right Shift keys to the same key
		[GdKey.Ctrl] = AvKey.LeftCtrl, // Godot maps Left Ctrl and Right Ctrl keys to the same key
		[GdKey.Alt] = AvKey.LeftAlt, // Godot maps Left Alt and Right Alt keys to the same key
		[GdKey.Pause] = AvKey.Pause,
		[GdKey.Capslock] = AvKey.CapsLock,
		[GdKey.Escape] = AvKey.Escape, // Godot maps Escape and Attn keys to the same key
		[GdKey.Space] = AvKey.Space,
		[GdKey.Pageup] = AvKey.PageUp,
		[GdKey.Pagedown] = AvKey.PageDown,
		[GdKey.End] = AvKey.End,
		[GdKey.Home] = AvKey.Home,
		[GdKey.Left] = AvKey.Left,
		[GdKey.Up] = AvKey.Up,
		[GdKey.Right] = AvKey.Right,
		[GdKey.Down] = AvKey.Down,
		[GdKey.Print] = AvKey.Snapshot, // Godot maps Print and Snapshot keys to the same key
		[GdKey.Insert] = AvKey.Insert,
		[GdKey.Delete] = AvKey.Delete,
		[GdKey.Help] = AvKey.Help,
		[GdKey.A] = AvKey.A,
		[GdKey.B] = AvKey.B,
		[GdKey.C] = AvKey.C,
		[GdKey.D] = AvKey.D,
		[GdKey.E] = AvKey.E,
		[GdKey.F] = AvKey.F,
		[GdKey.G] = AvKey.G,
		[GdKey.H] = AvKey.H,
		[GdKey.I] = AvKey.I,
		[GdKey.J] = AvKey.J,
		[GdKey.K] = AvKey.K,
		[GdKey.L] = AvKey.L,
		[GdKey.M] = AvKey.M,
		[GdKey.N] = AvKey.N,
		[GdKey.O] = AvKey.O,
		[GdKey.P] = AvKey.P,
		[GdKey.Q] = AvKey.Q,
		[GdKey.R] = AvKey.R,
		[GdKey.S] = AvKey.S,
		[GdKey.T] = AvKey.T,
		[GdKey.U] = AvKey.U,
		[GdKey.V] = AvKey.V,
		[GdKey.W] = AvKey.W,
		[GdKey.X] = AvKey.X,
		[GdKey.Y] = AvKey.Y,
		[GdKey.Z] = AvKey.Z,
		[GdKey.Meta] = AvKey.LWin, // Godot maps Left Win and Right Win keys to the same key
		[GdKey.Menu] = AvKey.Apps, // Godot maps Left Menu and Right Menu keys to the same key
		[GdKey.Standby] = AvKey.Sleep,
		[GdKey.Kp0] = AvKey.NumPad0,
		[GdKey.Kp1] = AvKey.NumPad1,
		[GdKey.Kp2] = AvKey.NumPad2,
		[GdKey.Kp3] = AvKey.NumPad3,
		[GdKey.Kp4] = AvKey.NumPad4,
		[GdKey.Kp5] = AvKey.NumPad5,
		[GdKey.Kp6] = AvKey.NumPad6,
		[GdKey.Kp7] = AvKey.NumPad7,
		[GdKey.Kp8] = AvKey.NumPad8,
		[GdKey.Kp9] = AvKey.NumPad9,
		[GdKey.KpMultiply] = AvKey.Multiply,
		[GdKey.KpAdd] = AvKey.Add,
		[GdKey.KpSubtract] = AvKey.Subtract,
		[GdKey.KpPeriod] = AvKey.Decimal, // Godot maps Separator and Decimal keys to the same key
		[GdKey.KpDivide] = AvKey.Divide,
		[GdKey.F1] = AvKey.F1,
		[GdKey.F2] = AvKey.F2,
		[GdKey.F3] = AvKey.F3,
		[GdKey.F4] = AvKey.F4,
		[GdKey.F5] = AvKey.F5,
		[GdKey.F6] = AvKey.F6,
		[GdKey.F7] = AvKey.F7,
		[GdKey.F8] = AvKey.F8,
		[GdKey.F9] = AvKey.F9,
		[GdKey.F10] = AvKey.F10,
		[GdKey.F11] = AvKey.F11,
		[GdKey.F12] = AvKey.F12,
		[GdKey.F13] = AvKey.F13,
		[GdKey.F14] = AvKey.F14,
		[GdKey.F15] = AvKey.F15,
		[GdKey.F16] = AvKey.F16,
		[GdKey.F17] = AvKey.F17,
		[GdKey.F18] = AvKey.F18,
		[GdKey.F19] = AvKey.F19,
		[GdKey.F20] = AvKey.F20,
		[GdKey.F21] = AvKey.F21,
		[GdKey.F22] = AvKey.F22,
		[GdKey.F23] = AvKey.F23,
		[GdKey.F24] = AvKey.F24,
		[GdKey.Numlock] = AvKey.NumLock,
		[GdKey.Scrolllock] = AvKey.Scroll,
		[GdKey.Back] = AvKey.BrowserBack,
		[GdKey.Forward] = AvKey.BrowserForward,
		[GdKey.Refresh] = AvKey.BrowserRefresh,
		[GdKey.Stop] = AvKey.BrowserStop,
		[GdKey.Search] = AvKey.BrowserSearch,
		[GdKey.Favorites] = AvKey.BrowserFavorites,
		[GdKey.Homepage] = AvKey.Home,
		[GdKey.Volumemute] = AvKey.VolumeMute,
		[GdKey.Volumedown] = AvKey.VolumeDown,
		[GdKey.Volumeup] = AvKey.VolumeUp,
		[GdKey.Medianext] = AvKey.MediaNextTrack,
		[GdKey.Mediaprevious] = AvKey.MediaPreviousTrack,
		[GdKey.Mediastop] = AvKey.MediaStop,
		[GdKey.Mediaplay] = AvKey.MediaPlayPause, // Godot maps bot Media Play/Pause and Play keys to the same key
		[GdKey.Launchmail] = AvKey.LaunchMail,
		[GdKey.Launchmedia] = AvKey.SelectMedia,
		[GdKey.Launch0] = AvKey.LaunchApplication1,
		[GdKey.Launch1] = AvKey.LaunchApplication2,
		[GdKey.Semicolon] = AvKey.Oem1,
		[GdKey.Equal] = AvKey.OemPlus,
		[GdKey.Comma] = AvKey.OemComma,
		[GdKey.Minus] = AvKey.OemMinus,
		[GdKey.Period] = AvKey.OemPeriod,
		[GdKey.Slash] = AvKey.Oem2,
		[GdKey.Quoteleft] = AvKey.Oem3,
		[GdKey.Bracketleft] = AvKey.Oem4,
		[GdKey.Backslash] = AvKey.Oem5,
		[GdKey.Bracketright] = AvKey.Oem6,
		[GdKey.Apostrophe] = AvKey.Oem7,
		[GdKey.Bar] = AvKey.Oem102
	};

	private static readonly Dictionary<StandardCursorType, GdCursorShape> s_cursorMap = new() {
		[StandardCursorType.Arrow] = GdCursorShape.Arrow,
		[StandardCursorType.Ibeam] = GdCursorShape.Ibeam,
		[StandardCursorType.Wait] = GdCursorShape.Wait,
		[StandardCursorType.Cross] = GdCursorShape.Cross,
		[StandardCursorType.UpArrow] = GdCursorShape.Arrow,
		[StandardCursorType.SizeWestEast] = GdCursorShape.Hsize,
		[StandardCursorType.SizeNorthSouth] = GdCursorShape.Vsize,
		[StandardCursorType.SizeAll] = GdCursorShape.Drag,
		[StandardCursorType.No] = GdCursorShape.Forbidden,
		[StandardCursorType.Hand] = GdCursorShape.PointingHand,
		[StandardCursorType.AppStarting] = GdCursorShape.Busy,
		[StandardCursorType.Help] = GdCursorShape.Help,
		[StandardCursorType.TopSide] = GdCursorShape.Vsize,
		[StandardCursorType.BottomSide] = GdCursorShape.Vsize,
		[StandardCursorType.LeftSide] = GdCursorShape.Hsize,
		[StandardCursorType.RightSide] = GdCursorShape.Hsize,
		[StandardCursorType.TopLeftCorner] = GdCursorShape.Fdiagsize,
		[StandardCursorType.TopRightCorner] = GdCursorShape.Bdiagsize,
		[StandardCursorType.BottomLeftCorner] = GdCursorShape.Bdiagsize,
		[StandardCursorType.BottomRightCorner] = GdCursorShape.Fdiagsize,
		[StandardCursorType.DragMove] = GdCursorShape.Drag,
		[StandardCursorType.DragCopy] = GdCursorShape.Drag,
		[StandardCursorType.DragLink] = GdCursorShape.Drag
	};

	public static AvKey ToAvaloniaKey(this GdKey source)
		=> s_keyMap.TryGetValue(source, out var result) ? result : AvKey.None;

	public static Size ToAvaloniaSize(this Vector2 source)
		=> new(source.X, source.Y);

	public static Point ToAvaloniaPoint(this Vector2 source)
		=> new(source.X, source.Y);

	public static AvColor ToAvaloniaColor(this GdColor source)
		=> new((byte) source.A8, (byte) source.R8, (byte) source.G8, (byte) source.B8);

	public static GdCursorShape ToGodotCursorShape(this StandardCursorType source)
		=> s_cursorMap.TryGetValue(source, out var result) ? result : GdCursorShape.Arrow;

}
