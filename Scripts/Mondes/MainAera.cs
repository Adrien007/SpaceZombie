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
        [Export] private ProchainNiveauUi prochainNiveauUi;
        [Export] private MenuUpgrade menuUpgrade;
        [Export] private LevelManager levelManager;

        public override void _Ready()
        {
            menuUpgrade.Upgrade += Upgrade;
            GameEvents.Instance.ChooseUpgrade += ChooseUpgrade;
            GameEvents.Instance.ShowEndScreen += ShowEndScreen;
        }
        public void Initialiser()
        {
            joueur.Initialize(GetRect());
            var ltm = new LevelTransitionManager(prochainNiveauUi, levelManager);
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

        private void ShowEndScreen()
        {
            PackedScene endScreenLoader = (PackedScene)ResourceLoader.Load($"res://Scenes/end_screen.tscn");
            EndScreen endScreen = endScreenLoader.Instantiate<EndScreen>();
            endScreen.score = joueur.jState.Score;
            GetTree().Root.AddChild(endScreen);
        }
    }
}
