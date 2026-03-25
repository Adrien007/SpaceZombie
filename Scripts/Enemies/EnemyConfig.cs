//EnemyConfig.cs
using Godot;
using SpaceZombie.Niveaux.Configs;

namespace SpaceZombie.Enemies.Configs
{
    public static class EnemyConfig
    {
        public static readonly NiveauEnemySettings enemyType1 = new NiveauEnemySettings()
        {
            EnemyObj = GD.Load<PackedScene>("res://Prefabs/enemy1.tscn"),
            Enemy = new Enemy(1, 100)
        };
        public static readonly NiveauEnemySettings enemyType2 = new NiveauEnemySettings()
        {
            EnemyObj = GD.Load<PackedScene>("res://Prefabs/enemy2.tscn"),
            Enemy = new Enemy(2, 200)
        };
        public static readonly NiveauEnemySettings enemyType3 = new NiveauEnemySettings()
        {
            EnemyObj = GD.Load<PackedScene>("res://Prefabs/enemy3.tscn"),
            Enemy = new Enemy(3, 300)
        };
        public static readonly NiveauEnemySettings enemyType4 = new NiveauEnemySettings()
        {
            EnemyObj = GD.Load<PackedScene>("res://Prefabs/enemy4.tscn"),
            Enemy = new Enemy(5, 500)
        };
        public static readonly NiveauEnemySettings enemyType5 = new NiveauEnemySettings()
        {
            EnemyObj = GD.Load<PackedScene>("res://Prefabs/enemy.tscn"),
            Enemy = new Enemy(7, 5)
        };
        public static readonly NiveauEnemySettings enemyType6 = new NiveauEnemySettings()
        {
            EnemyObj = GD.Load<PackedScene>("res://Prefabs/enemy.tscn"),
            Enemy = new Enemy(10, 6)
        };

        public static NiveauEnemySettings Mapp(int i)
        {
            switch (i)
            {
                case 1:
                    return EnemyConfig.enemyType1;
                case 2:
                    return EnemyConfig.enemyType2;
                case 3:
                    return EnemyConfig.enemyType3;
                case 4:
                    return EnemyConfig.enemyType4;
                case 5:
                    return EnemyConfig.enemyType5;
                case 6:
                    return EnemyConfig.enemyType6;
                default:
                    throw new System.ArgumentOutOfRangeException(nameof(i), "Invalid enemy type index");
            }
        }
    }
}
