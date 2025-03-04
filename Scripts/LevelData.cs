using Godot;
using System;

public partial class LevelData : Resource
{
    [Export]
    public PackedScene enemyPackedScene;

    [Export]
    public int enemyCount;
}
