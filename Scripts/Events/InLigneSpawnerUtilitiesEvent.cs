//InLigneSpawnerUtilitiesEvent.cs
using Godot;
using SpaceZombie.Ammunitions;
using SpaceZombie.Enemies;
using SpaceZombie.Mondes.Utilitaires;

namespace SpaceZombie.Events
{
    public class InLigneSpawnerUtilitiesEventService
    {
        private Rect2 areaPlay;

        public InLigneSpawnerUtilitiesEventService(Rect2 areaPlay)
        {
            this.areaPlay = areaPlay; ;
        }
        public void DesactiverEnemyPasEnSandwitch(InLigneSpawnerObjet[] inLigneSpawnersObjet)
        {
            foreach (var ligne in inLigneSpawnersObjet)
            {
                ligne.DesactiverEnemyPasEnSandwitch();
            }
        }

        public void DeplacerLigne(InLigneSpawnerObjet[] inLigneSpawnersObjet, float delta, bool deplaceEnBlock)
        {
            if (deplaceEnBlock)
            {
                DeplacerLigneEnBlock(inLigneSpawnersObjet, delta);
            }
            else
            {
                DeplacerLigneLibre(inLigneSpawnersObjet, delta);
            }
        }
        private void DeplacerLigneEnBlock(InLigneSpawnerObjet[] inLigneSpawnersObjet, float delta)
        {
            float minX = areaPlay.Position.X;
            float maxX = areaPlay.Position.X + areaPlay.Size.X;
            int premierIndexVisibleToutLigne = int.MaxValue;
            int dernierIndexVisibleToutLigne = -1;
            for (int i = 0; i < inLigneSpawnersObjet.Length; i++)
            {
                if (!inLigneSpawnersObjet[i].IsFixe)
                {
                    int premierIndexVisible = inLigneSpawnersObjet[i].GetPremierIndexVisible();
                    if (premierIndexVisible < 0)
                    {
                        continue; // Skip if no visible index
                    }
                    if (premierIndexVisible < premierIndexVisibleToutLigne)
                    {
                        premierIndexVisibleToutLigne = premierIndexVisible;
                    }

                    int dernierIndexVisible = inLigneSpawnersObjet[i].GetDernierIndexVisible();
                    if (dernierIndexVisible > dernierIndexVisibleToutLigne)
                    {
                        dernierIndexVisibleToutLigne = dernierIndexVisible;
                    }
                }
            }
            for (int i = 0; i < inLigneSpawnersObjet.Length; i++)
            {
                if (!inLigneSpawnersObjet[i].IsFixe)
                {
                    inLigneSpawnersObjet[i].MoveChildsAlongXAxis(delta, minX, maxX, premierIndexVisibleToutLigne, dernierIndexVisibleToutLigne);
                }
            }
        }
        private void DeplacerLigneLibre(InLigneSpawnerObjet[] inLigneSpawnersObjet, float delta)
        {
            float minX = areaPlay.Position.X;
            float maxX = areaPlay.Position.X + areaPlay.Size.X;
            for (int i = 0; i < inLigneSpawnersObjet.Length; i++)
            {
                if (!inLigneSpawnersObjet[i].IsFixe)
                {
                    int premierIndexVisible = inLigneSpawnersObjet[i].GetPremierIndexVisible();
                    if (premierIndexVisible < 0)
                    {
                        continue; // Skip if no visible index
                    }
                    int dernierIndexVisible = inLigneSpawnersObjet[i].GetDernierIndexVisible();
                    inLigneSpawnersObjet[i].MoveChildsAlongXAxis(delta, minX, maxX, premierIndexVisible, dernierIndexVisible);
                }
            }
        }
    }
}