using Godot;
using System;

public partial class BaseWeapon : Node2D
{
	[Export]
	public string WeaponName = "默认枪械";

	[Export]
	public int BulletMax = 30; // 最大子弹数量

	[Export]
	public int Damage = 5; // 枪械伤害

	[Export]
	public float WeaponRof = 0.2f; // 枪械射速

	private static readonly PackedScene _preBulletPackedScene = GD.Load<PackedScene>("res://Scenes/bullet/BaseBullet.tscn");

	private int _currentBulletCount = 0; // 当前子弹数量

	private GpuParticles2D _fireParticles;

	private float _currentRofTick = 0;

	private AudioStreamPlayer2D _firingSoundAudioStreamPlayer; // 开枪音效

	public Sprite2D sprite;

	private Node2D bulletPointNode;

	public Player player; // 属于某个玩家

	public override void _Ready()
	{
		_fireParticles = GetNode<GpuParticles2D>("GPUParticles2D");
		_firingSoundAudioStreamPlayer = GetNode<AudioStreamPlayer2D>("FiringSoundAudioStreamPlayer");

		sprite = GetNode<Sprite2D>("Sprite2D");
		bulletPointNode = GetNode<Node2D>("BulletPoint");

		_fireParticles.Lifetime = WeaponRof - 0.01; // 设置粒子存活时间略小于射击间隔

		// 初始化当前子弹数
		_currentBulletCount = BulletMax;

		// 枪械加载完成后,发射一次切换枪械的信号,说明此时已经切换到当前武器
		PlayerManager.Instance.EmitSignal(PlayerManager.SignalName.OnWeaponChanged, this);
		// 切换武器的时候,同时更新子弹数量文本
		PlayerManager.Instance.EmitSignal(PlayerManager.SignalName.OnBulletCountChanged, _currentBulletCount, BulletMax);
	}

	public override void _Process(double delta)
	{
		_currentRofTick += (float)delta;

		// 鼠标左键发射子弹
		if (Input.IsActionJustPressed("fire") && !PlayerManager.Instance.IsDeath() && _currentRofTick >= WeaponRof && _currentBulletCount > 0)
		{
			Shoot();
			_currentRofTick = 0;
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
		_currentBulletCount = BulletMax;

		// 切换弹匣完成,再次更新显示数据
		PlayerManager.Instance.EmitSignal(PlayerManager.SignalName.OnBulletCountChanged, _currentBulletCount, BulletMax);
	}

	// 射击
	private void Shoot()
	{
		// 实例化子弹节点
		BaseBullet instantiate = _preBulletPackedScene.Instantiate<BaseBullet>();
		// 初始化子弹位置
		instantiate.GlobalPosition = bulletPointNode.GlobalPosition;
		// 方向跟随鼠标
		instantiate.Dir = GlobalPosition.DirectionTo(GetGlobalMousePosition());
		// 当前枪械的对象
		instantiate.currentWeapon = this;

		GetTree().Root.AddChild(instantiate);

		_currentBulletCount -= 1;
		// 发送信号到PlayerManager的OnBulletCountChanged,告诉它子弹数量减少了
		PlayerManager.Instance.EmitSignal(PlayerManager.SignalName.OnBulletCountChanged, _currentBulletCount, BulletMax);

		// 子弹清空,则自动切换弹匣
		if (_currentBulletCount <= 0)
		{
			Reload();
		}

		WeaponAnim();
	}

	private void WeaponAnim()
	{
		_fireParticles.Restart();

		Tween tween = CreateTween().SetEase(Tween.EaseType.In);
		tween.TweenProperty(sprite, "scale:x", 0.7, WeaponRof / 2);
		tween.TweenProperty(sprite, "scale:x", 1.0, WeaponRof / 2);

		_firingSoundAudioStreamPlayer.Play();

		Vector2 offset = new Vector2(-0.5f, 1);
		Game.Instance.CameraOffset(offset, WeaponRof);
	}
}
