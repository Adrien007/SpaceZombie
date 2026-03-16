using Godot;
using SpaceZombie.Events;
using SpaceZombie.Joueurs;
using System;

namespace SpaceZombie.Mondes.Utilitaires
{
    public partial class BossScene : Node
    {
        public override void _Ready()
        {
            CallDeferred(nameof(Initialize));
        }

        private void Initialize()
        {
            GameBounderies gameBounderies = GetNode<GameBounderies>("GameBounderies");
            Joueur joueur = GetNode<Joueur>("MainAera/Joueur");

            joueur.InitialiserSize(gameBounderies.screenSize);
            joueur.InitialiserPosition(Vector2.Zero);
            joueur.Initialize(3, new ResetEtatManager());
        }
    }
}

