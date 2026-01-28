//MainAera.cs
using Godot;
using SpaceZombie.Enemies;
using SpaceZombie.Events;
using SpaceZombie.Joueurs;
using SpaceZombie.Niveaux;
using SpaceZombie.Scores.GameScore;
using SpaceZombie.Utilitaires.Layers;
using System;

namespace SpaceZombie.Mondes.Utilitaires
{
    public partial class MainAera : Control
    {
        [Export] private AeraPlayBound area;
        [Export] private Joueur joueur;
        [Export] private ZombiesSpawn zombiesSpawn;
        private LevelManager lm;
        private EnemyEventSystem ees;
        private JoueurEventSystem jes;
        private static LayerDictionnary ld;
        public override void _Ready()
        {
            AeraPlayBoundAccessor.Initialize(area);

            ld = new LayerDictionnary();
            ees = new EnemyEventSystem(new BulletCollisionManager(64, new BulletCollisionOnEnemyService()));
            jes = new JoueurEventSystem(new BulletCollisionManager(32, new BulletCollisionOnPlayerService()));
        }
        public void Initialiser()
        {
            area.InitialiserSize(this.Size);

            var endLevelSystemEnemySide = new EndLevelSystem();
            endLevelSystemEnemySide.EndLevelSignal += ChangerNiveauLogic;
            var endLevelSystemPlayerSide = new EndLevelSystemPlayer();
            endLevelSystemPlayerSide.EndLevelSignal += QUITTER;

            ees.Register(zombiesSpawn, endLevelSystemEnemySide);
            jes.Register(endLevelSystemPlayerSide);

            lm = new LevelManager(endLevelSystemEnemySide, endLevelSystemEnemySide, zombiesSpawn);

            joueur.InitialiserSize(this.Size);
            joueur.InitialiserPosition(this.Position);
            joueur.Initialize(this, 14, new Ammunitions.Projectile(1, 250f, false), ees);

            //lm.DemarrerPremierNiveau();
            lm.DemarrerNiveau(0, 1);
        }

        public override void _PhysicsProcess(double delta)
        {
            ees.Notify();
        }

        private void QUITTER()
        {
            GetTree().Quit();
        }
        private void ChangerNiveauLogic()
        {
            //Resetter toutes les objets et position.
            //Desactiver input
            lm.CreerNiveau();
            //ReactiverInput
        }
    }
}