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

	public override void _Ready()
	{
		_bodyNode = GetNode<Node2D>("Body");
		_animatedSprite = _bodyNode.GetNode<AnimatedSprite2D>("AnimatedSprite");
		_enemyCollisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
		_enemyShadowSprite = GetNode<Sprite2D>("Shadow");
		_hitAudioStreamPlayer = GetNode<AudioStreamPlayer2D>("HitAudioStreamPlayer");

		EnemyManager.Instance.enemyList.Add(this);
		enemyData = new EnemyData(); // 暂时直接创建，后续会修改为动态创建

		enemyData.Connect(EnemyData.SignalName.OnHit, Callable.From<int>(OnHit));
		enemyData.Connect(EnemyData.SignalName.OnDeath, Callable.From(OnDeath));
	}

	public override void _PhysicsProcess(double delta)
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
			// 怪物的世界坐标
			Velocity = GlobalPosition.DirectionTo(Game.Instance.player.GlobalPosition) * Speed;
		}

		MoveAndSlide();

		ChangeAnimated();
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
			_bodyNode.Scale = new Vector2(x: Velocity.X < 0 ? -1 : 1, y: 1);
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

}
