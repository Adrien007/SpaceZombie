//MainAera.cs
using Godot;
using SpaceZombie.Enemies;
using SpaceZombie.Events;
using SpaceZombie.Joueurs;
using SpaceZombie.Niveaux;
using SpaceZombie.Scores.GameScore;
using SpaceZombie.Ui;
using SpaceZombie.Utilitaires.Layers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceZombie.Mondes.Utilitaires
{
    public partial class MainAera : Control
    {
        [Export] private AeraPlayBound area;
        [Export] private Joueur joueur;
        [Export] private ZombiesSpawn zombiesSpawn;
        [Export] private ProchainNiveauUi prochainNiveauUi;
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
            GetTree().Paused = true;

            area.InitialiserSize(this.Size);

            var res = new ResetEtatManager();

            var endLevelSystemEnemySide = new EndLevelSystem();
            var endLevelSystemPlayerSide = new EndLevelSystemPlayer();
            endLevelSystemPlayerSide.EndLevelSignal += QUITTER;

            ees.Register(zombiesSpawn, endLevelSystemEnemySide);
            jes.Register(endLevelSystemPlayerSide);

            joueur.InitialiserSize(this.Size);
            joueur.InitialiserPosition(this.Position);
            uint collisionLayer = LayerDictionnary.GetLayer(LayerDictionnary.ProjectileJoueur);
            uint collisionMask = LayerDictionnary.GetLayer(LayerDictionnary.OutOfBound)
                                 | LayerDictionnary.GetLayer(LayerDictionnary.Enemy);
            joueur.Initialize(2, 1, this, 14, collisionLayer, collisionMask, new Ammunitions.Projectile(1, 250f, false), ees, res);


            Timer enemyFireOptionsTimer = new Timer();
            EnemyFireOptions enemyFireOptions = new EnemyFireOptions(new Random(1), enemyFireOptionsTimer);
            EnemyFireService enemyFireService = new EnemyFireService(enemyFireOptions);
            collisionLayer = LayerDictionnary.GetLayer(LayerDictionnary.Enemy);
            collisionMask = LayerDictionnary.GetLayer(LayerDictionnary.OutOfBound)
                                 | LayerDictionnary.GetLayer(LayerDictionnary.Joueur);
            EnemyAttackManager enemyAttackManager = new EnemyAttackManager(this, 14, collisionLayer, collisionMask, new Ammunitions.Projectile(1, 200f, false), 
                                                                            jes, res, enemyFireService);

            var lm = new LevelManager(endLevelSystemEnemySide, endLevelSystemEnemySide, zombiesSpawn, enemyFireOptions, enemyAttackManager);
            lm.SetNiveau(0, 0);

            var ltm = new LevelTransitionManager(GetTree(), prochainNiveauUi, lm, res, endLevelSystemEnemySide);
            ltm.ChangerNiveauLogic();
        }

        public override void _PhysicsProcess(double delta)
        {
            ees.Notify();
            jes.Notify();
        }

        private void QUITTER()
        {
            GetTree().Quit();
        }
    }
}