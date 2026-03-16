//LevelManager.cs
using System;
using System.Linq;
using SpaceZombie.Enemies;
using SpaceZombie.Events;
using SpaceZombie.Niveaux.Configs;
using SpaceZombie.Niveaux.Configs.V;

namespace SpaceZombie.Niveaux
{
    public class LevelManager
    {
        public delegate void FinCreationNiveauSignalEventHandler();
        public event FinCreationNiveauSignalEventHandler FinCreationNiveau;

        private int stage = 0;
        private int globalLevel = 0;
        private int levelLocal = 0;
        private int nbEnemy;
        private NiveauCreatorManager ncr;
        private GameDataIterator gdi;
        private ZombiesSpawn zombiesSpawn;
        private IEnemyFireOptionsSettings enemyFireAttackOption;
        private IEnemyAttackManagerSetEnemy enemyAttackManager;
        private InLigneSpawnerUtilitiesEventService spawnService;
        public int Stage { get => stage; }
        public int GlobalLevel { get => globalLevel; }

        public LevelManager(ZombiesSpawn zombiesSpawn,
                            IEnemyFireOptionsSettings enemyFireAttackOption, IEnemyAttackManagerSetEnemy enemyAttackManager)
        {
            var gd = ExempleDeserialisation.Deserialize();
            gdi = new GameDataIterator(gd);
            ncr = new NiveauCreatorManager(gd);
            spawnService = new InLigneSpawnerUtilitiesEventService();
            this.zombiesSpawn = zombiesSpawn;
            this.enemyFireAttackOption = enemyFireAttackOption;
            this.enemyAttackManager = enemyAttackManager;
            GameEvents.Instance.EnemyDied += OnEnemyDied;
        }

        private void OnEnemyDied(EnemyObjet enemy)
        {
            nbEnemy -= 1;
            if (nbEnemy <= 0)
            {
                if (!gdi.HasNext())
                {
                    throw new InvalidOperationException("No more levels available.");
                }
                globalLevel++;
                var stageLevelLocal = gdi.Next();
                stage = stageLevelLocal.Item1;
                levelLocal = stageLevelLocal.Item2;
                GameEvents.Instance.EmitSignal(nameof(GameEvents.EndLevel));
                GameEvents.Instance.EmitSignal(nameof(GameEvents.LevelUp));
            }
            else
            {
                if (!enemy.enemyFlagLogic.scoreGiven)
                {
                    enemy.enemyFlagLogic.scoreGiven = true; // Set the score given flag to true
                }
            }
            zombiesSpawn.DesactiverEnemyPasEnSandwitch(spawnService);
        }

        public void SetNiveau(int stage, int levelLocal)
        {
            this.stage = stage;
            this.levelLocal = levelLocal;
            this.globalLevel = levelLocal;
        }
        public void CreerNiveau()
        {
            CreerNiveau(stage, levelLocal);
            FinCreationNiveau.Invoke();
        }
        private void CreerNiveau(int stage, int niveau)
        {
            var niveauSettings = ncr.CreerNiveau(stage, niveau);
            nbEnemy = CountNumberOfEnemy(niveauSettings);
            ncr.AppliquerNiveau(zombiesSpawn, niveauSettings);
            enemyAttackManager.SetEnemyForLevel(zombiesSpawn.GetAllEnemy(new ObtainEnemyObjectService()).ToList<Godot.Node2D>());
            enemyFireAttackOption.NewSettings(niveauSettings.EnemyAttackSettings.NbProjectilePerAttack, niveauSettings.EnemyAttackSettings.FireRate);
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
