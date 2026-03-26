using System.Collections.Generic;
using Godot;
using SpaceZombie.Joueurs;

public partial class UpgradeLoader : Node2D
{
    private float areaPlayWidth;
    private PackedScene upgradeLoader;
    private RandomNumberGenerator randomUpgradeApparition = new RandomNumberGenerator();
    private int upgradeApparition;

    public override void _Ready()
    {
        upgradeLoader = (PackedScene)ResourceLoader.Load($"res://Prefabs/upgrade.tscn");
    }

    public void InitialiseAreaPlaySize(Vector2 size)
    {
        areaPlayWidth = size.X;
    }

    public void EnemyDied(int enemieStillAlive)
    {
        if (enemieStillAlive <= upgradeApparition)
        {
            upgradeApparition = 0;
            NewUpgrade();
        }
    }

    public void UpdateUpgradeApparition(int nbEnemy)
    {
        upgradeApparition = (int)(randomUpgradeApparition.RandfRange(0.3f, 0.7f) * nbEnemy) + 1;
    }

    private void NewUpgrade()
    {
        Upgrade upgrade = (Upgrade)upgradeLoader.Instantiate();
        upgrade.Initialize(areaPlayWidth);
        CallDeferred("add_child", upgrade);
    }


}
