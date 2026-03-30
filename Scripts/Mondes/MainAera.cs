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
        [Export] public Joueur joueur;
        [Export] private ProchainNiveauUi prochainNiveauUi;
        [Export] private MenuUpgrade menuUpgrade;
        [Export] private LevelManager levelManager;

        public override void _Ready()
        {
            menuUpgrade.Upgrade += Upgrade;
            GameEvents.Instance.ChooseUpgrade += ChooseUpgrade;
        }
        public void Initialiser()
        {
            joueur.Initialize(GetRect());
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
    }
}
