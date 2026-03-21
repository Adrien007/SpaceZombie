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
        [Export] public Upgrade upgrade;
        [Export] private MenuUpgrade menuUpgrade;
        private static LayerDictionnary ld;

        public override void _Ready()
        {
            AeraPlayBoundAccessor.Initialize(area);
            menuUpgrade.Upgrade += Upgrade;
            GameEvents.Instance.ChooseUpgrade += ChooseUpgrade;
            GameEvents.Instance.PlayerDied += QUITTER;
        }
        public void Initialiser(Vector2 outOfBoundSize)
        {
            GetTree().Paused = true;

            area.InitialiserSize(outOfBoundSize);

            var res = new ResetEtatManager();

            joueur.InitialiserSize(this.Size);
            joueur.InitialiserPosition(this.Position);
            joueur.Initialize(99, res);

            upgrade.InitializePlayAreaSize(Size);

            EnemyFireOptions enemyFireOptions = new EnemyFireOptions(new Random(1));
            EnemyFireService enemyFireService = new EnemyFireService(enemyFireOptions);
            EnemyAttackManager enemyAttackManager = new EnemyAttackManager(this, res, enemyFireService);

            var lm = new LevelManager(zombiesSpawn, enemyFireOptions, enemyAttackManager, upgrade);
            lm.SetNiveau(0, 1);

            var ltm = new LevelTransitionManager(GetTree(), prochainNiveauUi, lm, res);
            ltm.ChangerNiveauLogic();
        }

        private void ChooseUpgrade()
        {
            GetTree().Paused = true;
            menuUpgrade.ChooseUpgrade();
        }

        private void Upgrade(int option)
        {
            joueur.Upgrade(option);
            GetTree().Paused = false;
        }

        private void QUITTER()
        {
            GetTree().Quit();
        }
    }
}
