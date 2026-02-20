//NiveauxConfig.cs
using System;
using Godot;
using SpaceZombie.Enemies;
using SpaceZombie.Enemies.Configs;
using SpaceZombie.Niveaux.Configs.V;

namespace SpaceZombie.Niveaux.Configs
{
    public static class NiveauxConfig
    {

    }

    public class NiveauxConf
    {
        private int stage;
        private int niveau;

    }

    public class Niveau
    {
        private EnemyContainer enemyContainer;

        public Niveau(EnemyContainer enemyContainer)
        {
            this.enemyContainer = enemyContainer;
        }
    }

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

        public NiveauZombiesSpawnSettings CreerNiveau(int niveau)
        {
            return nc.CreerNiveau(niveau);
        }
        public NiveauZombiesSpawnSettings CreerNiveau()
        {
            return nc.CreerNiveau();
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
        // pv go up * stage / chifre afin d'éviter monter exp.
        //HP = BaseHP * (1 + GrowthRate * (Stage ^ Exponent)) / DampingFactor;
        public static int CalculateHP(int baseHP, int stage, float growthRate = 0.5f, float exponent = 1.5f, float dampingFactor = 2f)
        {
            return (int)MathF.Ceiling(baseHP * (1 + growthRate * MathF.Pow(stage, exponent)) / dampingFactor);
        }

        public NiveauZombiesSpawnSettings CreerNiveau(int niveau)
        {
            R.LevelGenerator lg = new R.LevelGenerator();
            var niveauGen = lg.GenerateLevel(niveau);
            var map = MapLevelGeneratorToNiveauConfig.MapEnemiesToNiveauSettings(niveauGen);
            return map;
        }
        public NiveauZombiesSpawnSettings CreerNiveau(int stage, int niveau)
        {
            return lg.GenerateLevel(stage, niveau);
        }

        public NiveauZombiesSpawnSettings CreerNiveau()
        {
            int stage = 1;

            NiveauLigneSettings[] ligneSettings = new NiveauLigneSettings[6];
            int ligne = 0;

            //ligne 0
            ligneSettings[ligne++] = new NiveauLigneSettings()
            {
                EnemySlotSettings = CreerNiveauEnemySlotSettings(5, true, EnemyConfig.enemyType1, stage),
                Vitesse = 100f,
                IsFixe = false,
                DirectionX = 1.0f
            };

            //ligne 1
            ligneSettings[ligne++] = new NiveauLigneSettings()
            {
                EnemySlotSettings = CreerNiveauEnemySlotSettings(5, true, EnemyConfig.enemyType1, stage),
                Vitesse = 100f,
                IsFixe = false,
                DirectionX = 1.0f
            };

            //ligne 2
            ligneSettings[ligne++] = new NiveauLigneSettings()
            {
                EnemySlotSettings = CreerNiveauEnemySlotSettings(5, true, EnemyConfig.enemyType2, stage),
                Vitesse = 100f,
                IsFixe = false,
                DirectionX = 1.0f
            };

            //ligne 3
            ligneSettings[ligne++] = new NiveauLigneSettings()
            {
                EnemySlotSettings = CreerNiveauEnemySlotSettings(1, false, EnemyConfig.enemyType1, stage),
                Vitesse = 100f,
                IsFixe = true,
                DirectionX = 1.0f
            };

            //ligne 4
            ligneSettings[ligne++] = new NiveauLigneSettings()
            {
                EnemySlotSettings = CreerNiveauEnemySlotSettings(1, false, EnemyConfig.enemyType1, stage),
                Vitesse = 100f,
                IsFixe = true,
                DirectionX = 1.0f
            };

            //ligne 5
            ligneSettings[ligne++] = new NiveauLigneSettings()
            {
                EnemySlotSettings = CreerNiveauEnemySlotSettings(1, false, EnemyConfig.enemyType1, stage),
                Vitesse = 100f,
                IsFixe = true,
                DirectionX = 1.0f
            };

            return new NiveauZombiesSpawnSettings()
            {
                LigneSettings = ligneSettings
            };
        }
        private NiveauLigneSettings CreerNiveauLigneSettings(int length, bool isVisible, NiveauEnemySettings enemy, int stage)
        {
            return new NiveauLigneSettings()
            {
                EnemySlotSettings = CreerNiveauEnemySlotSettings(length, isVisible, enemy, stage),
                Vitesse = 100f,
                IsFixe = false,
                DirectionX = 1.0f
            };
        }
        private NiveauEnemySlotSettings[] CreerNiveauEnemySlotSettings(int length, bool isVisible, NiveauEnemySettings enemy, int stage)
        {
            NiveauEnemySlotSettings[] slots = new NiveauEnemySlotSettings[length];
            for (int i = 0; i < length; i++)
            {
                slots[i] = CreerNiveauEnemySlotSettings(isVisible, enemy, stage);
            }
            return slots;
        }
        private NiveauEnemySlotSettings CreerNiveauEnemySlotSettings(bool isVisible, NiveauEnemySettings enemy, int stage)
        {
            return new NiveauEnemySlotSettings() { enemyObjSettings = CreerNiveauEnemyObjSettings(isVisible, enemy, stage) };
        }
        private NiveauEnemyObjSettings CreerNiveauEnemyObjSettings(bool isVisible, NiveauEnemySettings enemy, int stage)
        {
            return new NiveauEnemyObjSettings()
            {
                IsVisible = isVisible,
                enemySettings = CreerNiveauEnemySettings(enemy, stage)
            };
        }
        private NiveauEnemySettings CreerNiveauEnemySettings(NiveauEnemySettings enemy, int stage)
        {
            return new NiveauEnemySettings()
            {
                Color = enemy.Color,
                Enemy = new Enemy(CalculateHP(enemy.Enemy.Hp, stage), enemy.Enemy.Score)
            };
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
                    enemyObjSettings.enemySettings.Color,
                    enemyObjSettings.enemySettings.Enemy
                );
            }
            return enemyObjetMappers;
        }

    }
    public interface IEnemyNiveauConfig
    {
        //
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
        public Color Color { get; set; }
        public Enemy Enemy { get; set; }
    }
}