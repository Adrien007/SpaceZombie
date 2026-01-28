//EnemyEventHandler.cs

namespace SpaceZombie.Enemies
{
    public static class EnemyEventHandler
    {
        private static int RetirerHp(int hp, int hitValue)
        {
            hp -= hitValue;
            if (hp < 0)
            {
                hp = 0;
            }
            return hp;
        }
        public static void GestionHitSurEnemy(EnemyObjet enemyObj, int damage)
        {
            if (!IsEnemyDie(enemyObj))
            {
                enemyObj.Enemy.Hp = RetirerHp(enemyObj.Enemy.Hp, damage);
                if (enemyObj.Enemy.Hp <= 0)
                {
                    enemyObj.enemyFlagLogic.isDead = true;
                    //enemyObj.DisableCallDefered();
                    enemyObj.Disable();
                }
            }
        }
        public static bool IsEnemyDie(EnemyObjet enemyObj)
        {
            return enemyObj.enemyFlagLogic.isDead;
        }
        public static bool IsDeadSoundPlayed(EnemyObjet enemyObj)
        {
            return enemyObj.enemyFlagLogic.deadSoundPlayed;
        }
        public static bool IsScoreGiven(EnemyObjet enemyObj)
        {
            return enemyObj.enemyFlagLogic.scoreGiven;
        }
        public static bool IsEndLevelAdded(EnemyObjet enemyObj)
        {
            return enemyObj.enemyFlagLogic.endLevelAdded;
        }
        public static EnemyObjet TrouverParent(Godot.Area2D aera2D)
        {
            //GD.Print(aera2D.GetParent().GetType());
            return (EnemyObjet)aera2D.GetParent();
        }
    }
}
