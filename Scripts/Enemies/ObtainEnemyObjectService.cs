//ObtainEnemyObjectService.cs
using System.Collections.Generic;

namespace SpaceZombie.Enemies
{
    public class ObtainEnemyObjectService
    {
        public List<EnemyObjet> GetAllEnemy(InLigneSpawnerObjet[] inLigneSpawnersObjet)
        {
            List<EnemyObjet> enemies = new List<EnemyObjet>();
            foreach (var ligne in inLigneSpawnersObjet)
            {
                var slots = ligne.GetAllEnemySlot();
                foreach (var s in slots)
                {
                    enemies.Add(s.GetEnemyObjet());
                }
            }
            return enemies;
        }
    }
}