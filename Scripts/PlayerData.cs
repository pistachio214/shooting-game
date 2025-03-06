using Godot;
using System;

public partial class PlayerData : Resource
{
	/**
	 * 玩家属性
	 */
	[Export]
	public int MaxHp = 10; // 玩家最大生命值

	[Export]
	public int Damage = 5; // 玩家伤害值

	/**
	 * 玩家存档
	 */
	[Export]
	public int Gold = 0; // 玩家持有金币数

	// 玩家当前血量
	private int _currentHp;

	public int CurrentHp
	{
		get => _currentHp;
		set
		{
			_currentHp = value;

			// 触发血量变化信号
			PlayerManager.Instance.EmitSignal(PlayerManager.SignalName.OnPlayerHpChanged, _currentHp, MaxHp);

			// 如果血量小于等于0，触发死亡信号
			if (_currentHp <= 0)
			{
				PlayerManager.Instance.EmitSignal(PlayerManager.SignalName.OnPlayerDeath);
			}
		}
	}

	/**
	 * 初始化当前血量
	 */
	public PlayerData()
	{
		_currentHp = MaxHp;
	}
}
