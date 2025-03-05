using Godot;
using System;

/**
 * 玩家管理单例
 */
public partial class PlayerManager : Node
{
	public static PlayerManager Instance { get; private set; }

	/**
	 * 2号武器
	 */
	private static readonly PackedScene preTwoWeaponPackedScene = GD.Load<PackedScene>("res://Scenes/weapon/Gun2.tscn");

	/**
	 * 玩家信号
	 */
	[Signal]
	public delegate void OnPlayerHpChangedEventHandler(int current, int maxHp); // 玩家血量变化信号
	[Signal]
	public delegate void OnPlayerDeathEventHandler(); // 玩家死亡信号

	/**
	 * 枪械信号
	 */
	[Signal]
	public delegate void OnWeaponChangedEventHandler(BaseWeapon weapon); // 切换枪械信号
	[Signal]
	public delegate void OnBulletCountChangedEventHandler(int current, int max); // 子弹数量改变信号
	[Signal]
	public delegate void OnWeaponReloadEventHandler(); // 切换弹匣信号

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

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_accept"))
		{
			ChangeWeapon(preTwoWeaponPackedScene.Instantiate<BaseWeapon>());
		}
	}

	/**
	 * 更换枪械
	 */
	private void ChangeWeapon(BaseWeapon weapon)
	{
		BaseWeapon currentWeapon = Game.Instance.player.weaponNode.GetChild<BaseWeapon>(0);
		if (currentWeapon != null)
		{
			currentWeapon.QueueFree();
		}

		weapon.player = Game.Instance.player;
		Game.Instance.player.weaponNode.AddChild(weapon);
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
