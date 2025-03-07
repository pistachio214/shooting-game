using Godot;
using System;

enum State
{
	IDLE,
	MOVE,
	ATK,
	DETAH,
	HIT
}

public partial class BaseEnemy : CharacterBody2D
{
	[Export]
	private float Speed = 30f; // 移动速度

	private Node2D _bodyNode;

	private AnimatedSprite2D _animatedSprite; // 怪物动画

	private State _currentState = State.IDLE; // 当前怪物状态

	private Player _currentPlayer = null; // 当前目标玩家

	public EnemyData enemyData;

	private CollisionShape2D _enemyCollisionShape;

	private Sprite2D _enemyShadowSprite;

	private AudioStreamPlayer2D _hitAudioStreamPlayer;

	private NavigationAgent2D _navigationAgent; // 动态避障

	private float _movementDelta;

	private double _tick = 0;

	public override void _Ready()
	{
		_tick = GD.RandRange(120, 240);

		_bodyNode = GetNode<Node2D>("Body");
		_animatedSprite = _bodyNode.GetNode<AnimatedSprite2D>("AnimatedSprite");
		_enemyCollisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
		_enemyShadowSprite = GetNode<Sprite2D>("Shadow");
		_hitAudioStreamPlayer = GetNode<AudioStreamPlayer2D>("HitAudioStreamPlayer");
		_navigationAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");

		// 设置导航最大的速度等于怪物的移动速度
		// _navigationAgent.MaxSpeed = Speed;

		EnemyManager.Instance.enemyList.Add(this);
		enemyData = new EnemyData(); // 暂时直接创建，后续会修改为动态创建

		enemyData.Connect(EnemyData.SignalName.OnHit, Callable.From<int>(OnHit));
		enemyData.Connect(EnemyData.SignalName.OnDeath, Callable.From(OnDeath));
	}

	public override void _Process(double delta)
	{
		// 切换到空闲帧来执行怪物的移动 每2个物理帧进行物理更新
		if (Engine.GetProcessFrames() % _tick == 0)
		{
			SetMovementTarget(Game.Instance.player.GlobalPosition);
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_currentState == State.DETAH || _currentState == State.ATK || _currentState == State.HIT)
		{
			return;
		}

		if (NavigationServer2D.MapGetIterationId(_navigationAgent.GetNavigationMap()) == 0)
		{
			return;
		}

		if (_navigationAgent.IsNavigationFinished())
		{
			return;
		}

		_movementDelta = Speed;
		Vector2 nextPathPosition = _navigationAgent.GetNextPathPosition();
		Vector2 newVelocity = GlobalPosition.DirectionTo(nextPathPosition) * _movementDelta;
		if (_navigationAgent.AvoidanceEnabled)
		{
			_navigationAgent.SetVelocity(newVelocity);
		}

		MoveAndSlide();

		ChangeAnimated();
	}

	private void SetMovementTarget(Vector2 movementTarget)
	{
		_navigationAgent.SetTargetPosition(movementTarget);
	}

	private void OnHit(int damage)
	{
		_currentState = State.HIT;
		// 中枪音效
		_hitAudioStreamPlayer.Play();

		Game.Instance.ShowLabel(this, $"-{damage}");

		_animatedSprite.Play("hit");
	}

	private void OnDeath()
	{
		if (_currentState == State.DETAH)
		{
			return;
		}
		_currentState = State.DETAH;
		_enemyCollisionShape.CallDeferred("set_disabled", true);
		_animatedSprite.Play("death");
		_enemyShadowSprite.Hide();
	}

	// 离开场景树时自动触发
	public override void _ExitTree()
	{
		EnemyManager.Instance.enemyList.Remove(this); // 怪物死亡，移除List中指定元素
		EnemyManager.Instance.EmitSignal(EnemyManager.SignalName.OnEnemyDeath);
		EnemyManager.Instance.CheckEnemyList(); // 怪物死亡的时调用检测
	}

	private void ChangeAnimated()
	{
		if (Velocity == Vector2.Zero)
		{
			_animatedSprite.Play("idle");
			_currentState = State.IDLE;
		}
		else
		{
			_animatedSprite.Play("move");
			_currentState = State.MOVE;
			_bodyNode.Scale = new Vector2(x: !IsFacingTarget() && Velocity.X < 0 ? -1 : 1, y: 1);
		}
	}

	public void OnAtkAreaBodyEntered(Node2D body)
	{
		if (body is Player player && _currentState != State.DETAH)
		{
			_currentPlayer = player;
			_currentState = State.ATK;
			_animatedSprite.Play("atk");
		}
	}

	public void OnAtkAreaBodyExited(Node2D body)
	{
		if (body is Player player && player == _currentPlayer)
		{
			_currentPlayer = null;
		}
	}

	public void OnAnimatedSpriteFrameChanged()
	{
		if (_currentState == State.ATK && _animatedSprite.Animation == "atk")
		{
			// 确定动画第二帧为攻击帧
			if (_currentPlayer != null && _animatedSprite.Frame == 2)
			{
				// 造成伤害动作
				Game.Damage(this, _currentPlayer);
			}
		}
	}

	// 动画结束后，判断是否继续攻击或者恢复到move状态
	public void OnAnimatedSpriteAnimationFinished()
	{
		if (_currentState == State.HIT && _animatedSprite.Animation == "hit")
		{
			_currentState = State.IDLE;
		}

		if (_currentState == State.DETAH && _animatedSprite.Animation == "death")
		{
			QueueFree();
		}

		if (_currentState == State.ATK && _animatedSprite.Animation == "atk")
		{
			if (_currentPlayer != null && !PlayerManager.Instance.IsDeath())
			{
				_animatedSprite.Play("atk");
			}
			else
			{
				_currentState = State.IDLE;
			}
		}
	}

	// 安全避障信号
	public void OnNavigationAgentVelocityComputed(Vector2 safeVelocity)
	{
		if (_currentState == State.DETAH || _currentState == State.ATK || _currentState == State.HIT)
		{
			return;
		}

		if (PlayerManager.Instance.IsDeath())
		{
			Velocity = Vector2.Zero;
		}
		else
		{
			Velocity = safeVelocity * Speed * 50;
			MoveAndSlide();
		}
	}

	// 编写方法，实现怪物是否朝向玩家
	private bool IsFacingTarget()
	{
		Vector2 dirToTarget = (Game.Instance.player.GlobalPosition - GlobalPosition).Normalized();

		Vector2 facingDir = Transform.X.Normalized();

		float dot = facingDir.Dot(dirToTarget);

		return dot >= (1 - 0.7);
	}

}
