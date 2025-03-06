using Godot;
using System;

public partial class Main : Node2D
{
    // Godot4.4 引入玩家场景uid
    private static readonly PackedScene _prePlayerPackedScene = GD.Load<PackedScene>("uid://dp0bev3xgutux");

    private GameCanvasLayer _gameCanvasLayer;

    public Area2D enemyArea; // 怪物刷新范围

    public override void _Ready()
    {
        _gameCanvasLayer = GetNode<GameCanvasLayer>("GameCanvasLayer");
        enemyArea = GetNode<Area2D>("EnemyArea");

        Game.Instance.map = this;
        Game.Instance.Connect(Game.SignalName.OnGameStart, Callable.From(OnGameStart));
    }

    public override void _Process(double delta)
    {
    }

    private void OnGameStart()
    {
        // 游戏开始的时候，调用关卡初始化
        LevelManager.Instance.NewLevel();

        // 显示hub
        _gameCanvasLayer.Show();

        Tween tween = CreateTween();
        tween.Parallel().TweenProperty(GetParent<MainUi>().colorRect, "modulate:a", 0.0, 0.2); // Parallel表示并行运行动画
        tween.TweenCallback(Callable.From(
            GetParent<MainUi>().colorRect.Hide
        ));

        // 添加玩家
        Player player = _prePlayerPackedScene.Instantiate<Player>();
        AddChild(player);
    }
}
