using Godot;
using System;
using System.Threading.Tasks;

enum State
{
	IDLE,
	MOVE,
	ATK,
	DETAH
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

	public override void _Ready()
	{
		bodyNode = GetNode<Node2D>("Body");
		animatedSprite = bodyNode.GetNode<AnimatedSprite2D>("AnimatedSprite");

		EnemyManager.Instance.enemyList.Add(this);
		enemyData = new EnemyData(); // 暂时直接创建，后续会修改为动态创建

		enemyData.Connect(EnemyData.SignalName.OnDeath, Callable.From(OnDeath));
	}

	public override void _PhysicsProcess(double delta)
	{
		if (currentState == State.DETAH || currentState == State.ATK)
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

	private void OnDeath()
	{
		currentState = State.DETAH;
		animatedSprite.Play("death");
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
