using Godot;
using SpaceZombie.Events;
using SpaceZombie.Joueurs;
using System;

namespace SpaceZombie.Mondes.Utilitaires
{
    public partial class BossScene : Node2D
    {
        [Export] Boss.Boss boss;
        [Export] Joueur joueur;
        public override void _Ready()
        {
            joueur.Initialize(GetViewportRect());
            Callable.From(boss.Foward).CallDeferred();
        }
    }
}
