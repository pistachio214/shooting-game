using Godot;
using System;

public partial class EnemyData : Resource
{
	/**
	 * 怪物属性
	 */
	[Export]
	public int MaxHp = 30; // 最大生命值

	[Export]
	public int Damage = 5; // 伤害值

	[Signal]
	public delegate void OnDeathEventHandler(); // 怪物死亡通知

	[Signal]
	public delegate void OnHitEventHandler(int damage); // 怪物受到伤害通知

	// 怪物当前血量
	private int _currentHp;
	public int CurrentHp
	{
		get => _currentHp;
		set
		{
			int num = _currentHp - value;
			if (_currentHp != 0 || num != 0)
			{
				EmitSignal(SignalName.OnHit, num);
			}

			_currentHp = value;
			if (_currentHp <= 0)
			{
				EmitSignal(SignalName.OnDeath);
			}
		}
	}

	public EnemyData()
	{
		_currentHp = MaxHp;
	}

}
