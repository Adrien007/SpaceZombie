using Godot;
using SpaceZombie.Ammunitions;
using SpaceZombie.Canons;
using SpaceZombie.Events;
using SpaceZombie.Joueurs;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;

namespace SpaceZombie.Boss
{
    public partial class Boss : Path2D, IResetEtatNotifier
    {
        private int hp = 300;
        private Area2D area;
        private PathFollow2D pathFollow2D;
        private AnimationPlayer animation;
        private BossAttacks bossAttacks;
        private RandomNumberGenerator random = new RandomNumberGenerator();
        private List<(Action<int>, int)> actions = new List<(Action<int>, int)>();
        private Timer nextActionTimer = new Timer();
        private int nextActionIndex = 0;
        [Export] public Joueur joueur;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            area = (Area2D)FindChild("Area2D");
            pathFollow2D = GetNode<PathFollow2D>("PathFollow2D");
            area.AreaEntered += OnAreaEntered;
            animation = GetNode<AnimationPlayer>("AnimationPlayer");
            bossAttacks = (BossAttacks)FindChild("Attacks");
            bossAttacks.Initialize(joueur);
            bossAttacks.RegisterOnAttackEnd(DoNextAction);
            nextActionTimer.Timeout += DoNextAction;
            nextActionTimer.OneShot = true;
            AddChild(nextActionTimer);
            CallDeferred(nameof(InitializeActions));
        }

        private void InitializeActions()
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

        private void OnAreaEntered(Area2D area)
        {
            if (area.GetParent() is ProjectileObjet projectile)
            {
                hp -= projectile.Projectile.Damage;
                Callable.From(projectile.Disable).CallDeferred();

                if (hp <= 0)
                {
                    CallDeferred(nameof(DisableCallDefered));
                }
            }
        }

        private void DisableCallDefered()
        {
            area.Monitoring = false;
            Visible = false;
        }

        public void Register(IResetEtatObserver observer)
        {
            //TODO Remove IResetEtatObserver
        }

        public void Unregister(IResetEtatObserver observer)
        {
            //TODO Remove IResetEtatObserver
        }
    }

    class BossAction
    {
        private Action<int> action;
        private int parameter;
        BossAction(Action<int> action, int parameter = 0)
        {
            this.action = action;
            this.parameter = parameter;
        }

        public void execute()
        {
            action(parameter);
        }
    }
}


