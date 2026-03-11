//MainAera.cs
using Godot;
using SpaceZombie.Enemies;
using SpaceZombie.Events;
using SpaceZombie.Joueurs;
using SpaceZombie.Niveaux;
using SpaceZombie.Ui;
using SpaceZombie.Utilitaires.Layers;
using System;

namespace SpaceZombie.Mondes.Utilitaires
{
    public partial class MainAera : Control
    {
        [Export] private AeraPlayBound area;
        [Export] private Joueur joueur;
        [Export] private ZombiesSpawn zombiesSpawn;
        [Export] private ProchainNiveauUi prochainNiveauUi;
        private static LayerDictionnary ld;
        public override void _Ready()
        {
            AeraPlayBoundAccessor.Initialize(area);

            ld = new LayerDictionnary();

            GameEvents.Instance.PlayerDied += QUITTER;
        }
        public void Initialiser()
        {
            GetTree().Paused = true;

            area.InitialiserSize(this.Size);

            var res = new ResetEtatManager();

            joueur.InitialiserSize(this.Size);
            joueur.InitialiserPosition(this.Position);
            joueur.Initialize(3, res);

            Timer enemyFireOptionsTimer = new Timer();
            EnemyFireOptions enemyFireOptions = new EnemyFireOptions(new Random(1), enemyFireOptionsTimer);
            EnemyFireService enemyFireService = new EnemyFireService(enemyFireOptions);
            EnemyAttackManager enemyAttackManager = new EnemyAttackManager(this, res, enemyFireService);

            var lm = new LevelManager(zombiesSpawn, enemyFireOptions, enemyAttackManager);
            lm.SetNiveau(0, 0);

            var ltm = new LevelTransitionManager(GetTree(), prochainNiveauUi, lm, res);
            ltm.ChangerNiveauLogic();
        }

        private void QUITTER()
        {
            GetTree().Quit();
        }
    }
}
