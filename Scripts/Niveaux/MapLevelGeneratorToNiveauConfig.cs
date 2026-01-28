//MapLevelGeneratorToNiveauConfig.cs
using System.Collections.Generic;
using System.Linq;
using Godot;
using SpaceZombie.Enemies;
using SpaceZombie.Enemies.Configs;
using SpaceZombie.Niveaux.Configs.R;

namespace SpaceZombie.Niveaux.Configs
{
    public class MapLevelGeneratorToNiveauConfig
    {

        public static NiveauZombiesSpawnSettings MapEnemiesToNiveauSettings(List<EnemyGen> enemies)
        {
            var rows = new Dictionary<int, List<EnemyGen>>();

            // Group enemies by row
            foreach (var enemy in enemies)
            {
                if (!rows.ContainsKey(enemy.Row))
                    rows[enemy.Row] = new List<EnemyGen>();
                rows[enemy.Row].Add(enemy);
            }

            // Prepare LigneSettings array
            var ligneSettingsList = new List<NiveauLigneSettings>();

            foreach (var rowPair in rows)
            {
                var rowEnemies = rowPair.Value;
                var slots = new NiveauEnemySlotSettings[rowEnemies.Count]; // always 25 cols

                foreach (var enemy in rowEnemies)
                {
                    var slot = new NiveauEnemySlotSettings
                    {
                        enemyObjSettings = new NiveauEnemyObjSettings
                        {
                            IsVisible = enemy.Visible,
                            enemySettings = new NiveauEnemySettings
                            {
                                // For now, use a default color — you can adjust this logic
                                Color = GetEnemyFromType(enemy.Type).Color,
                                Enemy = GetEnemyFromType(enemy.Type).Enemy
                            }
                        }
                    };
                    slots[enemy.Col] = slot;
                }

                /*// Fill in missing slots with invisible placeholders
                for (int col = 0; col < 25; col++)
                {
                    if (slots[col] == null)
                    {
                        slots[col] = new NiveauEnemySlotSettings
                        {
                            enemyObjSettings = new NiveauEnemyObjSettings
                            {
                                IsVisible = false,
                                enemySettings = new NiveauEnemySettings
                                {
                                    Color = EnemyConfig.enemyType1.Color,
                                    Enemy = EnemyConfig.enemyType1.Enemy
                                }
                            }
                        };
                    }
                }*/

                var firstVisible = rowEnemies.FirstOrDefault(e => e.Visible);
                ligneSettingsList.Add(new NiveauLigneSettings
                {
                    EnemySlotSettings = slots,
                    Vitesse = firstVisible?.Speed ?? 0f,
                    IsFixe = firstVisible?.IsFixed ?? false,
                    DirectionX = firstVisible?.DirectionX ?? 1
                });
            }

            return new NiveauZombiesSpawnSettings
            {
                LigneSettings = ligneSettingsList.ToArray()//.OrderBy(ls => ls.EnemySlotSettings[0].enemyObjSettings.enemySettings.Enemy.Row).ToArray()
            };
        }

        public static NiveauEnemySettings GetEnemyFromType(EnemyType type)
        {
            return type switch
            {
                EnemyType.Type1 => EnemyConfig.enemyType1,
                EnemyType.Type2 => EnemyConfig.enemyType2,
                EnemyType.Type3 => EnemyConfig.enemyType3,
                EnemyType.Type4 => EnemyConfig.enemyType4,
                EnemyType.Type5 => EnemyConfig.enemyType5,
                EnemyType.Type6 => EnemyConfig.enemyType6,
                _ => EnemyConfig.enemyType1
            };
        }

    }
}