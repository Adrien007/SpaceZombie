//LevelManager.cs
using System;
using SpaceZombie.Enemies;
using SpaceZombie.Events;
using SpaceZombie.Niveaux.Configs;
using SpaceZombie.Niveaux.Configs.V;

namespace SpaceZombie.Niveaux
{
	public class LevelManager
	{
		private int stage = 0;
		private int globalLevel = 0;
		private int levelLocal = 0;
		private NiveauCreatorManager ncr;
		private GameDataIterator gdi;
		private IEndLevelSystem endLevelSystem;
		private INbEnemy endLevelNbEnemy;
		private ZombiesSpawn zombiesSpawn;

		public int Stage { get => stage; }
		public int GlobalLevel { get => globalLevel; }

		public LevelManager(IEndLevelSystem endLevelSystem, INbEnemy endLevelNbEnemy, ZombiesSpawn zombiesSpawn)
		{
			var gd = ExempleDeserialisation.Deserialize();
			gdi = new GameDataIterator(gd);
			ncr = new NiveauCreatorManager(gd);
			this.endLevelSystem = endLevelSystem;
			this.endLevelNbEnemy = endLevelNbEnemy;
			this.zombiesSpawn = zombiesSpawn;

			this.endLevelSystem.EndLevelSignal += OnEndLevelSignal;
		}

		private void OnEndLevelSignal()
		{
			if (!gdi.HasNext())
			{
				throw new InvalidOperationException("No more levels available.");
			}
			globalLevel++;
			var stageLevelLocal = gdi.Next();
			stage = stageLevelLocal.Item1;
			levelLocal = stageLevelLocal.Item2;
		}

		public void SetNiveau(int stage, int levelLocal)
		{
            this.stage = stage;
            this.levelLocal = levelLocal;
            this.globalLevel = levelLocal;
		}
		public void DemarrerNiveau(int stage, int globalLevel)
		{
			CreerNiveau(stage, globalLevel);
		}
		public void CreerNiveau()
		{
			CreerNiveau(stage, levelLocal);
		}
		private void CreerNiveau(int stage, int niveau)
		{
			var niveauSettings = ncr.CreerNiveau(stage, niveau);
			endLevelNbEnemy.NbEnemy = CountNumberOfEnemy(niveauSettings);
			ncr.AppliquerNiveau(zombiesSpawn, niveauSettings);
		}

		private int CountNumberOfEnemy(NiveauZombiesSpawnSettings niveau)
		{
			int nbEnemies = 0;
			foreach (var ligne in niveau.LigneSettings)
			{
				foreach (var slot in ligne.EnemySlotSettings)
				{
					if (slot.enemyObjSettings.IsVisible)
					{
						nbEnemies++;
					}
				}
			}
			return nbEnemies;
		}
	}

	public class GameDataIterator
	{
		private GameData gd;
		private int currentLocalLevel = 0;
		private int currentStage = 0;

		public GameDataIterator(GameData gameData)
		{
			gd = gameData;
		}

		public bool HasNext()
		{
			int nextIndex = currentLocalLevel + 1;
			int cc = gd.Stages[currentStage].Levels.Count;
			if (gd.Stages[currentStage].Levels.Count > nextIndex)
			{
				return true;
			}
			else if (currentStage + 1 < gd.Stages.Count)
			{
				return true;
			}
			return false;
		}

		public (int, int) Next()
		{
			int nextIndex = currentLocalLevel + 1;
			if (gd.Stages[currentStage].Levels.Count > nextIndex)
			{
				currentLocalLevel = nextIndex;
				return (currentStage, nextIndex);
			}
			else if (currentStage + 1 < gd.Stages.Count)
			{
				int nextStage = currentStage + 1;
				currentLocalLevel = 0;
				return (nextStage, currentLocalLevel);
			}
			return (-1, -1);
		}
	}
}
