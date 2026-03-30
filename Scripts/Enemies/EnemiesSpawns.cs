using Godot;
using SpaceZombie.Events;
using SpaceZombie.Joueurs;
using SpaceZombie.Ui;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpaceZombie.Enemies
{
    public partial class EnemiesSpawns : Node2D
    {
        const string zombie1 = "res://Prefabs/Enemies/zombie1.tscn";
        const string zombie2 = "res://Prefabs/Enemies/zombie2.tscn";
        const string zombie3 = "res://Prefabs/Enemies/zombie3.tscn";
        static float spawnAreaY = -100;
        static float spawnAreaHeight = 100f;
        static float spawnMargin = 80f;
        static float spawnLeft = spawnMargin;
        static float spawnCenter;
        static float spawnRight;

        private Joueur joueur;
        private Vector2 spawnAreaSize;
        private Timer spawnDelayTimer;
        private int levelIndex;
        private int waveIndex;
        private EnemySpawn[][] levels;
        public int numberOfEnemyInLevel = 0;
        private UpgradeLoader upgradeLoader;

        public override void _Ready()
        {
            spawnDelayTimer = new Timer
            {
                OneShot = true,
            };
            spawnDelayTimer.Timeout += SpawnWaves;
            levelIndex = 0;
            waveIndex = 0;
            AddChild(spawnDelayTimer);
        }

        public void Initialize(Vector2 playAreaSize, Joueur joueur, UpgradeLoader upgradeLoader)
        {
            this.joueur = joueur;
            this.upgradeLoader = upgradeLoader;
            spawnAreaSize = new Vector2(playAreaSize.X, spawnAreaHeight);
            spawnCenter = spawnAreaSize.X / 2;
            spawnRight = spawnAreaSize.X - spawnMargin;

            Position = new Vector2(0, -spawnAreaHeight + spawnAreaY);
            levels = [
                //Niveau 1
                [
                    //"On spawn 3 zombie1 a des positon random et on attend 6 secondes pour spawner les prochains zombie
                    new EnemySpawn(zombie1, (3, null), 6f),

                    //"On spawn 4 zombie1 a des positon random et on attend 8 secondes et on répète une deuxième fois
                    new EnemySpawn(zombie1, (4, null), 8f, 2),

                    //Fin du niveau lorsque tout les zombies du niveau sont morts.
                ],
                //Niveau 2
                [
                    //"On spawn 4 zombie2 a des positon random et on attend 8 secondes et on répète 3 fois
                    new EnemySpawn(zombie2, (4, null), 8f, 3),

                    //Fin du niveau lorsque tout les zombies du niveau sont morts.
                ],
                //Niveau 3
                [
                    //"On spawn 1 zombie3 au centre et on attend 8 secondes et on répète une deuxième fois
                    //new EnemySpawn(zombie3, (1, spawnCenter), 2f, 2),

                    //"On spawn 1 zombie3 à gauche et on spawn immétiatement le prochain
                    //new EnemySpawn(zombie3, (1, spawnLeft), 0),

                    //"On spawn 1 zombie3 à droite
                    //new EnemySpawn(zombie3, (1, spawnRight), 0),

                    //Fin du niveau lorsque tout les zombies du niveau sont morts.
                ],
                //Niveau 4
                [
                ],
                //Niveau 5
                [
                ],
                //Niveau 6
                [
                ],
                //Niveau 7
                [
                ],
                //Niveau 8
                [
                ],
                //Niveau 9
                [
                ],
            ];
        }

        public void InitializeLevel()
        {
            if (levelIndex >= levels.Length) return;
            waveIndex = 0;
            numberOfEnemyInLevel = 0;
            foreach (EnemySpawn wave in levels[levelIndex])
            {
                numberOfEnemyInLevel += wave.quantityAtPositionX.Item1 * wave.repeat;
            }
            upgradeLoader.UpdateUpgradeApparition(numberOfEnemyInLevel);
        }

        public void SpawnWaves()
        {

            EnemySpawn[] spawns = levels[levelIndex];

            if (waveIndex >= spawns.Length) return;

            EnemySpawn spawn = spawns[waveIndex];
            if (spawn.repeat > 0)
            {
                spawn.repeat -= 1;
                if (spawn.repeat == 0) waveIndex += 1;
                if (spawn.quantityAtPositionX.Item2 == null)
                {
                    SpawnRandomPosition(spawn.loader, spawn.quantityAtPositionX.Item1);
                }
                else
                {
                    SpawnAtPostion(spawn.loader, ((int, float))spawn.quantityAtPositionX);
                }
                StartNextSpawnTimer(spawn.delayTillNext);
            }
            else
            {
                throw new($"Spawning Error : Wanting to spawn 0 Enemy : At level :{levelIndex}, Wave : {waveIndex}");
            }
        }

        private void StartNextSpawnTimer(float delayTillNext)
        {
            if (delayTillNext >= 0)
            {
                spawnDelayTimer.Start(delayTillNext);
            }
            else
            {
                SpawnWaves();
            }
        }

        private void _OnEnemyDied()
        {
            numberOfEnemyInLevel -= 1;
            upgradeLoader.EnemyDied(numberOfEnemyInLevel);
            if (numberOfEnemyInLevel == 0)
            {
                levelIndex += 1;
                if (levelIndex < levels.Length)
                {
                    GameEvents.Instance.EmitSignal(GameEvents.SignalName.EndLevel, (levelIndex + 1).ToString());
                }
                else
                {
                    GD.Print("Spawn Boss");
                }
            }
        }

        private void SpawnAtPostion(PackedScene loader, (int quantity, float positionX) spawn)
        {
            for (int i = 0; i < spawn.quantity; i++)
            {
                BaseEnemy enemy = (BaseEnemy)loader.Instantiate();
                enemy.Initialize(GetRandomPositionY(spawn.positionX), joueur);
                AddChild(enemy);
            }
        }

        private void SpawnRandomPosition(PackedScene loader, int quantity)
        {
            for (int i = 0; i < quantity; i++)
            {
                BaseEnemy enemy = (BaseEnemy)loader.Instantiate();
                enemy.Initialize(GetRandomPosition(), joueur);
                AddChild(enemy);
            }
        }

        private Vector2 GetRandomPosition() =>
            new Vector2(GD.Randf() * spawnAreaSize.X, GD.Randf() * spawnAreaSize.Y);

        private Vector2 GetRandomPositionY(float x) =>
            new Vector2(x, GD.Randf() * spawnAreaSize.Y);
    }

    public class EnemySpawn
    {
        public (int, float?) quantityAtPositionX;
        public float delayTillNext;
        public int repeat;
        public PackedScene loader;

        public EnemySpawn(string resourcePath, (int, float?) quantityAtPositionX, float delayTillNext, int repeat = 1)
        {
            this.quantityAtPositionX = quantityAtPositionX;
            this.delayTillNext = delayTillNext;
            this.repeat = repeat;
            loader = (PackedScene)ResourceLoader.Load(resourcePath);
        }
    }


}
