using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class LevelManager : Node
{
	public static LevelManager Instance { get; private set; }

	const string LEVEL_PATH = "res://Resources/Levels/";

	public int currentLevel = 0; // 当前关卡

	private List<LevelData> levelList = new List<LevelData>();

	[Signal]
	public delegate void OnLevelChangeEventHandler(); // 关卡改变信号,用于通知关卡改变,刷新UI等功能

	public override void _Ready()
	{
		// 初始化单例
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			QueueFree(); // 防止重复创建
		}

		GD.Print("到底是不是到这里了");

		string[] files = DirAccess.GetFilesAt(LEVEL_PATH);

		for (int i = 0; i < files.Count(); i++)
		{
			levelList.Add(GD.Load<LevelData>(LEVEL_PATH + files[i]));
		}

		GD.Print("[] -->> ", levelList);
	}

	// 下一关方法
	public void NewLevel()
	{
		currentLevel += 1;

		EmitSignal(SignalName.OnLevelChange);
	}
}
