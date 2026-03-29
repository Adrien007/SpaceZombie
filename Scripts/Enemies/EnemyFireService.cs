//EnemyFireService.cs
using System;
using System.Collections.Generic;
using Godot;
using SpaceZombie.Utilitaires;

namespace SpaceZombie.Enemies
{
    public class EnemyFireService
    {
        public EnemyFireOptions options = new EnemyFireOptions(new Random(1));
        public static void UpdateEnemyAvailable(List<Node2D> list)
        {
            List<int> indexToRemove = new List<int>();
            for (int i = list.Count - 1; i >= 0; i--)
            {
                //GD.Print($"list[{i}].Visible = {list[i].Visible}");
                if (!list[i].Visible)
                {
                    indexToRemove.Add(i);
                }
            }

            //GD.Print($"indexToRemove: {indexToRemove.Count}");
            for (int i = 0; i < indexToRemove.Count; i++)
            {
                ListsUtils.RemoveAtSwapBack(list, indexToRemove[i]);
            }
            //GD.Print("END RemoveAtSwapBack");
        }

        public List<Node2D> PickRandom(List<Node2D> list)
        {
            return ListsUtils.PickRandom(list, options.NbOfElementToSelect, options.Rng);
        }

        public Timer GetTimerRateOfFire() { return options.RateOfFire; }

    }

    public class EnemyFireOptions
    {
        private Random rng;
        private Timer rateOfFire = new Timer();

        private int nbOfElementToSelectPerShotFire;

        public Random Rng { get => rng; }
        public Timer RateOfFire { get => rateOfFire; }

        public int NbOfElementToSelect { get => nbOfElementToSelectPerShotFire; }

        public EnemyFireOptions(Random rng)
        {
            this.rng = rng;
        }

        public void NewSettings(int nbOfElementToSelectPerShotFire, float tempsRelaod)
        {
            this.nbOfElementToSelectPerShotFire = nbOfElementToSelectPerShotFire;
            rateOfFire.WaitTime = tempsRelaod;
        }
    }
}
