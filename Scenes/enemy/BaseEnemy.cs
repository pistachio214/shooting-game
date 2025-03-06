using Godot;
using System;
using System.Threading.Tasks;

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
	private float speed = 30f; // 移动速度

	private Node2D bodyNode;

	private AnimatedSprite2D animatedSprite; // 怪物动画

	private State currentState = State.IDLE; // 当前怪物状态

	private Player currentPlayer = null; // 当前目标玩家

	public EnemyData enemyData;

	private CollisionShape2D enemyCollisionShape;

	private Sprite2D enemyShadowSprite;

	private AudioStreamPlayer2D hitAudioStreamPlayer;
	public override void _Ready()
	{
		bodyNode = GetNode<Node2D>("Body");
		animatedSprite = bodyNode.GetNode<AnimatedSprite2D>("AnimatedSprite");
		enemyCollisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
		enemyShadowSprite = GetNode<Sprite2D>("Shadow");
		hitAudioStreamPlayer = GetNode<AudioStreamPlayer2D>("HitAudioStreamPlayer");

		EnemyManager.Instance.enemyList.Add(this);
		enemyData = new EnemyData(); // 暂时直接创建，后续会修改为动态创建

		enemyData.Connect(EnemyData.SignalName.OnHit, Callable.From<int>(OnHit));
		enemyData.Connect(EnemyData.SignalName.OnDeath, Callable.From(OnDeath));
	}

	public override void _PhysicsProcess(double delta)
	{
		if (currentState == State.DETAH || currentState == State.ATK || currentState == State.HIT)
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
			Velocity = GlobalPosition.DirectionTo(Game.Instance.player.GlobalPosition) * speed;
		}

		MoveAndSlide();

		ChangeAnimated();
	}

	private void OnHit(int damage)
	{
		currentState = State.HIT;
		// 中枪音效
		hitAudioStreamPlayer.Play();

		Game.Instance.ShowLabel(this, $"-{damage}");

		animatedSprite.Play("hit");
	}

	private void OnDeath()
	{
		if (currentState == State.DETAH)
		{
			return;
		}
		currentState = State.DETAH;
		enemyCollisionShape.CallDeferred("set_disabled", true);
		animatedSprite.Play("death");
		enemyShadowSprite.Hide();
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
			animatedSprite.Play("idle");
			currentState = State.IDLE;
		}
		else
		{
			animatedSprite.Play("move");
			currentState = State.MOVE;
			bodyNode.Scale = new Vector2(x: Velocity.X < 0 ? -1 : 1, y: 1);
		}
	}

	public void OnAtkAreaBodyEntered(Node2D body)
	{
		if (body is Player player && currentState != State.DETAH)
		{
			currentPlayer = player;
			currentState = State.ATK;
			animatedSprite.Play("atk");
		}
	}

	public void OnAtkAreaBodyExited(Node2D body)
	{
		if (body is Player player && player == currentPlayer)
		{
			currentPlayer = null;
		}
	}

	public void OnAnimatedSpriteFrameChanged()
	{
		if (currentState == State.ATK && animatedSprite.Animation == "atk")
		{
			// 确定动画第二帧为攻击帧
			if (currentPlayer != null && animatedSprite.Frame == 2)
			{
				// 造成伤害动作
				Game.Damage(this, currentPlayer);
			}
		}
	}

	// 动画结束后，判断是否继续攻击或者恢复到move状态
	public void OnAnimatedSpriteAnimationFinished()
	{
		if (currentState == State.HIT && animatedSprite.Animation == "hit")
		{
			currentState = State.IDLE;
		}

		if (currentState == State.DETAH && animatedSprite.Animation == "death")
		{
			QueueFree();
		}

		if (currentState == State.ATK && animatedSprite.Animation == "atk")
		{
			if (currentPlayer != null && !PlayerManager.Instance.IsDeath())
			{
				animatedSprite.Play("atk");
			}
			else
			{
				currentState = State.IDLE;
			}
		}
	}

}
