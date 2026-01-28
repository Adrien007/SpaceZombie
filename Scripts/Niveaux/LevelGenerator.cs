//LevelGenerator.cs
using System;
using System.Collections.Generic;
using Godot;
using SpaceZombie.Enemies.Configs;

namespace SpaceZombie.Niveaux.Configs.R
{
    public enum EnemyType
    {
        Type1 = 1,
        Type2,
        Type3,
        Type4,
        Type5,
        Type6
    }

    public class EnemyGen
    {
        public EnemyType Type { get; set; }
        public int Row { get; set; }  // 0..5
        public int Col { get; set; }  // 0..24
        public bool Visible { get; set; }
        public float Speed { get; set; }
        public int DirectionX { get; set; }  // +1 or -1
        public bool IsFixed { get; set; }
        public int Hp { get; set; }
    }

    public class LevelGenerator
    {
        private readonly int _baseSeed;
        private const int TotalRows = 6;
        private const int TotalCols = 25;
        private const int LevelsPerStage = 10;
        private const int BaseVisibleRows = 3;     // stage 1 shows 3 rows
        private const float GlobalMinSpeed = 30f;
        private const float GlobalMaxSpeed = 150f;
        private const float FormationChance = 0.15f; // 15% of rows get a "formation"

        public LevelGenerator(int baseSeed = 12345)
        {
            _baseSeed = baseSeed;
        }

        public List<EnemyGen> GenerateLevel(int levelNumber)
        {
            // 1. Seed a new Random for this level to guarantee determinism
            var rng = new Random(_baseSeed + levelNumber);

            // 2. Compute stage and intra-stage progress (0..1)
            int stage = ((levelNumber - 1) / LevelsPerStage) + 1;
            float levelProgress = ((levelNumber - 1) % LevelsPerStage) / (float)(LevelsPerStage - 1);

            // 3. How many rows should be “active” this stage?
            int rowsToShow = Math.Min(TotalRows, BaseVisibleRows + (stage - 1));

            // 4. How many enemy‐types are unlocked?
            int typesUnlocked = Math.Min(6, 1 + (int)Math.Floor(levelNumber / 5f));
            var availableTypes = new List<EnemyType>();
            for (int t = 1; t <= typesUnlocked; t++)
                availableTypes.Add((EnemyType)t);

            // 5. Decide per‐row enemy count by linear interp: min 4 → max 20
            int minEnemiesPerRow = 4 + (stage - 1) * 2;
            int maxEnemiesPerRow = 10 + (stage - 1) * 3;
            int enemiesThisRow(int rowIndex) =>
                Math.Clamp(
                    (int)Math.Round(
                        minEnemiesPerRow + (maxEnemiesPerRow - minEnemiesPerRow) * levelProgress
                    ),
                    1, TotalCols
                );

            var enemies = new List<EnemyGen>();

            /*// 6. Optional Formation helper
            void ApplyFormation(int topRow, int leftCol, string shape)
            {
                switch (shape)
                {
                    case "Triangle":
                        // Simple right‐angled triangle of size 3
                        for (int dr = 0; dr < 3; dr++)
                        {
                            for (int dc = 0; dc <= dr; dc++)
                            {
                                var e = CreerEnemy(topRow + dr, leftCol + dc, true, isRowFixed, dirX, speed, rng, availableTypes);
                                PlaceEnemy(enemies, e, true);
                                //PlaceEnemy(topRow + dr, leftCol + dc, visible: true);
                            }
                        }
                        break;
                    case "Block":
                        // 2×2 block
                        for (int dr = 0; dr < 2; dr++)
                        {
                            for (int dc = 0; dc < 2; dc++)
                            {
                                var e = CreerEnemy(topRow + dr, leftCol + dc, true, isRowFixed, dirX, speed, rng, availableTypes);
                                PlaceEnemy(enemies, e, true);
                                //PlaceEnemy(topRow + dr, leftCol + dc, visible: true);
                            }
                        }
                        break;
                        // add more as you like…
                }
            }*/

            // 7. Core placement loop
            for (int r = 0; r < rowsToShow; r++)
            {
                bool isRowFixed = rng.NextDouble() < 0.1;  // 10% of rows are “fixed”
                int dirX = rng.Next(2) == 0 ? +1 : -1;
                float speed = (float)(
                    GlobalMinSpeed + (GlobalMaxSpeed - GlobalMinSpeed) *
                    Math.Min(1.0, (stage - 1 + levelProgress) / 5.0)  // ramp up over 5 stages
                );

                // Maybe drop a formation on this row?
                if (rng.NextDouble() < FormationChance)
                {
                    int leftCol = rng.Next(0, TotalCols - 3);
                    ApplyFormation(r, leftCol, rng.NextDouble() < 0.5 ? "Triangle" : "Block", enemies, isRowFixed, dirX, speed, rng, availableTypes);
                    continue;
                }

                // Otherwise fill by count + invisible placeholders
                int count = enemiesThisRow(r);
                // pick 'count' distinct columns out of TotalCols
                var chosenCols = new HashSet<int>();
                while (chosenCols.Count < count)
                    chosenCols.Add(rng.Next(0, TotalCols));

                for (int c = 0; c < TotalCols; c++)
                {
                    bool placeReal = chosenCols.Contains(c);
                    var e = CreerEnemy(r, c, placeReal, isRowFixed, dirX, speed, rng, availableTypes);
                    PlaceEnemy(enemies, e);
                    //PlaceEnemy(r, c, placeReal);
                }

                /*// Local helper to add one enemy
                void PlaceEnemy(int row, int col, bool visible)
                {
                    enemies.Add(new EnemyGen
                    {
                        Row = row,
                        Col = col,
                        Visible = visible,
                        IsFixed = isRowFixed,
                        DirectionX = dirX,
                        Speed = speed,
                        Type = visible
                            ? availableTypes[rng.Next(availableTypes.Count)]
                            : EnemyType.Type1  // type doesn’t matter if invisible
                    });
                }*/
            }

            //En commentaire. Dois être tester et reviser!!!!!
            // // 8. You can also randomly “unlock” one extra hidden row every few levels:
            // if (stage > BaseVisibleRows && rowsToShow < TotalRows)
            // {
            //     // e.g. every 3 stages we reveal one more hidden row
            //     int extra = (stage - BaseVisibleRows) / 3;
            //     for (int i = 0; i < extra; i++)
            //     {
            //         int hiddenRow = BaseVisibleRows + i;
            //         if (hiddenRow < TotalRows)
            //             PlaceEnemy(hiddenRow, rng.Next(TotalCols), visible: false);
            //     }
            // }

            return enemies;
        }

        /// <summary>
        /// 6. Optional Formation helper
        /// </summary>
        /// <param name="topRow"></param>
        /// <param name="leftCol"></param>
        /// <param name="shape"></param>
        /// <param name="enemies"></param>
        /// <param name="isRowFixed"></param>
        /// <param name="dirX"></param>
        /// <param name="speed"></param>
        /// <param name="rng"></param>
        /// <param name="availableTypes"></param>
        void ApplyFormation(int topRow, int leftCol, string shape, List<EnemyGen> enemies, bool isRowFixed, int dirX, float speed, Random rng, List<EnemyType> availableTypes)
        {
            switch (shape)
            {
                case "Triangle":
                    // Simple right‐angled triangle of size 3
                    for (int dr = 0; dr < 3; dr++)
                    {
                        for (int dc = 0; dc <= dr; dc++)
                        {
                            var e = CreerEnemy(topRow + dr, leftCol + dc, true, isRowFixed, dirX, speed, rng, availableTypes);
                            PlaceEnemy(enemies, e, true);
                            //PlaceEnemy(topRow + dr, leftCol + dc, visible: true);
                        }
                    }
                    break;
                case "Block":
                    // 2×2 block
                    for (int dr = 0; dr < 2; dr++)
                    {
                        for (int dc = 0; dc < 2; dc++)
                        {
                            var e = CreerEnemy(topRow + dr, leftCol + dc, true, isRowFixed, dirX, speed, rng, availableTypes);
                            PlaceEnemy(enemies, e, true);
                            //PlaceEnemy(topRow + dr, leftCol + dc, visible: true);
                        }
                    }
                    break;
                    // add more as you like…
            }
        }

        private EnemyGen CreerEnemy(int row, int col, bool visible, bool isRowFixed, int dirX, float speed, Random rng, List<EnemyType> availableTypes)
        {
            return new EnemyGen
            {
                Row = row,
                Col = col,
                Visible = visible,
                IsFixed = isRowFixed,
                DirectionX = dirX,
                Speed = speed,
                Type = visible
                            ? availableTypes[rng.Next(availableTypes.Count)]
                            : EnemyType.Type1  // type doesn’t matter if invisible
            };
        }
        private void PlaceEnemy(List<EnemyGen> enemies, EnemyGen enemy, bool overrider = false)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                var item = enemies[i];
                // If we find an existing enemy at this position and overrider is true, update it
                if (item.Row == enemy.Row && item.Col == enemy.Col)
                {
                    if (overrider)
                    {
                        enemies[i] = enemy;
                    }
                    return;
                }
            }

            enemies.Add(enemy);
        }
    }
}


#region Version 2

namespace SpaceZombie.Niveaux.Configs.V
{
    /// <summary>
    /// Stage 1:
    ///     Niveau 1:
    ///         2 ligne fixe.
    ///     Niveau 2:
    ///         2 ligne bouge même direction et vitesse basse.
    ///     Niveau 3:
    ///         3 lignes fixe
    ///     Niveau 4:
    ///         3 ligne bouge même direction et vitesse basse.
    ///     Niveau 5:
    ///         3 ligne bouge même direction et vitesse basse.
    ///     Intermission
    ///     Niveau 6:
    ///         2 lignes fixes. Introduction à 2 lignes de niveau 2
    ///     Niveau 7:
    ///         2 ligne bouge même direction et vitesse basse. de niveau 2
    ///     Niveau 8:
    ///         3 ligne bouge même direction et vitesse basse. (1 ligne niveau 2)
    ///     Niveau 9:
    ///         3 ligne bouge même direction et vitesse basse. (1 ligne niveau 2)
    ///     Niveau 10:
    ///         Boss
    ///         
    /// Stage 2:
    ///     Niveau 1:
    ///         3 lignes bouge même direction, vitesse augmenter, niveau 1.
    ///     Niveau 2:
    ///         3 lignes bouge même direction, vitesse comme precedente, (1 ligne niveau 2)
    ///     Niveau 3:
    ///         3 lignes bouge même direction, vitesse comme precedente, (2 ligne niveau 2)
    ///     Niveau 4:
    ///         3 lignes bouge même direction, vitesse comme precedente, (3 ligne niveau 2)
    ///     Intermission
    ///     Niveau 5:
    ///         4 lignes bouge même direction, vitesse comme precedente, (1 ligne niveau 2)
    ///     Niveau 6:
    ///         4 lignes bouge même direction, vitesse comme precedente, (2 ligne niveau 2)
    ///     Niveau 7:
    ///         4 lignes bouge même direction, vitesse comme precedente, (3 ligne niveau 2)
    ///     Niveau 8:
    ///         3 ligne bouge pas même direction direction et vitesse augmenter. (2 ligne niveau 2)
    ///     Niveau 9:
    ///         Boss       
    ///     
    /// Stage 3:
    ///     Niveau 1:
    ///         2 lignes bouge même direction, vitesse augmenter, niveau 3.
    ///     Niveau 2:
    ///         5 lignes bouge, vitesse comme precedente, (5 ligne niveau 1)
    ///     Niveau 3:
    ///         5 lignes bouge, vitesse comme precedente, vitesse comme precedente, (2 ligne niveau 2)
    ///     Niveau 4:
    ///         5 lignes bouge, vitesse comme precedente, vitesse comme precedente, (3 ligne niveau 2)
    ///     Niveau 5:
    ///         5 lignes bouge, vitesse comme precedente, vitesse comme precedente, (4 ligne niveau 2)
    ///     Intermission
    ///     Niveau 6:
    ///         4 lignes bouge même direction, vitesse comme precedente, (1 ligne niveau 3, 2 ligne niveau 2)
    ///     Niveau 7:
    ///         4 lignes bouge même direction, vitesse comme precedente, (1 ligne niveau 3, 3 ligne niveau 2)
    ///     Niveau 8:
    ///         4 lignes bouge même direction, vitesse comme precedente, (2 ligne niveau 3, 2 ligne niveau 2)
    ///     Niveau 9:
    ///         4 lignes bouge même direction, vitesse comme precedente, (3 ligne niveau 3, 1 ligne niveau 2)
    ///     Niveau 10:
    ///         Boss
    ///         
    /// Stage 4:
    ///     Niveau 1:
    ///         6 lignes bouge même direction, vitesse augmenter, niveau 1.
    ///     Niveau 2:
    ///         5 lignes bouge même direction, vitesse comme precedente, (1 ligne niveau 3, 2 ligne niveau 2, 2 ligne niveau 1)
    ///     Niveau 3:
    ///         5 lignes bouge même direction, vitesse comme precedente, (1 ligne niveau 3, 3 ligne niveau 2, 1 ligne niveau 1)
    ///     Niveau 4:
    ///         5 lignes bouge même direction, vitesse comme precedente, (2 ligne niveau 3, 3 ligne niveau 2, 0 ligne niveau 1)
    ///     Niveau 5:
    ///         5 lignes bouge même direction, vitesse comme precedente, (3 ligne niveau 3, 2 ligne niveau 2, 0 ligne niveau 1)
    ///     Intermission
    ///     Niveau 6:
    ///         5 lignes bouge direction, vitesse augmente, (2 ligne niveau 3, 2 ligne niveau 2, 1 ligne niveau 1)
    ///     Niveau 7:
    ///         5 lignes bouge direction, vitesse comme precedente, (2 ligne niveau 3, 3 ligne niveau 2, 0 ligne niveau 1)
    ///     Niveau 8:
    ///         5 lignes bouge direction, vitesse comme precedente, (3 ligne niveau 3, 2 ligne niveau 2, 0 ligne niveau 1)
    ///     Niveau 9:
    ///         5 lignes bouge direction, vitesse augmente, (2 ligne niveau 3, 2 ligne niveau 2, 1 ligne niveau 1)
    ///     Niveau 10:
    ///         Boss 
    ///         
    /// Stage 5:
    ///     Niveau 1:
    ///         6 lignes bouge même direction, vitesse augmenter, (1 ligne niveau 3, 2 ligne niveau 2, 3 ligne niveau 1).
    ///     Niveau 2:
    ///         6 lignes bouge même direction, vitesse comme precedente, (0 ligne niveau 3, 3 ligne niveau 2, 3 ligne niveau 1).
    ///     Niveau 3:
    ///         6 lignes bouge même direction, vitesse comme precedente, (2 ligne niveau 3, 2 ligne niveau 2, 2 ligne niveau 1).
    ///     Niveau 4:
    ///         6 lignes bouge même direction, vitesse comme precedente, (2 ligne niveau 3, 3 ligne niveau 2, 1 ligne niveau 1).
    ///     Niveau 5:
    ///         6 lignes bouge même direction, vitesse comme precedente, (4 ligne niveau 3, 1 ligne niveau 2, 1 ligne niveau 1).
    ///     Intermission
    ///     Niveau 6:
    ///         6 lignes bouge direction, vitesse augmenter, (4 ligne niveau 3, 1 ligne niveau 2, 1 ligne niveau 1).
    ///     Niveau 7:
    ///         6 lignes bouge direction, vitesse comme precedente, (4 ligne niveau 3, 2 ligne niveau 2, 0 ligne niveau 1)
    ///     Niveau 8:
    ///         6 lignes bouge direction, vitesse comme precedente, (5 ligne niveau 3, 1 ligne niveau 2, 0 ligne niveau 1)
    ///     Niveau 9:
    ///         6 lignes bouge direction, vitesse comme precedente, (6 ligne niveau 3, 0 ligne niveau 2, 0 ligne niveau 1)
    ///     Niveau 10:
    ///         Boss 
    ///         
    ///     3 lignes; bouge à vitesse base même direction
    ///     enemy niveau 1 pour 5 1er niveau
    /// </summary>
    public enum EnemyType
    {
        Type1 = 1,
        Type2,
        Type3,
        Type4,
        Type5,
        Type6
    }
    public class LevelGenerator
    {
        private readonly int _baseSeed;
        private const int TotalRows = 6;
        private const int TotalCols = 25;
        private const int LevelsPerStage = 10;
        private const int BaseVisibleRows = 3;     // stage 1 shows 3 rows
        private const float GlobalMinSpeed = 30f;
        private const float GlobalMaxSpeed = 150f;
        private const float DIRECTION_DEFAUT = 1f;
        //private const float FormationChance = 0.15f; // 15% of rows get a "formation"

        private GameData gd;

        public LevelGenerator(GameData gd, int baseSeed = 12345)
        {
            this.gd = gd;
            _baseSeed = baseSeed;
        }

        public NiveauZombiesSpawnSettings GenerateLevel(int stage, int levelNumber)
        {
            return GenererNiveauService(gd.Stages[stage].Levels[levelNumber], stage);
        }

        private NiveauZombiesSpawnSettings GenererNiveauService(Level setting, int stage)
        {
            NiveauLigneSettings[] ligneSettings = new NiveauLigneSettings[TotalRows];
            int NB_LIGNES_VISIBLE = setting.Lines.Count;
            for (int i = 0; i < ligneSettings.Length; i++)
            {
                if (i < NB_LIGNES_VISIBLE)
                {
                    var ligne = setting.Lines[i];
                    ligneSettings[i] = CreerLigne(ligne.Speed, ligne.Fixed, ligne.Direction, setting.NbEnemiesParLigne, EnemyConfig.Mapp(ligne.EnemyLevels[0]), stage);
                }
                else
                {
                    ligneSettings[i] = CreerLigneNonVisible();
                }
            }
            return new NiveauZombiesSpawnSettings()
            {
                DeplaceEnBlock = setting.DeplaceEnBlock,
                LigneSettings = ligneSettings
            };
        }

        #region Commun
        private NiveauLigneSettings CreerLigne(float vitesseLigne, bool fixe, float directionX, int nbEnemies, NiveauEnemySettings enemyTypeMapp, int stage)
        {
            return new NiveauLigneSettings()
            {
                EnemySlotSettings = CreerNiveauEnemySlotSettings(nbEnemies, true, enemyTypeMapp, stage),
                Vitesse = vitesseLigne,
                IsFixe = fixe,
                DirectionX = directionX
            };
        }
        private NiveauLigneSettings CreerLigneNonVisible()
        {
            return new NiveauLigneSettings()
            {
                EnemySlotSettings = CreerNiveauEnemySlotSettings(1, false, Enemies.Configs.EnemyConfig.enemyType1, 1),
                Vitesse = GlobalMinSpeed,
                IsFixe = true,
                DirectionX = DIRECTION_DEFAUT
            };
        }
        private NiveauEnemySlotSettings[] CreerNiveauEnemySlotSettings(int length, bool isVisible, NiveauEnemySettings enemy, int stage)
        {
            NiveauEnemySlotSettings[] slots = new NiveauEnemySlotSettings[length];
            for (int i = 0; i < length; i++)
            {
                slots[i] = CreerNiveauEnemySlotSettings(isVisible, enemy, stage);
            }
            return slots;
        }
        private NiveauEnemySlotSettings CreerNiveauEnemySlotSettings(bool isVisible, NiveauEnemySettings enemy, int stage)
        {
            return new NiveauEnemySlotSettings() { enemyObjSettings = CreerNiveauEnemyObjSettings(isVisible, enemy, stage) };
        }
        private NiveauEnemyObjSettings CreerNiveauEnemyObjSettings(bool isVisible, NiveauEnemySettings enemy, int stage)
        {
            return new NiveauEnemyObjSettings()
            {
                IsVisible = isVisible,
                enemySettings = CreerNiveauEnemySettings(enemy, stage)
            };
        }
        private NiveauEnemySettings CreerNiveauEnemySettings(NiveauEnemySettings enemy, int stage)
        {
            return new NiveauEnemySettings()
            {
                Color = enemy.Color,
                Enemy = new Enemies.Enemy(CalculateHP(enemy.Enemy.Hp, stage), enemy.Enemy.Score)
            };
        }
        /// <summary>
        /// pv go up * stage / chifre afin d'éviter monter exp.
        /// HP = BaseHP * (1 + GrowthRate * (Stage ^ Exponent)) / DampingFactor;
        /// </summary>
        /// <param name="baseHP"></param>
        /// <param name="stage"></param>
        /// <param name="growthRate"></param>
        /// <param name="exponent"></param>
        /// <param name="dampingFactor"></param>
        /// <returns></returns>
        public static int CalculateHP(int baseHP, int stage, float growthRate = 0.5f, float exponent = 1.5f, float dampingFactor = 2f)
        {
            return (int)MathF.Ceiling(baseHP * (1 + growthRate * MathF.Pow(stage, exponent)) / dampingFactor);
        }
        #endregion


        #region Stage 1
        private NiveauZombiesSpawnSettings GenererStageI(int stage, int levelNumber)
        {
            switch (levelNumber)
            {
                // case 1:
                //     return GenererStageINiveau1(stage);
                // case 2:
                //     return GenererStageINiveau2(stage);
                case 1:
                    return GenererStageINiveauX(stage, 2, true, DIRECTION_DEFAUT);
                case 2:
                    return GenererStageINiveauX(stage, 2, false, DIRECTION_DEFAUT);
                case 3:
                    return GenererStageINiveauX(stage, 2, true, DIRECTION_DEFAUT);
                case 4:
                    return GenererStageINiveauX(stage, 2, false, DIRECTION_DEFAUT);
                default:
                    throw new ArgumentOutOfRangeException($"Cannot find the function associate with the level. Stage: {stage} ; Level: {levelNumber}");
            }
        }
        private NiveauZombiesSpawnSettings GenererStageINiveau1(int stage, int nbEnemiesParLigne = 19)
        {
            NiveauLigneSettings[] ligneSettings = new NiveauLigneSettings[TotalRows];
            const int NB_LIGNES_VISIBLE = 2;
            for (int i = 0; i < ligneSettings.Length; i++)
            {
                if (i < NB_LIGNES_VISIBLE)
                {
                    ligneSettings[i] = CreerLigne(GlobalMinSpeed, true, DIRECTION_DEFAUT, nbEnemiesParLigne, Enemies.Configs.EnemyConfig.enemyType1, stage);
                }
                else
                {
                    ligneSettings[i] = CreerLigneNonVisible();
                }
            }
            return new NiveauZombiesSpawnSettings()
            {
                LigneSettings = ligneSettings
            };
        }
        private NiveauZombiesSpawnSettings GenererStageINiveau2(int stage, int nbEnemiesParLigne = 19)
        {
            NiveauLigneSettings[] ligneSettings = new NiveauLigneSettings[TotalRows];
            const int NB_LIGNES_VISIBLE = 2;
            for (int i = 0; i < ligneSettings.Length; i++)
            {
                if (i < NB_LIGNES_VISIBLE)
                {
                    ligneSettings[i] = CreerLigne(GlobalMinSpeed, false, DIRECTION_DEFAUT, nbEnemiesParLigne, Enemies.Configs.EnemyConfig.enemyType1, stage);
                }
                else
                {
                    ligneSettings[i] = CreerLigneNonVisible();
                }
            }
            return new NiveauZombiesSpawnSettings()
            {
                LigneSettings = ligneSettings
            };
        }
        private NiveauZombiesSpawnSettings GenererStageINiveauX(int stage, int nbLigneVisible, bool ligneFixe, float directionX, int nbEnemiesParLigne=19)
        {
            NiveauLigneSettings[] ligneSettings = new NiveauLigneSettings[TotalRows];
            for (int i = 0; i < ligneSettings.Length; i++)
            {
                if (i < nbLigneVisible)
                {
                    ligneSettings[i] = CreerLigne(GlobalMinSpeed, ligneFixe, directionX, nbEnemiesParLigne, Enemies.Configs.EnemyConfig.enemyType1, stage);
                }
                else
                {
                    ligneSettings[i] = CreerLigneNonVisible();
                }
            }
            return new NiveauZombiesSpawnSettings()
            {
                LigneSettings = ligneSettings
            };
        }
        #endregion
    }

    public class EnemyGen
    {
        public EnemyType Type { get; set; }
        public int Row { get; set; }  // 0..5
        public int Col { get; set; }  // 0..24
        public bool Visible { get; set; }
        public float Speed { get; set; }
        public int DirectionX { get; set; }  // +1 or -1
        public bool IsFixed { get; set; }
        public int Hp { get; set; }
    }

    public class LevelGeneratorQ
    {
        private readonly int _baseSeed;
        private const int TotalRows = 6;
        private const int TotalCols = 25;
        private const int LevelsPerStage = 10;
        private const int BaseVisibleRows = 3;     // stage 1 shows 3 rows
        private const float GlobalMinSpeed = 30f;
        private const float GlobalMaxSpeed = 150f;
        private const float FormationChance = 0.15f; // 15% of rows get a "formation"

        public LevelGeneratorQ(int baseSeed = 12345)
        {
            _baseSeed = baseSeed;
        }

        public List<EnemyGen> GenerateLevel(int levelNumber)
        {
            // 1. Seed a new Random for this level to guarantee determinism
            var rng = new Random(_baseSeed + levelNumber);

            // 2. Compute stage and intra-stage progress (0..1)
            int stage = ((levelNumber - 1) / LevelsPerStage) + 1;
            float levelProgress = ((levelNumber - 1) % LevelsPerStage) / (float)(LevelsPerStage - 1);

            // 3. How many rows should be “active” this stage?
            int rowsToShow = Math.Min(TotalRows, BaseVisibleRows + (stage - 1));

            // 4. How many enemy‐types are unlocked?
            int typesUnlocked = Math.Min(6, 1 + (int)Math.Floor(levelNumber / 5f));
            var availableTypes = new List<EnemyType>();
            for (int t = 1; t <= typesUnlocked; t++)
                availableTypes.Add((EnemyType)t);

            // 5. Decide per‐row enemy count by linear interp: min 4 → max 20
            int minEnemiesPerRow = 4 + (stage - 1) * 2;
            int maxEnemiesPerRow = 10 + (stage - 1) * 3;
            int enemiesThisRow(int rowIndex) =>
                Math.Clamp(
                    (int)Math.Round(
                        minEnemiesPerRow + (maxEnemiesPerRow - minEnemiesPerRow) * levelProgress
                    ),
                    1, TotalCols
                );

            var enemies = new List<EnemyGen>();

            /*// 6. Optional Formation helper
            void ApplyFormation(int topRow, int leftCol, string shape)
            {
                switch (shape)
                {
                    case "Triangle":
                        // Simple right‐angled triangle of size 3
                        for (int dr = 0; dr < 3; dr++)
                        {
                            for (int dc = 0; dc <= dr; dc++)
                            {
                                var e = CreerEnemy(topRow + dr, leftCol + dc, true, isRowFixed, dirX, speed, rng, availableTypes);
                                PlaceEnemy(enemies, e, true);
                                //PlaceEnemy(topRow + dr, leftCol + dc, visible: true);
                            }
                        }
                        break;
                    case "Block":
                        // 2×2 block
                        for (int dr = 0; dr < 2; dr++)
                        {
                            for (int dc = 0; dc < 2; dc++)
                            {
                                var e = CreerEnemy(topRow + dr, leftCol + dc, true, isRowFixed, dirX, speed, rng, availableTypes);
                                PlaceEnemy(enemies, e, true);
                                //PlaceEnemy(topRow + dr, leftCol + dc, visible: true);
                            }
                        }
                        break;
                        // add more as you like…
                }
            }*/

            // 7. Core placement loop
            for (int r = 0; r < rowsToShow; r++)
            {
                bool isRowFixed = rng.NextDouble() < 0.1;  // 10% of rows are “fixed”
                int dirX = rng.Next(2) == 0 ? +1 : -1;
                float speed = (float)(
                    GlobalMinSpeed + (GlobalMaxSpeed - GlobalMinSpeed) *
                    Math.Min(1.0, (stage - 1 + levelProgress) / 5.0)  // ramp up over 5 stages
                );

                // Maybe drop a formation on this row?
                if (rng.NextDouble() < FormationChance)
                {
                    int leftCol = rng.Next(0, TotalCols - 3);
                    ApplyFormation(r, leftCol, rng.NextDouble() < 0.5 ? "Triangle" : "Block", enemies, isRowFixed, dirX, speed, rng, availableTypes);
                    continue;
                }

                // Otherwise fill by count + invisible placeholders
                int count = enemiesThisRow(r);
                // pick 'count' distinct columns out of TotalCols
                var chosenCols = new HashSet<int>();
                while (chosenCols.Count < count)
                    chosenCols.Add(rng.Next(0, TotalCols));

                for (int c = 0; c < TotalCols; c++)
                {
                    bool placeReal = chosenCols.Contains(c);
                    var e = CreerEnemy(r, c, placeReal, isRowFixed, dirX, speed, rng, availableTypes);
                    PlaceEnemy(enemies, e);
                    //PlaceEnemy(r, c, placeReal);
                }

                /*// Local helper to add one enemy
                void PlaceEnemy(int row, int col, bool visible)
                {
                    enemies.Add(new EnemyGen
                    {
                        Row = row,
                        Col = col,
                        Visible = visible,
                        IsFixed = isRowFixed,
                        DirectionX = dirX,
                        Speed = speed,
                        Type = visible
                            ? availableTypes[rng.Next(availableTypes.Count)]
                            : EnemyType.Type1  // type doesn’t matter if invisible
                    });
                }*/
            }

            //En commentaire. Dois être tester et reviser!!!!!
            // // 8. You can also randomly “unlock” one extra hidden row every few levels:
            // if (stage > BaseVisibleRows && rowsToShow < TotalRows)
            // {
            //     // e.g. every 3 stages we reveal one more hidden row
            //     int extra = (stage - BaseVisibleRows) / 3;
            //     for (int i = 0; i < extra; i++)
            //     {
            //         int hiddenRow = BaseVisibleRows + i;
            //         if (hiddenRow < TotalRows)
            //             PlaceEnemy(hiddenRow, rng.Next(TotalCols), visible: false);
            //     }
            // }

            return enemies;
        }

        /// <summary>
        /// 6. Optional Formation helper
        /// </summary>
        /// <param name="topRow"></param>
        /// <param name="leftCol"></param>
        /// <param name="shape"></param>
        /// <param name="enemies"></param>
        /// <param name="isRowFixed"></param>
        /// <param name="dirX"></param>
        /// <param name="speed"></param>
        /// <param name="rng"></param>
        /// <param name="availableTypes"></param>
        void ApplyFormation(int topRow, int leftCol, string shape, List<EnemyGen> enemies, bool isRowFixed, int dirX, float speed, Random rng, List<EnemyType> availableTypes)
        {
            switch (shape)
            {
                case "Triangle":
                    // Simple right‐angled triangle of size 3
                    for (int dr = 0; dr < 3; dr++)
                    {
                        for (int dc = 0; dc <= dr; dc++)
                        {
                            var e = CreerEnemy(topRow + dr, leftCol + dc, true, isRowFixed, dirX, speed, rng, availableTypes);
                            PlaceEnemy(enemies, e, true);
                            //PlaceEnemy(topRow + dr, leftCol + dc, visible: true);
                        }
                    }
                    break;
                case "Block":
                    // 2×2 block
                    for (int dr = 0; dr < 2; dr++)
                    {
                        for (int dc = 0; dc < 2; dc++)
                        {
                            var e = CreerEnemy(topRow + dr, leftCol + dc, true, isRowFixed, dirX, speed, rng, availableTypes);
                            PlaceEnemy(enemies, e, true);
                            //PlaceEnemy(topRow + dr, leftCol + dc, visible: true);
                        }
                    }
                    break;
                    // add more as you like…
            }
        }

        private EnemyGen CreerEnemy(int row, int col, bool visible, bool isRowFixed, int dirX, float speed, Random rng, List<EnemyType> availableTypes)
        {
            return new EnemyGen
            {
                Row = row,
                Col = col,
                Visible = visible,
                IsFixed = isRowFixed,
                DirectionX = dirX,
                Speed = speed,
                Type = visible
                            ? availableTypes[rng.Next(availableTypes.Count)]
                            : EnemyType.Type1  // type doesn’t matter if invisible
            };
        }
        private void PlaceEnemy(List<EnemyGen> enemies, EnemyGen enemy, bool overrider = false)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                var item = enemies[i];
                // If we find an existing enemy at this position and overrider is true, update it
                if (item.Row == enemy.Row && item.Col == enemy.Col)
                {
                    if (overrider)
                    {
                        enemies[i] = enemy;
                    }
                    return;
                }
            }

            enemies.Add(enemy);
        }
    }
}
#endregion