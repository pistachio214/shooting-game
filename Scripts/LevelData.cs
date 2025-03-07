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
            instance.Position = GetRandomPoint();

            Game.Instance.map.AddChild(instance);

            _currentCount += 1;
        }
    }

    // 获取刷新区域(Area2D)中的随机点
    private static Vector2 GetRandomPoint()
    {
        Main gameMain = Game.Instance.map as Main;
        TileMapLayer mapLayer = gameMain.mapTileMapLayer;

        Rect2I rect = mapLayer.GetUsedRect(); // 获取实际使用的矩阵

        // CollisionShape2D collision = enemyArea.GetNode<CollisionShape2D>("CollisionShape2D");
        // Rect2 rect = collision.Shape.GetRect();

        int Vector2IX = GD.RandRange(rect.Position.X, rect.Position.X + rect.Size.X);
        int Vector2IY = GD.RandRange(rect.Position.Y, rect.Position.Y + rect.Size.Y);

        Vector2I point = new Vector2I(Vector2IX, Vector2IY);

        return mapLayer.MapToLocal(point);
    }
}
