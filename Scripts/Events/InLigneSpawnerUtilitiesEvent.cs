//InLigneSpawnerUtilitiesEvent.cs
using Godot;
using SpaceZombie.Ammunitions;
using SpaceZombie.Enemies;
using SpaceZombie.Mondes.Utilitaires;

namespace SpaceZombie.Events
{
    public interface IZombiesSpawnServiceUtilitiesEvent
    {
        public void DesactiverEnemyPasEnSandwitch(InLigneSpawnerUtilitiesEventService service);
    }
    public class InLigneSpawnerUtilitiesEvent : IBulletCollisionOberser
    {
        private IZombiesSpawnServiceUtilitiesEvent zp;
        private InLigneSpawnerUtilitiesEventService service;
        public InLigneSpawnerUtilitiesEvent(IZombiesSpawnServiceUtilitiesEvent zombiesSpawnServiceUtilitiesEvent, InLigneSpawnerUtilitiesEventService service)
        {
            zp = zombiesSpawnServiceUtilitiesEvent;
            this.service = service;
        }
        public void OnBulletCollision(ProjectileObjet projectile, Area2D aera2D)
        {
            zp.DesactiverEnemyPasEnSandwitch(service);
        }
    }


    public class InLigneSpawnerUtilitiesEventService
    {
        public void DesactiverEnemyPasEnSandwitch(InLigneSpawnerObjet[] inLigneSpawnersObjet)
        {
            foreach (var ligne in inLigneSpawnersObjet)
            {
                ligne.DesactiverEnemyPasEnSandwitch();
            }
        }

        public static void DeplacerLigne(InLigneSpawnerObjet[] inLigneSpawnersObjet, float delta, bool deplaceEnBlock)
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
        static void DeplacerLigneEnBlock(InLigneSpawnerObjet[] inLigneSpawnersObjet, float delta)
        {
            float minX = AeraPlayBoundAccessor.GetInstance().Position.X;
            float maxX = AeraPlayBoundAccessor.GetInstance().Position.X + AeraPlayBoundAccessor.GetInstance().Size.X;
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
        static void DeplacerLigneLibre(InLigneSpawnerObjet[] inLigneSpawnersObjet, float delta)
        {
            float minX = AeraPlayBoundAccessor.GetInstance().Position.X;
            float maxX = AeraPlayBoundAccessor.GetInstance().Position.X + AeraPlayBoundAccessor.GetInstance().Size.X;
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