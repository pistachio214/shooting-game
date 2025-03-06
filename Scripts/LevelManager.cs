using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class LevelManager : Node
{
	[Signal]
	public delegate void OnLevelChangeEventHandler(LevelData data); // 关卡改变信号,用于通知关卡改变,刷新UI等功能

	public static LevelManager Instance { get; private set; }

	const string LEVEL_PATH = "res://Resources/Levels/";

	public int currentLevel = 0; // 当前关卡

	private readonly List<LevelData> _levelList = [];

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

		string[] files = DirAccess.GetFilesAt(LEVEL_PATH);

		for (int i = 0; i < files.Count(); i++)
		{
			_levelList.Add(GD.Load<LevelData>(LEVEL_PATH + files[i]));
		}
	}

	// 下一关方法
	public void NewLevel()
	{
		currentLevel += 1;

		EmitSignal(SignalName.OnLevelChange, _levelList[currentLevel - 1]);
	}

	// 回合结束
	public void Stop()
	{
		// 停止刷怪计时器
		EnemyManager.Instance.timer.Stop();
	}
}
