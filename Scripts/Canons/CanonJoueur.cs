using Godot;
using SpaceZombie.Ammunitions;
using SpaceZombie.Canons;
using SpaceZombie.Events;
using System;
using System.Collections.Generic;
namespace SpaceZombie.Canons
{
    public partial class CanonJoueur : Node2D
    {
        [Export] private AudioStreamPlayer sonFire;
        [Export] public int initialDamage = 1;
        [Export] public float initialReloadSpeedInSeconds = 1.0f;
        [Export] public float initialProjectileSpeed = 250f;
        [Export] public float projectileAngleInPiPercentage = 0.01f;
        [Export] public int maxProjectileStraightInMiddle = 2;
        [Export] public int spaceBetweenProjectile = 40;
        [Export] public float upgradeAttackSpeed = 0.2f;
        private Timer reloadTimer;
        private PackedScene canonPrefab;
        private Projectile projectile;
        private int canonMilieuPair;
        private int canonMilieuImpair;

        private List<CanonObjet> canons = new List<CanonObjet>();

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            canonPrefab = GD.Load<PackedScene>("res://Prefabs/canon.tscn");
            projectile = new Projectile(initialDamage, initialProjectileSpeed);
            reloadTimer = new Timer();
            AddChild(reloadTimer);
            reloadTimer.WaitTime = initialReloadSpeedInSeconds;
            reloadTimer.OneShot = true;
            Rotation = Vector2.Up.Angle();
            InitializeMiddleCanon();
        }

        private void InitializeMiddleCanon()
        {
            if (maxProjectileStraightInMiddle % 2 == 0)
            {
                canonMilieuPair = maxProjectileStraightInMiddle;
                canonMilieuImpair = maxProjectileStraightInMiddle - 1;
            }
            else
            {
                canonMilieuPair = maxProjectileStraightInMiddle - 1;
                canonMilieuImpair = maxProjectileStraightInMiddle;
            }
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta)
        {
        }

        public void Fire()
        {
            if (reloadTimer.TimeLeft == 0)
            {
                sonFire.Play(0.79f);
                foreach (CanonObjet canon in canons)
                {
                    canon.Fire(GetGlobalDirection(canon.Rotation));
                }
                reloadTimer.Start();
            }

        }
        private Vector2 GetGlobalDirection(float rotation)
        {
            //return Vector2.Up.Normalized();
            return new Vector2(Mathf.Cos(rotation), Mathf.Sin(rotation)).Normalized();
        }

        public void UpgradeDamage()
        {
            projectile.UpgradeDamage();
        }

        public void UpgradeVitesse()
        {
            initialReloadSpeedInSeconds -= initialReloadSpeedInSeconds * upgradeAttackSpeed;
            reloadTimer.WaitTime = initialReloadSpeedInSeconds;
            //projectile.UpgradeVitesse(upgradeAttackSpeed);
        }

        public void UpgradeTraverse()
        {
            projectile.UpgradeTraverse();
        }

        public void UpgradeCanons()
        {
            AddCanon();
            if (canons.Count <= maxProjectileStraightInMiddle)
            {
                AligneCanonDroit(canons.Count);
            }
            else
            {
                int aligneDroitCount = (canons.Count % 2 == 0) ? canonMilieuPair : canonMilieuImpair;
                int rightPosition = AligneCanonDroit(aligneDroitCount);
                AligneCanonAngle(aligneDroitCount, rightPosition);
            }
        }

        private int AligneCanonDroit(int total)
        {
            total -= 1;
            int rightPosition = total * spaceBetweenProjectile / 2;
            for (int i = 0; i <= total; i++)
            {
                canons[i].Position = new Vector2(0, rightPosition);
                canons[i].Rotation = Vector2.Up.Angle();
                rightPosition -= spaceBetweenProjectile;
            }
            return -rightPosition;
        }

        private void AligneCanonAngle(int alreadyDone, int position)
        {
            float rotation = Mathf.Pi * projectileAngleInPiPercentage;
            for (int i = alreadyDone; i < canons.Count; i += 2)
            {
                canons[i].Position = new Vector2(0, position);
                canons[i].Rotation = Vector2.Up.Angle() + rotation;
                canons[i + 1].Position = new Vector2(0, -position);
                canons[i + 1].Rotation = Vector2.Up.Angle() - rotation;
                rotation += rotation;
                position += spaceBetweenProjectile;
            }
        }

        private void AddCanon()
        {
            var canon = canonPrefab.Instantiate<CanonObjet>();
            AddChild(canon);
            canon.Initialize("projectile_joueur", projectile);
            canon.Rotation = Vector2.Up.Angle();
            canons.Add(canon);
        }

        public void Initialize()
        {
            AddCanon();
        }
    }
}

