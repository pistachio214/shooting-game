using Godot;
using System;

public partial class EnemyData : Resource
{
	/**
	 * 怪物属性
	 */
	[Export]
	public int maxHp = 10; // 玩家最大生命值

	[Export]
	public int damage = 5; // 玩家伤害值

	// 怪物当前血量
	private int currentHp;
	public int CurrentHp
	{
		get => currentHp;
		set
		{
			int num = currentHp - value;
			if (currentHp != 0 || num != 0)
			{
				EmitSignal(SignalName.OnHit, num);
			}

			currentHp = value;
			if (currentHp <= 0)
			{
				EmitSignal(SignalName.OnDeath);
			}
		}
	}

	[Signal]
	public delegate void OnDeathEventHandler(); // 怪物死亡通知

	[Signal]
	public delegate void OnHitEventHandler(int damage); // 怪物受到伤害通知

	public EnemyData()
	{
		currentHp = maxHp;
	}

}
