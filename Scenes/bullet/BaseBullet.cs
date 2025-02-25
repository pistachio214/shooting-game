using Godot;
using System;

public partial class BaseBullet : Node2D
{
	[Export]
	public float speed = 500f; // 子弹速度

	[Export]
	public Vector2 dir = Vector2.Zero; // 子弹飞行向量

	public override void _Ready()
	{
		LookAt(GetGlobalMousePosition());
	}

	public override void _PhysicsProcess(double delta)
	{
		GlobalPosition += dir * (float)delta * speed;
	}
}
