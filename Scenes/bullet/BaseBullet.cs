using Godot;
using System;

public partial class BaseBullet : Node2D
{
	[Export]
	public float Speed = 500f; // 子弹速度

	[Export]
	public Vector2 Dir = Vector2.Zero; // 子弹飞行向量

	// 击中的粒子器
	private static readonly PackedScene _preHitEffectPackedScene = GD.Load<PackedScene>("res://Scenes/HitEffect.tscn");

	public BaseWeapon currentWeapon; // 当期子弹来自的枪械


	private double _tick = 0;

	public override void _Ready()
	{
		LookAt(GetGlobalMousePosition());
	}

	public override void _PhysicsProcess(double delta)
	{
		GlobalPosition += Dir * (float)delta * Speed;

		_tick += delta;
		//子弹3秒之后就销毁
		if (Engine.GetPhysicsFrames() % 20 == 0)
		{
			if (_tick >= 3)
			{
				QueueFree();
			}
		}
	}

	public void OnAreaBodyEntered(Node2D body)
	{
		if (body is BaseEnemy enemy)
		{
			Game.Damage(Game.Instance.player, enemy);

			SetPhysicsProcess(false);

			HitEffect instance = _preHitEffectPackedScene.Instantiate<HitEffect>();
			instance.GlobalPosition = GlobalPosition;

			Game.Instance.map.AddChild(instance);
			QueueFree(); // 子弹碰撞到敌人就消失
		}
	}
}
