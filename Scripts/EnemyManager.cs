using Godot;
using System;
using System.Collections.Generic;

public partial class EnemyManager : Node
{
	private List<dynamic> enemyList = new List<dynamic>();

	private Timer timer = new Timer();

	public override void _Ready()
	{
		// 将计时器,添加到场景树
		AddChild(timer);

		
	}

	public override void _Process(double delta)
	{
	}
}
