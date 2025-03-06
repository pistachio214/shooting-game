using Godot;
using System;

public partial class HitLabel : Node2D
{
	private Label _damageLabel;

	public override void _Ready()
	{
		_damageLabel = GetNode<Label>("Label");

		Tween tween = CreateTween().SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Linear);
		tween.TweenProperty(_damageLabel, "scale", new Vector2(1.3f, 1.3f), 0.2);
		tween.Parallel().TweenProperty(_damageLabel, "scale", new Vector2(1.0f, 1.0f), 0.2).SetDelay(0.1);
		tween.Parallel().TweenProperty(_damageLabel, "position:y", _damageLabel.Position.Y - 15, 0.3);
		tween.TweenProperty(_damageLabel, "position:y", _damageLabel.Position.Y - 12, 0.3);
		tween.Parallel().TweenProperty(_damageLabel, "modulate:a", 0.0, 0.3);

		tween.TweenCallback(Callable.From(QueueFree));
	}

	public override void _Process(double delta)
	{
	}

	public void SetText(string text)
	{ 
		_damageLabel.Text = text;
	}
}
