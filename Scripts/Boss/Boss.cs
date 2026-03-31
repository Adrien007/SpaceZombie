using Godot;
using SpaceZombie.Ammunitions;
using SpaceZombie.Canons;
using SpaceZombie.Events;
using SpaceZombie.Joueurs;
using SpaceZombie.Utilitaires;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;

namespace SpaceZombie.Boss
{
    public partial class Boss : Path2D
    {
        private int hp = 300;
        private PathFollow2D pathFollow2D;
        private AnimationPlayer animation;
        private BossAttacks bossAttacks;
        private RandomNumberGenerator random = new RandomNumberGenerator();
        private List<(Action<int>, int)> actions = new List<(Action<int>, int)>();
        private Timer nextActionTimer = new Timer();
        private int nextActionIndex = 0;
        private bool died = false;
        private ProgressBar bossHealthBar;
        [Export] public Joueur joueur;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            pathFollow2D = GetNode<PathFollow2D>("PathFollow2D");
            animation = GetNode<AnimationPlayer>("AnimationPlayer");
            bossAttacks = (BossAttacks)FindChild("Attacks");
            bossAttacks.Initialize(joueur);
            bossAttacks.RegisterOnAttackEnd(DoNextAction);
            nextActionTimer.Timeout += DoNextAction;
            nextActionTimer.OneShot = true;
            AddChild(nextActionTimer);

            animation.AnimationFinished += AnimationFinished;
            bossHealthBar = GetNode<ProgressBar>("/root/BossScene/BossUI/ProgressBar");
            bossHealthBar.MaxValue = hp;
            bossHealthBar.Value = hp;
            CallDeferred(nameof(Foward));
            
            var introPlusBossFight = GetNode<AudioStreamPlayer>("IntroPlusBossFight");
            introPlusBossFight.Play(21.50f);
        }

        private void Foward()
        {
            animation.Play("Foward");
        }

        private void AnimationFinished(StringName animName)
        {
            animation.AnimationFinished -= AnimationFinished;
            Attack();
        }

        private void Attack()
        {
            actions.Add((StartMovement, 3));
            actions.Add((StartFireBullets, 5));
            actions.Add((StopFireBullets, 0));
            actions.Add((StopMovement, 0));
            actions.Add((bossAttacks.FireRayLazers, 2));
            actions.Add((StartMovement, 1));
            actions.Add((StartFireBullets, 5));
            actions.Add((StopMovement, 0));
            actions.Add((StopFireBullets, 0));
            actions.Add((bossAttacks.FireZoneLazer, 2));

            DoNextAction();
        }
        

        private void StartMovement(int seconds)
        {
            animation.Play("Move");
            StartNextActionTimer(seconds);
        }

        private void StopMovement(int seconds)
        {
            animation.Pause();
            StartNextActionTimer(seconds);
        }
    
        private void StartFireBullets(int seconds)
        {
            bossAttacks.FireBullets();
            StartNextActionTimer(seconds);
        }

        private void StopFireBullets(int seconds)
        {
            bossAttacks.StopFireBullets();
            StartNextActionTimer(seconds);
        }

        private void DoNextAction()
        {
            if (nextActionIndex >= actions.Count)
            {
                nextActionIndex = 0;
            }
            (Action<int> method, int parameter) action = actions[nextActionIndex];
            action.method(action.parameter);
            nextActionIndex += 1;
        }

        private void StartNextActionTimer(int seconds)
        {
            if (seconds > 0)
            {
                nextActionTimer.Stop();
                nextActionTimer.Start(seconds);
            }
            else
            {
                CallDeferred(nameof(DoNextAction));
            }
        }

        public void TakeDamage(int damage)
        {
            hp -= damage;
            bossHealthBar.Value = hp;
            if (hp <= 0 && !died)
            {
                died = true;
                StopAttacks();
                animation.Play("Die");
                animation.AnimationFinished += (StringName animName) => CallDeferred(nameof(Disable));
            }
        }

        private void StopAttacks()
        {
            nextActionTimer.Stop();
            bossAttacks.Stop();
        }

        private async void Disable()
        {
            Visible = false;
            QueueFree();
            GameEvents.Instance.EmitSignal(GameEvents.SignalName.ShowEndScreen);

        }
    }
}
