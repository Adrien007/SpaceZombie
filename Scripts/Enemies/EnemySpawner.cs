//EnemySpawner.cs
namespace SpaceZombie.Enemies
{
    public class EnemySpawner
    {
        private Enemy enemy;

        public EnemySpawner(Enemy enemy)
        {
            this.enemy = enemy;
        }

        public void SpawnEnemy()
        {
            // Logic to spawn the enemy in the game world
            // For example, instantiate the enemy object at a specific position
            // and add it to the game manager or scene.
        }
    }
}