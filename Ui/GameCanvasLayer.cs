using Godot;
using System;

public partial class GameCanvasLayer : CanvasLayer
{
	public Control HUDControl;

	public override void _Ready()
	{
		HUDControl = GetNode<Control>("HUD");
	}

	public override void _Process(double delta)
	{
	}
}
