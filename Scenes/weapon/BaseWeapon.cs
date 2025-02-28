using Godot;
using System;

public partial class BaseWeapon : Node2D
{
	private static readonly PackedScene preBulletPackedScene = GD.Load<PackedScene>("res://Scenes/bullet/BaseBullet.tscn");

	private Node2D bulletPointNode;

	public override void _Ready()
	{
		bulletPointNode = GetNode<Node2D>("BulletPoint");
	}

	public override void _Process(double delta)
	{
		// 鼠标左键发射子弹
		if (Input.IsActionJustPressed("fire") && !PlayerManager.Instance.IsDeath())
		{
			Shoot();
		}
	}

	// 射击
	private void Shoot()
	{
		// 实例化子弹节点
		BaseBullet instantiate = preBulletPackedScene.Instantiate<BaseBullet>();
		// 初始化子弹位置
		instantiate.GlobalPosition = bulletPointNode.GlobalPosition;
		// 方向跟随鼠标
		instantiate.dir = GlobalPosition.DirectionTo(GetGlobalMousePosition());

		GetTree().Root.AddChild(instantiate);
	}
}
