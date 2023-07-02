using System;
using Godot;

namespace GameMenu;

public partial class GdAvSphere : MeshInstance3D {

	private Vector3 _pivot = Vector3.Zero;
	private float _pivotDistance = 6.0f;
	private float _pivotAngle;
	private float _pivotRotationSpeed = MathF.PI / 4.0f;

	private float _selfRotationSpeed = MathF.PI;
	private float _selfAngle;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		_pivotAngle += (float) (delta * _pivotRotationSpeed);
		GlobalPosition = _pivot + new Vector3(MathF.Cos(_pivotAngle), 0.0f, MathF.Sin(_pivotAngle)) * _pivotDistance;

		_selfAngle += (float) (delta * _selfRotationSpeed);
		Rotation = new Vector3(0.0f, _selfAngle, 0.0f);
	}

}
