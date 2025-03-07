using Godot;
using System;

public partial class Main : Node2D
{
    // Godot4.4 引入玩家场景uid
    private static readonly PackedScene _prePlayerPackedScene = GD.Load<PackedScene>("uid://dp0bev3xgutux");

    private GameCanvasLayer _gameCanvasLayer;

    public TileMapLayer mapTileMapLayer;

    public override void _Ready()
    {
        _gameCanvasLayer = GetNode<GameCanvasLayer>("GameCanvasLayer");
        mapTileMapLayer = GetNode<TileMapLayer>("Land");

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

        Rect2I rect = mapTileMapLayer.GetUsedRect(); // 获取实际使用的矩阵
        player.Position = mapTileMapLayer.MapToLocal(rect.GetCenter()); // 让玩家在矩阵中心出现
        player.CollisionMask = 1;

        AddChild(player);
    }
}
