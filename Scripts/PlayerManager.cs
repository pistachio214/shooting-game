using Godot;
using System;

/**
 * 玩家管理单例
 */
public partial class PlayerManager : Node
{
	public static PlayerManager Instance { get; private set; }

	/**
	 * 玩家血量变化信号
	 */
	[Signal]
	public delegate void OnPlayerHpChangedEventHandler(int value, int maxHp);

	/**
	 * 玩家死亡信号
	 */
	[Signal]
	public delegate void OnPlayerDeathEventHandler();

	public PlayerData playerData;

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


		// 先直接创建一个，后续做存档时候会用到
		playerData = new PlayerData();
	}

	public override void _Process(double delta)
	{
	}

	/**
	 * 判断玩家是否死亡
	 */
	public bool IsDeath()
	{
		if (playerData != null)
		{
			return playerData.CurrentHp <= 0;
		}

		return false;
	}
}
