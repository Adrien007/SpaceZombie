//JoueurEventHandler.cs

using Godot;

namespace SpaceZombie.Joueurs
{
    public static class JoueurEventHandler
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
        public static void GestionHitSurEnemy(Joueur j, int damage)
        {
            if (!IsObjetDie(j.jState) && !IsObjetInvincible(j.jState))
            {
                j.jState.IsInvicible = true;
                j.jState.InvincibilityTimer.Start();
                j.jState.Hp = RetirerHp(j.jState.Hp, damage);
                if (j.jState.Hp <= 0)
                {
                    j.jState.IsDead = true;
                    j.CallDeferred(nameof(j.Disable));
                }
            }
        }
        public static bool IsObjetDie(JoueurEtat j)
        {
            return j.IsDead;
        }
        public static bool IsObjetInvincible(JoueurEtat j)
        {
            return j.IsInvicible;
        }
        public static bool IsDeadSoundPlayed(JoueurEtat j)
        {
            return j.DeadSoundPlayed;
        }
        // public static bool IsScoreGiven(EnemyObjet enemyObj)
        // {
        //     return enemyObj.enemyFlagLogic.scoreGiven;
        // }
        public static bool IsEndLevel(JoueurEtat j)
        {
            return j.EndLevel;
        }
        public static Joueur TrouverParent(Area2D aera2D)
        {
            GD.Print(aera2D.GetParent().GetType());
            return (Joueur)aera2D.GetParent();
        }
    }
}
