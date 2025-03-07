using Godot;
using System;

public partial class Player : CharacterBody2D
{
	private float _speed = 70f;

	// 记录当前朝向
	private string _currentAnimated = "down_";

	private Node2D _playerBodyNode;

	private AnimatedSprite2D _playerAnimatedSprite;

	public Node2D weaponNode;

	public Camera2D camera;

	public override void _Ready()
	{
		// 将玩家对象加入到游戏单例中
		Game.Instance.player = this;

		_playerBodyNode = GetNode<Node2D>("Body");
		_playerAnimatedSprite = _playerBodyNode.GetNode<AnimatedSprite2D>("AnimatedSprite");
		weaponNode = _playerBodyNode.GetNode<Node2D>("WeaponNode");
		camera = GetNode<Camera2D>("Camera2D");

		// 链接信号
		ConnectSignals();
	}

	public override void _PhysicsProcess(double delta)
	{
		if (PlayerManager.Instance.IsDeath())
		{
			return;
		}

		Vector2 dir = Vector2.Zero;

		dir.X = Input.GetAxis("move_left", "move_right");
		dir.Y = Input.GetAxis("move_up", "move_down");

		// 归一化解决斜着跑比直线更快的问题
		Velocity = dir.Normalized() * _speed;

		ChangeAnimated();
		MoveAndSlide();
	}

	public void ConnectSignals()
	{
		// 连接到PlayerManager的信号
		PlayerManager.Instance.Connect(PlayerManager.SignalName.OnPlayerDeath, Callable.From(OnPlayerDeath));

		// PlayerManager.Instance.Connect(PlayerManager.SignalName.OnPlayerHpChanged, Callable.From<int, int>(OnPlayerHpChanged));
	}

	// 玩家死亡信号链接操作
	private void OnPlayerDeath()
	{
		weaponNode.Hide(); // 隐藏武器
		_playerAnimatedSprite.Play("death"); // 播放玩家死亡动画 
	}

	// 玩家血量变化信号链接操作
	// private void OnPlayerHpChanged(int currentHp, int maxHp)
	// {
	// 	GD.Print($"玩家血量变化：{currentHp}/{maxHp}");
	// }

	// 切换动画
	private void ChangeAnimated()
	{
		if (Velocity == Vector2.Zero)
		{
			_playerAnimatedSprite.Play(_currentAnimated + "idle");
		}
		else
		{
			_currentAnimated = GetMovementDir();
			_playerAnimatedSprite.Play(_currentAnimated + "move");

			// 反转镜像
			// _playerBodyNode.Scale = new Vector2(x: Velocity.X < 0 ? -1 : 1, y: 1);
		}

		Vector2 position = GetGlobalMousePosition();
		weaponNode.LookAt(position);

		if (position.X > Position.X && _playerBodyNode.Scale.X != 1)
		{
			_playerBodyNode.Scale = new Vector2(x: 1, y: 1);
		}

		if (position.X < Position.X && _playerBodyNode.Scale.X != -1)
		{
			_playerBodyNode.Scale = new Vector2(x: -1, y: 1);
		}
	}

	//  获取移动时的方向
	private string GetMovementDir()
	{
		weaponNode.ZIndex = 1;

		if (Velocity == Vector2.Zero)
		{
			return "lr_";
		}

		//  获取速度向量的角度
		float angle = Velocity.Angle();
		float degree = Mathf.RadToDeg(angle);

		if (45 <= degree && degree < 135)
		{
			return "down_";
		}
		else if (-135 <= degree && degree < -45)
		{
			weaponNode.ZIndex = 0;
			return "up_";
		}
		else
		{
			return "lr_";
		}
	}
}
