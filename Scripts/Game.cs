using Godot;
using System;

public partial class Game : Node
{
	public static Game Instance { get; private set; }

	private static readonly PackedScene preHitLabelPackedScene = GD.Load<PackedScene>("uid://b66ohsw4tcoux");

	[Signal]
	public delegate void OnGameStartEventHandler(); // 游戏开始信号

	public Node2D map; // 游戏场景节点

	public Player player;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// 初始化单例
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			QueueFree(); // 防止重复创建
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	/**
	 * 伤害计算
	 * origin 原始
	 * target 目标
	 */
	public static void Damage(Node2D origin, Node2D target)
	{
		// 玩家对怪物造成伤害 
		if (origin is Player && target is BaseEnemy targetEnemy)
		{
			int damage = PlayerManager.Instance.playerData.damage;

			targetEnemy.enemyData.CurrentHp -= damage;
		}

		// 怪物对玩家造成伤害 
		if (origin is BaseEnemy originEnemy && target is Player)
		{
			PlayerManager.Instance.playerData.CurrentHp -= originEnemy.enemyData.damage;
		}
	}

	// 展示飘字
	public void ShowLabel(Node2D origin, string text)
	{
		HitLabel instance = preHitLabelPackedScene.Instantiate<HitLabel>();
		instance.GlobalPosition = origin.GlobalPosition;

		map.AddChild(instance);
		instance.SetText(text);
	}

	// 相机震动,增强视觉表现
	public void CameraOffset(Vector2 offset, double time)
	{
		Tween tween = CreateTween();

		tween.TweenProperty(player.camera, "offset", Vector2.Zero, time).From(offset);
	}
}
