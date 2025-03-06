using Godot;
using System;
using System.Collections.Generic;

public partial class EnemyManager : Node
{
	[Signal]
	public delegate void OnEnemyDeathEventHandler(LevelData data); // 怪物死亡信号

	public static EnemyManager Instance { get; private set; }

	public List<BaseEnemy> enemyList = [];

	private LevelData _currentLevelData;

	public Timer timer = new Timer();

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

		// 链接刷怪计时器信号
		timer.OneShot = false; // 关闭单次触发

		// timer.Timeout += timeOut;
		timer.Connect(Timer.SignalName.Timeout, Callable.From(OnTimerTimeout));

		// 将计时器,添加到场景树
		AddChild(timer);

		LevelManager.Instance.Connect(LevelManager.SignalName.OnLevelChange, Callable.From<LevelData>(OnLevelChange));
	}

	public override void _Process(double delta)
	{
	}

	// 刷怪代码
	private void OnLevelChange(LevelData data)
	{
		_currentLevelData = data;
		timer.Start(data.Tick);
	}

	private void OnTimerTimeout()
	{
		if (_currentLevelData != null)
		{
			_currentLevelData.CreateEnemy();
		}
	}

	public void CheckEnemyList()
	{
		if (enemyList.Count == 0)
		{
			LevelManager.Instance.NewLevel();
		}
	}

}
