using Godot;
using System;

public partial class MainUi : Control
{
    private Control _gameEntryControl;

    public ColorRect colorRect;

    public override void _Ready()
    {
        _gameEntryControl = GetNode<Control>("GameEntryControl");
        colorRect = GetNode<ColorRect>("CanvasLayer/ColorRect");
    }

    // 点击开始游戏
    public void OnStartPressed()
    {
        StartGame();
    }

    // 点击退出游戏
    public void OnExitPressed()
    {
        GetTree().Quit();
    }

    /**
     * 开始游戏的时候,增加转场效果
     *
     */
    private void StartGame()
    {
        colorRect.Show();

        Tween tween = CreateTween();
        tween.Parallel().TweenProperty(colorRect, "modulate:a", 1.0, 0.2).From(0.0); // Parallel表示并行运行动画
        tween.TweenCallback(Callable.From(() =>
        {
            _gameEntryControl.Hide();

            // Plan A: 直接跳转到主界面
            // GetTree().ChangeSceneToFile("uid://bso2svt0qvblh");

            // Plan B: 发送游戏开始信号
            Game.Instance.EmitSignal(Game.SignalName.OnGameStart);
        }));
    }
}
