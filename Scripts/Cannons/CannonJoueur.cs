using Godot;
using SpaceZombie.Ammunitions;
using SpaceZombie.Cannons;
using SpaceZombie.Events;
using System;
using System.Collections.Generic;
namespace SpaceZombie.Cannons
{
	public partial class CannonJoueur : Node2D
	{
		[Export] private int maxCannonAuMilieu = 4;
		[Export] private int espaceEntreCannon = 40;
		[Export] private float angleCannonEnPercentageDePi = 0.02f;
		[Export] public float tempsRelaod = 1.0f;
        [Export] private AudioStreamPlayer sonFire;
        private Timer reloadTimer;
		private PackedScene cannonPrefab;
		private Projectile projectile;
		private int level = 1;
		private int maxLevel = 10;
		private int cannonMilieuPair;
		private int cannonMilieuImpair;

		private List<CannonObjet> cannons = new List<CannonObjet>();
		IResetEtatNotifier resetEtatNotifier;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			cannonPrefab = GD.Load<PackedScene>("res://Prefabs/cannon.tscn");
			projectile = new Projectile(1, 250f, false);
			reloadTimer = new Timer();
			AddChild(reloadTimer);
			reloadTimer.WaitTime = tempsRelaod;
			reloadTimer.OneShot = true;
			reloadTimer.Timeout += OnReloadTimeout;
			Rotation = Vector2.Up.Angle();
			InitializeMiddleCannon();
			level = 4;
		}

		private void InitializeMiddleCannon()
		{
			if (maxCannonAuMilieu % 2 == 0)
			{
				cannonMilieuPair = maxCannonAuMilieu;
				cannonMilieuImpair = maxCannonAuMilieu - 1;
			}
			else
			{
				cannonMilieuPair = maxCannonAuMilieu - 1;
				cannonMilieuImpair = maxCannonAuMilieu;
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
                foreach (CannonObjet cannon in cannons)
				{
					cannon.Fire(GetGlobalDirection(cannon.Rotation));
				}
				reloadTimer.Start();
			}

		}
		private Vector2 GetGlobalDirection(float rotation)
		{
			//return Vector2.Up.Normalized();
			return new Vector2(Mathf.Cos(rotation), Mathf.Sin(rotation)).Normalized();
		}

		public void LevelUp()
		{
			if (level < maxLevel)
			{
				LevelUpTempsReload();
				LevelUpVitesse();
				CallDeferred(nameof(LevelUpCannon));
			}
		}

		private void LevelUpTempsReload()
		{
			tempsRelaod -= 0.1f;
			reloadTimer.WaitTime = tempsRelaod;
		}

		private void LevelUpVitesse()
		{
			projectile.upgradeVitesse(0.1f);
		}

		private void LevelUpCannon()
		{
			AddCannon();
			if (cannons.Count <= maxCannonAuMilieu)
			{
				AligneCannonDroit(cannons.Count);
			}
			else
			{
				int aligneDroitCount = (cannons.Count % 2 == 0) ? cannonMilieuPair : cannonMilieuImpair;
				int rightPosition = AligneCannonDroit(aligneDroitCount);
				AligneCannonAngle(aligneDroitCount, rightPosition);
			}
		}

		private int AligneCannonDroit(int total)
		{
			total -= 1;
			int rightPosition = total * espaceEntreCannon / 2;
			for (int i = 0; i <= total; i++)
			{
				cannons[i].Position = new Vector2(0, rightPosition);
				cannons[i].Rotation = Vector2.Up.Angle();
				rightPosition -= espaceEntreCannon;
			}
			return -rightPosition;
		}

		private void AligneCannonAngle(int alreadyDone, int position)
		{
			float rotation = angleCannonEnPercentageDePi;
			for (int i = alreadyDone; i < cannons.Count; i += 2)
			{
				cannons[i].Position = new Vector2(0, position);
				cannons[i].Rotation = Vector2.Up.Angle() + (Mathf.Pi * rotation);
				cannons[i + 1].Position = new Vector2(0, -position);
				cannons[i + 1].Rotation = Vector2.Up.Angle() - (Mathf.Pi * rotation);
				rotation += rotation;
				position += espaceEntreCannon;
			}
		}

		private void AddCannon()
		{
			var cannon = cannonPrefab.Instantiate<CannonObjet>();
			AddChild(cannon);
			cannon.Initialize(0, "projectile_joueur", projectile, resetEtatNotifier);
			cannon.Rotation = Vector2.Up.Angle();
			cannons.Add(cannon);
		}

		public void Initialize(IResetEtatNotifier resetEtatNotifier)
		{
			this.resetEtatNotifier = resetEtatNotifier;
			InitializeLevel();
		}

		private void InitializeLevel()
		{
			AddCannon();
			for (int i = 1; i < level && i < maxLevel; i++)
			{
				LevelUpCannon();
			}
		}

		public void StopReloadTimer()
		{
			reloadTimer.Stop();
		}

		// Reload timer timeout handler
		private void OnReloadTimeout()
		{
			// You can add any additional logic for what happens when the reload timer finishes
			//GD.Print("Reload finished. You can fire again!");
		}

	}
}

