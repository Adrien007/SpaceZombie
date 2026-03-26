using Godot;
using SpaceZombie.Joueurs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpaceZombie.Enemies
{
    public partial class EnemiesSpawns : Node2D
    {
        static private float spawnAreaHeight = 150f;
        private Joueur joueur;
        private Vector2 spawnAreaSize;
        private Timer spawnEveryTimer;
        private int nextSpawnIndex;
        private List<EnemySpawn> spawns;

        public override void _Ready()
        {
            spawnEveryTimer = new Timer
            {
                OneShot = true,
            };
            spawnEveryTimer.Timeout += DoNextSpawn;
            nextSpawnIndex = 0;
            AddChild(spawnEveryTimer);
        }

        public void Initialize(Vector2 playAreaSize, Joueur joueur)
        {
            this.joueur = joueur;
            spawnAreaSize = new Vector2(playAreaSize.X, spawnAreaHeight);
            Position = new Vector2(0, -spawnAreaHeight - 50);
        }

        public void Spawn(List<EnemySpawn> spawns)
        {
            this.spawns = spawns;
            nextSpawnIndex = 0;
            DoNextSpawn();
        }

        private void DoNextSpawn()
        {
            if (nextSpawnIndex >= spawns.Count)
            {
                return;
            }

            EnemySpawn spawn = spawns[nextSpawnIndex];
            if (spawn.repeat > 0)
            {
                SpawnRandomPosition(spawn.loader, spawn.number);
                spawn.repeat -= 1;
                if (spawn.every >= 0)
                {
                    spawnEveryTimer.Start(spawn.every);
                }
                else
                {
                    DoNextSpawn();
                }
            }
            else
            {
                nextSpawnIndex += 1;
                DoNextSpawn();
            }
        }

        private void SpawnRandomPosition(PackedScene loader, int number)
        {
            for (int i = 0; i < number; i++)
            {
                Zombie zombie = (Zombie)loader.Instantiate();
                zombie.Position = GetRandomPosition(spawnAreaSize);
                zombie.joueur = joueur;
                AddChild(zombie);
            }
        }

        private static Vector2 GetRandomPosition(Vector2 groupArea) =>
            new Vector2(GD.Randf() * groupArea.X, GD.Randf() * groupArea.Y);

    }

    public class EnemySpawn
    {
        public int number;
        public float every;
        public int repeat;
        public PackedScene loader;

        public EnemySpawn(string resourcePath, int number, float every = 0, int repeat = 1)
        {
            this.number = number;
            this.every = every;
            this.repeat = repeat;
            loader = (PackedScene)ResourceLoader.Load(resourcePath);
        }
    }
}
