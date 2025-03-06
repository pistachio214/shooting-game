using Godot;
using System;

public partial class BaseWeapon : Node2D
{

	private static readonly PackedScene preBulletPackedScene = GD.Load<PackedScene>("res://Scenes/bullet/BaseBullet.tscn");

	[Export]
	public string weaponName = "默认枪械";

	[Export]
	public int bulletMax = 30; // 最大子弹数量

	[Export]
	public int damage = 5; // 枪械伤害

	[Export]
	public float weaponRof = 0.2f; // 枪械射速

	private int currentBulletCount = 0; // 当前子弹数量

	private GpuParticles2D fireParticles;

	private float currentRofTick = 0;

	public Sprite2D sprite;

	private Node2D bulletPointNode;

	public Player player; // 属于某个玩家

	private AudioStreamPlayer2D firingSoundAudioStreamPlayer; // 开枪音效

	public override void _Ready()
	{
		sprite = GetNode<Sprite2D>("Sprite2D");
		bulletPointNode = GetNode<Node2D>("BulletPoint");
		fireParticles = GetNode<GpuParticles2D>("GPUParticles2D");
		firingSoundAudioStreamPlayer = GetNode<AudioStreamPlayer2D>("FiringSoundAudioStreamPlayer");

		fireParticles.Lifetime = weaponRof - 0.01; // 设置粒子存活时间略小于射击间隔

		// 初始化当前子弹数
		currentBulletCount = bulletMax;

		// 枪械加载完成后,发射一次切换枪械的信号,说明此时已经切换到当前武器
		PlayerManager.Instance.EmitSignal(PlayerManager.SignalName.OnWeaponChanged, this);

		// 切换武器的时候,同时更新子弹数量文本
		PlayerManager.Instance.EmitSignal(PlayerManager.SignalName.OnBulletCountChanged, currentBulletCount, bulletMax);
	}

	public override void _Process(double delta)
	{
		currentRofTick += (float)delta;

		// 鼠标左键发射子弹
		if (Input.IsActionJustPressed("fire") && !PlayerManager.Instance.IsDeath() && currentRofTick >= weaponRof && currentBulletCount > 0)
		{
			Shoot();
			currentRofTick = 0;
		}
	}

	// 更换弹匣(重置枪械子弹)
	private void Reload()
	{
		// 发送信号到PlayerManager的OnWeaponReload信号,告诉它切换弹匣了
		PlayerManager.Instance.EmitSignal(PlayerManager.SignalName.OnWeaponReload);

		// 模拟切换弹匣需要2秒 Plan 1:
		// GetTree().CreateTimer(2f).Timeout += OnWeaponReload;

		// 模拟切换弹匣需要2秒 Plan 2:
		SceneTreeTimer treeTimer = GetTree().CreateTimer(2f);
		treeTimer.Connect(SceneTreeTimer.SignalName.Timeout, Callable.From(OnWeaponReload));
	}

	private void OnWeaponReload()
	{
		currentBulletCount = bulletMax;

		// 切换弹匣完成,再次更新显示数据
		PlayerManager.Instance.EmitSignal(PlayerManager.SignalName.OnBulletCountChanged, currentBulletCount, bulletMax);
	}

	// 射击
	private void Shoot()
	{
		// 实例化子弹节点
		BaseBullet instantiate = preBulletPackedScene.Instantiate<BaseBullet>();
		// 初始化子弹位置
		instantiate.GlobalPosition = bulletPointNode.GlobalPosition;
		// 方向跟随鼠标
		instantiate.dir = GlobalPosition.DirectionTo(GetGlobalMousePosition());
		// 当前枪械的对象
		instantiate.currentWeapon = this;

		GetTree().Root.AddChild(instantiate);

		currentBulletCount -= 1;
		// 发送信号到PlayerManager的OnBulletCountChanged,告诉它子弹数量减少了
		PlayerManager.Instance.EmitSignal(PlayerManager.SignalName.OnBulletCountChanged, currentBulletCount, bulletMax);

		// 子弹清空,则自动切换弹匣
		if (currentBulletCount <= 0)
		{
			Reload();
		}

		WeaponAnim();
	}

	private void WeaponAnim()
	{
		fireParticles.Restart();

		Tween tween = CreateTween().SetEase(Tween.EaseType.In);
		tween.TweenProperty(sprite, "scale:x", 0.7, weaponRof / 2);
		tween.TweenProperty(sprite, "scale:x", 1.0, weaponRof / 2);

		firingSoundAudioStreamPlayer.Play();

		Vector2 offset = new Vector2(-0.5f, 1);
		Game.Instance.CameraOffset(offset, weaponRof);
	}
}
