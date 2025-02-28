using Godot;
using System;

public partial class PlayerData : Resource
{
	/**
	 * 玩家属性
	 */
	[Export]
	public int maxHp = 10; // 玩家最大生命值

	[Export]
	public int damage = 5; // 玩家伤害值

	/**
	 * 玩家存档
	 */
	[Export]
	public int gold = 0; // 玩家持有金币数

	// 玩家当前血量
	private int currentHp;
	public int CurrentHp
	{
		get => currentHp;
		set
		{
			currentHp = value;
			if (currentHp > 0)
			{
				// 触发血量变化信号
				PlayerManager.Instance.EmitSignal(PlayerManager.SignalName.OnPlayerHpChanged, currentHp, maxHp);
			}

			// 如果血量小于等于0，触发死亡信号
			if (currentHp <= 0)
			{
				PlayerManager.Instance.EmitSignal(PlayerManager.SignalName.OnPlayerDeath);
			}
		}
	}

	public PlayerData()
	{
		currentHp = maxHp;
	}
}
