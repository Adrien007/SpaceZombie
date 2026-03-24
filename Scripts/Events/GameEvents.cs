using System;
using Godot;
using SpaceZombie.Enemies;
namespace SpaceZombie.Events
{
    public partial class GameEvents : Node
    {
        public static GameEvents Instance { get; private set; }

        public override void _Ready()
        {
            Instance = this;

            if (Instance != this)
            {
                GD.PrintErr("Multiple GameEvents detected! Removing duplicate.");
                QueueFree();
            }
        }


        [Signal]
        public delegate void EnemyDiedEventHandler(EnemyObjet enemy);

        [Signal]
        public delegate void ChooseUpgradeEventHandler();

        [Signal]
        public delegate void EndLevelEventHandler();

        [Signal]
        public delegate void PlayerScoreUpdatedEventHandler(int playerScore);

        [Signal]
        public delegate void PlayerHealthUpdatedEventHandler(int playerHealth);

        [Signal]
        public delegate void ShowEndScreenEventHandler();
    }
}

