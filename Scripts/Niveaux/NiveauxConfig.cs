//NiveauxConfig.cs
using System;
using Godot;
using SpaceZombie.Enemies;
using SpaceZombie.Enemies.Configs;
using SpaceZombie.Niveaux.Configs.V;

namespace SpaceZombie.Niveaux.Configs
{
    public class NiveauCreatorManager
    {
        private static NiveauCreator nc;

        public NiveauCreatorManager()
        {
            nc ??= new NiveauCreator();
        }
        public NiveauCreatorManager(GameData gd)
        {
            nc = new NiveauCreator(gd);
        }

        public NiveauZombiesSpawnSettings CreerNiveau(int stage, int niveau)
        {
            return nc.CreerNiveau(stage, niveau);
        }
        public void AppliquerNiveau(ZombiesSpawn spawn, NiveauZombiesSpawnSettings settings)
        {
            nc.AppliquerNiveau(spawn, settings);
        }
    }

    public class NiveauCreator
    {
        private V.LevelGenerator lg;
        public NiveauCreator()
        {
            // Initialisation si nécessaire
        }
        public NiveauCreator(GameData gd)
        {
            this.lg = new V.LevelGenerator(gd);
        }

        public NiveauZombiesSpawnSettings CreerNiveau(int stage, int niveau)
        {
            return lg.GenerateLevel(stage, niveau);
        }
        public void AppliquerNiveau(ZombiesSpawn spawn, NiveauZombiesSpawnSettings settings)
        {
            PackedScene enemySlotPrefab = (PackedScene)ResourceLoader.Load("res://Prefabs/enemy_slot.tscn");
            spawn.DeplaceEnBlock = settings.DeplaceEnBlock;
            for (int i = 0; i < settings.LigneSettings.Length; i++)
            {
                spawn.SetLignePhysicAttributes(i, new InLigneSpawnerObjetAttributsMapper(
                    settings.LigneSettings[i].IsFixe,
                    settings.LigneSettings[i].DirectionX,
                    settings.LigneSettings[i].Vitesse,
                    Vector2.Zero
                ));
                spawn.SetEnemySlot(i, new InLigneSpawnerObjetSetEnemySlotMapper(
                    settings.LigneSettings[i].EnemySlotSettings.Length,
                    enemySlotPrefab
                ));
                spawn.SetEnemyObjet(i, ConvertEnemyObjSettingToEnemyObjetMapper(settings.LigneSettings[i].EnemySlotSettings));
            }
        }
        private EnemyObjetMapper[] ConvertEnemyObjSettingToEnemyObjetMapper(NiveauEnemySlotSettings[] enemySlotSettings)
        {
            EnemyObjetMapper[] enemyObjetMappers = new EnemyObjetMapper[enemySlotSettings.Length];
            for (int i = 0; i < enemySlotSettings.Length; i++)
            {
                NiveauEnemyObjSettings enemyObjSettings = enemySlotSettings[i].enemyObjSettings;
                enemyObjetMappers[i] = new EnemyObjetMapper(
                    enemyObjSettings.IsVisible,
                    enemyObjSettings.enemySettings.EnemyObj,
                    enemyObjSettings.enemySettings.Enemy
                );
            }
            return enemyObjetMappers;
        }

    }
    public class NiveauEnemyAttackSettings
    {
        public float FireRate { get; set; }
        public int NbProjectilePerAttack { get; set; }
    }
    public class NiveauZombiesSpawnSettings
    {
        public bool DeplaceEnBlock { get; set; }
        public NiveauLigneSettings[] LigneSettings { get; set; }
        public NiveauEnemyAttackSettings EnemyAttackSettings { get; set; }
    }
    public class NiveauLigneSettings
    {
        public NiveauEnemySlotSettings[] EnemySlotSettings { get; set; }
        public float Vitesse { get; set; }
        public bool IsFixe { get; set; }
        public float DirectionX { get; set; }
    }
    public class NiveauEnemySlotSettings
    {
        public NiveauEnemyObjSettings enemyObjSettings { get; set; }
    }
    public class NiveauEnemyObjSettings
    {
        public bool IsVisible { get; set; }
        public NiveauEnemySettings enemySettings { get; set; }
    }
    public class NiveauEnemySettings
    {
        public PackedScene EnemyObj { get; set; }
        public Enemy Enemy { get; set; }
    }
}