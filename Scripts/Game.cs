using Godot;
using System;

public partial class Game : Node
{
	public static Game Instance { get; private set; }

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

			targetEnemy.enemyData.currentHp -= damage;
		}

		// 怪物对玩家造成伤害 
		if (origin is BaseEnemy originEnemy && target is Player)
		{
			PlayerManager.Instance.playerData.currentHp -= originEnemy.enemyData.damage;
		}
	}
}
