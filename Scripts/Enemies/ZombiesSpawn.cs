//ZombiesSpawn.cs
using System;
using System.Collections.Generic;
using Godot;
using SpaceZombie.Events;
using SpaceZombie.Mondes.Utilitaires;

namespace SpaceZombie.Enemies
{
    public interface IZombiesSpawnService : IInLigneSpawnerObjetService, IObtainEnemyObject
    {

    }
    public interface IInLigneSpawnerObjetService
    {
        public void SetLignePhysicAttributes(int indexLigne, InLigneSpawnerObjetAttributsMapper mapper);
        public void SetEnemySlot(int indexLigne, InLigneSpawnerObjetSetEnemySlotMapper mapper);
        public void SetEnemyObjet(int indexLigne, EnemyObjetMapper[] mapper);
    }
    public interface IObtainEnemyObject
    {
        public List<EnemyObjet> GetAllEnemy(ObtainEnemyObjectService service);
    }
    public partial class ZombiesSpawn : VBoxContainer, IZombiesSpawnService
    {
        [Export] private InLigneSpawnerObjet[] inLigneSpawnersObjet;
        public bool DeplaceEnBlock { get; set; }

        public void SetLignePhysicAttributes(int indexLigne, InLigneSpawnerObjetAttributsMapper mapper)
        {
            InLigneSpawnerObjetService.SetPhysicAttributes(indexLigne, mapper, inLigneSpawnersObjet);
        }
        public void SetEnemySlot(int indexLigne, InLigneSpawnerObjetSetEnemySlotMapper mapper)
        {
            InLigneSpawnerObjetService.SetEnemySlot(indexLigne, mapper, inLigneSpawnersObjet);
        }
        public void SetEnemyObjet(int indexLigne, EnemyObjetMapper[] mapper)
        {
            InLigneSpawnerObjetService.SetEnemyObjet(indexLigne, mapper, inLigneSpawnersObjet);
        }

        public void DesactiverEnemyPasEnSandwitch(InLigneSpawnerUtilitiesEventService service)
        {
            service.DesactiverEnemyPasEnSandwitch(inLigneSpawnersObjet);
        }

        public List<EnemyObjet> GetAllEnemy(ObtainEnemyObjectService service)
        {
            return service.GetAllEnemy(inLigneSpawnersObjet);
        }


        public override void _PhysicsProcess(double delta)
        {
            InLigneSpawnerUtilitiesEventService.DeplacerLigne(inLigneSpawnersObjet, (float)delta, DeplaceEnBlock);
        }
    }








    public static class InLigneSpawnerObjetService
    {
        public static void ArgumentOutOfRangeCheck<T>(int indexLigne, T[] tableau)
        {
            if (indexLigne < 0 || indexLigne >= tableau.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(indexLigne), $"Index out of bounds for array of length: {tableau.Length}.");
            }
        }
        public static void SetPhysicAttributes(int indexLigne, InLigneSpawnerObjetAttributsMapper mapper, InLigneSpawnerObjet[] inLigneSpawnersObjet)
        {
            ArgumentOutOfRangeCheck(indexLigne, inLigneSpawnersObjet);

            InLigneSpawnerObjet spawner = inLigneSpawnersObjet[indexLigne];
            spawner.SetPhysicAttributes(mapper);
        }
        public static void SetEnemySlot(int indexLigne, InLigneSpawnerObjetSetEnemySlotMapper mapper, InLigneSpawnerObjet[] inLigneSpawnersObjet)
        {
            ArgumentOutOfRangeCheck(indexLigne, inLigneSpawnersObjet);

            InLigneSpawnerObjet spawner = inLigneSpawnersObjet[indexLigne];
            spawner.SetEnemySlot(mapper);
        }
        public static void SetEnemyObjet(int indexLigne, EnemyObjetMapper[] mapper, InLigneSpawnerObjet[] inLigneSpawnersObjet)
        {
            ArgumentOutOfRangeCheck(indexLigne, inLigneSpawnersObjet);

            InLigneSpawnerObjet spawner = inLigneSpawnersObjet[indexLigne];
            spawner.SetEnemyObjet(mapper);
        }
    }
}
