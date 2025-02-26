using Godot;
using System;

public partial class Game : Node
{
	public static Player player;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
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
		if (origin is Player originPlayer && target is BaseEnemy targetEnemy)
		{
            int damage = PlayerManager.Instance.playerData.damage;

			targetEnemy.enemyData.CurrentHp -= damage;
		}

		// 怪物对玩家造成伤害 
		if (origin is BaseEnemy originEnemy && target is Player targetPlayer)
		{
			PlayerManager.Instance.playerData.CurrentHp -= originEnemy.enemyData.damage;
		}
	}
}
