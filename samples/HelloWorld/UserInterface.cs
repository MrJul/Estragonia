using Godot;
using JLeb.Estragonia;

namespace HelloWorld;

public partial class UserInterface : AvaloniaControl {

	public override void _Ready() {
		Control = new HelloWorldView();

		base._Ready();
	}

	public override void _Process(double delta) {
		((HelloWorldView) Control!).FpsCounter.Text = $"FPS: {Engine.GetFramesPerSecond():F0}";

		base._Process(delta);
	}

}
