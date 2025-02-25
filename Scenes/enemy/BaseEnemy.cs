using Godot;
using System;

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

	private Node2D _body;
	// 怪物动画
	private AnimatedSprite2D _animatedSprite;

	private State _currentState = State.IDLE; // 当前怪物状态

	private Player _currentPlayer = null; // 当前目标玩家

	public override void _Ready()
	{
		_body = GetNode<Node2D>("Body");
		_animatedSprite = GetNode<AnimatedSprite2D>("Body/AnimatedSprite");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_currentState == State.DETAH || _currentState == State.ATK)
		{
			return;
		}
		// 怪物的世界坐标
		Velocity = GlobalPosition.DirectionTo(Game.player.GlobalPosition) * speed;

		MoveAndSlide();

		ChangeAnimated();
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
			_body.Scale = new Vector2(x: Velocity.X < 0 ? -1 : 1, y: 1);
		}
	}

	public void OnAtkAreaBodyEntered(Node2D body)
	{
		if (body is Player player)
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
			}
		}
	}

	// 动画结束后，判断是否继续攻击或者恢复到move状态
	public void OnAnimatedSpriteAnimationFinished()
	{
		if (_currentState == State.ATK && _animatedSprite.Animation == "atk")
		{
			if (_currentPlayer != null)
			{
				_animatedSprite.Play("atk");
			}
			else
			{
				_currentState = State.MOVE;
			}
		}
	}

}
