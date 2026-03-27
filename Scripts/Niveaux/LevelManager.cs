//LevelManager.cs
using System;
using System.Linq;
using Godot;
using SpaceZombie.Enemies;
using SpaceZombie.Events;
using SpaceZombie.Niveaux.Configs;
using SpaceZombie.Niveaux.Configs.V;
using SpaceZombie.Joueurs;
using System.Text.Encodings.Web;
using SpaceZombie.Ui;

namespace SpaceZombie.Niveaux
{
    public partial class LevelManager : Control
    {
        private int nbEnemy;
        private NiveauCreatorManager ncr;
        private GameDataIterator gdi;
        [Export] private Joueur joueur;
        [Export] private ProchainNiveauUi prochainNiveauUi;
        [Export] private ZombiesSpawn zombiesSpawn;
        [Export] private EnemiesSpawns spawns;
        private EnemyAttackManager enemyAttackManager;
        [Export] private UpgradeLoader upgradeLoader;
        private PackedScene bossLoader;
        [Export] public int stage;
        [Export] public int level;

        public override void _Ready()
        {
            enemyAttackManager = new EnemyAttackManager(this);
            var gd = ExempleDeserialisation.Deserialize();
            gdi = new GameDataIterator(gd, stage, level);
            SetNiveau(gdi.currentStage, gdi.currentLevel);
            ncr = new NiveauCreatorManager(gd);
            bossLoader = (PackedScene)ResourceLoader.Load($"res://Prefabs/boss.tscn");
            CallDeferred(nameof(Initialize));
        }

        private void Initialize()
        {
            GameEvents.Instance.EnemyDied += OnEnemyDied;
            GameEvents.Instance.EndLevel += ChangerNiveauLogic;
            upgradeLoader.InitialiseAreaPlaySize(Size);
            zombiesSpawn.Initialize();
            spawns.Initialize(Size, joueur);
            prochainNiveauUi.timer.Timeout += WaitForTimerToFinish;
            //ChangerNiveauLogic("1");
        }

        public void ChangerNiveauLogic(string level)
        {
            prochainNiveauUi.ProcessMode = ProcessModeEnum.Always;
            prochainNiveauUi.UpdateLabelTexte(level);
            prochainNiveauUi.Visible = true;
            prochainNiveauUi.StartTimer();
        }

        private void WaitForTimerToFinish()
        {
            //CreerNiveau();
            spawns.SpawnLevel();
            prochainNiveauUi.ProcessMode = ProcessModeEnum.Disabled;
            prochainNiveauUi.Visible = false;
        }

        private void OnEnemyDied()
        {
            nbEnemy -= 1;
            if (nbEnemy <= 0)
            {
                if (!gdi.HasNext())
                {
                    zombiesSpawn.ProcessMode = ProcessModeEnum.Disabled;
                    ShowBoss();
                }
                else
                {
                    level++;
                    var stageLevelLocal = gdi.Next();
                    stage = stageLevelLocal.Item1;
                    level = stageLevelLocal.Item2;
                    zombiesSpawn.ProcessMode = ProcessModeEnum.Disabled;
                    enemyAttackManager.StopFire();
                    GameEvents.Instance.EmitSignal(GameEvents.SignalName.EndLevel, level.ToString());
                }
            }
            else
            {
                upgradeLoader.EnemyDied(nbEnemy);
            }
        }

        public void SetNiveau(int stage, int level)
        {
            this.stage = stage;
            this.level = level;
        }
        public void CreerNiveau()
        {
            CreerNiveau(stage, level);
            zombiesSpawn.ProcessMode = ProcessModeEnum.Inherit;
            enemyAttackManager.StartFire();
        }

        private void CreerNiveau(int stage, int niveau)
        {
            var niveauSettings = ncr.CreerNiveau(stage, niveau);
            nbEnemy = CountNumberOfEnemy(niveauSettings);
            ncr.AppliquerNiveau(zombiesSpawn, niveauSettings);
            enemyAttackManager.SetEnemyForLevel(zombiesSpawn.GetAllEnemy(new ObtainEnemyObjectService()).ToList<Node2D>(), niveauSettings);
            upgradeLoader.UpdateUpgradeApparition(nbEnemy);
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

        private void ShowBoss()
        {
            Boss.Boss boss = (Boss.Boss)bossLoader.Instantiate();
            boss.Position = new Vector2(Size.X / 2, 0);
            boss.joueur = joueur;
            CallDeferred(nameof(AddBoss), boss);
        }

        private void AddBoss(Boss.Boss boss)
        {
            AddChild(boss);
            boss.Foward();
        }

        public override void _ExitTree()
        {
            base._ExitTree();
            GameEvents.Instance.EnemyDied -= OnEnemyDied;
        }
    }

    public class GameDataIterator
    {
        private GameData gd;
        public int currentLevel { get; private set; }
        public int currentStage { get; private set; }

        public GameDataIterator(GameData gameData, int stage, int level)
        {
            gd = gameData;
            SetLevel(stage, level);
        }

        private void SetLevel(int stage, int level)
        {
            if (stage < 0)
            {
                currentStage = 0;
            }
            else if (stage >= gd.Stages.Count)
            {
                currentStage = gd.Stages.Count - 1;
            }
            else
            {
                currentStage = stage;
            }

            if (level < 0)
            {
                currentLevel = 0;
            }
            else if (level >= gd.Stages[currentStage].Levels.Count)
            {
                currentLevel = gd.Stages[stage].Levels.Count - 1;
            }
            else
            {
                currentLevel = level;
            }
        }

        public bool HasNext()
        {
            int nextIndex = currentLevel + 1;
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
            int nextIndex = currentLevel + 1;
            if (gd.Stages[currentStage].Levels.Count > nextIndex)
            {
                currentLevel = nextIndex;
                return (currentStage, nextIndex);
            }
            else if (currentStage + 1 < gd.Stages.Count)
            {
                int nextStage = currentStage + 1;
                currentLevel = 0;
                return (nextStage, currentLevel);
            }
            return (-1, -1);
        }
    }
}
