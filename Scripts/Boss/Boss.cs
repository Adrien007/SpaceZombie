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
    public partial class Boss : Control
    {
        [Export] private int hp = 1000;
        [Export] private ProgressBar bossHealthBar;
        [Export] private PathFollow2D pathFollow2D;
        [Export] private BossAttacks bossAttacks;
        [Export] private AudioStreamPlayer takeDamageSound;
        [Export] private AudioStreamPlayer introPlusBossFight;
        [Export] Damagable area;
        [Export] Sprite2D sprite;
        private ShaderMaterial damageShader;
        [Export] private AnimationPlayer animation;
        [Export] public Joueur joueur;
        private RandomNumberGenerator random = new RandomNumberGenerator();
        private (Action<int>, int)[] actions;
        private (Action<int>, int)[] phase1;
        private (Action<int>, int)[] phase2;
        private (Action<int>, int)[] phase3;
        private Timer nextActionTimer = new Timer();
        private int nextActionIndex = 0;
        private int phase2Trigger;
        private int phase3Trigger;
        private Action nextPhase;
        private bool died = false;
        private float moveSpeed = 1;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            area.take_damage += TakeDamage;
            damageShader = (ShaderMaterial)sprite.Material;
            bossAttacks.Initialize(joueur);
            bossAttacks.RegisterOnAttackEnd(DoNextAction);
            nextActionTimer.Timeout += DoNextAction;
            nextActionTimer.OneShot = true;
            AddChild(nextActionTimer);

            animation.AnimationFinished += AnimationFinished;
            bossHealthBar.MaxValue = hp;
            bossHealthBar.Value = hp;
            phase2Trigger = (int)(hp * 0.66);
            phase3Trigger = (int)(hp * 0.33);
            InitAttackPhases();
            bossAttacks.Phase3();
        }

        public void Foward()
        {

            introPlusBossFight.Play(21.50f);
            animation.Play("Foward");
        }

        private void AnimationFinished(StringName animName)
        {
            animation.AnimationFinished -= AnimationFinished;
            actions = phase3;
            animation.Play("Move", moveSpeed);
            DoNextAction();
        }

        private void StartMovement(int seconds)
        {
            animation.Play("Move", moveSpeed);
            StartNextActionTimer(seconds);
        }

        private void StopMovement(int seconds)
        {
            animation.Pause();
            StartNextActionTimer(seconds);
        }

        private void StartFireAllBullets(int seconds)
        {
            bossAttacks.FireAllBullets();
            StartNextActionTimer(seconds);
        }

        private void StartFireOneBulletAtTimes(int seconds)
        {
            bossAttacks.FireOneBulletAtTime();
            StartNextActionTimer(seconds);
        }

        private void StopFireBullets(int seconds)
        {
            bossAttacks.StopFireBullets();
            StartNextActionTimer(seconds);
        }

        private void DoNextAction()
        {
            if (nextPhase != null)
            {
                nextPhase();
                nextPhase = null;
                nextActionIndex = 0;
            }
            else if (nextActionIndex >= actions.Length)
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
            ShowHitShader();
            takeDamageSound.Play();
            if (hp <= 0 && !died)
            {
                died = true;
                StopAttacks();
                animation.Play("Die");
                CallDeferred(nameof(Disable));
                GameEvents.Instance.EmitSignal(GameEvents.SignalName.UpdateScore, 15978, this.GlobalPosition);
            }
            else if (hp <= phase3Trigger && actions != phase3)
            {
                //nextPhase = Phase3;
            }
            else if (hp <= phase2Trigger && actions != phase2)
            {
                //nextPhase = Phase2;
            }
        }

        private void Phase2()
        {
            actions = phase2;
            moveSpeed = 2f;
            animation.Pause();
            bossAttacks.Phase2();
        }

        private void Phase3()
        {
            actions = phase3;
            moveSpeed = 4f;
            animation.Pause();
            bossAttacks.Phase3();
        }

        private void ShowHitShader()
        {
            damageShader.SetShaderParameter("hit_amount", 1.0f);
            var tween = CreateTween();
            tween.TweenMethod(
            Callable.From<float>(value => damageShader.SetShaderParameter("hit_amount", value)),
            0.4f,
            0.0f,
            0.4f
        ).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Quad);

        }

        private void StopAttacks()
        {
            nextActionTimer.Stop();
            bossAttacks.Stop();
        }

        private async void Disable()
        {
            GameEvents.Instance.EmitSignal(GameEvents.SignalName.ShowEndScreen);
            QueueFree();
        }

        private void InitAttackPhases()
        {
            phase1 = [
                (StartMovement, 5),
                //(StartFireBullets, 5),
                (StopFireBullets, 0),
                (StopMovement, 0),
                (bossAttacks.FireRayLazers, 2),
                (StartMovement, 1),
                //(StartFireBullets, 5),
                (StopMovement, 0),
                (StopFireBullets, 0),
                (bossAttacks.FireZoneLazer, 2),
            ];

            phase2 = [
                (bossAttacks.FireZoneLazer, 2),
                (bossAttacks.FireRayLazers, 2),
                (StartMovement, 3),
                //(StartFireBullets, 5),
                (StopFireBullets, 0),
                (StopMovement, 0),
                (bossAttacks.FireRayLazers, 2),
                (StartMovement, 1),
                //(StartFireBullets, 5),
                (StopMovement, 0),
                (StopFireBullets, 0),
                (bossAttacks.FireZoneLazer, 2),
            ];

            phase3 = [
                (StartFireAllBullets, 5),
                (StopFireBullets, 0),
                (bossAttacks.FireRayLazers, 1),
                (bossAttacks.FireZoneLazer, 1),
                (StartFireOneBulletAtTimes, 5),
                (bossAttacks.FireRayLazers, 2),
                (StopFireBullets, 0),
                (bossAttacks.FireZoneLazer, 2),
            ];
        }
    }
}
