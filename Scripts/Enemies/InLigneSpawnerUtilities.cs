//InLigneSpawnerUtilities.cs

using Godot;

namespace SpaceZombie.Enemies.Utilitaires
{
    public static class InLigneSpawnerUtilities
    {
        public static void DesactiverEnemyPasEnSandwitch(EnemySlot[] enemies)
        {
            int firtsVisibleIndex = TrouverPremierIndexVisible(enemies);
            int lastVisibleIndex = TrouverDernierIndexVisible(enemies, firtsVisibleIndex);
            DesactiverEnemyPasEnSandwitch(enemies, firtsVisibleIndex, lastVisibleIndex);
        }
        public static void DesactiverEnemyPasEnSandwitch(EnemySlot[] enemies, int firtsVisibleIndex, int lastVisibleIndex)
        {
            DesactiverIndexAvantPremierVisible(enemies, firtsVisibleIndex);
            DesactiverIndexApresDernierVisible(enemies, lastVisibleIndex);
        }

        public static int TrouverPremierIndexVisible(EnemySlot[] enemies)
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                if (enemies[i].EstEnemieVisible)
                {
                    return i;
                }
            }
            return -1;
        }
        public static int TrouverDernierIndexVisible(EnemySlot[] enemies, int firtsVisibleIndex)
        {
            int lastVisibleIndex = enemies.Length - 1; // If all are visible, return the last (visible) index;
            for (int i = firtsVisibleIndex + 1; i < enemies.Length; i++)
            {
                if (enemies[i].EstEnemieVisible)
                {
                    lastVisibleIndex = i;
                }
            }
            return lastVisibleIndex;
        }
        private static void DesactiverIndexAvantPremierVisible(EnemySlot[] enemies, int firtsVisibleIndex)
        {
            for (int i = 0; i < firtsVisibleIndex; i++)
            {
                enemies[i].Visible = false;
            }
        }
        private static void DesactiverIndexApresDernierVisible(EnemySlot[] enemies, int lastVisibleIndex)
        {
            for (int i = lastVisibleIndex + 1; i < enemies.Length; i++)
            {
                enemies[i].Visible = false;
            }
        }
    }
}
