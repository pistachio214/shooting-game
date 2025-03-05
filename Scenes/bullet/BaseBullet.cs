using Godot;
using System;

public partial class BaseBullet : Node2D
{
	[Export]
	public float speed = 500f; // 子弹速度

	[Export]
	public Vector2 dir = Vector2.Zero; // 子弹飞行向量

	public BaseWeapon currentWeapon; // 当期子弹来自的枪械

	public override void _Ready()
	{
		LookAt(GetGlobalMousePosition());
	}

	public override void _PhysicsProcess(double delta)
	{
		//TODO 子弹超出可视范围就销毁

		GlobalPosition += dir * (float)delta * speed;
	}

	public void OnAreaBodyEntered(Node2D body)
	{
		if (body is BaseEnemy enemy)
		{
			Game.Damage(Game.Instance.player, enemy);
			QueueFree(); // 子弹碰撞到敌人就消失
		}
	}
}
