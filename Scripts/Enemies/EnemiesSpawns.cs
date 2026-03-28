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
        private int nextLevelIndex;
        private int nextWaveIndex;
        private EnemySpawn[][] levels;
        private int numberOfEnemySpawned = 0;

        public override void _Ready()
        {
            spawnDelayTimer = new Timer
            {
                OneShot = true,
            };
            spawnDelayTimer.Timeout += SpawnWave;
            nextLevelIndex = 0;
            nextWaveIndex = 0;
            AddChild(spawnDelayTimer);
        }

        public void Initialize(Vector2 playAreaSize, Joueur joueur)
        {
            this.joueur = joueur;
            spawnAreaSize = new Vector2(playAreaSize.X, spawnAreaHeight);
            spawnCenter = spawnAreaSize.X / 2;
            spawnRight = spawnAreaSize.X - spawnMargin;

            Position = new Vector2(0, -spawnAreaHeight + spawnAreaY);
            levels = [
                [
                    new EnemySpawn(zombie3, (1, null), 5f, 4),
                    //new EnemySpawn(zombie1, (3, null), 6f, 4),
                    //new EnemySpawn(zombie2, (5, null), 10, 3),
                ],
                [
                    new EnemySpawn(zombie1, (3, null), 6f, 4),
                    //new EnemySpawn("res://Prefabs/Enemies/zombie.tscn", (3, null), 7f, 2),
                ],
                [
                    new EnemySpawn(zombie2, (2, null), 5, 3),
                ],
            ];
            GameEvents.Instance.EmitSignal(GameEvents.SignalName.EndLevel, "1");
        }

        public void SpawnLevel()
        {
            if (nextLevelIndex >= levels.Length) return;
            nextWaveIndex = 0;
            SpawnWave();
        }

        private void SpawnWave()
        {

            EnemySpawn[] spawns = levels[nextLevelIndex];

            if (nextWaveIndex >= spawns.Length) return;

            EnemySpawn spawn = spawns[nextWaveIndex];
            if (spawn.repeat > 0)
            {
                spawn.repeat -= 1;
                if (spawn.repeat == 0) nextWaveIndex += 1;
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
                throw new($"Spawning Error : Wanting to spawn 0 Enemy : At level :{nextLevelIndex}, Wave : {nextWaveIndex}");
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
                SpawnWave();
            }
        }

        private void OnEnemyDied()
        {
            numberOfEnemySpawned -= 1;
            if (numberOfEnemySpawned == 0)
            {
                nextLevelIndex += 1;
                GameEvents.Instance.EmitSignal(GameEvents.SignalName.EndLevel, (nextLevelIndex + 1).ToString());
            }
        }

        private void SpawnAtPostion(PackedScene loader, (int quantity, float positionX) spawn)
        {
            for (int i = 0; i < spawn.quantity; i++)
            {
                BaseEnemy enemy = (BaseEnemy)loader.Instantiate();
                enemy.Initialize(GetRandomPositionY(spawn.positionX), joueur, OnEnemyDied);
                AddChild(enemy);
            }
            numberOfEnemySpawned += spawn.quantity;
        }

        private void SpawnRandomPosition(PackedScene loader, int quantity)
        {
            for (int i = 0; i < quantity; i++)
            {
                BaseEnemy enemy = (BaseEnemy)loader.Instantiate();
                enemy.Initialize(GetRandomPosition(), joueur, OnEnemyDied);
                AddChild(enemy);
            }
            numberOfEnemySpawned += quantity;
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
