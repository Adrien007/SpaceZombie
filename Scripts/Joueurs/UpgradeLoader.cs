using System.Collections.Generic;
using Godot;
using SpaceZombie.Joueurs;

public partial class UpgradeLoader : Node2D
{
    private float areaPlayWidth;
    private PackedScene upgradeLoader;

    public override void _Ready()
    {
        upgradeLoader = (PackedScene)ResourceLoader.Load($"res://Prefabs/upgrade.tscn");
    }

    public void InitialiseAreaPlaySize(Vector2 size)
    {
        areaPlayWidth = size.X;
    }

    public void NewUpgrade()
    {
        Upgrade upgrade = (Upgrade)upgradeLoader.Instantiate();
        upgrade.Initialize(areaPlayWidth);
        CallDeferred("add_child", upgrade);
    }
}
