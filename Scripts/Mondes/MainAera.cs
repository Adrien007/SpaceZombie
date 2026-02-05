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
        private EnemyAttackManager enemyAttackManager;
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
            uint collisionLayer = LayerDictionnary.GetLayer(LayerDictionnary.ProjectileJoueur);
            uint collisionMask = LayerDictionnary.GetLayer(LayerDictionnary.OutOfBound)
                                 | LayerDictionnary.GetLayer(LayerDictionnary.Enemy);
            joueur.Initialize(2, 0, this, 14, collisionLayer, collisionMask, new Ammunitions.Projectile(1, 250f, false), ees);

            collisionLayer = LayerDictionnary.GetLayer(LayerDictionnary.Enemy);
            collisionMask = LayerDictionnary.GetLayer(LayerDictionnary.OutOfBound)
                                 | LayerDictionnary.GetLayer(LayerDictionnary.Joueur);
            enemyAttackManager = new EnemyAttackManager(this, 14, collisionLayer, collisionMask, new Ammunitions.Projectile(1, 200f, false), jes);
            

            //lm.DemarrerPremierNiveau();
            lm.DemarrerNiveau(0, 1);
        }

        public override void _PhysicsProcess(double delta)
        {
            enemyAttackManager.Fire();//Tempo
            ees.Notify();
            jes.Notify();
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