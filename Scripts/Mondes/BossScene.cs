using Godot;
using SpaceZombie.Events;
using SpaceZombie.Joueurs;
using System;

namespace SpaceZombie.Mondes.Utilitaires
{
    public partial class BossScene : Node2D
    {
        public override void _Ready()
        {
            Joueur joueur = GetNode<Joueur>("MainAera/Joueur");
            joueur.Initialize(GetViewportRect());
        }
    }
}

