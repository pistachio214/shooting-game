using Godot;
using System;

public partial class LevelData : Resource
{
    [Export]
    public PackedScene EnemyPackedScene; // 关卡怪物

    [Export]
    public int EnemyCount; // 怪物数量

    [Export]
    public float Tick; // 刷怪间隔

    [Export]
    public int OnceCount; // 单次数量


    private int _currentCount = 0; // 当前已刷新多少怪物

    public void CreateEnemy()
    {
        for (int i = 0; i < OnceCount; i++)
        {
            // 刷怪数量超过怪物数量,就停止刷新更多
            if (_currentCount >= EnemyCount)
            {
                LevelManager.Instance.Stop();
                return;
            }

            BaseEnemy instance = EnemyPackedScene.Instantiate<BaseEnemy>();
            instance.GlobalPosition = GetRandomPoint();

            Game.Instance.map.AddChild(instance);

            _currentCount += 1;
        }
    }

    // 获取刷新区域(Area2D)中的随机点
    private static Vector2 GetRandomPoint()
    {
        Main gameMain = Game.Instance.map as Main;
        Area2D enemyArea = gameMain.enemyArea;

        CollisionShape2D collision = enemyArea.GetNode<CollisionShape2D>("CollisionShape2D");
        Rect2 rect = collision.Shape.GetRect();

        float vectorX = (float)GD.RandRange(-rect.Size.X, rect.Size.X);
        float vectorY = (float)GD.RandRange(-rect.Size.Y, rect.Size.Y);

        Vector2 point = new Vector2(vectorX, vectorY);

        return rect.Position + point;

    }
}
