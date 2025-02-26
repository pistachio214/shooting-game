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
	private int _currentHp;
	public int CurrentHp
	{
		get => _currentHp;
		set
		{
			_currentHp = value;
		}
	}

	public EnemyData()
	{
		_currentHp = maxHp;
	}

}
