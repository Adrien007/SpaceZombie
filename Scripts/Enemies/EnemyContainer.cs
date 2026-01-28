//EnemyContainer.cs
namespace SpaceZombie.Enemies
{
    public class EnemyContainer
    {
        private InLigneSpawnerObjet[] inLigneEnemyObjets;

        public EnemyContainer(InLigneSpawnerObjet[] inLigneEnemyObjets)
        {
            this.inLigneEnemyObjets = inLigneEnemyObjets;
        }
    }
}