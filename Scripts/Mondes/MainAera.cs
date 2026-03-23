//MainAera.cs
using Godot;
using SpaceZombie.Enemies;
using SpaceZombie.Events;
using SpaceZombie.Joueurs;
using SpaceZombie.Niveaux;
using SpaceZombie.Ui;
using System;

namespace SpaceZombie.Mondes.Utilitaires
{
    public partial class MainAera : Control
    {
        [Export] private Joueur joueur;
        [Export] private ZombiesSpawn zombiesSpawn;
        [Export] private ProchainNiveauUi prochainNiveauUi;
        [Export] public UpgradeLoader upgradeLoader;
        [Export] private MenuUpgrade menuUpgrade;

        public override void _Ready()
        {
            menuUpgrade.Upgrade += Upgrade;
            GameEvents.Instance.ChooseUpgrade += ChooseUpgrade;
            GameEvents.Instance.PlayerDied += QUITTER;
        }
        public void Initialiser()
        {
            joueur.Initialize(GetRect());
            zombiesSpawn.Initialize(GetRect());
            upgradeLoader.InitialiseAreaPlaySize(Size);

            EnemyFireOptions enemyFireOptions = new EnemyFireOptions(new Random(1));
            EnemyFireService enemyFireService = new EnemyFireService(enemyFireOptions);
            EnemyAttackManager enemyAttackManager = new EnemyAttackManager(this, enemyFireService);

            var lm = new LevelManager(zombiesSpawn, enemyFireOptions, enemyAttackManager, upgradeLoader);
            lm.SetNiveau(0, 1);

            var ltm = new LevelTransitionManager(prochainNiveauUi, lm);
            ltm.ChangerNiveauLogic();
        }

        private void ChooseUpgrade()
        {
            GetTree().Paused = true;
            menuUpgrade.ChooseUpgrade(joueur);
        }

        private void Upgrade(int option)
        {
            joueur.Upgrade((UpgradeOptions)option);
            GetTree().Paused = false;
        }

        private void QUITTER()
        {
            GetTree().Quit();
        }
    }
}
